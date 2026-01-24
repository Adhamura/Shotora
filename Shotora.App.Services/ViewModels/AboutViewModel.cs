using System.Reflection;
using Shotora.App.Interfaces.Providers;
using Shotora.App.Models.ViewModels;

namespace Shotora.App.Services.ViewModels;

public sealed class AboutViewModel : ViewModelBase, IDisposable
{
	private readonly ILocalizationProvider _localizationProvider;

	public AboutViewModel(ILocalizationProvider localizationProvider)
	{
		_localizationProvider                 =  localizationProvider;
		_localizationProvider.LanguageChanged += OnLanguageChanged;
	}

	public static string Version => GetVersion();

	public void Dispose()
	{
		_localizationProvider.LanguageChanged -= OnLanguageChanged;
	}

	public void Refresh()
	{
		OnPropertyChanged(string.Empty);
		OnPropertyChanged(string.Empty);
	}

	private void OnLanguageChanged(object? sender, EventArgs e)
	{
		Refresh();
	}

	private static string GetVersion()
	{
		var version = Assembly.GetExecutingAssembly().GetName().Version;
		return version == null ? "1.0.0" : $"{version.Major}.{version.Minor}.{version.Build}";
	}
}
