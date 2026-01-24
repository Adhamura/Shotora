namespace Shotora.App.Interfaces.Adapters;

public interface IFolderPickerAdapter
{
	Task<string?> PickFolderAsync(string? initialPath, object? owner = null);
}
