using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using Shotora.App.Interfaces.Adapters;

namespace Shotora.App.Services.Adapters;

public class FolderPickerAdapter : IFolderPickerAdapter
{
	public async Task<string?> PickFolderAsync(string? initialPath, object? owner = null)
	{
		var window = owner as Window ?? (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
		if (window?.StorageProvider == null)
		{
			return null;
		}

		var suggested = string.IsNullOrWhiteSpace(initialPath)
			? null
			: await window.StorageProvider.TryGetFolderFromPathAsync(initialPath);

		var result = await window.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
		{
			AllowMultiple          = false,
			SuggestedStartLocation = suggested
		});

		var picked = result.FirstOrDefault();
		var path   = picked?.TryGetLocalPath();
		return string.IsNullOrWhiteSpace(path) ? null : path;
	}
}
