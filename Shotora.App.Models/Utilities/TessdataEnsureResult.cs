using System.Diagnostics.CodeAnalysis;

namespace Shotora.App.Models.Utilities;

[ExcludeFromCodeCoverage] public record TessdataEnsureResult(bool Success, string? TessdataPath, string[] ResolvedLanguages, string? Error);
