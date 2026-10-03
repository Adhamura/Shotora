using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Shotora.App.Interfaces.PeripheralServices;
using Shotora.App.Interfaces.Providers;
using Shotora.App.Services.ViewModels;

namespace Shotora.App.Views;

[ExcludeFromCodeCoverage]
public partial class AboutWindow : Window
{
	private readonly EventHandler _languageHandler;

	private readonly ILocalizationProvider _localizationProvider;

	public AboutWindow(ILocalizationProvider localizationProvider, IMouseInteractionService mouseInteractionService)
	{
		_localizationProvider = localizationProvider;
		InitializeComponent();
		Shell.ResizeService                   =  mouseInteractionService;
		_languageHandler                      =  (_, _) => RefreshContent();
		_localizationProvider.LanguageChanged += _languageHandler;
	}

	protected override void OnClosed(EventArgs e)
	{
		_localizationProvider.LanguageChanged -= _languageHandler;

		if (DataContext is IDisposable disposable)
		{
			disposable.Dispose();
		}

		base.OnClosed(e);
	}

	private void Close_Click(object? sender, RoutedEventArgs e)
	{
		Close();
	}

	private void RefreshContent()
	{
		if (DataContext is AboutViewModel vm)
		{
			vm.Refresh();
		}
	}
}
