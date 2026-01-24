using System.Diagnostics.CodeAnalysis;
using Shared.Models.Enums;
using Shotora.App.Models.Enums;

namespace Shotora.App.Models.Extensions;

[ExcludeFromCodeCoverage]
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class SettingsExtension(SettingsTypes type, SettingsPropertyTypes propertyKind, params object[] parameters)
	: Attribute
{
	public SettingsExtension(SettingsTypes type, params object[] parameters)
		: this(type, SettingsPropertyTypes.Tracked, parameters)
	{
	}

	public SettingsTypes         Type         { get; } = type;
	public SettingsPropertyTypes PropertyKind { get; } = propertyKind;

	public object[] Parameters { get; } = parameters ?? [];

	public object? Apply(object? value, Type targetType)
	{
		switch (Type)
		{
			case SettingsTypes.NullOrEmpty:
			{
				var fallback = Parameters.FirstOrDefault()?.ToString() ?? string.Empty;
				var text     = value as string;
				return string.IsNullOrWhiteSpace(text) ? fallback : text;
			}
			case SettingsTypes.Path:
			{
				var folder   = Parameters.FirstOrDefault() is Environment.SpecialFolder sf ? sf : Environment.SpecialFolder.MyPictures;
				var fallback = Environment.GetFolderPath(folder);
				var text     = value as string;
				return string.IsNullOrWhiteSpace(text) ? fallback : text;
			}
			case SettingsTypes.Clamp:
			{
				if (Parameters.Length >= 2 && value is IConvertible convertible)
				{
					var min     = Convert.ToDouble(Parameters[0]);
					var max     = Convert.ToDouble(Parameters[1]);
					var current = Convert.ToDouble(convertible);
					var clamped = Math.Clamp(current, min, max);
					return Convert.ChangeType(clamped, targetType);
				}
				break;
			}
			case SettingsTypes.PositiveDefault:
			{
				if (Parameters.FirstOrDefault() is IConvertible fallback && value is IConvertible current)
				{
					var fallbackValue = Convert.ToDouble(fallback);
					var currentValue  = Convert.ToDouble(current);
					var result        = currentValue <= 0 ? fallbackValue : currentValue;
					return Convert.ChangeType(result, targetType);
				}
				break;
			}
		}

		return value;
	}
}
