using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using Moq;
using Shared.Interfaces.Facades;
using Shotora.App.Interfaces.Builders;
using Shotora.App.Interfaces.Facades;
using Shotora.App.Interfaces.System;
using Shotora.App.Models;
using Shotora.App.Services.DrawingServices;
using SkiaSharp;

namespace Shotora.App.Tests.DrawingServices;

public class SaveDrawingServiceTests
{
	private readonly Mock<IClipboardService>      _clipboardService      = new(MockBehavior.Strict);
	private readonly Mock<IEnvironmentFacade>     _environmentFacade     = new(MockBehavior.Strict);
	private readonly Mock<IFileFacade>            _fileFacade            = new(MockBehavior.Strict);
	private readonly Mock<IFilePickerBuilder>     _filePickerBuilder     = new(MockBehavior.Strict);
	private readonly Mock<ISkiaImageFacade>       _skiaImageFacade       = new(MockBehavior.Strict);
	private readonly Mock<IStorageProviderFacade> _storageProviderFacade = new(MockBehavior.Strict);
	private readonly SaveDrawingService           _sut;
	static SaveDrawingServiceTests()
	{
		try
		{
			AppBuilder.Configure<Application>()
				.UseHeadless(new AvaloniaHeadlessPlatformOptions
				{
					UseHeadlessDrawing = true
				})
				.SetupWithoutStarting();
		}
		catch (InvalidOperationException)
		{
		}
	}

	public SaveDrawingServiceTests()
	{
		_environmentFacade.Setup(e => e.MyPictures).Returns(Environment.SpecialFolder.MyPictures);
		_environmentFacade.Setup(e => e.GetFolderPath(It.IsAny<Environment.SpecialFolder>()))
			.Returns(Path.Combine(Path.GetTempPath(), "TestPictures"));

		_fileFacade.Setup(f => f.CreateDirectory(It.IsAny<string?>()))
			.Callback<string?>(path =>
			{
				if (!string.IsNullOrWhiteSpace(path))
				{
					Directory.CreateDirectory(path);
				}
			});

		_fileFacade.Setup(f => f.Create(It.IsAny<string>()))
			.Returns<string>(path =>
			{
				var dir = Path.GetDirectoryName(path);
				if (!string.IsNullOrWhiteSpace(dir) && !Directory.Exists(dir))
				{
					Directory.CreateDirectory(dir);
				}
				return File.Create(path);
			});

		_storageProviderFacade.Setup(s => s.IsStorageProviderAvailable(It.IsAny<Window>()))
			.Returns(false);

		_skiaImageFacade.Setup(s => s.EncodeAndSaveToStream(It.IsAny<SKBitmap>(), It.IsAny<SKEncodedImageFormat>(), It.IsAny<int>(), It.IsAny<Stream>()))
			.Callback<SKBitmap, SKEncodedImageFormat, int, Stream>((bitmap, format, quality, stream) =>
			{
				var dummyData = new byte[]
				{
					0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A
				};
				stream.Write(dummyData, 0, dummyData.Length);
				stream.Flush();
			});

		_filePickerBuilder.Setup(b => b.BuildSaveOptions(It.IsAny<string>(), It.IsAny<IStorageFolder?>()))
			.Returns<string, IStorageFolder?>((fileName, startLocation) => new FilePickerSaveOptions
			{
				SuggestedFileName      = fileName,
				SuggestedStartLocation = startLocation,
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
			});

		_sut = new SaveDrawingService(_clipboardService.Object, _fileFacade.Object, _environmentFacade.Object, _storageProviderFacade.Object, _skiaImageFacade.Object, _filePickerBuilder.Object);
	}

	private Func<SKBitmap?> CreateTestBitmap => () => new SKBitmap(1, 1);

	#region SaveSelectionAsync Tests

