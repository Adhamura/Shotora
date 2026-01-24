using System.Diagnostics.CodeAnalysis;

namespace Shotora.App.Models.Constants;

[ExcludeFromCodeCoverage]
public static class LocalizationKeys
{
	public const  string LanguagePrefix = "Loc_Language_";
	private const string CommonPrefix   = "Loc_Common_";

	public const string CommonRepair  = $"{CommonPrefix}Repair";
	public const string CommonDelete  = $"{CommonPrefix}Delete";
	public const string CommonInstall = $"{CommonPrefix}Install";

	public const string EasyOcrStatusNotInstalled       = "Loc_EasyOcr_Status_NotInstalled";
	public const string EasyOcrStatusReady              = "Loc_EasyOcr_Status_Ready";
	public const string EasyOcrStatusBroken             = "Loc_EasyOcr_Status_Broken";
	public const string EasyOcrStatusPythonMissing      = "Loc_EasyOcr_Status_PythonMissing";
	public const string EasyOcrStatusDownloadingPython  = "Loc_EasyOcr_Status_DownloadingPython";
	public const string EasyOcrStatusInstallingPython   = "Loc_EasyOcr_Status_InstallingPython";
	public const string EasyOcrStatusInstallingEasyOcr  = "Loc_EasyOcr_Status_InstallingEasyOcr";
	public const string EasyOcrStatusPythonNotInstalled = "Loc_EasyOcr_Status_PythonNotInstalled";
	public const string EasyOcrStatusNotFound           = "Loc_EasyOcr_Status_EasyOcrNotFound";
	public const string EasyOcrStatusInstallFailedFmt   = "Loc_EasyOcr_Status_InstallFailed_Format";
	public const string EasyOcrStatusRemove             = "Loc_EasyOcr_Status_Remove";
	public const string EasyOcrStatusRemoved            = "Loc_EasyOcr_Status_Removed";
	public const string EasyOcrStatusDeleteFailedFmt    = "Loc_EasyOcr_Status_DeleteFailed_Format";
	public const string EasyOcrStatusRepairing          = "Loc_EasyOcr_Status_Repairing";
	public const string EasyOcrStatusRepairFailed       = "Loc_EasyOcr_Status_RepairFailed";
	public const string EasyOcrStatusRepaired           = "Loc_EasyOcr_Status_Repaired";
	public const string EasyOcrStatusCleaningOldFmt     = "Loc_EasyOcr_Status_CleaningOldPython_Format";

	public const string TessStatusReady            = "Loc_Tess_Status_Ready";
	public const string TessStatusInstalled        = "Loc_Tess_Status_Installed";
	public const string TessStatusDownloading      = "Loc_Tess_Status_Downloading";
	public const string TessStatusNotInstalled     = "Loc_Tess_Status_NotInstalled";
	public const string TessStatusAlreadyInstalled = "Loc_Tess_Status_AlreadyInstalled";
	public const string TessStatusStarting         = "Loc_Tess_Status_Starting";
	public const string TessStatusDownloaded       = "Loc_Tess_Status_Downloaded";
	public const string TessStatusFailedFmt        = "Loc_Tess_Status_Failed_Format";

	public const string TrayCaptureRegion = "LocTrayCaptureRegion";
	public const string TrayCaptureFull   = "LocTrayCaptureFull";
	public const string TraySettings      = "LocTraySettings";
	public const string TrayAbout         = "LocTrayAbout";
	public const string TrayExit          = "LocTrayExit";
}
