using System.Diagnostics.CodeAnalysis;

namespace Shotora.App.Models.Utilities;

[ExcludeFromCodeCoverage] public record TessdataDownloadResult(bool Success, string? Error);