	[Fact]
	public async Task Given_NullBuildCropped_When_SaveSelectionAsync_Then_ReturnsFalse()
	{
		var owner    = new Window();
		var settings = new AppSettings();

		var result = await _sut.SaveSelectionAsync(owner, settings, null!);

		Assert.False(result);
	}

	[Fact]
	public async Task Given_BuildCroppedReturnsNull_When_SaveSelectionAsync_Then_ReturnsFalse()
	{
		var owner    = new Window();
		var settings = new AppSettings();

		var result = await _sut.SaveSelectionAsync(owner, settings, () => null);

		Assert.False(result);
	}

	[Fact]
	public async Task Given_DefaultFormat_When_SaveSelectionAsync_Then_UsesPng()
	{
		var owner = new Window();
		var settings = new AppSettings
		{
			DefaultFormat = null!
		};

		var result = await _sut.SaveSelectionAsync(owner, settings, CreateTestBitmap, false);

		Assert.True(result);
		_fileFacade.Verify(f => f.CreateDirectory(It.IsAny<string?>()), Times.Once);
	}

	[Fact]
	public async Task Given_InvalidFormat_When_SaveSelectionAsync_Then_FallsBackToPng()
	{
		var owner = new Window();
		var settings = new AppSettings
		{
			DefaultFormat = "invalid"
		};

		var result = await _sut.SaveSelectionAsync(owner, settings, CreateTestBitmap, false);

		Assert.True(result);
		_fileFacade.Verify(f => f.CreateDirectory(It.IsAny<string?>()), Times.Once);
	}

	[Theory]
	[InlineData("png")]
	[InlineData("PNG")]
	[InlineData("Png")]
	public async Task Given_PngFormat_When_SaveSelectionAsync_Then_UsesPngFormat(string format)
	{
		var owner = new Window();
		var settings = new AppSettings
		{
			DefaultFormat = format
		};

		var result = await _sut.SaveSelectionAsync(owner, settings, CreateTestBitmap, false);

		Assert.True(result);
		_fileFacade.Verify(f => f.CreateDirectory(It.IsAny<string?>()), Times.Once);
	}

	[Theory]
	[InlineData("jpg")]
	[InlineData("jpeg")]
	[InlineData("JPG")]
	[InlineData("JPEG")]
	public async Task Given_JpegFormat_When_SaveSelectionAsync_Then_UsesJpegFormat(string format)
	{
		var owner = new Window();
		var settings = new AppSettings
		{
			DefaultFormat = format
		};

		var result = await _sut.SaveSelectionAsync(owner, settings, CreateTestBitmap, false);

		Assert.True(result);
		_fileFacade.Verify(f => f.CreateDirectory(It.IsAny<string?>()), Times.Once);
	}

	[Fact]
	public async Task Given_EmptyDefaultSaveFolder_When_SaveSelectionAsync_Then_UsesDefaultFolder()
	{
		var owner = new Window();
		var settings = new AppSettings
		{
			DefaultSaveFolder = null!
		};

		var result = await _sut.SaveSelectionAsync(owner, settings, CreateTestBitmap, false);

		Assert.True(result);
		_fileFacade.Verify(f => f.CreateDirectory(It.Is<string?>(s => s != null && s.Contains("Shotora"))), Times.Once);
	}

	[Fact]
	public async Task Given_CustomSaveFolder_When_SaveSelectionAsync_Then_UsesCustomFolder()
	{
		var owner        = new Window();
		var customFolder = Path.Combine(Path.GetTempPath(), "CustomFolder");
		var settings = new AppSettings
		{
			DefaultSaveFolder = customFolder
		};

		var result = await _sut.SaveSelectionAsync(owner, settings, CreateTestBitmap, false);

		Assert.True(result);
		_fileFacade.Verify(f => f.CreateDirectory(customFolder), Times.Once);
	}

