using System.ComponentModel;
using System.Reflection;
using CommunityToolkit.Mvvm.Input;
using Shared.Interfaces.Adapters;
using Shared.Models.Constants;
using Shared.Models.Enums;
using Shotora.App.Interfaces.Adapters;
using Shotora.App.Interfaces.Ocr.EastOcr;
using Shotora.App.Interfaces.Ocr.Tesseract;
using Shotora.App.Interfaces.Providers;
using Shotora.App.Interfaces.System;
using Shotora.App.Interfaces.ViewModels;
using Shotora.App.Models;
using Shotora.App.Models.Constants;
using Shotora.App.Models.Enums;
using Shotora.App.Models.Localization;
using Shotora.App.Models.Utilities;
using Shotora.App.Models.ViewModels;

namespace Shotora.App.Services.ViewModels;

public class SettingsViewModelController(
	ISettingsSystemService     settingsSystemService,
	ILanguageTesseractService  languageTesseractService,
	IMaintenanceEasyOcrService maintenanceEasyOcrService,
	IFolderPickerAdapter       folderPickerAdapter,
	IColorAdapter              colorAdapter,
	IIconVisibilityProvider    iconVisibilityProvider,
	ISettingsProvider          settingsProvider,
	IPropertyAdapter           propertyAdapter,
	ILocalizationProvider      localizationProvider) : ISettingsViewModelController
{
	private readonly SemaphoreSlim            _saveGate = new(1, 1);
	private          TesseractLanguageOption? _attachedLanguage;
	private          bool                     _initialized;

	private SettingsViewModel _model = null!;

	private AppSettings _settings = new();
	private bool        _suppressColorSync;
	private bool        _suspendSave;

	public void Init()
	{
		if (_initialized)
		{
			return;
		}

		_model = new SettingsViewModel
		{
			LanguageButtonText = localizationProvider.GetString("LocSettingsOcrInstallLang", "Install")
		};

		WireCommands();
		UpdateEasyOcrActionLabel();
		localizationProvider.LanguageChanged += (_, _) => HandleLanguageChanged();

		_model.PropertyChanged += OnPropertyChangedInternal;
		_initialized           =  true;
	}

	public async Task SaveNowAsync()
	{
		await _saveGate.WaitAsync();
		try
		{
			await SaveAsync();
		}
		finally
		{
			_saveGate.Release();
		}
	}
	public void SettingsHandlersAttach(Action<SettingsViewModel> action)
	{
		action(_model);
	}

	public async Task LoadAsync(AppSettings? settings = null)
	{
		Init();
		if (!_model.IsLoaded)
		{
			_suspendSave = true;
			_settings    = settings ?? await settingsSystemService.LoadAsync();

			BuildDisplayLanguageOptions();
			ApplySettingsToView(_settings);
			InitializeTesseractLanguages();
			await RefreshEasyOcrStateAsync();

			_model.IsLoaded = true;
			_suspendSave    = false;
		}
	}

	private void WireCommands()
	{
		_model.SaveCommand             = new AsyncRelayCommand(SaveAsync);
		_model.BrowseFolderCommand     = new AsyncRelayCommand<object?>(BrowseFolderAsync);
		_model.ManageEasyOcrCommand    = new AsyncRelayCommand(ManageEasyOcrAsync,            () => !_model.IsEasyOcrInstalling);
		_model.DownloadLanguageCommand = new AsyncRelayCommand(DownloadSelectedLanguageAsync, () => _model.SelectedTesseractLanguage?.CanDownload == true);
		_model.RemoveLanguageCommand = new RelayCommand(RemoveSelectedLanguage, () => _model.SelectedTesseractLanguage is
		{
			IsInstalled: true, IsDownloading: false
		});
		_model.LanguageActionCommand = new AsyncRelayCommand(ExecuteLanguageActionAsync, () => _model.IsLanguageButtonEnabled);
	}

	private void ApplySettingsToView(AppSettings settings)
	{
		var simpleNames = propertyAdapter
			.GetPropertyNames<SettingsViewModel>(SettingsPropertyTypes.Simple)
			.Where(n => !(n.StartsWith("Show", StringComparison.OrdinalIgnoreCase) && n.EndsWith("Icon", StringComparison.OrdinalIgnoreCase)));

		var trackedNames = propertyAdapter.GetPropertyNames<SettingsViewModel>(SettingsPropertyTypes.Tracked);

		foreach (var name in simpleNames.Concat(trackedNames))
		{
			var value = propertyAdapter.GetValue<AppSettings, object?>(settings, name);
			propertyAdapter.SetValue(_model, name, value);
		}
		ApplyHotkeys(settings);
		ApplyHiddenIcons(settings.EditorHiddenIcons);
		_model.EditorDefaultColorValue = colorAdapter.CreateColor(_model.EditorDefaultColor);
		_model.EditorDefaultColorBrush = colorAdapter.CreateBrush(_model.EditorDefaultColor);
	}

	private void ApplyHotkeys(AppSettings settings)
	{
		var map = new (string PropertyName, HotkeySetting Hotkey)[]
		{
			(nameof(SettingsViewModel.RegionHotkeyText), settings.RegionHotkey), (nameof(SettingsViewModel.FullscreenHotkeyText), settings.FullscreenHotkey),
			(nameof(SettingsViewModel.ActiveWindowHotkeyText), settings.ActiveWindowHotkey), (nameof(SettingsViewModel.EditorUndoHotkeyText), settings.EditorUndoHotkey),
			(nameof(SettingsViewModel.EditorRedoHotkeyText), settings.EditorRedoHotkey), (nameof(SettingsViewModel.EditorCopyHotkeyText), settings.EditorCopyHotkey),
			(nameof(SettingsViewModel.EditorSaveHotkeyText), settings.EditorSaveHotkey)
		};

		foreach (var (property, hotkey) in map)
		{
			propertyAdapter.SetValue(_model, property, settingsProvider.HotkeyToString(hotkey));
		}
	}

	private void UpdateHiddenIconsFromStates()
	{
		_settings.EditorHiddenIcons = BuildHiddenIcons().ToList();
	}

	private void ApplyHiddenIcons(IEnumerable<string>? hidden)
	{
		var visibility = iconVisibilityProvider.BuildVisibilityMap(hidden);
		foreach (var icon in Enum.GetValues<EditorIcon>())
		{
			propertyAdapter.SetValue(_model, $"Show{icon}Icon", visibility.GetValueOrDefault(icon, true));
		}
	}

	private IEnumerable<string> BuildHiddenIcons()
	{
		var visibility = Enum.GetValues<EditorIcon>()
			.ToDictionary(icon => icon, icon => propertyAdapter.GetValue<SettingsViewModel, bool>(_model, $"Show{icon}Icon"));

		return iconVisibilityProvider.BuildHiddenIconIds(visibility);
	}

	private void BuildDisplayLanguageOptions()
	{
		_model.DisplayLanguageOptions.Clear();
		foreach (var lang in settingsProvider.BuildDisplayLanguages())
		{
			_model.DisplayLanguageOptions.Add(lang);
		}

		var current = _model.DisplayLanguageOptions.FirstOrDefault(l =>
			string.Equals(l.Code, _model.DisplayLanguage, StringComparison.OrdinalIgnoreCase));
		_model.DisplayLanguage = current.Code;
	}

	private void InitializeTesseractLanguages()
	{
		_model.TesseractLanguages.Clear();
		languageTesseractService.PopulateLanguages(_model.TesseractLanguages, option => option.PropertyChanged += LanguageChangedHandler);
		_model.SelectedTesseractLanguage = _model.TesseractLanguages.FirstOrDefault();
		RefreshLanguageCommands();
	}

	private void LanguageChangedHandler(object? sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName is nameof(TesseractLanguageOption.IsInstalled)
			or nameof(TesseractLanguageOption.IsDownloading)
			or nameof(TesseractLanguageOption.CanDownload)
			or nameof(TesseractLanguageOption.Progress)
			or nameof(TesseractLanguageOption.StatusText))
		{
			if (sender == _model.SelectedTesseractLanguage)
			{
				RefreshLanguageCommands();
			}
		}
	}

	private void NotifyLanguageButtonPropertiesChanged()
	{
		UpdateLanguageButtonText();
		_model.RaisePropertyChanged(nameof(SettingsViewModel.IsLanguageButtonEnabled));
		_model.RaisePropertyChanged(nameof(SettingsViewModel.IsSelectedLanguageInstalled));
		_model.RaisePropertyChanged(nameof(SettingsViewModel.IsSelectedLanguageDownloading));
		_model.RaisePropertyChanged(nameof(SettingsViewModel.SelectedLanguageProgress));
		_model.RaisePropertyChanged(nameof(SettingsViewModel.SelectedLanguageStatusText));
	}

	private void UpdateLanguageButtonText()
	{
		_model.LanguageButtonText = _model.SelectedTesseractLanguage?.IsInstalled == true
			? localizationProvider.GetString("LocSettingsDelete",         "Delete")
			: localizationProvider.GetString("LocSettingsOcrInstallLang", "Install");
		UpdateEasyOcrActionLabel();
	}

	private async Task ExecuteLanguageActionAsync()
	{
		var option = _model.SelectedTesseractLanguage;
		if (option == null)
		{
			return;
		}

		if (option.IsInstalled)
		{
			languageTesseractService.RemoveLanguage(option);
		}
		else if (option.CanDownload)
		{
			await languageTesseractService.DownloadAsync(option, new Progress<double>(p => option.Progress = p));
		}

		RefreshLanguageCommands();
	}

	private async Task BrowseFolderAsync(object? owner)
	{
		var picked = await folderPickerAdapter.PickFolderAsync(_model.DefaultSaveFolder, owner);
		if (!string.IsNullOrWhiteSpace(picked))
		{
			_model.DefaultSaveFolder = picked;
		}
	}

	private async Task SaveAsync()
	{
		var snapshot = BuildSettingsSnapshot();
		await settingsSystemService.SaveAsync(snapshot);
		_model.RaiseSettingsSaved(snapshot);
	}

	private AppSettings BuildSettingsSnapshot(string? propertyName = null)
	{
		ApplyModelToSettings(propertyName);
		return _settings;
	}

	private void OnPropertyChangedInternal(object? _, PropertyChangedEventArgs args)
	{
		if (_suspendSave || string.IsNullOrWhiteSpace(args.PropertyName))
		{
			return;
		}

		if (args.PropertyName == nameof(SettingsViewModel.SelectedTesseractLanguage))
		{
			if (_attachedLanguage != null)
			{
				_attachedLanguage.PropertyChanged -= LanguageChangedHandler;
			}

			_attachedLanguage = _model.SelectedTesseractLanguage;
			if (_attachedLanguage != null)
			{
				_attachedLanguage.PropertyChanged += LanguageChangedHandler;
			}

			_model.DownloadLanguageCommand?.NotifyCanExecuteChanged();
			_model.RemoveLanguageCommand?.NotifyCanExecuteChanged();
			_model.LanguageActionCommand?.NotifyCanExecuteChanged();
			RefreshLanguageCommands();
		}

		if (args.PropertyName.StartsWith("Show", StringComparison.OrdinalIgnoreCase) &&
			args.PropertyName.EndsWith("Icon", StringComparison.OrdinalIgnoreCase))
		{
			UpdateHiddenIconsFromStates();
		}

		switch (args.PropertyName)
		{
			case nameof(SettingsViewModel.EditorDefaultColorValue) when _suppressColorSync:
				return;
			case nameof(SettingsViewModel.EditorDefaultColorValue):
				_suppressColorSync        = true;
				_model.EditorDefaultColor = colorAdapter.ToHex(_model.EditorDefaultColorValue);
				_suppressColorSync        = false;
				break;
			case nameof(SettingsViewModel.EditorDefaultColor):
				_suppressColorSync             = true;
				_model.EditorDefaultColorValue = colorAdapter.CreateColor(_model.EditorDefaultColor);
				_suppressColorSync             = false;
				_model.EditorDefaultColorBrush = colorAdapter.CreateBrush(_model.EditorDefaultColor);
				break;
			case nameof(SettingsViewModel.IsEasyOcrInstalling):
				_model.ManageEasyOcrCommand?.NotifyCanExecuteChanged();
				break;
			case nameof(SettingsViewModel.DisplayLanguage):
				_model.RaiseLanguageChanged(string.IsNullOrWhiteSpace(_model.DisplayLanguage) ? LanguageCollection.DefaultOcrLanguageCode : _model.DisplayLanguage);
				break;
			case nameof(SettingsViewModel.Theme):
				_model.RaiseThemeChanged(_model.Theme);
				break;
		}

		var snapshot = BuildSettingsSnapshot(args.PropertyName);
		_model.RaiseLiveSettingsChanged(snapshot);
		_ = SaveSnapshotAsync(snapshot);
	}

	private async Task SaveSnapshotAsync(AppSettings snapshot)
	{
		await _saveGate.WaitAsync();
		try
		{
			await settingsSystemService.SaveAsync(snapshot);
			_model.RaiseSettingsSaved(snapshot);
		}
		finally
		{
			_saveGate.Release();
		}
	}
	private void RefreshLanguageCommands()
	{
		NotifyLanguageButtonPropertiesChanged();
		_model.DownloadLanguageCommand?.NotifyCanExecuteChanged();
		_model.RemoveLanguageCommand?.NotifyCanExecuteChanged();
		_model.LanguageActionCommand?.NotifyCanExecuteChanged();
	}

	private void HandleLanguageChanged()
	{
		BuildDisplayLanguageOptions();
		RefreshLanguageCommands();
		UpdateEasyOcrActionLabel();
	}

	private void ApplyModelToSettings(string? propertyName)
	{
		_settings.SettingsSchemaVersion ??= 4;

		var filter = propertyName is null
			? null
			: new HashSet<string>(StringComparer.OrdinalIgnoreCase)
			{
				propertyName
			};

		var trackedNames = propertyAdapter.GetPropertyNames<SettingsViewModel>(SettingsPropertyTypes.Tracked, true);
		if (propertyName == null || trackedNames.Contains(propertyName, StringComparer.OrdinalIgnoreCase))
		{
			propertyAdapter.ApplyProperties(_model, _settings, propertyName, SettingsPropertyTypes.Tracked, settingsProvider.ParseHotkey, true);
		}

		CopySimpleProperties(filter);

		if (filter is null ||
			propertyName is not null                                            &&
			propertyName.StartsWith("Show", StringComparison.OrdinalIgnoreCase) &&
			propertyName.EndsWith("Icon", StringComparison.OrdinalIgnoreCase))
		{
			_settings.EditorHiddenIcons = BuildHiddenIcons().ToList();
		}
	}

	private void CopySimpleProperties(HashSet<string>? filter)
	{
		var modelType    = typeof(SettingsViewModel);
		var settingsType = typeof(AppSettings);

		foreach (var name in propertyAdapter.GetPropertyNames<SettingsViewModel>(SettingsPropertyTypes.Simple))
		{
			if (filter != null && !filter.Contains(name))
			{
				continue;
			}

			var source = modelType.GetProperty(name, BindingFlags.Instance    | BindingFlags.Public);
			var target = settingsType.GetProperty(name, BindingFlags.Instance | BindingFlags.Public);
			if (source?.CanRead == true && target?.CanWrite == true)
			{
				target.SetValue(_settings, source.GetValue(_model));
			}
		}
	}

	private async Task ManageEasyOcrAsync()
	{
		if (_model.IsEasyOcrInstalling)
		{
			return;
		}

		_model.IsEasyOcrInstalling = true;
		var texts = localizationProvider.BuildEasyOcrStatusTexts();

		try
		{
			var status = new Progress<string>(s => _model.EasyOcrInstallStatus = s);

			if (_model is
				{
					IsEasyOcrInstalled: true, IsEasyOcrBroken: false
				})
			{
				_model.EasyOcrInstallStatus = texts.Removing;
				await maintenanceEasyOcrService.DeleteAsync(texts, status);
			}
			else
			{
				_model.EasyOcrInstallStatus = texts.InstallingPython;
				var runtime = await maintenanceEasyOcrService.EnsureRuntimeAsync(
					true,
					texts,
					status);
				if (!runtime.Success)
				{
					_model.IsEasyOcrInstalled   = false;
					_model.IsEasyOcrBroken      = true;
					_model.EasyOcrInstallStatus = runtime.Error ?? string.Format(texts.InstallFailedFormat, "Unknown error");
				}
				else
				{
					_model.EasyOcrInstallStatus = texts.Ready;
				}
			}

			await RefreshEasyOcrStateAsync();
		}
		catch (Exception ex)
		{
			_model.IsEasyOcrInstalled   = false;
			_model.IsEasyOcrBroken      = true;
			_model.EasyOcrInstallStatus = string.Format(texts.InstallFailedFormat, ex.GetBaseException().Message);
			await RefreshEasyOcrStateAsync();
		}
		finally
		{
			_model.IsEasyOcrInstalling = false;
			UpdateEasyOcrActionLabel();
		}
	}

	private async Task RefreshEasyOcrStateAsync()
	{
		var texts = localizationProvider.BuildEasyOcrStatusTexts();
		var state = await maintenanceEasyOcrService.GetStateAsync(texts);
		ApplyEasyOcrState(state);
	}

	private void ApplyEasyOcrState(EasyOcrViewState state)
	{
		_model.IsEasyOcrInstalled     = state.IsInstalled;
		_model.IsEasyOcrBroken        = state.IsBroken;
		_model.EasyOcrInstallStatus   = state.Status;
		_model.EasyOcrInstallProgress = state.Progress;
		UpdateEasyOcrActionLabel();
	}

	private void UpdateEasyOcrActionLabel()
	{
		if (_model.IsEasyOcrBroken)
		{
			_model.EasyOcrActionLabel = L(LocalizationKeys.CommonRepair, LocalizationFallbacks.Common.Repair);
			return;
		}

		_model.EasyOcrActionLabel = _model.IsEasyOcrInstalled
			? L(LocalizationKeys.CommonDelete,  LocalizationFallbacks.Common.Delete)
			: L(LocalizationKeys.CommonInstall, LocalizationFallbacks.Common.Install);
	}

	private async Task DownloadSelectedLanguageAsync()
	{
		var option = _model.SelectedTesseractLanguage;
		if (option is not
			{
				CanDownload: true
			})
		{
			return;
		}

		await languageTesseractService.DownloadAsync(option, new Progress<double>(p => option.Progress = p));
		_model.DownloadLanguageCommand?.NotifyCanExecuteChanged();
		_model.RemoveLanguageCommand?.NotifyCanExecuteChanged();
	}

	private void RemoveSelectedLanguage()
	{
		var option = _model.SelectedTesseractLanguage;
		if (option is not
			{
				IsInstalled: true
			})
		{
			return;
		}

		languageTesseractService.RemoveLanguage(option);
		_model.DownloadLanguageCommand?.NotifyCanExecuteChanged();
		_model.RemoveLanguageCommand?.NotifyCanExecuteChanged();
	}

	private string L(string key, string fallback)
	{
		return localizationProvider.GetString(key, fallback);
	}
}
