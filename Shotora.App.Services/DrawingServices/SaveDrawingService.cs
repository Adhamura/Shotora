using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using Shared.Interfaces.Facades;
using Shotora.App.Interfaces.Builders;
using Shotora.App.Interfaces.DrawingServices;
using Shotora.App.Interfaces.Facades;
using Shotora.App.Interfaces.System;
using Shotora.App.Models;
using SkiaSharp;

namespace Shotora.App.Services.DrawingServices;

public class SaveDrawingService(IClipboardService      clipboardService,
								IFileFacade            fileFacade,
								IEnvironmentFacade     environmentFacade,
								IStorageProviderFacade storageProviderFacade,
								ISkiaImageFacade       skiaImageFacade,
								IFilePickerBuilder     filePickerBuilder) : ISaveDrawingService
{
	public async Task<bool> SaveSelectionAsync(Window owner, AppSettings settings, Func<SKBitmap?> buildCropped, bool prompt = true)
	{
		if (buildCropped == null)
		{
			return false;
		}

		var format = string.IsNullOrWhiteSpace(settings.DefaultFormat) ? "png" : settings.DefaultFormat.ToLowerInvariant();
		if (format is not ("png" or "jpg" or "jpeg"))
		{
			format = "png";
		}
		var folder = string.IsNullOrWhiteSpace(settings.DefaultSaveFolder)
			? Path.Combine(environmentFacade.GetFolderPath(environmentFacade.MyPictures), "Shotora")
			: settings.DefaultSaveFolder;
		fileFacade.CreateDirectory(folder);

		var pattern     = string.IsNullOrWhiteSpace(settings.FilenamePattern) ? "Shotora_{0:yyyy-MM-dd_HH-mm-ss}" : settings.FilenamePattern;
		var filename    = string.Format(pattern, DateTime.Now);
		var defaultPath = Path.Combine(folder, $"{filename}.{format}");

		var targetPath   = defaultPath;
		var targetFormat = format is "jpg" or "jpeg" ? SKEncodedImageFormat.Jpeg : SKEncodedImageFormat.Png;

		IStorageFile? saveFile = null;

		if (prompt)
		{
			if (!storageProviderFacade.IsStorageProviderAvailable(owner))
			{
				return false;
			}

			var suggestedStartLocation = await storageProviderFacade.TryGetFolderFromPathAsync(owner, folder);
			var saveOptions            = filePickerBuilder.BuildSaveOptions(Path.GetFileName(defaultPath), suggestedStartLocation);
			var saveResult             = await storageProviderFacade.SaveFilePickerAsync(owner, saveOptions);

			if (saveResult == null)
			{
				return false;
			}

			saveFile   = saveResult;
			targetPath = storageProviderFacade.TryGetLocalPath(saveResult);
			if (string.IsNullOrWhiteSpace(targetPath))
			{
				targetPath = null;
			}

			var chosenExt = Path.GetExtension(targetPath ?? saveResult.Name).Trim('.').ToLowerInvariant();
			targetFormat = chosenExt is "jpg" or "jpeg" ? SKEncodedImageFormat.Jpeg : SKEncodedImageFormat.Png;
		}

		using var cropped = buildCropped();
		if (cropped == null)
		{
			return false;
		}

		await using var stream = saveFile != null && targetPath == null
			? await storageProviderFacade.OpenWriteAsync(saveFile)
			: fileFacade.Create(targetPath ?? defaultPath);

		var quality = targetFormat == SKEncodedImageFormat.Jpeg ? settings.JpegQuality : 100;
		skiaImageFacade.EncodeAndSaveToStream(cropped, targetFormat, quality, stream);
		return true;
	}

	public async Task<bool> CopySelectionAsync(Func<SKBitmap?> buildCropped)
	{
		if (buildCropped == null)
		{
			return false;
		}

		await using var stream = new MemoryStream();
		using (var cropped = buildCropped())
		{
			if (cropped == null)
			{
				return false;
			}
			skiaImageFacade.EncodeAndSaveToStream(cropped, SKEncodedImageFormat.Png, 100, stream);
		}

		stream.Position = 0;
		using var bmp = new Bitmap(stream);
		await clipboardService.SetImageAsync(bmp);
		return true;
	}
}
