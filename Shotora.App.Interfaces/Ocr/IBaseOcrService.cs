using Avalonia;
using Shotora.App.Models.Enums;
using Shotora.App.Models.ItemModels;
using SkiaSharp;

namespace Shotora.App.Interfaces.Ocr;

public interface IBaseOcrService
{
	Task<OcrResultDataItemModel> RecognizeAsync(SKBitmap source, Rect selection, string languages, OcrEngineType engine);
}
