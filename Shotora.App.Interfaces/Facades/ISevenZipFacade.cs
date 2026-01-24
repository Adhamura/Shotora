namespace Shotora.App.Interfaces.Facades;

public interface ISevenZipFacade
{
	void ExtractToDir(string zipPath, string destination, bool overwrite, bool fullPaths, string password);
}
