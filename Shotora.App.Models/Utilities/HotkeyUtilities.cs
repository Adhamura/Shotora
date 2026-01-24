using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;

namespace Shotora.App.Models.Utilities;

[ExcludeFromCodeCoverage]
public static class HotkeyUtilities
{
	private static readonly FrozenDictionary<string, int> NameToKey = BuildKeyLookup();

	public static string GetKeyName(int keyCode)
	{
		switch (keyCode)
		{
			case 0x2C:
				return "PrintScreen";
			case >= 0x30 and <= 0x39:
			case >= 0x41 and <= 0x5A:
				return ((char)keyCode).ToString();
			case >= 0x70 and <= 0x87:
			{
				var offset = keyCode - 0x6F;
				return $"F{offset}";
			}
			default:
				return $"0x{keyCode:X2}";
		}
	}

	public static int? KeyFromName(string name)
	{
		if (string.IsNullOrWhiteSpace(name))
		{
			return null;
		}

		if (NameToKey.TryGetValue(name.Trim().ToLowerInvariant(), out var key))
		{
			return key;
		}

		return null;
	}

	private static FrozenDictionary<string, int> BuildKeyLookup()
	{
		var map = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
		{
			["printscreen"] = 0x2C,
			["snapshot"]    = 0x2C,
			["prtsc"]       = 0x2C,
			["prtscr"]      = 0x2C,
			["prtscn"]      = 0x2C,
			["space"]       = 0x20,
			["spacebar"]    = 0x20
		};

		for (var code = 0x30; code <= 0x39; code++)
		{
			var c = (char)code;
			map[c.ToString()] = code;
		}

		for (var code = 0x41; code <= 0x5A; code++)
		{
			var c = (char)code;
			map[c.ToString()] = code;
		}

		for (var i = 1; i <= 24; i++)
		{
			map[$"f{i}"] = 0x6F + i;
		}

		return map.ToFrozenDictionary();
	}
}
