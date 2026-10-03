using Shotora.App.Models.Updates;

namespace Shotora.App.Interfaces.Updates;

public interface IGitHubReleaseClient
{
	/// <summary>Returns the latest published (non-draft, non-prerelease) release, or null when none exists.</summary>
	Task<GitHubReleaseModel?> GetLatestReleaseAsync(CancellationToken cancellationToken);
}
