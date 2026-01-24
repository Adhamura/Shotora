using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Shotora.App.Views;

[ExcludeFromCodeCoverage]
public partial class TrayMenuWindow : Window
{
	public TrayMenuWindow()
	{
		InitializeComponent();
		Deactivated += (_, _) => Hide();
	}

	private void CaptureRegionButton_OnClick(object? sender, RoutedEventArgs e)
	{
		Hide();
	}

	private void CaptureFullButton_OnClick(object? sender, RoutedEventArgs e)
	{
		Hide();
	}

	private void SettingsButton_OnClick(object? sender, RoutedEventArgs e)
	{
		Hide();
	}

	private void AboutButton_OnClick(object? sender, RoutedEventArgs e)
	{
		Hide();
	}

	private void ExitButton_OnClick(object? sender, RoutedEventArgs e)
	{
		Hide();
	}
}
