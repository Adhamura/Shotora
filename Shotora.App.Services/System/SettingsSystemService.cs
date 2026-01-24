using System.Text.Json;
using Shared.Interfaces.Facades;
using Shotora.App.Interfaces.System;
using Shotora.App.Models;
using KeyModifiers=Shared.Models.Enums.KeyModifiers;

namespace Shotora.App.Services.System;

public class SettingsSystemService : ISettingsSystemService
{
	private const int CurrentSchemaVersion = 3;

	private readonly string             _configPath;
	private readonly IEnvironmentFacade _environment;
	private readonly IFileFacade        _fileFacade;
	private readonly JsonSerializerOptions _options = new()
	{
		WriteIndented = true
	};

	public SettingsSystemService(IFileFacade fileFacade, IEnvironmentFacade environment)
	{
		_fileFacade  = fileFacade;
		_environment = environment;

		var folder = Path.Combine(_environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Shotora");
		_fileFacade.CreateDirectory(folder);
		_configPath = Path.Combine(folder, "config.json");
	}

	public event EventHandler<AppSettings>? SettingsChanged;

	public async Task<AppSettings> LoadAsync()
	{
		AppSettings? settings = null;
		try
		{
			if (_fileFacade.FileExists(_configPath))
			{
				await using var stream = _fileFacade.OpenRead(_configPath);
				settings = await JsonSerializer.DeserializeAsync<AppSettings>(stream, _options);
			}
		}
		catch
		{
		}

		if (settings != null)
		{
			var needsSave = false;

			if (settings.SettingsSchemaVersion is null or < CurrentSchemaVersion)
			{
				const int printScreenKey = 0x2C;

				var legacyRegion  = (Modifiers: KeyModifiers.None, Key: printScreenKey);
				var legacyFull    = (Modifiers: KeyModifiers.Control | KeyModifiers.Alt, Key: 0x70);
				var legacyActive  = (Modifiers: KeyModifiers.Control | KeyModifiers.Alt, Key: 0x71);
				var currentRegion = (settings.RegionHotkey.Modifiers, settings.RegionHotkey.Key);
				var currentFull   = (settings.FullscreenHotkey.Modifiers, settings.FullscreenHotkey.Key);
				var currentActive = (settings.ActiveWindowHotkey.Modifiers, settings.ActiveWindowHotkey.Key);

				if (currentRegion == legacyRegion)
				{
					settings.RegionHotkey = HotkeySetting.RegionDefault();
				}

				if (currentFull == legacyFull)
				{
					settings.FullscreenHotkey = HotkeySetting.FullscreenDefault();
				}

				if (currentActive == legacyActive)
				{
					settings.ActiveWindowHotkey = HotkeySetting.ActiveWindowDefault();
				}

				settings.SettingsSchemaVersion = CurrentSchemaVersion;
				needsSave                      = true;
			}

			if (needsSave)
			{
				await SaveAsync(settings);
			}

			return settings;
		}

		return new AppSettings();
	}

	public async Task SaveAsync(AppSettings settings)
	{
		try
		{
			var folder = _fileFacade.GetDirectoryName(_configPath);
			if (!string.IsNullOrWhiteSpace(folder))
			{
				_fileFacade.CreateDirectory(folder);
			}

			await using var stream = new FileStream(_configPath, FileMode.Create, FileAccess.Write, FileShare.Read);
			await JsonSerializer.SerializeAsync(stream, settings, _options);
		}
		catch
		{
			return;
		}

		SettingsChanged?.Invoke(this, settings);
	}
}
