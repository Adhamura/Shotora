namespace Shotora.App.Models.Updates;

/// <summary>
///     Rules for unattended (background) update checks and prompts.
/// </summary>
public static class UpdatePolicy
{
	public static bool ShouldRunAutomaticCheck(bool autoCheckEnabled, DateTimeOffset? lastCheckUtc, DateTimeOffset nowUtc)
	{
		if (!autoCheckEnabled)
		{
			return false;
		}

		return lastCheckUtc is not { } lastCheck ||
		       lastCheck > nowUtc ||
		       nowUtc - lastCheck >= UpdateConstants.AutomaticCheckInterval;
	}

	/// <summary>Background checks stay silent for versions the user chose to skip; manual checks always show.</summary>
	public static bool ShouldPromptUser(UpdateCheckResult result, string? skippedVersion, bool userInitiated)
	{
		if (!result.IsUpdateAvailable)
		{
			return userInitiated;
		}

		return userInitiated ||
		       string.IsNullOrWhiteSpace(skippedVersion) ||
		       !string.Equals(VersionComparer.Normalize(skippedVersion), result.LatestVersion, StringComparison.OrdinalIgnoreCase);
	}
}
