using Avalonia.Controls;
using Shotora.App.Interfaces.Adapters;

namespace Shotora.App.Services.Adapters;

public class AvaloniaEnumAdapter : IAvaloniaEnumAdapter
{
	public void EnsureControls<TEnum, TControl>(Control root, IDictionary<TEnum, TControl> map, Func<TEnum, string> nameSelector)
		where TEnum : struct, Enum
		where TControl : Control
	{
		foreach (var value in Enum.GetValues<TEnum>())
		{
			if (map.ContainsKey(value))
			{
				continue;
			}

			var name    = nameSelector(value);
			var control = root.FindControl<TControl>(name);
			if (control != null)
			{
				map[value] = control;
			}
		}
	}
}
