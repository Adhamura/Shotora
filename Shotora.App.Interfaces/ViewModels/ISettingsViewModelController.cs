using Shotora.App.Models;
using Shotora.App.Models.ViewModels;

namespace Shotora.App.Interfaces.ViewModels;

public interface ISettingsViewModelController
{
	void Init();

	Task LoadAsync(AppSettings? settings = null);

	Task SaveNowAsync();
	void SettingsHandlersAttach(Action<SettingsViewModel> action);
}
