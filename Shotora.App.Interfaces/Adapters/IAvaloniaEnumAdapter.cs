using Avalonia.Controls;

namespace Shotora.App.Interfaces.Adapters;

public interface IAvaloniaEnumAdapter
{
	void EnsureControls<TEnum, TControl>(Control root, IDictionary<TEnum, TControl> map, Func<TEnum, string> nameSelector)
		where TEnum : struct, Enum
		where TControl : Control;
}
