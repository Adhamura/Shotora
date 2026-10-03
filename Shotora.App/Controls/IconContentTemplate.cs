using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Shapes;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Media;

namespace Shotora.App.Controls;

/// <summary>
///     Renders <see cref="Geometry" /> content (the <c>IconXxx</c> resources in Styles/Icons.axaml) as a uniformly scaled icon
///     filled with the hosting presenter's <see cref="ContentPresenter.Foreground" />, so icons follow the button's
///     hover/checked/disabled foreground. Any other content falls through to the default templates.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class IconContentTemplate : IDataTemplate
{
	/// <summary>Upper bound for the rendered icon so a button without a fixed size never grows to the geometry's native size.</summary>
	public double MaxIconSize { get; set; } = 24;

	public bool Match(object? data)
	{
		return data is Geometry;
	}

	public Control? Build(object? param)
	{
		var path = new Path
		{
			Data    = param as Geometry,
			Stretch = Stretch.Uniform
		};
		path.Bind(Shape.FillProperty, new Binding(nameof(ContentPresenter.Foreground))
		{
			RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor)
			{
				AncestorType = typeof(ContentPresenter)
			}
		});

		return new Viewbox
		{
			Stretch   = Stretch.Uniform,
			MaxWidth  = MaxIconSize,
			MaxHeight = MaxIconSize,
			Child     = path
		};
	}
}
