using Shotora.App.Models;
using Shotora.App.Models.Utilities;

namespace Shotora.App.Interfaces.Ocr.EastOcr;

public interface IMaintenanceEasyOcrService
{
	Task<EasyOcrViewState> GetStateAsync(EasyOcrStatusTexts texts);

	Task DeleteAsync(EasyOcrStatusTexts texts, IProgress<string>? status = null);

	Task<EasyOcrRuntimeResult> EnsureRuntimeAsync(bool installMissing = false, EasyOcrStatusTexts? texts = null, IProgress<string>? status = null, CancellationToken cancellationToken = default);

	Task<string> EnsureSupportScriptAsync(CancellationToken cancellationToken = default);
}
