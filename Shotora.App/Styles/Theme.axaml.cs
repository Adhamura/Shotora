using System.Diagnostics.CodeAnalysis;
using Avalonia.Markup.Xaml;

namespace Shotora.App.Styles;

[ExcludeFromCodeCoverage]
public class Theme : Avalonia.Styling.Styles
{
	public Theme()
	{
		InitializeComponent();
	}

	private void InitializeComponent()
	{
		AvaloniaXamlLoader.Load(this);
	}
}
