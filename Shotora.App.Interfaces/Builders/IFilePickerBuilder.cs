using Avalonia.Platform.Storage;

namespace Shotora.App.Interfaces.Builders;

public interface IFilePickerBuilder
{
	FilePickerSaveOptions BuildSaveOptions(string suggestedFileName, IStorageFolder? suggestedStartLocation);
}
