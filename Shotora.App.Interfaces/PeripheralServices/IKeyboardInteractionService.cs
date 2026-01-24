using Avalonia.Input;
using Shotora.App.Models;

namespace Shotora.App.Interfaces.PeripheralServices;

public interface IKeyboardInteractionService
{
	bool MatchesHotkey(HotkeySetting setting, KeyEventArgs e);
}
