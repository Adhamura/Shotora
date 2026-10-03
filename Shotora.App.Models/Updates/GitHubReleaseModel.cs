namespace Shotora.App.Models.Updates;

public sealed record GitHubReleaseModel(
	string  TagName,
	string? Name,
	string? Body,
	string  HtmlUrl,
	bool    Prerelease,
	bool    Draft);
