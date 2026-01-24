using Shotora.App.Models;

namespace Shotora.App.Interfaces.System;

public interface ISettingsSystemService
{
	event EventHandler<AppSettings>? SettingsChanged;

	Task<AppSettings> LoadAsync();

	Task SaveAsync(AppSettings settings);
}
