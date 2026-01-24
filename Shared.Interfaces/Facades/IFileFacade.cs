namespace Shared.Interfaces.Facades;

public interface IFileFacade
{
	bool                FileExists(string           path);
	Stream              OpenRead(string             path);
	bool                DirectoryExists(string      path);
	void                CreateDirectory(string?     path);
	string?             GetDirectoryName(string     path);
	bool                TryDelete(string            path);
	Task                WriteAll(string             tempFile,     byte[]       body, CancellationToken cancellationToken, bool configureAwait = false);
	Task                WriteAll(string             tempFile,     byte[]       body);
	void                Move(string                 tempPath,     string       destinationPath);
	void                Copy(string                 sourcePath,   string       outputPath,      bool         b, bool isLinux = false);
	IEnumerable<string> EnumerateDirectories(string sitePackages, string       easyocrDistInfo, SearchOption topDirectoryOnly);
	IEnumerable<string> EnumerateFiles(string       sitePackages, string       easyocrWhl,      SearchOption topDirectoryOnly);
	void                WriteAllLines(string        pthFile,      List<string> list);
	FileStream          Create(string               destinationPath);
}
