using Avalonia.Input;
using Shotora.App.Interfaces.PeripheralServices;
using Shotora.App.Models;
using KeyModifiers=Shared.Models.Enums.KeyModifiers;

namespace Shotora.App.Services.PeripheralServices;

public class KeyboardInteractionService : IKeyboardInteractionService
{
	public bool MatchesHotkey(HotkeySetting setting, KeyEventArgs e)
	{
		var vk = VirtualKeyFromKey(e.Key);
		if (vk == null || vk.Value != setting.Key)
		{
			return false;
		}

		return NormalizeModifiers(e.KeyModifiers) == NormalizeModifiers(setting.Modifiers);
	}
	private static int? VirtualKeyFromKey(Key key)
	{
		return key switch
		{
			Key.PrintScreen                   => 0x2C,
			Key.Space                         => 0x20,
			>= Key.D0 and <= Key.D9           => 0x30 + (key - Key.D0),
			>= Key.NumPad0 and <= Key.NumPad9 => 0x30 + (key - Key.NumPad0),
			>= Key.A and <= Key.Z             => 0x41 + (key - Key.A),
			>= Key.F1 and <= Key.F24          => 0x70 + (key - Key.F1),
			_                                 => null
		};
	}

	private static KeyModifiers NormalizeModifiers(KeyModifiers modifiers)
	{
		const KeyModifiers mask = KeyModifiers.Control | KeyModifiers.Shift | KeyModifiers.Alt | KeyModifiers.Win;
		return modifiers & mask;
	}

	private static KeyModifiers NormalizeModifiers(Avalonia.Input.KeyModifiers modifiers)
	{
		var result = KeyModifiers.None;
		if (modifiers.HasFlag(Avalonia.Input.KeyModifiers.Control))
		{
			result |= KeyModifiers.Control;
		}
		if (modifiers.HasFlag(Avalonia.Input.KeyModifiers.Shift))
		{
			result |= KeyModifiers.Shift;
		}
		if (modifiers.HasFlag(Avalonia.Input.KeyModifiers.Alt))
		{
			result |= KeyModifiers.Alt;
		}
		if (modifiers.HasFlag(Avalonia.Input.KeyModifiers.Meta))
		{
			result |= KeyModifiers.Win;
		}
		return NormalizeModifiers(result);
	}
}
