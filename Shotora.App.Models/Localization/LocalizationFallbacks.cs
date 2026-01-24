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
}