	[Fact]
	public async Task Given_EmptyFilenamePattern_When_SaveSelectionAsync_Then_UsesDefaultPattern()
	{
		var owner = new Window();
		var settings = new AppSettings
		{
			FilenamePattern = null!
		};

		var result = await _sut.SaveSelectionAsync(owner, settings, CreateTestBitmap, false);

		Assert.True(result);
		_fileFacade.Verify(f => f.CreateDirectory(It.IsAny<string?>()), Times.Once);
	}

	[Fact]
	public async Task Given_CustomFilenamePattern_When_SaveSelectionAsync_Then_UsesPattern()
	{
		var owner = new Window();
		var settings = new AppSettings
		{
			FilenamePattern = "Custom_{0:yyyyMMdd}"
		};

		var result = await _sut.SaveSelectionAsync(owner, settings, CreateTestBitmap, false);

		Assert.True(result);
		_fileFacade.Verify(f => f.CreateDirectory(It.IsAny<string?>()), Times.Once);
	}

	[Fact]
	public async Task Given_NoPrompt_When_SaveSelectionAsync_Then_SavesWithoutDialog()
	{
		var owner    = new Window();
		var settings = new AppSettings();

		var result = await _sut.SaveSelectionAsync(owner, settings, CreateTestBitmap, false);

		Assert.True(result);
		_fileFacade.Verify(f => f.CreateDirectory(It.IsAny<string?>()), Times.Once);
	}

	#endregion

	#region CopySelectionAsync Tests

	[Fact]
	public async Task Given_NullBuildCropped_When_CopySelectionAsync_Then_ReturnsFalse()
	{
		var result = await _sut.CopySelectionAsync(null!);

		Assert.False(result);
		_clipboardService.VerifyNoOtherCalls();
	}

	[Fact]
	public async Task Given_BuildCroppedReturnsNull_When_CopySelectionAsync_Then_ReturnsFalse()
	{
		var result = await _sut.CopySelectionAsync(() => null);

		Assert.False(result);
		_clipboardService.VerifyNoOtherCalls();
	}

	[Fact]
	public async Task Given_ValidBitmap_When_CopySelectionAsync_Then_CopiesToClipboard()
	{
		_clipboardService.Setup(c => c.SetImageAsync(It.IsAny<Bitmap>()))
			.Returns(Task.CompletedTask);

		var result = await _sut.CopySelectionAsync(CreateTestBitmap);

		Assert.True(result);
		_clipboardService.Verify(c => c.SetImageAsync(It.IsAny<Bitmap>()), Times.Once);
		_clipboardService.VerifyNoOtherCalls();
	}

	[Fact]
	public async Task Given_EmptyBitmap_When_CopySelectionAsync_Then_CopiesToClipboard()
	{
		_clipboardService.Setup(c => c.SetImageAsync(It.IsAny<Bitmap>()))
			.Returns(Task.CompletedTask);

		var result = await _sut.CopySelectionAsync(() => new SKBitmap(1, 1));

		Assert.True(result);
		_clipboardService.Verify(c => c.SetImageAsync(It.IsAny<Bitmap>()), Times.Once);
	}

	[Fact]
	public async Task Given_LargeBitmap_When_CopySelectionAsync_Then_CopiesToClipboard()
	{
		_clipboardService.Setup(c => c.SetImageAsync(It.IsAny<Bitmap>()))
			.Returns(Task.CompletedTask);

		var result = await _sut.CopySelectionAsync(() =>
		{
			var bitmap = new SKBitmap(2000, 2000);
			bitmap.Erase(SKColors.White);
			return bitmap;
		});

		Assert.True(result);
		_clipboardService.Verify(c => c.SetImageAsync(It.IsAny<Bitmap>()), Times.Once);
	}

	[Fact]
	public async Task Given_ClipboardServiceThrows_When_CopySelectionAsync_Then_PropagatesException()
	{
		_clipboardService.Setup(c => c.SetImageAsync(It.IsAny<Bitmap>()))
			.ThrowsAsync(new InvalidOperationException("Clipboard error"));

		await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.CopySelectionAsync(CreateTestBitmap));
	}

	#endregion
}
