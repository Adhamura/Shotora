using System.Diagnostics.CodeAnalysis;

namespace Shotora.App.Models.Utilities;

[ExcludeFromCodeCoverage] public record EasyOcrRuntimeResult(bool Success, string? PythonPath, string? Error);
