using Shared.Models.Enums;
using Shotora.App.Models;

namespace Shared.Interfaces.Adapters;

public interface IPropertyAdapter
{
	IEnumerable<string> GetPropertyNames<TModel>(SettingsPropertyTypes kind,  bool   includeHotkeys = false);
	TValue              GetValue<TModel, TValue>(TModel                model, string propertyName);

	void SetValue<TModel>(TModel model, string propertyName, object? value);

	void ApplyProperties<TModel, TTarget>(TModel                                      model,
										  TTarget                                     target,
										  string?                                     propertyName,
										  SettingsPropertyTypes                       kind,
										  Func<string, HotkeySetting, HotkeySetting>? hotkeyParser   = null,
										  bool                                        includeHotkeys = false);
}
