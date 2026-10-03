using System.Diagnostics.CodeAnalysis;
using Shared.Models.Constants;
using Shared.Models.Enums;
using Shotora.App.Models.Enums;

namespace Shotora.App.Models;

[ExcludeFromCodeCoverage]
public class AppSettings
{
	public int? SettingsSchemaVersion { get; set; }

	public string DefaultSaveFolder { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "Shotora");

	public string DefaultFormat { get; set; } = "png";

	public int JpegQuality { get; set; } = 90;

	public string DefaultTool { get; init; } = nameof(AnnotationToolType.Arrow);

	public string Theme { get; set; } = "Dark";

	public HotkeySetting RegionHotkey { get; set; } = HotkeySetting.RegionDefault();

	public HotkeySetting FullscreenHotkey { get; set; } = HotkeySetting.FullscreenDefault();

	public HotkeySetting ActiveWindowHotkey { get; set; } = HotkeySetting.ActiveWindowDefault();

	public bool RunOnStartup { get; set; } = false;

	public bool AutoCheckForUpdates { get; set; } = true;

	public DateTimeOffset? LastUpdateCheckUtc { get; set; }

	public string? SkippedUpdateVersion { get; set; }

	public string FilenamePattern { get; set; } = "Shotora_{0:yyyy-MM-dd_HH-mm-ss}";

	public OcrEngineType OcrEngine { get; set; } = OcrEngineType.Tesseract;

	public string OcrLanguages { get; set; } = LanguageCollection.DefaultOcrTesseractCode;

	public string DisplayLanguage { get; set; } = LanguageCollection.DefaultOcrLanguageCode;

	public bool EditorShowThicknessSlider { get; set; } = true;

	public double EditorDefaultThickness { get; set; } = 6;

	public bool EditorShowColorPalette { get; set; } = true;

	public double LastSelectionX { get; set; }

	public double LastSelectionY { get; set; }

	public double LastSelectionWidth { get; set; }

	public double LastSelectionHeight { get; set; }

	public string EditorDefaultColor { get; set; } = "#FF00FF00";

	public List<string> EditorHiddenIcons { get; set; } = [];

	public HotkeySetting EditorUndoHotkey { get; set; } = new()
	{
		Modifiers = KeyModifiers.Control,
		Key       = 0x5A
	};

	public HotkeySetting EditorRedoHotkey { get; set; } = new()
	{
		Modifiers = KeyModifiers.Control,
		Key       = 0x59
	};

	public HotkeySetting EditorCopyHotkey { get; set; } = new()
	{
		Modifiers = KeyModifiers.Control,
		Key       = 0x43
	};

	public HotkeySetting EditorSaveHotkey { get; set; } = new()
	{
		Modifiers = KeyModifiers.Control,
		Key       = 0x53
	};

	public string EditorCopyCloseMouseButton { get; set; } = "Right";

	public string EditorMoveSelectionMouseButton { get; set; } = "Right";
}
