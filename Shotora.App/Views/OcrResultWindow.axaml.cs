using System.Diagnostics.CodeAnalysis;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Shotora.App.Views;

[ExcludeFromCodeCoverage]
public partial class OcrResultWindow : Window
{
	private Rect _anchor;

	public OcrResultWindow()
	{
		InitializeComponent();
		Opened += (_, _) => PositionNearAnchor();
	}

	public OcrResultWindow(string text, Rect anchorScreenBounds)
	{
		_anchor = anchorScreenBounds;
		InitializeComponent();
		ResultBox.Text =  text;
		Opened         += (_, _) => PositionNearAnchor();
	}

	public void UpdateResult(string text, Rect anchorScreenBounds)
	{
		_anchor        = anchorScreenBounds;
		ResultBox.Text = text;
		if (IsVisible)
		{
			PositionNearAnchor();
		}
	}

	private void PositionNearAnchor()
	{
		var height     = double.IsNaN(Height) || Height == 0 ? Bounds.Height : Height;
		var desiredTop = _anchor.Top - height - 8;
		if (desiredTop < 0)
		{
			desiredTop = _anchor.Bottom + 8;
		}

		Position = new PixelPoint((int)_anchor.Left, (int)desiredTop);
	}

	private async void Copy_Click(object? sender, RoutedEventArgs e)
	{
		var text = ResultBox.Text ?? string.Empty;
		if (string.IsNullOrEmpty(text))
		{
			return;
		}

		if (Clipboard != null)
		{
			await Clipboard.SetTextAsync(text);
		}
	}

	private void Close_Click(object? sender, RoutedEventArgs e)
	{
		Close();
	}
}
