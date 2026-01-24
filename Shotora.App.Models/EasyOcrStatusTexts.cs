using System.Diagnostics.CodeAnalysis;
using Shotora.App.Models.Localization;

namespace Shotora.App.Models;

[ExcludeFromCodeCoverage]
public class EasyOcrStatusTexts
{
	public string Ready                   { get; init; } = LocalizationFallbacks.EasyOcr.Ready;
	public string NotInstalled            { get; init; } = LocalizationFallbacks.EasyOcr.NotInstalled;
	public string Broken                  { get; init; } = LocalizationFallbacks.EasyOcr.Broken;
	public string PythonMissing           { get; init; } = LocalizationFallbacks.EasyOcr.PythonMissing;
	public string DownloadingPython       { get; init; } = LocalizationFallbacks.EasyOcr.DownloadingPython;
	public string InstallingPython        { get; init; } = LocalizationFallbacks.EasyOcr.InstallingPython;
	public string InstallingEasyOcr       { get; init; } = LocalizationFallbacks.EasyOcr.InstallingEasyOcr;
	public string PythonNotInstalled      { get; init; } = LocalizationFallbacks.EasyOcr.PythonNotInstalled;
	public string EasyOcrNotFound         { get; init; } = LocalizationFallbacks.EasyOcr.EasyOcrNotFound;
	public string InstallFailedFormat     { get; init; } = LocalizationFallbacks.EasyOcr.InstallFailedFormat;
	public string Removing                { get; init; } = LocalizationFallbacks.EasyOcr.Remove;
	public string Removed                 { get; init; } = LocalizationFallbacks.EasyOcr.Removed;
	public string DeleteFailedFormat      { get; init; } = LocalizationFallbacks.EasyOcr.DeleteFailedFormat;
	public string Repairing               { get; init; } = LocalizationFallbacks.EasyOcr.Repairing;
	public string RepairFailed            { get; init; } = LocalizationFallbacks.EasyOcr.RepairFailed;
	public string Repaired                { get; init; } = LocalizationFallbacks.EasyOcr.Repaired;
	public string CleaningOldPythonFormat { get; init; } = LocalizationFallbacks.EasyOcr.CleaningOldFormat;
}
