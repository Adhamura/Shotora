namespace Shared.Interfaces.Adapters;

public interface IEnumAdapter
{
	TAttribute? GetAttribute<TEnum, TAttribute>(TEnum value)
		where TEnum : struct, Enum
		where TAttribute : Attribute;
	TProperty? TryGetStaticPropertyValue<TEnum, TAttribute, TProperty>(
		TEnum                    value,
		Func<TAttribute, string> nameSelector)
		where TEnum : struct, Enum
		where TAttribute : Attribute
		where TProperty : struct;
	TEnum? TryFind<TEnum>(Func<TEnum, bool> predicate)
		where TEnum : struct, Enum;
}
