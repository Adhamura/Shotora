using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Shotora.App.Views;

[ExcludeFromCodeCoverage]
public partial class OcrResultWindow : Window
{
	private static readonly TimeSpan CopiedFeedbackDuration = TimeSpan.FromSeconds(1.5);

	/// <summary>Selection bounds on screen, in physical pixels.</summary>
	private Rect _anchor;

	private int _copyFeedbackVersion;

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
		// Window sizes are DIPs, screen positions are physical pixels.
		var scale        = RenderScaling;
		var heightDips   = double.IsNaN(Height) || Height == 0 ? Bounds.Height : Height;
		var heightPixels = heightDips * scale;
		var gap          = 8 * scale;

		var desiredTop = _anchor.Top - heightPixels - gap;
		if (desiredTop < 0)
		{
			desiredTop = _anchor.Bottom + gap;
		}

		Position = new PixelPoint((int)_anchor.Left, (int)desiredTop);
	}

	private async void Copy_Click(object? sender, RoutedEventArgs e)
	{
		var text = ResultBox.Text ?? string.Empty;
		if (string.IsNullOrEmpty(text) || Clipboard == null)
		{
			return;
		}

		try
		{
			await Clipboard.SetTextAsync(text);
			await ShowCopiedFeedbackAsync();
		}
		catch (Exception)
		{
			// Clipboard access can fail (e.g. another process holds it); the window stays open so the user can retry.
		}
	}

	/// <summary>Briefly swaps the Copy label for the localized "Copied to clipboard" confirmation.</summary>
	private async Task ShowCopiedFeedbackAsync()
	{
		var version = ++_copyFeedbackVersion;
		CopyButton.Content = this.FindResource("LocNotificationCopySuccess") ?? CopyButton.Content;

		await Task.Delay(CopiedFeedbackDuration);

		if (version == _copyFeedbackVersion)
		{
			CopyButton.Bind(ContentControl.ContentProperty, this.GetResourceObservable("LocToolCopy"));
		}
	}

	private void Close_Click(object? sender, RoutedEventArgs e)
	{
		Close();
	}
}
