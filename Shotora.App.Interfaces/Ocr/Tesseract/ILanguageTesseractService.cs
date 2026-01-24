using Shotora.App.Models.Utilities;

namespace Shotora.App.Interfaces.Ocr.Tesseract;

public interface ILanguageTesseractService
{
	void PopulateLanguages(ICollection<TesseractLanguageOption> target, Action<TesseractLanguageOption>? onCreated = null);
	Task DownloadAsync(TesseractLanguageOption                  option, IProgress<double>?               progress  = null, CancellationToken cancellationToken = default);
	void RemoveLanguage(TesseractLanguageOption                 option);
}
