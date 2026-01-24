namespace Shotora.App.Interfaces.Ocr.EastOcr;

public interface IEasyOcrRuntime
{
	string InstallDirectory { get; }
	string PythonExePath    { get; }
	string SitePackagesPath { get; }
}
