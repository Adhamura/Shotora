using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Shotora.App.Models.Enums;

namespace Shotora.App.Models;

[ExcludeFromCodeCoverage]
public static class EditorIconExtensions
{
	public static string ToDescriptionString(this EditorIcon icon)
	{
		var member = icon.GetType().GetField(icon.ToString());
		if (member == null)
		{
			return string.Empty;
		}

		if (member.GetCustomAttributes(typeof(DescriptionAttribute), false) is DescriptionAttribute[]
			{
				Length: > 0
			} attributes)
		{
			return attributes[0].Description;
		}

		return string.Empty;
	}
}
