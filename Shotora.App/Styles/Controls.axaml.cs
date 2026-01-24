using System.Diagnostics.CodeAnalysis;
using Avalonia.Markup.Xaml;

namespace Shotora.App.Styles;

[ExcludeFromCodeCoverage]
public class Controls : Avalonia.Styling.Styles
{
	public Controls()
	{
		InitializeComponent();
	}

	private void InitializeComponent()
	{
		AvaloniaXamlLoader.Load(this);
	}
}
