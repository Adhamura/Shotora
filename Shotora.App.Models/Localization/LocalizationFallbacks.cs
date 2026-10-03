using System.Diagnostics.CodeAnalysis;

namespace Shotora.App.Models.Localization;

[ExcludeFromCodeCoverage]
public static class LocalizationFallbacks
{
	public static class Common
	{
		public const string Repair  = "Repair";
		public const string Delete  = "Delete";
		public const string Install = "Install";
		public const string Close   = "Close";
	}

	public static class Tray
	{
		public const string CaptureRegion = "Capture Region";
		public const string CaptureFull   = "Capture Full Screen";
		public const string Settings      = "Settings";
		public const string About         = "About";
		public const string Exit          = "Exit";
	}

	public static class EasyOcr
	{
		public const string NotInstalled        = "Not installed";
		public const string Ready               = "Ready";
		public const string Broken              = "Broken (Repair)";
		public const string PythonMissing       = "Python installed, easyocr missing";
		public const string DownloadingPython   = "Downloading Python...";
		public const string InstallingPython    = "Installing Python...";
		public const string InstallingEasyOcr   = "Installing easyocr (this may take a minute)...";
		public const string PythonNotInstalled  = "Python was not installed.";
		public const string EasyOcrNotFound     = "Installation finished but easyocr was not found.";
		public const string InstallFailedFormat = "Failed: {0}";
		public const string Remove              = "Removing Python + easyocr...";
		public const string Removed             = "Removed";
		public const string DeleteFailedFormat  = "Delete failed: {0}";
		public const string Repairing           = "Repairing Python...";
		public const string RepairFailed        = "Repair failed (Reinstall)";
		public const string Repaired            = "Repaired";
		public const string CleaningOldFormat   = "Cleaning old Python ({0})...";
	}

	public static class Tesseract
	{
		public const string Installed        = "Installed";
		public const string Downloading      = "Downloading...";
		public const string NotInstalled     = "Not installed";
		public const string Ready            = "Ready";
		public const string AlreadyInstalled = "Already installed.";
		public const string Starting         = "Starting...";
		public const string Downloaded       = "Downloaded";
		public const string FailedFormat     = "Failed: {0}";
	}

	public static class Update
	{
		public const string Checking             = "Checking for updates…";
		public const string UpToDate             = "Shotora is up to date";
		public const string UpToDateDetailFormat = "Version {0} is the latest version.";
		public const string AvailableFormat      = "Shotora {0} is available";
		public const string AvailableDetailFormat = "You have version {0}. The update downloads in the background and installs when Shotora restarts.";
		public const string ManualDetail         = "Download the new version from GitHub to update this copy of Shotora.";
		public const string Failed               = "Couldn't check for updates";
		public const string FailedDetail         = "Check your internet connection and try again.";
		public const string DownloadingFormat    = "Downloading update… {0}%";
		public const string Ready                = "Update ready to install";
		public const string ReadyDetail          = "Restart Shotora to finish installing the update.";
		public const string Installing           = "Installing update…";
		public const string Idle                 = "Check whether a newer version of Shotora is available.";
		public const string CurrentVersionFormat = "Version {0}";
		public const string LaterButton          = "Remind me later";
	}
}
