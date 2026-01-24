using Avalonia.Controls;
using Avalonia.Platform.Storage;

namespace Shotora.App.Interfaces.Facades;

public interface IStorageProviderFacade
{
	Task<IStorageFile?>   SaveFilePickerAsync(Window        owner, FilePickerSaveOptions options);
	Task<IStorageFolder?> TryGetFolderFromPathAsync(Window  owner, string                folderPath);
	string?               TryGetLocalPath(IStorageFile      storageFile);
	Task<Stream>          OpenWriteAsync(IStorageFile       storageFile);
	bool                  IsStorageProviderAvailable(Window owner);
}
