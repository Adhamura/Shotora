using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Shared.Interfaces.Adapters;
using Shared.Models.Enums;
using Shotora.App.Models;
using Shotora.App.Models.Extensions;

namespace Shared.Services.Adapters;

[ExcludeFromCodeCoverage]
public class PropertyAdapter : IPropertyAdapter
{
	private const BindingFlags AllInstanceFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

	public IEnumerable<string> GetPropertyNames<TModel>(SettingsPropertyTypes kind, bool includeHotkeys = false)
	{
		var modelType = typeof(TModel);

		var annotated = GetAllMembers(modelType)
			.Select(m => (Member: m, Attribute: m.GetCustomAttribute<SettingsExtension>()))
			.Where(t => t.Attribute?.PropertyKind == kind)
			.Select(t => t.Member switch
			{
				PropertyInfo p => p.Name,
				FieldInfo f    => ToPropertyName(f),
				_              => null
			});

		var hotkeys = includeHotkeys
			? modelType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
				.Where(p => p.Name.EndsWith("HotkeyText", StringComparison.OrdinalIgnoreCase))
				.Select(p => p.Name)
			: [];

		return annotated.Concat(hotkeys).OfType<string>().Distinct(StringComparer.OrdinalIgnoreCase);
	}

	public TValue GetValue<TModel, TValue>(TModel model, string propertyName)
	{
		var prop = FindProperty(typeof(TModel), propertyName);
		if (prop?.CanRead != true)
		{
			return default!;
		}

		var raw = prop.GetValue(model);
		return raw is TValue casted ? casted : default!;
	}

	public void SetValue<TModel>(TModel model, string propertyName, object? value)
	{
		var prop = FindProperty(typeof(TModel), propertyName);
		prop?.SetValue(model, value);
	}

	public void ApplyProperties<TModel, TTarget>(
		TModel                                      model,
		TTarget                                     target,
		string?                                     propertyName,
		SettingsPropertyTypes                       kind,
		Func<string, HotkeySetting, HotkeySetting>? hotkeyParser   = null,
		bool                                        includeHotkeys = false)
	{
		var names = propertyName is null ? GetPropertyNames<TModel>(kind, includeHotkeys) : [propertyName];

		foreach (var name in names)
		{
			if (includeHotkeys && name.EndsWith("HotkeyText", StringComparison.OrdinalIgnoreCase) && hotkeyParser is not null)
			{
				MapHotkey(model, target, name, hotkeyParser);
				continue;
			}
			ApplyAttribute(name, model, target);
		}
	}

	private static void ApplyAttribute<TModel, TTarget>(string propertyName, TModel model, TTarget target)
	{
		var modelType  = typeof(TModel);
		var targetType = typeof(TTarget);

		var modelProp  = FindProperty(modelType, propertyName);
		var field      = FindField(modelType, $"_{char.ToLowerInvariant(propertyName[0])}{propertyName[1..]}");
		var attribute  = modelProp?.GetCustomAttribute<SettingsExtension>() ?? field?.GetCustomAttribute<SettingsExtension>();
		var targetProp = FindProperty(targetType, propertyName);

		if (attribute == null || targetProp == null)
		{
			return;
		}

		var value    = modelProp?.GetValue(model) ?? field?.GetValue(model);
		var computed = attribute.Apply(value, targetProp.PropertyType);
		targetProp.SetValue(target, computed);
	}

	private static void MapHotkey<TModel, TTarget>(TModel model, TTarget target, string hotkeyTextProp, Func<string, HotkeySetting, HotkeySetting> parser)
	{
		var targetName   = hotkeyTextProp.EndsWith("Text", StringComparison.OrdinalIgnoreCase) ? hotkeyTextProp[..^4] : hotkeyTextProp;
		var modelProp    = typeof(TModel).GetProperty(hotkeyTextProp, BindingFlags.Instance | BindingFlags.Public);
		var settingsProp = typeof(TTarget).GetProperty(targetName, BindingFlags.Instance    | BindingFlags.Public);

		if (modelProp?.PropertyType != typeof(string) || settingsProp?.PropertyType != typeof(HotkeySetting))
		{
			return;
		}

		var text     = modelProp.GetValue(model) as string            ?? string.Empty;
		var fallback = settingsProp.GetValue(target) as HotkeySetting ?? new HotkeySetting();
		settingsProp.SetValue(target, parser(text, fallback));
	}

	private static PropertyInfo? FindProperty(Type type, string name)
	{
		return SearchHierarchy(type, t => t.GetProperty(name, AllInstanceFlags | BindingFlags.DeclaredOnly));
	}

	private static FieldInfo? FindField(Type type, string name)
	{
		return SearchHierarchy(type, t => t.GetField(name, AllInstanceFlags | BindingFlags.DeclaredOnly));
	}

	private static T? SearchHierarchy<T>(Type type, Func<Type, T?> finder) where T : class
	{
		for (var t = type; t != null && t != typeof(object); t = t.BaseType)
		{
			var result = finder(t);
			if (result != null)
			{
				return result;
			}
		}
		return null;
	}

	private static IEnumerable<MemberInfo> GetAllMembers(Type type)
	{
		for (var t = type; t != null && t != typeof(object); t = t.BaseType)
		{
			foreach (var member in t.GetMembers(AllInstanceFlags | BindingFlags.DeclaredOnly))
			{
				yield return member;
			}
		}
	}

	private static string ToPropertyName(FieldInfo f)
	{
		return f.Name.StartsWith('_') && f.Name.Length > 1 ? char.ToUpperInvariant(f.Name[1]) + f.Name[2..] : f.Name;
	}
}
