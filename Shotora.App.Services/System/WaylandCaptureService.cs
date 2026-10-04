using Shared.Interfaces.Facades;
using Shotora.App.Interfaces.System;
using Shotora.App.Models.System;
using SkiaSharp;

namespace Shotora.App.Services.System;

/// <summary>
///     Captures the whole desktop under Wayland. Tries the XDG screenshot portal first (GNOME, KDE Plasma,
///     and wlroots compositors with xdg-desktop-portal-wlr or -hyprland), then the native screenshot tools.
/// </summary>
public sealed class WaylandCaptureService(
	IScreenshotPortal     screenshotPortal,
	IProcessSystemService processSystemService,
	IFileFacade           fileFacade,
	IEnvironmentFacade    environmentFacade) : IWaylandCaptureService
{
	public const string UnavailableMessage =
		"Shotora could not capture the screen in this Wayland session. Install xdg-desktop-portal together with the " +
		"backend for your desktop (xdg-desktop-portal-gnome, -kde, -wlr or -hyprland), or one of these tools: grim, spectacle, gnome-screenshot.";

	public const string DeniedMessage =
		"Screen capture was not allowed. Allow Shotora to take screenshots when your desktop asks, " +
		"or grant it in your system's privacy settings, then try again.";

	// Each tool writes a full-desktop PNG to the given path without user interaction.
	private static readonly (string FileName, Func<string, string[]> Arguments)[] CaptureTools =
	[
		("grim", path => [path]),
		("spectacle", path => ["--background", "--nonotify", "--fullscreen", "--output", path]),
		("gnome-screenshot", path => ["--file", path])
	];

	public bool IsWaylandSession()
	{
		return !string.IsNullOrEmpty(environmentFacade.GetEnvironmentVariable("WAYLAND_DISPLAY")) ||
			string.Equals(environmentFacade.GetEnvironmentVariable("XDG_SESSION_TYPE"), "wayland", StringComparison.OrdinalIgnoreCase);
	}

	public async Task<SKBitmap> CaptureDesktopAsync(CancellationToken cancellationToken = default)
	{
		var portal = await screenshotPortal.CaptureToFileAsync(cancellationToken);
		switch (portal.Status)
		{
			case ScreenshotPortalStatus.Captured when portal.FilePath != null:
			{
				// The portal saves into the user's folders; the file is only a hand-off, so remove it.
				var bitmap = DecodeAndDelete(portal.FilePath);
				if (bitmap != null)
				{
					return bitmap;
				}
				break;
			}
			case ScreenshotPortalStatus.Denied:
				// Respect the user's choice instead of working around it with another tool.
				throw new InvalidOperationException(DeniedMessage);
		}

		foreach (var (fileName, arguments) in CaptureTools)
		{
			var path = Path.Combine(Path.GetTempPath(), $"shotora-{Guid.NewGuid():N}.png");
			try
			{
				await processSystemService.RunAsync(fileName, arguments(path), cancellationToken: cancellationToken);
				var bitmap = DecodeAndDelete(path);
				if (bitmap != null)
				{
					return bitmap;
				}
			}
			catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
			{
				throw;
			}
			catch (Exception)
			{
				// Tool missing or unsupported by this compositor; try the next one.
			}
			finally
			{
				if (fileFacade.FileExists(path))
				{
					fileFacade.TryDelete(path);
				}
			}
		}

		throw new InvalidOperationException(UnavailableMessage);
	}

	private SKBitmap? DecodeAndDelete(string path)
	{
		if (!fileFacade.FileExists(path))
		{
			return null;
		}

		try
		{
			using var stream = fileFacade.OpenRead(path);
			return SKBitmap.Decode(stream);
		}
		catch (Exception)
		{
			return null;
		}
		finally
		{
			fileFacade.TryDelete(path);
		}
	}
}
