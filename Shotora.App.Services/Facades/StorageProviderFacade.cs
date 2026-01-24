using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Shotora.App.Interfaces.Facades;

namespace Shotora.App.Services.Facades;

[ExcludeFromCodeCoverage]
public class StorageProviderFacade : IStorageProviderFacade
{
	public bool IsStorageProviderAvailable(Window owner)
	{
		var top = TopLevel.GetTopLevel(owner);
		return top?.StorageProvider != null;
	}

	public async Task<IStorageFile?> SaveFilePickerAsync(Window owner, FilePickerSaveOptions options)
	{
		var top = TopLevel.GetTopLevel(owner);
		if (top?.StorageProvider is null)
		{
			return null;
		}

		return await top.StorageProvider.SaveFilePickerAsync(options);
	}

	public async Task<IStorageFolder?> TryGetFolderFromPathAsync(Window owner, string folderPath)
	{
		var top = TopLevel.GetTopLevel(owner);
		if (top?.StorageProvider is null)
		{
			return null;
		}

		return await top.StorageProvider.TryGetFolderFromPathAsync(folderPath);
	}

	public string? TryGetLocalPath(IStorageFile storageFile)
	{
		return storageFile.TryGetLocalPath();
	}

	public async Task<Stream> OpenWriteAsync(IStorageFile storageFile)
	{
		return await storageFile.OpenWriteAsync();
	}
}
