using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace Shotora.App.Views;

[ExcludeFromCodeCoverage]
public partial class TextInputWindow : Window
{
	private static readonly string[] sourceArray = ["Segoe UI", "Arial", "Calibri", "Consolas", "Tahoma", "Times New Roman"];

	public TextInputWindow() : this(null)
	{
	}

	public TextInputWindow(string? initialText = null, string? fontFamily = null, double? fontSize = null, bool bold = true, bool italic = false)
	{
		InitializeComponent();

		var fonts = sourceArray.ToList();
		FontFamilyBox.ItemsSource = fonts;
		FontFamilyBox.SelectedItem = !string.IsNullOrWhiteSpace(fontFamily) && fonts.Contains(fontFamily)
			? fontFamily
			: fonts.FirstOrDefault() ?? "Segoe UI";

		FontSizeBox.Text       = (fontSize ?? 16).ToString("0");
		BoldToggle.IsChecked   = bold;
		ItalicToggle.IsChecked = italic;
		TextBox.Text           = initialText ?? string.Empty;
		TextBox.CaretIndex     = TextBox.Text?.Length ?? 0;
		UpdatePreview();

		TextBox.PropertyChanged        += (_, _) => UpdatePreview();
		FontFamilyBox.SelectionChanged += (_, _) => UpdatePreview();
		FontSizeBox.PropertyChanged    += (_, _) => UpdatePreview();
		BoldToggle.IsCheckedChanged    += (_, _) => UpdatePreview();
		ItalicToggle.IsCheckedChanged  += (_, _) => UpdatePreview();

		// Enter adds a line break in the multi-line box, so Ctrl+Enter is the keyboard shortcut for the primary action.
		TextBox.AddHandler(KeyDownEvent, TextBox_OnKeyDown, RoutingStrategies.Tunnel);
		Opened += (_, _) => TextBox.Focus();
	}

	public string TextValue => TextBox.Text ?? string.Empty;

	public string FontFamilyValue => FontFamilyBox.SelectedItem as string ?? "Segoe UI";

	public double FontSizeValue
	{
		get
		{
			if (double.TryParse(FontSizeBox.Text, out var size) && size > 0)
			{
				return size;
			}
			return 16;
		}
	}

	public bool IsBold => BoldToggle.IsChecked == true;

	public bool IsItalic => ItalicToggle.IsChecked == true;

	private void TextBox_OnKeyDown(object? sender, KeyEventArgs e)
	{
		if (e.Key == Key.Enter && e.KeyModifiers.HasFlag(KeyModifiers.Control))
		{
			e.Handled = true;
			Close(true);
		}
	}

	private void Ok_Click(object? sender, RoutedEventArgs e)
	{
		Close(true);
	}

	private void Cancel_Click(object? sender, RoutedEventArgs e)
	{
		Close(false);
	}

	private void UpdatePreview()
	{
		// With no text yet, preview the font itself by showing its name.
		PreviewText.Text       = string.IsNullOrWhiteSpace(TextBox.Text) ? FontFamilyValue : TextBox.Text;
		PreviewText.FontFamily = new FontFamily(FontFamilyValue);
		PreviewText.FontSize   = FontSizeValue;
		PreviewText.FontWeight = IsBold ? FontWeight.Bold : FontWeight.Normal;
		PreviewText.FontStyle  = IsItalic ? FontStyle.Italic : FontStyle.Normal;
	}
}
