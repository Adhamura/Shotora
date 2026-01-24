using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Shotora.App.Interfaces.PeripheralServices;
using Shotora.App.Interfaces.Providers;
using Shotora.App.Models.ViewModels;

namespace Shotora.App.Views;

[ExcludeFromCodeCoverage]
public partial class MainWindow : Window
{
	private readonly EventHandler             _languageHandler;
	private readonly ILocalizationProvider    _localizationProvider;
	private readonly IMouseInteractionService _settingsSystemService;

	public MainWindow(ILocalizationProvider localizationProvider, IMouseInteractionService settingsSystemService)
	{
		_localizationProvider  = localizationProvider;
		_settingsSystemService = settingsSystemService;
		InitializeComponent();
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

	private void ShellBorder_OnPointerPressed(object? sender, PointerPressedEventArgs e)
	{
		if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
		{
			var edge = _settingsSystemService.GetResizeEdge(this, e.GetPosition(this));
			if (edge.HasValue)
			{
				BeginResizeDrag(edge.Value, e);
				e.Handled = true;
				return;
			}

			BeginMoveDrag(e);
		}
	}

	private void ShellBorder_OnPointerMoved(object? sender, PointerEventArgs e)
	{
		var edge = _settingsSystemService.GetResizeEdge(this, e.GetPosition(this));
		Cursor = edge.HasValue ? new Cursor(_settingsSystemService.GetResizeCursor(edge.Value)) : null;
	}

	private void ShellBorder_OnPointerLeave(object? sender, PointerEventArgs e)
	{
		Cursor = null;
	}

	private void Close_Click(object? sender, RoutedEventArgs e)
	{
		Close();
	}
}
