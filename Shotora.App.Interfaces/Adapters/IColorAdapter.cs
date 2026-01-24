namespace Shotora.App.Interfaces.Adapters;

public interface IColorAdapter
{
	object? CreateColor(string hex);
	object? CreateBrush(string hex);
	string  ToHex(object?      color);
}
