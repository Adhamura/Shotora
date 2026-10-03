using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Shotora.App.Interfaces.PeripheralServices;
using Shotora.App.Interfaces.Providers;
using Shotora.App.Models.ViewModels;

namespace Shotora.App.Views;

[ExcludeFromCodeCoverage]
public partial class MainWindow : Window
{
	private readonly EventHandler          _languageHandler;
	private readonly ILocalizationProvider _localizationProvider;

	public MainWindow(ILocalizationProvider localizationProvider, IMouseInteractionService mouseInteractionService)
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
		base.OnClosed(e);
	}

	private void RefreshContent()
	{
		if (DataContext is MainWindowViewModel vm)
		{
			vm.Refresh();
		}
	}

	private void Close_Click(object? sender, RoutedEventArgs e)
	{
		Close();
	}
}
