using Shared.Interfaces.Facades;
using Shotora.App.Interfaces.Ocr.EastOcr;
using Shotora.App.Models.Constants;

namespace Shotora.App.Services.Ocr.EasyOcr;

public class PackageDetectorEasyOcrService(IFileFacade fileFacade, IEasyOcrRuntime easyOcrRuntime) : IPackageDetectorEasyOcrService
{
	public bool HasEasyOcrPackage()
	{
		try
		{
			var sitePackages = easyOcrRuntime.SitePackagesPath;
			var installRoot  = easyOcrRuntime.InstallDirectory;
			var candidates = new[]
			{
				Path.Combine(sitePackages, OcrConstants.EasyOcrName), Path.Combine(installRoot, OcrConstants.EasyOcrName)
			};

			if (candidates.Any(fileFacade.DirectoryExists))
			{
				return true;
			}
			if (!fileFacade.DirectoryExists(sitePackages))
			{
				return false;
			}
			if (fileFacade.EnumerateDirectories(sitePackages, $"{OcrConstants.EasyOcrName}-*.dist-info", SearchOption.TopDirectoryOnly).Any())
			{
				return true;
			}
			if (fileFacade.EnumerateFiles(sitePackages, $"{OcrConstants.EasyOcrName}*.whl", SearchOption.TopDirectoryOnly).Any())
			{
				return true;
			}
		}
		catch
		{
		}

		return false;
	}
}
