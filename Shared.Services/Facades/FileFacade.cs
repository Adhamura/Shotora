using System.Diagnostics.CodeAnalysis;
using Shared.Interfaces.Facades;

namespace Shared.Services.Facades;

[ExcludeFromCodeCoverage]
public class FileFacade : IFileFacade
{
	public bool FileExists(string path)
	{
		return File.Exists(path);
	}
	public bool DirectoryExists(string path)
	{
		return Directory.Exists(path);
	}

	public Stream OpenRead(string path)
	{
		return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
	}

	public void CreateDirectory(string? path)
	{
		try
		{
			if (!string.IsNullOrWhiteSpace(path))
			{
				Directory.CreateDirectory(path);
			}
		}
		catch
		{
		}
	}

	public string? GetDirectoryName(string path)
	{
		return Path.GetDirectoryName(path);
	}

	public bool TryDelete(string path)
	{
		try
		{
			if (File.Exists(path))
			{
				File.Delete(path);
			}
			else if (Directory.Exists(path))
			{
				Directory.Delete(path, true);
			}
			return true;
		}
		catch
		{
			return false;
		}
	}
	public async Task WriteAll(string tempFile, byte[] body, CancellationToken cancellationToken, bool configureAwait = false)
	{
		await File.WriteAllBytesAsync(tempFile, body, cancellationToken).ConfigureAwait(configureAwait);
	}
	public async Task WriteAll(string tempFile, byte[] body)
	{
		await File.WriteAllBytesAsync(tempFile, body);
	}
	public void Move(string tempPath, string destinationPath)
	{
		File.Move(tempPath, destinationPath);
	}
	public void Copy(string sourcePath, string outputPath, bool b, bool isLinux = false)
	{
		try
		{
			File.Copy(sourcePath, outputPath, true);
			if (isLinux)
			{
#if LINUX || OSX
					File.SetUnixFileMode(outputPath,
										UnixFileMode.UserRead     |
										UnixFileMode.UserWrite    |
										UnixFileMode.UserExecute  |
										UnixFileMode.GroupRead    |
										UnixFileMode.GroupExecute |
										UnixFileMode.OtherRead    |
										UnixFileMode.OtherExecute);
#endif
			}
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
		}
	}
	public IEnumerable<string> EnumerateDirectories(string sitePackages, string easyocrDistInfo, SearchOption topDirectoryOnly)
	{
		return Directory.EnumerateDirectories(sitePackages, easyocrDistInfo, topDirectoryOnly);
	}
	public IEnumerable<string> EnumerateFiles(string sitePackages, string easyocrWhl, SearchOption topDirectoryOnly)
	{
		return Directory.EnumerateFiles(sitePackages, easyocrWhl, topDirectoryOnly);
	}
	public void WriteAllLines(string pthFile, List<string> list)
	{
		File.WriteAllLines(pthFile, list);
	}
	public FileStream Create(string destinationPath)
	{
		return File.Create(destinationPath);
	}

	public Stream CreateFile(string path)
	{
		return new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
	}
}
