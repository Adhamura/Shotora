using Shotora.App.Models.Utilities;

namespace Shotora.App.Interfaces.Providers;

public interface IPythonProvider
{
	string? GetEmbeddedZipPath();

	Task<EasyOcrRuntimeResult> EnsureExtractedAsync(CancellationToken cancellationToken = default);

	Task<EasyOcrRuntimeResult> EnsureEasyOcrAsync(IProgress<string>? status = null, CancellationToken cancellationToken = default);
}
