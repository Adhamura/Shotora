namespace NativeSupport.Extensions;

[AttributeUsage(AttributeTargets.Field)]
public sealed class OsPlatformNameAttribute(string name) : Attribute
{
	public string Name { get; } = name;
}
