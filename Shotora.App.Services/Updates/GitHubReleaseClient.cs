using System.Net;
using System.Text.Json;
using Shared.Interfaces.Adapters;
using Shotora.App.Interfaces.Updates;
using Shotora.App.Models.Updates;

namespace Shotora.App.Services.Updates;

public class GitHubReleaseClient(IHttpClientAdapter httpClientAdapter) : IGitHubReleaseClient
{
	public async Task<GitHubReleaseModel?> GetLatestReleaseAsync(CancellationToken cancellationToken)
	{
		try
		{
			var payload = await httpClientAdapter.Get(UpdateConstants.LatestReleaseApiUrl, cancellationToken);
			return Parse(payload);
		}
		catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
		{
			// GitHub answers 404 when the repository has no published release yet.
			return null;
		}
	}

	public static GitHubReleaseModel? Parse(byte[]? payload)
	{
		if (payload == null || payload.Length == 0)
		{
			return null;
		}

		using var document = JsonDocument.Parse(payload);
		var       root     = document.RootElement;
		if (root.ValueKind != JsonValueKind.Object)
		{
			return null;
		}

		var tagName = GetString(root, "tag_name");
		if (string.IsNullOrWhiteSpace(tagName))
		{
			return null;
		}

		var release = new GitHubReleaseModel(
			tagName,
			GetString(root, "name"),
			GetString(root, "body"),
			GetString(root, "html_url") ?? UpdateConstants.LatestReleaseUrl,
			GetBool(root, "prerelease"),
			GetBool(root, "draft"));

		return release.Draft || release.Prerelease ? null : release;
	}

	private static string? GetString(JsonElement element, string property)
	{
		return element.TryGetProperty(property, out var value) && value.ValueKind == JsonValueKind.String
			? value.GetString()
			: null;
	}

	private static bool GetBool(JsonElement element, string property)
	{
		return element.TryGetProperty(property, out var value) && value.ValueKind == JsonValueKind.True;
	}
}
