using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Shotora.App.Interfaces.PeripheralServices;
using Shotora.App.Interfaces.Providers;
using Shotora.App.Services.ViewModels;

namespace Shotora.App.Views;

[ExcludeFromCodeCoverage]
public partial class AboutWindow : Window
{
	private readonly EventHandler _languageHandler;

	private readonly ILocalizationProvider    _localizationProvider;
	private readonly IMouseInteractionService _settingsSystemService;

	public AboutWindow(ILocalizationProvider localizationProvider, IMouseInteractionService settingsSystemService)
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

	private void Link_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
	{
		if (e.InitialPressMouseButton != MouseButton.Left)
		{
			return;
		}

		if (sender is Control
			{
				Tag: string
				{
					Length: > 0
				} uri
			})
		{
			try
			{
				Process.Start(new ProcessStartInfo(uri)
				{
					UseShellExecute = true
				});
			}
			catch
			{
			}
		}

		e.Handled = true;
	}

	private void RefreshContent()
	{
		if (DataContext is AboutViewModel vm)
		{
			vm.Refresh();
		}
	}
}
