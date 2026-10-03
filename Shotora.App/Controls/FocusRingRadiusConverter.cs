using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace Shotora.App.Controls;

/// <summary>
///     Grows a control's <see cref="CornerRadius" /> by <see cref="Offset" /> so a focus ring drawn
///     <see cref="Offset" /> pixels outside the control (Margin="-3") stays concentric with it.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class FocusRingRadiusConverter : IValueConverter
{
	public double Offset { get; set; } = 3;

	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		return value is CornerRadius r
			? new CornerRadius(Grow(r.TopLeft), Grow(r.TopRight), Grow(r.BottomRight), Grow(r.BottomLeft))
			: new CornerRadius(Offset);
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		throw new NotSupportedException();
	}

	private double Grow(double radius)
	{
		return radius + Offset;
	}
}
