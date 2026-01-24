using System.Diagnostics.CodeAnalysis;
using Shared.Models.Enums;

namespace Shotora.App.Models;

[ExcludeFromCodeCoverage]
public class HotkeySetting
{
	public KeyModifiers Modifiers { get; init; }

	public int Key { get; init; }

	public static HotkeySetting RegionDefault()
	{
		return new HotkeySetting
		{
			Modifiers = KeyModifiers.Control | KeyModifiers.Shift,
			Key       = 0x2C
		};
	}

	public static HotkeySetting FullscreenDefault()
	{
		return new HotkeySetting
		{
			Modifiers = KeyModifiers.Alt,
			Key       = 0x2C
		};
	}

	public static HotkeySetting ActiveWindowDefault()
	{
		return new HotkeySetting
		{
			Modifiers = KeyModifiers.Shift | KeyModifiers.Win,
			Key       = 0x53
		};
	}
}
