using Shotora.App.Models;

namespace Shotora.App.Interfaces.System;

public interface IHotkeyService : IDisposable
{
	int Register(HotkeySetting setting, Action callback);

	void Reset();
}
