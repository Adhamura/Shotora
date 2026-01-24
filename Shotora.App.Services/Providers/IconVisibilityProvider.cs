using System.Collections.Frozen;
using Shotora.App.Interfaces.Providers;
using Shotora.App.Models;
using Shotora.App.Models.Enums;

namespace Shotora.App.Services.Providers;

public class IconVisibilityProvider : IIconVisibilityProvider
{
	public IReadOnlyDictionary<EditorIcon, bool> BuildVisibilityMap(IEnumerable<string>? hiddenIconIds)
	{
		var hidden = new HashSet<string>(
			(hiddenIconIds ?? []).Select(static id => id?.Trim() ?? string.Empty),
			StringComparer.OrdinalIgnoreCase);

		var map = new Dictionary<EditorIcon, bool>();
		foreach (var icon in Enum.GetValues<EditorIcon>())
		{
			map[icon] = !hidden.Contains(GetIconId(icon));
		}

		return map.ToFrozenDictionary();
	}

	public IEnumerable<string> BuildHiddenIconIds(IReadOnlyDictionary<EditorIcon, bool> visibility)
	{
		foreach (var (icon, isVisible) in visibility)
		{
			if (!isVisible)
			{
				yield return GetIconId(icon);
			}
		}
	}

	private static string GetIconId(EditorIcon icon)
	{
		var description = icon.ToDescriptionString();
		return string.IsNullOrWhiteSpace(description) ? icon.ToString() : description;
	}
}
