using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Color=Avalonia.Media.Color;

namespace Shotora.App.Models.Utilities;

[ExcludeFromCodeCoverage]
public static class ColorConversions
{
	public static (double H, double S, double V, double A) RgbToHsv(Color color)
	{
		var r = color.R / 255.0;
		var g = color.G / 255.0;
		var b = color.B / 255.0;
		var a = color.A / 255.0;

		var max   = Math.Max(r, Math.Max(g, b));
		var min   = Math.Min(r, Math.Min(g, b));
		var delta = max - min;

		double h = 0;
		if (delta > 0)
		{
			if (max == r)
			{
				h = (g - b) / delta % 6;
				if (h < 0)
				{
					h += 6;
				}
			}
			else if (max == g)
			{
				h = (b - r) / delta + 2;
			}
			else
			{
				h = (r - g) / delta + 4;
			}

			h /= 6;
		}

		var s = max == 0 ? 0 : delta / max;
		var v = max;

		return (h, s, v, a);
	}

	public static Color HsvToRgb(double h, double s, double v, double a = 1.0)
	{
		h = Math.Clamp(h, 0, 1) * 360;
		s = Math.Clamp(s, 0, 1);
		v = Math.Clamp(v, 0, 1);
		a = Math.Clamp(a, 0, 1);

		var c = v * s;
		var x = c * (1 - Math.Abs(h / 60 % 2 - 1));
		var m = v - c;

		double r, g, b;

		switch ((int)(h / 60))
		{
			case 0:
				r = c;
				g = x;
				b = 0;
				break;
			case 1:
				r = x;
				g = c;
				b = 0;
				break;
			case 2:
				r = 0;
				g = c;
				b = x;
				break;
			case 3:
				r = 0;
				g = x;
				b = c;
				break;
			case 4:
				r = x;
				g = 0;
				b = c;
				break;
			default:
				r = c;
				g = 0;
				b = x;
				break;
		}

		return Color.FromArgb(
			(byte)(a       * 255),
			(byte)((r + m) * 255),
			(byte)((g + m) * 255),
			(byte)((b + m) * 255)
		);
	}

	public static Color ColorFromHue(double hue)
	{
		return HsvToRgb(hue, 1, 1);
	}

	public static bool TryParseHex(string? hex, out Color color)
	{
		color = default;

		if (string.IsNullOrWhiteSpace(hex))
		{
			return false;
		}

		hex = hex.Trim();
		if (hex.StartsWith('#'))
		{
			hex = hex[1..];
		}

		try
		{
			switch (hex.Length)
			{
				case 3:
					var r3 = byte.Parse($"{hex[0]}{hex[0]}", NumberStyles.HexNumber);
					var g3 = byte.Parse($"{hex[1]}{hex[1]}", NumberStyles.HexNumber);
					var b3 = byte.Parse($"{hex[2]}{hex[2]}", NumberStyles.HexNumber);
					color = Color.FromRgb(r3, g3, b3);
					return true;

				case 4:
					var a4 = byte.Parse($"{hex[0]}{hex[0]}", NumberStyles.HexNumber);
					var r4 = byte.Parse($"{hex[1]}{hex[1]}", NumberStyles.HexNumber);
					var g4 = byte.Parse($"{hex[2]}{hex[2]}", NumberStyles.HexNumber);
					var b4 = byte.Parse($"{hex[3]}{hex[3]}", NumberStyles.HexNumber);
					color = Color.FromArgb(a4, r4, g4, b4);
					return true;

				case 6:
					var r6 = byte.Parse(hex[..2],  NumberStyles.HexNumber);
					var g6 = byte.Parse(hex[2..4], NumberStyles.HexNumber);
					var b6 = byte.Parse(hex[4..6], NumberStyles.HexNumber);
					color = Color.FromRgb(r6, g6, b6);
					return true;

				case 8:
					var a8 = byte.Parse(hex[..2],  NumberStyles.HexNumber);
					var r8 = byte.Parse(hex[2..4], NumberStyles.HexNumber);
					var g8 = byte.Parse(hex[4..6], NumberStyles.HexNumber);
					var b8 = byte.Parse(hex[6..8], NumberStyles.HexNumber);
					color = Color.FromArgb(a8, r8, g8, b8);
					return true;

				default:
					return false;
			}
		}
		catch
		{
			return false;
		}
	}

	public static string ToHex(Color color, bool includeAlpha = false)
	{
		return includeAlpha
			? $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}"
			: $"#{color.R:X2}{color.G:X2}{color.B:X2}";
	}

	public static byte ClampByte(int value)
	{
		return (byte)Math.Clamp(value, 0, 255);
	}

	public static bool TryParseByte(string? text, out byte value)
	{
		value = 0;
		if (string.IsNullOrWhiteSpace(text))
		{
			return false;
		}

		if (int.TryParse(text.Trim(), out var intVal) && intVal is >= 0 and <= 255)
		{
			value = (byte)intVal;
			return true;
		}

		return false;
	}
}
