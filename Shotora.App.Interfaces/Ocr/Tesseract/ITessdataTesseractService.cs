using Shotora.App.Models.Utilities;

namespace Shotora.App.Interfaces.Ocr.Tesseract;

public interface ITessdataTesseractService
{
	Task<TessdataEnsureResult> EnsureAsync(IEnumerable<string> languageCodes, CancellationToken cancellationToken = default);

	Task<TessdataDownloadResult> DownloadAsync(string languageCode, IProgress<double>? progress = null, CancellationToken cancellationToken = default);

	bool IsInstalled(string languageCode);
}
