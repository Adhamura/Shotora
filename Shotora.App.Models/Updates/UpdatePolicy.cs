namespace Shotora.App.Models.Updates;

/// <summary>
///     Rules for unattended (background) update checks and prompts.
/// </summary>
public static class UpdatePolicy
{
	public static bool ShouldRunAutomaticCheck(AppSettings settings, DateTimeOffset nowUtc)
	{
		if (!settings.AutoCheckForUpdates)
		{
			return false;
		}

		return settings.LastUpdateCheckUtc is not { } lastCheck ||
		       lastCheck > nowUtc ||
		       nowUtc - lastCheck >= UpdateConstants.AutomaticCheckInterval;
	}

	/// <summary>Background checks stay silent for versions the user chose to skip; manual checks always show.</summary>
	public static bool ShouldPromptUser(UpdateCheckResult result, AppSettings settings, bool userInitiated)
	{
		if (!result.IsUpdateAvailable)
		{
			return userInitiated;
		}

		return userInitiated ||
		       string.IsNullOrWhiteSpace(settings.SkippedUpdateVersion) ||
		       !string.Equals(VersionComparer.Normalize(settings.SkippedUpdateVersion), result.LatestVersion, StringComparison.OrdinalIgnoreCase);
	}
}
