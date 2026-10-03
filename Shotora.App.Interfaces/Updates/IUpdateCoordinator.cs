using Shotora.App.Models.Updates;

namespace Shotora.App.Interfaces.Updates;

/// <summary>
///     Runs unattended update checks in the background according to the user's settings.
/// </summary>
public interface IUpdateCoordinator : IDisposable
{
	/// <summary>Raised when a background check finds an update the user should be told about.</summary>
	event EventHandler<UpdateCheckResult>? UpdateAvailable;

	/// <summary>Starts the periodic background loop (first check after a short startup delay).</summary>
	void Start();

	/// <summary>Performs one automatic check if the policy allows it. Returns the result, or null when skipped.</summary>
	Task<UpdateCheckResult?> RunAutomaticCheckAsync(CancellationToken cancellationToken);

	/// <summary>Remembers that the user does not want to be reminded about <paramref name="version" />.</summary>
	Task SkipVersionAsync(string version);
}
