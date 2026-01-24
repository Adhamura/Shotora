using NativeSupport.Enums;
using Shared.Interfaces.Facades;
using Shotora.App.Interfaces.Ocr.EastOcr;
using Shotora.App.Interfaces.Providers;
using Shotora.App.Interfaces.System;
using Shotora.App.Models;
using Shotora.App.Models.Constants;
using Shotora.App.Models.Utilities;

namespace Shotora.App.Services.Ocr.EasyOcr;

public class MaintenanceEasyOcrService(IPythonProvider                pythonProvider,
									   IFileFacade                    fileFacade,
									   IPackageDetectorEasyOcrService packageDetectorEasyOcrService,
									   IProcessSystemService          processSystemService,
									   IEasyOcrRuntime                easyOcrRuntime) : IMaintenanceEasyOcrService
{
	public async Task<string> EnsureSupportScriptAsync(CancellationToken cancellationToken = default)
	{
		var scriptPath   = Path.Combine(Path.GetTempPath(),       "Shotora_easyocr.py");
		var scriptSource = Path.Combine(AppContext.BaseDirectory, OcrConstants.EasyocrRunner);
		if (!File.Exists(scriptSource))
		{
			throw new FileNotFoundException("EasyOCR helper script not found.", scriptSource);
		}

		var needsUpdate = !File.Exists(scriptPath);
		if (!needsUpdate)
		{
			var sourceTime = File.GetLastWriteTimeUtc(scriptSource);
			var cachedTime = File.GetLastWriteTimeUtc(scriptPath);
			needsUpdate = sourceTime > cachedTime;
		}

		if (needsUpdate)
		{
			var scriptBody = await File.ReadAllTextAsync(scriptSource, cancellationToken);
			var dir        = Path.GetDirectoryName(scriptPath);
			fileFacade.CreateDirectory(dir);
			await File.WriteAllTextAsync(scriptPath, scriptBody, cancellationToken);
		}

		return scriptPath;
	}
	public async Task<EasyOcrViewState> GetStateAsync(EasyOcrStatusTexts texts)
	{
		if (processSystemService.GetCurrentOs() != RuntimeOs.Windows && processSystemService.GetCurrentOs() != RuntimeOs.Linux && processSystemService.GetCurrentOs() != RuntimeOs.Mac)
		{
			return new EasyOcrViewState
			{
				IsInstalled = false,
				IsBroken    = true,
				Status      = texts.NotInstalled
			};
		}

		var pythonPath     = easyOcrRuntime.PythonExePath;
		var pythonExists   = fileFacade.FileExists(pythonPath);
		var easyOcrExists  = pythonExists && packageDetectorEasyOcrService.HasEasyOcrPackage();
		var packagePresent = pythonProvider.GetEmbeddedZipPath() != null;

		var state = new EasyOcrViewState
		{
			IsInstalled = pythonExists && easyOcrExists,
			IsBroken    = pythonExists && !easyOcrExists,
			Progress    = pythonExists ? easyOcrExists ? 100 : 75 : 0,
			Status = pythonExists
				? easyOcrExists ? texts.Ready : texts.EasyOcrNotFound
				: packagePresent
					? texts.NotInstalled
					: string.Format(texts.InstallFailedFormat, "Embedded Python zip not found.")
		};

		if (pythonExists && state.IsBroken)
		{
			state.Status = texts.Broken;
		}

		return state;
	}

	public async Task DeleteAsync(EasyOcrStatusTexts texts, IProgress<string>? status = null)
	{
		status?.Report(texts.Removing);
		fileFacade.TryDelete(easyOcrRuntime.InstallDirectory);
		status?.Report(texts.Removed);
	}

	public async Task<EasyOcrRuntimeResult> EnsureRuntimeAsync(bool                installMissing    = false,
															   EasyOcrStatusTexts? texts             = null,
															   IProgress<string>?  status            = null,
															   CancellationToken   cancellationToken = default)
	{
		var textsLocal = texts ?? new EasyOcrStatusTexts();
		status?.Report(textsLocal.InstallingPython);

		var result = await pythonProvider.EnsureExtractedAsync(cancellationToken);
		if (!result.Success)
		{
			return result;
		}

		if (!packageDetectorEasyOcrService.HasEasyOcrPackage())
		{
			if (!installMissing)
			{
				return new EasyOcrRuntimeResult(false, result.PythonPath, OcrMessages.EasyOcrPackageMissing);
			}

			status?.Report(textsLocal.InstallingEasyOcr);
			var install = await pythonProvider.EnsureEasyOcrAsync(status, cancellationToken);
			if (!install.Success)
			{
				return install;
			}

			if (!packageDetectorEasyOcrService.HasEasyOcrPackage())
			{
				return new EasyOcrRuntimeResult(false, result.PythonPath, OcrMessages.EasyOcrPackageMissing);
			}
		}

		return result;
	}
}
