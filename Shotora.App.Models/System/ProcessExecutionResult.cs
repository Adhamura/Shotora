using System.Diagnostics.CodeAnalysis;

namespace Shotora.App.Models.System;

[ExcludeFromCodeCoverage] public sealed record ProcessExecutionResult(int ExitCode, string StdOut, string StdErr);
