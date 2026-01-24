using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shotora.App.Models.Enums;
using Shotora.App.Models.Localization;
using Shotora.App.Models.Utilities;

namespace Shotora.App.Models.ViewModels;

[ExcludeFromCodeCoverage]
public partial class SettingsViewModel : ViewSettingsBaseModel
{
	[ObservableProperty]
	private string _easyOcrActionLabel = string.Empty;
	[ObservableProperty]
	private object? _editorDefaultColorBrush;
	[ObservableProperty]
	private object? _editorDefaultColorValue;
	[ObservableProperty]
	private string _languageButtonText = string.Empty;

	[ObservableProperty]
	private TesseractLanguageOption? _selectedTesseractLanguage;
	public ObservableCollection<LanguageOption>          DisplayLanguageOptions { get; } = [];
	public ObservableCollection<TesseractLanguageOption> TesseractLanguages     { get; } = [];

	public ObservableCollection<OcrEngineType> OcrEngines { get; } = new([OcrEngineType.EasyOcr, OcrEngineType.Tesseract]);

	public IAsyncRelayCommand?          SaveCommand             { get; set; }
	public IAsyncRelayCommand<object?>? BrowseFolderCommand     { get; set; }
	public IAsyncRelayCommand?          ManageEasyOcrCommand    { get; set; }
	public IAsyncRelayCommand?          DownloadLanguageCommand { get; set; }
	public IRelayCommand?               RemoveLanguageCommand   { get; set; }
	public IAsyncRelayCommand?          LanguageActionCommand   { get; set; }

	public bool IsLanguageButtonEnabled => SelectedTesseractLanguage is
		{
			IsDownloading: false
		} lang &&
		(lang.IsInstalled || lang.CanDownload);

	public bool   IsSelectedLanguageInstalled   => SelectedTesseractLanguage?.IsInstalled   == true;
	public bool   IsSelectedLanguageDownloading => SelectedTesseractLanguage?.IsDownloading == true;
	public double SelectedLanguageProgress      => SelectedTesseractLanguage?.Progress   ?? 0;
	public string SelectedLanguageStatusText    => SelectedTesseractLanguage?.StatusText ?? string.Empty;

	public event EventHandler<AppSettings>? SettingsSaved;
	public event EventHandler<AppSettings>? LiveSettingsChanged;
	public event EventHandler<string>?      ThemeChanged;
	public event EventHandler<string>?      LanguageChanged;

	public void RaisePropertyChanged(string propertyName)
	{
		OnPropertyChanged(propertyName);
	}

	public void RaiseSettingsSaved(AppSettings settings)
	{
		SettingsSaved?.Invoke(this, settings);
	}
	public void RaiseLiveSettingsChanged(AppSettings values)
	{
		LiveSettingsChanged?.Invoke(this, values);
	}
	public void RaiseThemeChanged(string theme)
	{
		ThemeChanged?.Invoke(this, theme);
	}
	public void RaiseLanguageChanged(string language)
	{
		LanguageChanged?.Invoke(this, language);
	}
}
