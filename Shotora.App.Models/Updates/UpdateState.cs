namespace Shotora.App.Models.Updates;

/// <summary>
///     Updater bookkeeping, persisted separately from <see cref="AppSettings" /> so that saving the settings
///     window (which holds its own snapshot) can never overwrite it.
/// </summary>
public sealed class UpdateState
{
	public DateTimeOffset? LastCheckUtc { get; set; }

	public string? SkippedVersion { get; set; }
}
