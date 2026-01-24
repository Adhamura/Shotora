using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Shotora.App.Controls;
using Shotora.App.Interfaces.PeripheralServices;
using Shotora.App.Interfaces.ViewModels;
using Shotora.App.Models.Utilities;
using Shotora.App.Models.ViewModels;

namespace Shotora.App.Views;

[ExcludeFromCodeCoverage]
public partial class SettingsWindow(ISettingsViewModelController settingsViewModelController, IMouseInteractionService settingsSystemService) : Window
{
	private bool _allowClose;
	private bool _closingScheduled;

	public void InitializeAsync()
	{
		InitializeComponent();
		settingsViewModelController.Init();
		settingsViewModelController.SettingsHandlersAttach(v => DataContext ??= v);
		AddHandler(SearchableComboBox.SelectionChangedEvent, OnComboBoxSelectionChanged);
	}

	private void OnComboBoxSelectionChanged(object? sender, SelectionChangedEventArgs e)
	{
		if (DataContext is SettingsViewModel vm && e.AddedItems.Count > 0)
		{
			if (e.AddedItems[0] is TesseractLanguageOption option)
			{
				vm.SelectedTesseractLanguage = option;
			}
			vm.SaveCommand?.Execute(null);
		}
	}

	protected override void OnClosing(WindowClosingEventArgs e)
	{
		if (_allowClose)
		{
			base.OnClosing(e);
			return;
		}

		e.Cancel = true;
		if (_closingScheduled)
		{
			return;
		}

		_closingScheduled = true;
		_                 = SaveAndCloseAsync();
	}

	private void Close_Click(object? sender, RoutedEventArgs e)
	{
		Close();
	}

	private void ShellBorder_OnPointerPressed(object? sender, PointerPressedEventArgs e)
	{
		if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
		{
			var edge = settingsSystemService.GetResizeEdge(this, e.GetPosition(this));
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
		var edge = settingsSystemService.GetResizeEdge(this, e.GetPosition(this));
		Cursor = edge.HasValue ? new Cursor(settingsSystemService.GetResizeCursor(edge.Value)) : null;
	}

	private void ShellBorder_OnPointerLeave(object? sender, PointerEventArgs e)
	{
		Cursor = null;
	}

	private void DefaultColorPick_Click(object? sender, RoutedEventArgs e)
	{
		DefaultColorPopup.IsOpen = !DefaultColorPopup.IsOpen;
	}

	private async Task SaveAndCloseAsync()
	{
		try
		{
			await settingsViewModelController.SaveNowAsync();
		}
		finally
		{
			_allowClose       = true;
			_closingScheduled = false;
			Close();
		}
	}
}
