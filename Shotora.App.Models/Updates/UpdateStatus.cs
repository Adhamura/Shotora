namespace Shotora.App.Models.Updates;

public enum UpdateStatus
{
	Idle,
	Checking,
	UpToDate,
	UpdateAvailable,
	Downloading,
	ReadyToInstall,
	Installing,
	Failed
}
