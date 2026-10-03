using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Mvvm.ComponentModel;
using Shared.Models.Constants;
using Shared.Models.Enums;
using Shotora.App.Models.Enums;
using Shotora.App.Models.Extensions;

namespace Shotora.App.Models.ViewModels;

[ExcludeFromCodeCoverage]
public partial class ViewSettingsBaseModel : ViewModelBase
{
	[ObservableProperty]
	private string _activeWindowHotkeyText = string.Empty;
	[ObservableProperty]
	[SettingsExtension(SettingsTypes.NullOrEmpty, SettingsPropertyTypes.Simple)]
	private string _defaultFormat = "png";
	[ObservableProperty]
	[SettingsExtension(SettingsTypes.Path, Environment.SpecialFolder.MyPictures)]
	private string _defaultSaveFolder = string.Empty;
	[ObservableProperty]
	[SettingsExtension(SettingsTypes.NullOrEmpty, LanguageCollection.DefaultOcrLanguageCode)]
	private string _displayLanguage = LanguageCollection.DefaultOcrLanguageCode;
	[ObservableProperty]
	private double _easyOcrInstallProgress;
	[ObservableProperty]
	private string _easyOcrInstallStatus = "Not installed";
	[ObservableProperty]
	[SettingsExtension(SettingsTypes.NullOrEmpty, "Right")]
	private string _editorCopyCloseMouseButton = "Right";
	[ObservableProperty]
	private string _editorCopyHotkeyText = string.Empty;
	[ObservableProperty]
	[SettingsExtension(SettingsTypes.NullOrEmpty, "#FF00FF00")]
	private string _editorDefaultColor = "#FF00FF00";
	[ObservableProperty]
	[SettingsExtension(SettingsTypes.PositiveDefault, 6)]
	private double _editorDefaultThickness = 6;
	[ObservableProperty]
	[SettingsExtension(SettingsTypes.NullOrEmpty, "Right")]
	private string _editorMoveSelectionMouseButton = "Right";
	[ObservableProperty]
	private string _editorRedoHotkeyText = string.Empty;
	[ObservableProperty]
	private string _editorSaveHotkeyText = string.Empty;
	[ObservableProperty]
	[SettingsExtension(SettingsTypes.NullOrEmpty, SettingsPropertyTypes.Simple)]
	private bool _editorShowColorPalette = true;
	[ObservableProperty]
	[SettingsExtension(SettingsTypes.NullOrEmpty, SettingsPropertyTypes.Simple)]
	private bool _editorShowThicknessSlider = true;
	[ObservableProperty]
	private string _editorUndoHotkeyText = string.Empty;
	[ObservableProperty]
	[SettingsExtension(SettingsTypes.NullOrEmpty, "Shotora_{0:yyyy-MM-dd_HH-mm-ss}")]
	private string _filenamePattern = "Shotora_{0:yyyy-MM-dd_HH-mm-ss}";
	[ObservableProperty]
	private string _fullscreenHotkeyText = string.Empty;
	[ObservableProperty]
	private bool _isEasyOcrBroken;
	[ObservableProperty]
	private bool _isEasyOcrInstalled;
	[ObservableProperty]
	private bool _isEasyOcrInstalling;
	[ObservableProperty]
	private bool _isLoaded;
	[ObservableProperty]
	[SettingsExtension(SettingsTypes.Clamp, 50, 100)]
	private int _jpegQuality = 90;
	[ObservableProperty]
	[SettingsExtension(SettingsTypes.NullOrEmpty, SettingsPropertyTypes.Simple)]
	private OcrEngineType _ocrEngine = OcrEngineType.Tesseract;
	[ObservableProperty]
	[SettingsExtension(SettingsTypes.NullOrEmpty, SettingsPropertyTypes.Simple)]
	private string _ocrLanguages = LanguageCollection.DefaultOcrTesseractCode;
	[ObservableProperty]
	private string _regionHotkeyText = string.Empty;
	[ObservableProperty]
	[SettingsExtension(SettingsTypes.NullOrEmpty, SettingsPropertyTypes.Simple)]
	private bool _runOnStartup;
	[ObservableProperty]
	[SettingsExtension(SettingsTypes.NullOrEmpty, SettingsPropertyTypes.Simple)]
	private bool _autoCheckForUpdates = true;
	[ObservableProperty]
	[SettingsExtension(SettingsTypes.NullOrEmpty, SettingsPropertyTypes.Simple)]
	private bool _showArrowIcon = true;
	[ObservableProperty]
	[SettingsExtension(SettingsTypes.NullOrEmpty, SettingsPropertyTypes.Simple)]
	private bool _showBlurIcon = true;
	[ObservableProperty]
	[SettingsExtension(SettingsTypes.NullOrEmpty, SettingsPropertyTypes.Simple)]
	private bool _showCancelIcon = true;
	[ObservableProperty]
	[SettingsExtension(SettingsTypes.NullOrEmpty, SettingsPropertyTypes.Simple)]
	private bool _showCopyIcon = true;
	[ObservableProperty]
	[SettingsExtension(SettingsTypes.NullOrEmpty, SettingsPropertyTypes.Simple)]
	private bool _showEllipseIcon = true;
	[ObservableProperty]
	[SettingsExtension(SettingsTypes.NullOrEmpty, SettingsPropertyTypes.Simple)]
	private bool _showHighlightIcon = true;
	[ObservableProperty]
	[SettingsExtension(SettingsTypes.NullOrEmpty, SettingsPropertyTypes.Simple)]
	private bool _showLineIcon = true;
	[ObservableProperty]
	[SettingsExtension(SettingsTypes.NullOrEmpty, SettingsPropertyTypes.Simple)]
	private bool _showMoveIcon = true;
	[ObservableProperty]
	[SettingsExtension(SettingsTypes.NullOrEmpty, SettingsPropertyTypes.Simple)]
	private bool _showPenIcon = true;
	[ObservableProperty]
	[SettingsExtension(SettingsTypes.NullOrEmpty, SettingsPropertyTypes.Simple)]
	private bool _showPixelateIcon = true;
	[ObservableProperty]
	[SettingsExtension(SettingsTypes.NullOrEmpty, SettingsPropertyTypes.Simple)]
	private bool _showRectangleIcon = true;
	[ObservableProperty]
	[SettingsExtension(SettingsTypes.NullOrEmpty, SettingsPropertyTypes.Simple)]
	private bool _showRedoIcon = true;
	[ObservableProperty]
	[SettingsExtension(SettingsTypes.NullOrEmpty, SettingsPropertyTypes.Simple)]
	private bool _showSaveIcon = true;
	[ObservableProperty]
	[SettingsExtension(SettingsTypes.NullOrEmpty, SettingsPropertyTypes.Simple)]
	private bool _showTextIcon = true;
	[ObservableProperty]
	[SettingsExtension(SettingsTypes.NullOrEmpty, SettingsPropertyTypes.Simple)]
	private bool _showUndoIcon = true;
	[ObservableProperty]
	[SettingsExtension(SettingsTypes.NullOrEmpty, SettingsPropertyTypes.Simple)]
	private bool _showUploadIcon = true;
	[ObservableProperty]
	[SettingsExtension(SettingsTypes.NullOrEmpty, SettingsPropertyTypes.Simple)]
	private string _theme = "Dark";

	public static IReadOnlyList<string> ThemeOptions       { get; } = ["Dark", "Light", "Sunset"];
	public static IReadOnlyList<string> FormatOptions      { get; } = ["png", "jpg"];
	public static IReadOnlyList<string> MouseButtonOptions { get; } = ["Right", "Middle", "Left", "None"];
	public static IReadOnlyList<string> HotkeyOptions { get; } =
	[
		"PrintScreen", "Ctrl+Shift+PrintScreen", "Alt+PrintScreen", "Shift+Win+S", "Ctrl+Alt+F1", "Ctrl+Alt+F2", "Ctrl+Z", "Ctrl+Y", "Ctrl+C", "Ctrl+S"
	];
}
