using System.Diagnostics.CodeAnalysis;

namespace Shotora.App.Models.Constants;

[ExcludeFromCodeCoverage]
public static class OcrConstants
{
	public const           string   TessdataBestBase                 = "https://github.com/tesseract-ocr/tessdata_best/raw/main";
	public const           string   TessdataFastBase                 = "https://github.com/tesseract-ocr/tessdata_fast/raw/main";
	public const           string   TessdataExtension                = ".traineddata";
	public const           string   EasyOcrName                      = "easyocr";
	public const           string   PythonName                       = "python";
	public const           string   PythonFolder                     = "PythonEmbed";
	public const           string   PythonArchive                    = $"{PythonName}.7z";
	public const           string   EasyocrRunner                    = "easyocr_runner.py";
	public static readonly string   TessDataDir                      = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Shotora", "tessdata");
	public static readonly string[] EasyOcrPackages                  = [EasyOcrName];
	public static readonly string[] UnixSevenZipExecutableCandidates = ["7zz", "7z", "7za", "7zr"];
	public static readonly string[] sourceArray                      = ["python3", PythonName];
	public static readonly string[] BuildDependencyPackages          = ["Cython", "scikit-build", "cmake", "ninja", "meson-python", "meson", "setuptools", "wheel"];
	public static          string   TesseractBaseDir => Path.Combine(Path.GetTempPath(), "Shotora", "tesseract");

	public static string GetLanguagePath(string languageCode, string path = null)
	{
		return Path.Combine(path ?? TessDataDir, $"{languageCode}{TessdataExtension}");
	}
}
