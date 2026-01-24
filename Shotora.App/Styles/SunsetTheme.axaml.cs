using System.Diagnostics.CodeAnalysis;
using Avalonia.Markup.Xaml;

namespace Shotora.App.Styles;

[ExcludeFromCodeCoverage]
public class SunsetTheme : Avalonia.Styling.Styles
{
	public SunsetTheme()
	{
		InitializeComponent();
	}

	private void InitializeComponent()
	{
		AvaloniaXamlLoader.Load(this);
	}
}
