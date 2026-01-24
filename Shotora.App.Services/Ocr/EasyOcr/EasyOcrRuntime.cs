using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Shared.Interfaces.Facades;
using Shotora.App.Interfaces.Ocr.EastOcr;
using Shotora.App.Models.Constants;

namespace Shotora.App.Services.Ocr.EasyOcr;

[ExcludeFromCodeCoverage]
public class EasyOcrRuntime(IFileFacade fileFacade) : IEasyOcrRuntime
{
	private bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

	public string InstallDirectory
	{
		get
		{
			if (IsWindows)
			{
				return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Shotora", OcrConstants.PythonName);
			}

			try
			{
				var baseDir   = AppContext.BaseDirectory;
				var candidate = Path.Combine(baseDir, OcrConstants.PythonName);
				fileFacade.CreateDirectory(candidate);
				return candidate;
			}
			catch
			{
				return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".shotora", OcrConstants.PythonName);
			}
		}
	}

	public string PythonExePath
	{
		get
		{
			if (IsWindows)
			{
				var rootExe = Path.Combine(InstallDirectory, $"{OcrConstants.PythonName}.exe");
				if (fileFacade.FileExists(rootExe))
				{
					return rootExe;
				}

				var binExe = Path.Combine(InstallDirectory, "bin", $"{OcrConstants.PythonName}.exe");
				return fileFacade.FileExists(binExe) ? binExe : rootExe;
			}

			var binPy = Path.Combine(InstallDirectory, "bin", "python3");
			if (fileFacade.FileExists(binPy))
			{
				return binPy;
			}

			var rootPy = Path.Combine(InstallDirectory, "python3");
			return fileFacade.FileExists(rootPy) ? rootPy : binPy;
		}
	}

	public string SitePackagesPath
	{
		get
		{
			if (IsWindows)
			{
				return Path.Combine(InstallDirectory, "Lib", "site-packages");
			}

			var libRoot = Path.Combine(InstallDirectory, "lib");
			if (fileFacade.DirectoryExists(libRoot))
			{
				var versioned = fileFacade.EnumerateDirectories(libRoot, $"{OcrConstants.PythonName}*", SearchOption.TopDirectoryOnly).FirstOrDefault();
				if (!string.IsNullOrWhiteSpace(versioned))
				{
					return Path.Combine(versioned, "site-packages");
				}
			}

			return Path.Combine(libRoot, "python3", "site-packages");
		}
	}
}
