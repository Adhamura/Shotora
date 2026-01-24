using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;
using NativeSupport.Enums;
using NativeSupport.Extensions;
using Shared.Interfaces.Adapters;
using Shotora.App.Interfaces.System;
using Shotora.App.Models.System;

namespace Shotora.App.Services.System;

[ExcludeFromCodeCoverage]
public class ProcessSystemService(IEnumAdapter enumAdapter) : IProcessSystemService
{
	public RuntimeOs GetCurrentOs()
	{
		var match = enumAdapter.TryFind<RuntimeOs>(value =>
		{
			var platform = enumAdapter.TryGetStaticPropertyValue<RuntimeOs, OsPlatformNameAttribute, OSPlatform>(value, attribute => attribute.Name);
			return platform.HasValue && RuntimeInformation.IsOSPlatform(platform.Value);
		});

		return match ?? RuntimeOs.Other;
	}

	public async Task<ProcessExecutionResult> RunAsync(ProcessStartInfo startInfo, CancellationToken cancellationToken = default)
	{
		if (!startInfo.RedirectStandardOutput)
		{
			startInfo.RedirectStandardOutput = true;
		}

		if (!startInfo.RedirectStandardError)
		{
			startInfo.RedirectStandardError = true;
		}

		using var process    = Process.Start(startInfo) ?? throw new InvalidOperationException("Failed to start process.");
		var       stdOutTask = process.StandardOutput.ReadToEndAsync(cancellationToken);
		var       stdErrTask = process.StandardError.ReadToEndAsync(cancellationToken);

		await process.WaitForExitAsync(cancellationToken).ConfigureAwait(false);

		var stdOut = await stdOutTask.ConfigureAwait(false);
		var stdErr = await stdErrTask.ConfigureAwait(false);
		var result = new ProcessExecutionResult(process.ExitCode, stdOut?.Trim() ?? string.Empty, stdErr?.Trim() ?? string.Empty);
		if (result.ExitCode != 0)
		{
			throw new InvalidOperationException($"Failed (exit {result.ExitCode}): {result.StdErr}\n{result.StdOut}");
		}
		return result;
	}

	public ProcessStartInfo CreateProcessStartInfo(string fileName, IEnumerable<string>? args = null, string? workingDirectory = null)
	{
		var psi = new ProcessStartInfo
		{
			FileName               = fileName,
			Arguments              = BuildArguments(args),
			RedirectStandardOutput = true,
			RedirectStandardError  = true,
			UseShellExecute        = false,
			CreateNoWindow         = true
		};

		if (!string.IsNullOrWhiteSpace(workingDirectory))
		{
			psi.WorkingDirectory = workingDirectory;
		}

		return psi;
	}

	public Task<ProcessExecutionResult> RunAsync(
		string               fileName,
		IEnumerable<string>? args              = null,
		string?              workingDirectory  = null,
		CancellationToken    cancellationToken = default)
	{
		var psi = CreateProcessStartInfo(fileName, args, workingDirectory);
		return RunAsync(psi, cancellationToken);
	}

	public ProcessStartInfo CreateUtf8ProcessStartInfo(string fileName, string arguments, string? workingDirectory = null)
	{
		var psi = new ProcessStartInfo
		{
			FileName               = fileName,
			Arguments              = arguments,
			RedirectStandardOutput = true,
			RedirectStandardError  = true,
			UseShellExecute        = false,
			CreateNoWindow         = true,
			StandardOutputEncoding = Encoding.UTF8,
			StandardErrorEncoding  = Encoding.UTF8
		};

		if (!string.IsNullOrWhiteSpace(workingDirectory))
		{
			psi.WorkingDirectory = workingDirectory;
		}

		return psi;
	}

	private static string BuildArguments(IEnumerable<string>? args)
	{
		return args == null ? string.Empty : string.Join(' ', args.Select(arg => arg.Contains(' ') ? $"\"{arg}\"" : arg));
	}
}
