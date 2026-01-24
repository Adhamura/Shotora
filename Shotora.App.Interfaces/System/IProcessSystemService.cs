using System.Diagnostics;
using NativeSupport.Enums;
using Shotora.App.Models.System;

namespace Shotora.App.Interfaces.System;

public interface IProcessSystemService
{
	RuntimeOs                    GetCurrentOs();
	Task<ProcessExecutionResult> RunAsync(ProcessStartInfo         startInfo, CancellationToken    cancellationToken = default);
	ProcessStartInfo             CreateProcessStartInfo(string     fileName,  IEnumerable<string>? args              = null, string? workingDirectory = null);
	Task<ProcessExecutionResult> RunAsync(string                   fileName,  IEnumerable<string>? args              = null, string? workingDirectory = null, CancellationToken cancellationToken = default);
	ProcessStartInfo             CreateUtf8ProcessStartInfo(string fileName,  string               arguments,                string? workingDirectory = null);
}
