using System.Diagnostics.CodeAnalysis;
using Avalonia.Markup.Xaml;

namespace Shotora.App.Styles;

[ExcludeFromCodeCoverage]
public class LightTheme : Avalonia.Styling.Styles
{
	public LightTheme()
	{
		InitializeComponent();
	}

	private void InitializeComponent()
	{
		AvaloniaXamlLoader.Load(this);
	}
}
