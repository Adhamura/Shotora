using System.Diagnostics.CodeAnalysis;

namespace Shotora.App.Models.Constants;

[ExcludeFromCodeCoverage]
public static class OcrMessages
{
	public const string SelectionTooSmall   = "Selection is too small for OCR.";
	public const string SelectionReadFailed = "Failed to read selection for OCR.";
	public const string GenericFailure      = "OCR failed. Ensure tesseract/easyocr are installed and accessible.";
	public const string TessdataMissing     = "tessdata missing";

	public const string TesseractStartFailed   = "Failed to start tesseract process.";
	public const string TesseractTimeout       = "tesseract CLI timed out.";
	public const string TesseractReturnedEmpty = "tesseract CLI returned empty text.";

	public const string EasyOcrRuntimeMissing              = "EasyOCR runtime unavailable.";
	public const string EasyOcrStartFailed                 = "Failed to start EasyOCR process.";
	public const string EasyOcrTimeout                     = "EasyOCR timed out.";
	public const string EasyOcrReturnedEmpty               = "EasyOCR returned empty text.";
	public const string EasyOcrEmbeddedPythonMissingFormat = "Embedded Python runtime missing for this platform ({0}).";
	public const string EasyOcrEmbeddedPythonFailedFormat  = "Failed to prepare embedded Python: {0}";
	public const string EasyOcrPythonMissingAfterExtract   = "Embedded Python was extracted but python executable is missing.";
	public const string EasyOcrPackageMissing              = "EasyOCR package missing inside embedded Python.";
	public const string EasyOcrUnsupportedPythonMac        = "EasyOCR on macOS requires embedded Python 3.12 or lower (NumPy/torch wheels are not available for 3.13+).";

	public const string TesseractCliMissing     = "tesseract CLI not available in bundled binaries.";
	public const string TesseractCliErrorFormat = "tesseract CLI error (code {0}): {1}";
	public const string EasyOcrErrorFormat      = "EasyOCR error (code {0}): {1}";
}
