using Avalonia.Media;
using Shotora.App.Interfaces.Adapters;

namespace Shotora.App.Services.Adapters;

public class ColorAdapter : IColorAdapter
{
	public object? CreateColor(string hex)
	{
		try
		{
			return Color.Parse(hex);
		}
		catch
		{
			return null;
		}
	}

	public object? CreateBrush(string hex)
	{
		var color = CreateColor(hex);
		if (color is Color c)
		{
			return new SolidColorBrush(c);
		}

		return null;
	}

	public string ToHex(object? color)
	{
		color = color switch
		{
			ISolidColorBrush brush => brush.Color,
			_                      => color
		};

		if (color is Color c)
		{
			return $"#{c.A:X2}{c.R:X2}{c.G:X2}{c.B:X2}";
		}

		return "#FF00FF00";
	}
}
