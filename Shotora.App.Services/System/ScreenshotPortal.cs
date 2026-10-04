using System.Diagnostics.CodeAnalysis;
using Shotora.App.Interfaces.System;
using Shotora.App.Models.System;
using Tmds.DBus.Protocol;

namespace Shotora.App.Services.System;

/// <summary>
///     Takes a full-desktop screenshot through the XDG desktop portal (org.freedesktop.portal.Screenshot),
///     the only capture route a Wayland compositor offers to ordinary applications.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class ScreenshotPortal : IScreenshotPortal
{
	private const string PortalService       = "org.freedesktop.portal.Desktop";
	private const string PortalPath          = "/org/freedesktop/portal/desktop";
	private const string ScreenshotInterface = "org.freedesktop.portal.Screenshot";
	private const string RequestInterface    = "org.freedesktop.portal.Request";
	private const uint   ResponseSuccess     = 0;
	private const uint   ResponseCancelled   = 1;

	// Long enough for the user to answer a first-use permission dialog.
	private static readonly TimeSpan ResponseTimeout = TimeSpan.FromMinutes(2);

	public async Task<ScreenshotPortalResult> CaptureToFileAsync(CancellationToken cancellationToken = default)
	{
		var address = Address.Session;
		if (string.IsNullOrEmpty(address))
		{
			return ScreenshotPortalResult.Unavailable;
		}

		try
		{
			using var connection = new Connection(address);
			await connection.ConnectAsync();

			var token       = $"shotora{Guid.NewGuid():N}";
			var sender      = (connection.UniqueName ?? string.Empty).TrimStart(':').Replace('.', '_');
			var requestPath = $"{PortalPath}/request/{sender}/{token}";
			var response    = new TaskCompletionSource<(uint Code, string? Uri)>(TaskCreationOptions.RunContinuationsAsynchronously);

			// Subscribe before calling so the Response signal cannot arrive first.
			using var subscription = await connection.AddMatchAsync(
				new MatchRule
				{
					Type      = MessageType.Signal,
					Interface = RequestInterface,
					Member    = "Response",
					Path      = requestPath
				},
				static (Message message, object? _) =>
				{
					var reader  = message.GetBodyReader();
					var code    = reader.ReadUInt32();
					var results = reader.ReadDictionaryOfStringToVariantValue();
					return (code, results.TryGetValue("uri", out var uri) ? uri.GetString() : null);
				},
				static (Exception? exception, (uint Code, string? Uri) value, object? _, object? state) =>
				{
					var completion = (TaskCompletionSource<(uint Code, string? Uri)>)state!;
					if (exception != null)
					{
						completion.TrySetException(exception);
					}
					else
					{
						completion.TrySetResult(value);
					}
				},
				null,
				response,
				false,
				ObserverFlags.None);

			await connection.CallMethodAsync(CreateScreenshotCall(connection, token));

			var (code, uri) = await response.Task.WaitAsync(ResponseTimeout, cancellationToken);
			if (code == ResponseCancelled)
			{
				return ScreenshotPortalResult.Denied;
			}

			if (code != ResponseSuccess || string.IsNullOrEmpty(uri) || !Uri.TryCreate(uri, UriKind.Absolute, out var fileUri) || !fileUri.IsFile)
			{
				return ScreenshotPortalResult.Unavailable;
			}

			return new ScreenshotPortalResult(ScreenshotPortalStatus.Captured, fileUri.LocalPath);
		}
		catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
		{
			throw;
		}
		catch (Exception)
		{
			// No portal service, no Screenshot backend, or the bus is unreachable.
			return ScreenshotPortalResult.Unavailable;
		}
	}

	private static MessageBuffer CreateScreenshotCall(Connection connection, string token)
	{
		using var writer = connection.GetMessageWriter();
		writer.WriteMethodCallHeader(PortalService, PortalPath, ScreenshotInterface, "Screenshot", "sa{sv}");
		writer.WriteString(string.Empty);
		writer.WriteDictionary(new Dictionary<string, VariantValue>
		{
			["handle_token"] = VariantValue.String(token),
			["interactive"]  = VariantValue.Bool(false)
		});
		return writer.CreateMessage();
	}
}
