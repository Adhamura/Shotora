using Shotora.App.Interfaces.Providers;
using Shotora.App.Models.ViewModels;

namespace Shotora.App.Services.ViewModels;

public sealed class AboutViewModel : ViewModelBase, IDisposable
{
	private readonly IAppVersionProvider   _appVersionProvider;
	private readonly ILocalizationProvider _localizationProvider;

	public AboutViewModel(ILocalizationProvider localizationProvider, IAppVersionProvider appVersionProvider, UpdateViewModel updates)
	{
		_localizationProvider                 =  localizationProvider;
		_appVersionProvider                   =  appVersionProvider;
		Updates                               =  updates;
		_localizationProvider.LanguageChanged += OnLanguageChanged;
	}

	public string Version => _appVersionProvider.Version;

	/// <summary>State and commands of the "Updates" card.</summary>
	public UpdateViewModel Updates { get; }

	public void Dispose()
	{
		_localizationProvider.LanguageChanged -= OnLanguageChanged;
		Updates.Dispose();
	}

	public void Refresh()
	{
		OnPropertyChanged(string.Empty);
		Updates.Refresh();
	}

	private void OnLanguageChanged(object? sender, EventArgs e)
	{
		Refresh();
	}
}
