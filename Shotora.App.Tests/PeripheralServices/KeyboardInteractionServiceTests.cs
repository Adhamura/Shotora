using Avalonia.Input;
using Shotora.App.Models;
using Shotora.App.Services.PeripheralServices;
using KeyModifiers=Shared.Models.Enums.KeyModifiers;

namespace Shotora.App.Tests.PeripheralServices;

public class KeyboardInteractionServiceTests
{
	private readonly KeyboardInteractionService _sut = new();

	[Fact]
	public void Given_MatchingKeyAndModifiers_When_MatchesHotkey_Then_ReturnsTrue()
	{
		var hotkey = new HotkeySetting
		{
			Modifiers = KeyModifiers.Control | KeyModifiers.Shift,
			Key       = 0x41
		};
		var args = CreateKeyEvent(Key.A, KeyModifiers.Control | KeyModifiers.Shift);

		var result = _sut.MatchesHotkey(hotkey, args);

		Assert.True(result);
	}

	[Fact]
	public void Given_DifferentKey_When_MatchesHotkey_Then_ReturnsFalse()
	{
		var hotkey = new HotkeySetting
		{
			Modifiers = KeyModifiers.Control,
			Key       = 0x41
		};
		var args = CreateKeyEvent(Key.B, KeyModifiers.Control);

		var result = _sut.MatchesHotkey(hotkey, args);

		Assert.False(result);
	}

	[Fact]
	public void Given_MismatchedModifiers_When_MatchesHotkey_Then_ReturnsFalse()
	{
		var hotkey = new HotkeySetting
		{
			Modifiers = KeyModifiers.Control | KeyModifiers.Alt,
			Key       = 0x2C
		};
		var args = CreateKeyEvent(Key.PrintScreen, KeyModifiers.Control);

		var result = _sut.MatchesHotkey(hotkey, args);

		Assert.False(result);
	}

	[Fact]
	public void Given_ExtraUnknownModifierBits_When_MatchesHotkey_Then_IgnoresUnknownAndMatches()
	{
		var hotkey = new HotkeySetting
		{
			Modifiers = KeyModifiers.Control | (KeyModifiers)0x40,
			Key       = 0x44
		};
		var args = CreateKeyEvent(Key.D, KeyModifiers.Control);

		var result = _sut.MatchesHotkey(hotkey, args);

		Assert.True(result);
	}

	[Fact]
	public void Given_NumpadKey_When_MatchesHotkey_Then_MapsToDigitVirtualKey()
	{
		var hotkey = new HotkeySetting
		{
			Modifiers = KeyModifiers.Shift,
			Key       = 0x33
		};
		var args = CreateKeyEvent(Key.NumPad3, KeyModifiers.Shift);

		var result = _sut.MatchesHotkey(hotkey, args);

		Assert.True(result);
	}

	[Fact]
	public void Given_UnmappedKey_When_MatchesHotkey_Then_ReturnsFalse()
	{
		var hotkey = new HotkeySetting
		{
			Modifiers = KeyModifiers.None,
			Key       = 0x30
		};
		var args = CreateKeyEvent(Key.System, KeyModifiers.None);

		var result = _sut.MatchesHotkey(hotkey, args);

		Assert.False(result);
	}

	[Fact]
	public void Given_AltModifier_When_MatchesHotkey_Then_ReturnsTrue()
	{
		var hotkey = new HotkeySetting
		{
			Modifiers = KeyModifiers.Alt,
			Key       = 0x45
		};
		var args = CreateKeyEvent(Key.E, KeyModifiers.Alt);

		var result = _sut.MatchesHotkey(hotkey, args);

		Assert.True(result);
	}

	[Fact]
	public void Given_WinModifier_When_MatchesHotkey_Then_ReturnsTrue()
	{
		var hotkey = new HotkeySetting
		{
			Modifiers = KeyModifiers.Win,
			Key       = 0x46
		};
		var args = CreateKeyEvent(Key.F, KeyModifiers.Win);

		var result = _sut.MatchesHotkey(hotkey, args);

		Assert.True(result);
	}

	private static KeyEventArgs CreateKeyEvent(Key key, KeyModifiers modifiers)
	{
		return new KeyEventArgs
		{
			Key          = key,
			KeyModifiers = ConvertModifiers(modifiers)
		};
	}

	private static Avalonia.Input.KeyModifiers ConvertModifiers(KeyModifiers modifiers)
	{
		var result = Avalonia.Input.KeyModifiers.None;
		if (modifiers.HasFlag(KeyModifiers.Control))
		{
			result |= Avalonia.Input.KeyModifiers.Control;
		}
		if (modifiers.HasFlag(KeyModifiers.Shift))
		{
			result |= Avalonia.Input.KeyModifiers.Shift;
		}
		if (modifiers.HasFlag(KeyModifiers.Alt))
		{
			result |= Avalonia.Input.KeyModifiers.Alt;
		}
		if (modifiers.HasFlag(KeyModifiers.Win))
		{
			result |= Avalonia.Input.KeyModifiers.Meta;
		}
		return result;
	}
}
