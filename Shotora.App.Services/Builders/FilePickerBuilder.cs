using Avalonia.Platform.Storage;
using Shotora.App.Interfaces.Builders;

namespace Shotora.App.Services.Builders;

public class FilePickerBuilder : IFilePickerBuilder
{
	public FilePickerSaveOptions BuildSaveOptions(string suggestedFileName, IStorageFolder? suggestedStartLocation)
	{
		return new FilePickerSaveOptions
		{
			SuggestedFileName      = suggestedFileName,
			SuggestedStartLocation = suggestedStartLocation,
			FileTypeChoices =
			[
				new FilePickerFileType("PNG")
				{
					Patterns = ["*.png"]
				},
				new FilePickerFileType("JPEG")
				{
					Patterns = ["*.jpg", "*.jpeg"]
				},
				new FilePickerFileType("All files")
				{
					Patterns = ["*.*"]
				}
			]
		};
	}
}
