using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace NativeSupport.Adapters;

[ExcludeFromCodeCoverage]
public static class EnumAdapter
{
	public static TAttribute? GetAttribute<TEnum, TAttribute>(TEnum value)
		where TEnum : struct, Enum
		where TAttribute : Attribute
	{
		var name = Enum.GetName(value);
		if (string.IsNullOrWhiteSpace(name))
		{
			return null;
		}

		var field = typeof(TEnum).GetField(name);
		return field?.GetCustomAttribute<TAttribute>();
	}

	public static TProperty? TryGetStaticPropertyValue<TEnum, TAttribute, TProperty>(
		TEnum                    value,
		Func<TAttribute, string> nameSelector)
		where TEnum : struct, Enum
		where TAttribute : Attribute
		where TProperty : struct
	{
		var attribute = GetAttribute<TEnum, TAttribute>(value);
		if (attribute == null)
		{
			return null;
		}

		var name = nameSelector(attribute);
		if (string.IsNullOrWhiteSpace(name))
		{
			return null;
		}

		var property = typeof(TProperty).GetProperty(name, BindingFlags.Public | BindingFlags.Static);
		if (property?.GetValue(null) is TProperty result)
		{
			return result;
		}

		return null;
	}

	public static TEnum? TryFind<TEnum>(Func<TEnum, bool> predicate)
		where TEnum : struct, Enum
	{
		foreach (var value in Enum.GetValues<TEnum>())
		{
			if (predicate(value))
			{
				return value;
			}
		}

		return null;
	}
}
