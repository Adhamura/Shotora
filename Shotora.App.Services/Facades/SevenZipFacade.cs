using SevenZipLib;
using Shotora.App.Interfaces.Facades;

namespace Shotora.App.Services.Facades;

public class SevenZipFacade : ISevenZipFacade
{
	public void ExtractToDir(string zipPath, string destination, bool overwrite, bool fullPaths, string password)
	{
		SevenZip.ExtractToDir(zipPath, destination, overwrite, fullPaths, password);
	}
}
