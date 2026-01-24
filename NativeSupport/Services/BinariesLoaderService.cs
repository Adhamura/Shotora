using System.Runtime.InteropServices;
using NativeSupport.Constants;

namespace NativeSupport.Services;

public static class BinariesLoaderService
{
	private static readonly Lock         Gate = new();
	private static          bool         _loaded;
	private static          string?      _loadedRoot;
	private static readonly List<IntPtr> Handles = [];

	public static void EnsureLoaded()
	{
		if (_loaded && _loadedRoot is not null)
		{
			return;
		}

		lock (Gate)
		{
			if (_loaded && _loadedRoot is not null)
			{
				return;
			}

			var interopArchFolder = NativeArchitecture.GetCurrentArchFolder.Contains("64") ? "x64" : "x86";

			var root = ResolveBinariesRoot();
			CopyLibrariesToBase(root);
			CopyLibrariesToArchFolder(root, interopArchFolder);

			var loadOrder = NativeArchitecture.GetPreferredLoadOrder;
			var loadedSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

			foreach (var name in loadOrder)
			{
				var candidate = Directory.GetFiles(root, name, SearchOption.TopDirectoryOnly)
					.FirstOrDefault(NativeArchitecture.IsLibraryForPlatform);
				if (candidate is null)
				{
					continue;
				}

				LoadLibrary(candidate);
				loadedSet.Add(Path.GetFileName(candidate));
			}

			foreach (var file in Directory.GetFiles(root))
			{
				var fileName = Path.GetFileName(file);
				if (loadedSet.Contains(fileName))
				{
					continue;
				}

				if (NativeArchitecture.IsLibraryForPlatform(file))
				{
					LoadLibrary(file);
				}
			}

			_loaded     = true;
			_loadedRoot = root;
			SetSearchEnv(root);
		}
	}

	private static void LoadLibrary(string path)
	{
		var handle = NativeLibrary.Load(path);
		Handles.Add(handle);
	}

	private static string ResolveBinariesRoot()
	{
		var baseDir = AppContext.BaseDirectory;
		var candidates = new List<string>
		{
			Path.Combine(baseDir, NativeArchitecture.BinariesFolder, NativeArchitecture.GetTesseractOsFolder, NativeArchitecture.GetCurrentArchFolder),
			Path.Combine(baseDir, NativeArchitecture.GetCurrentArchFolder),
			Path.Combine(baseDir, "x64"),
			Path.Combine(baseDir, "x86"),
			baseDir
		};

		var parent = Directory.GetParent(baseDir)?.FullName;
		if (!string.IsNullOrEmpty(parent))
		{
			candidates.Add(Path.Combine(parent, NativeArchitecture.BinariesFolder, NativeArchitecture.GetTesseractOsFolder, NativeArchitecture.GetCurrentArchFolder));
			candidates.Add(Path.Combine(parent, NativeArchitecture.GetCurrentArchFolder));
			candidates.Add(Path.Combine(parent, "x64"));
			candidates.Add(Path.Combine(parent, "x86"));
			candidates.Add(parent);
		}

		if (Directory.Exists(baseDir))
		{
			candidates.AddRange(Directory.GetDirectories(baseDir, NativeArchitecture.BinariesFolder, SearchOption.AllDirectories).Select(dir => Path.Combine(dir, NativeArchitecture.GetCurrentArchFolder)));
		}

		var root = candidates.FirstOrDefault(Directory.Exists);
		if (root is null)
		{
			throw new InvalidOperationException(
				$"Native binaries not found for {NativeArchitecture.GetTesseractOsFolder}/{NativeArchitecture.GetCurrentArchFolder}. Searched: {string.Join(", ", candidates)}");
		}

		var platformRoot = candidates
			.Where(Directory.Exists)
			.Select(dir => new
			{
				dir,
				hasLib = Directory.GetFiles(dir).Any(NativeArchitecture.IsLibraryForPlatform)
			})
			.FirstOrDefault(x => x.hasLib);

		return platformRoot is not null ? platformRoot.dir : root;
	}

	private static void CopyLibrariesToBase(string root)
	{
		var baseDir = AppContext.BaseDirectory;
		if (string.Equals(root.TrimEnd(Path.DirectorySeparatorChar), baseDir.TrimEnd(Path.DirectorySeparatorChar), StringComparison.OrdinalIgnoreCase))
		{
			return;
		}

		foreach (var file in Directory.GetFiles(root))
		{
			if (!NativeArchitecture.IsLibraryForPlatform(file))
			{
				continue;
			}

			var dest = Path.Combine(baseDir, Path.GetFileName(file));
			try
			{
				if (!File.Exists(dest))
				{
					File.Copy(file, dest);
				}
			}
			catch
			{
			}
		}
	}

	private static void CopyLibrariesToArchFolder(string root, string archFolder)
	{
		var baseDir = AppContext.BaseDirectory;
		var target  = Path.Combine(baseDir, archFolder);
		Directory.CreateDirectory(target);

		foreach (var file in Directory.GetFiles(root))
		{
			if (!NativeArchitecture.IsLibraryForPlatform(file))
			{
				continue;
			}

			var dest = Path.Combine(target, Path.GetFileName(file));
			try
			{
				if (!File.Exists(dest))
				{
					File.Copy(file, dest);
				}
			}
			catch
			{
			}
		}
	}

	private static void SetSearchEnv(string root)
	{
		var pathVar   = NativeArchitecture.GetPathVar;
		var existing  = Environment.GetEnvironmentVariable(pathVar) ?? string.Empty;
		var separator = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ";" : ":";
		var paths     = new List<string>(existing.Split(separator, StringSplitOptions.RemoveEmptyEntries));
		var baseDir   = AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar);
		var archDir   = Path.Combine(baseDir, "x64");

		foreach (var candidate in new[]
				 {
					 root, baseDir, archDir
				 })
		{
			if (!paths.Contains(candidate, StringComparer.OrdinalIgnoreCase))
			{
				paths.Insert(0, candidate);
			}
		}

		var updated = string.Join(separator, paths);
		Environment.SetEnvironmentVariable(pathVar, updated);
	}
}
