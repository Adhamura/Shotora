using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using NativeSupport.Adapters;
using NativeSupport.Enums;
using NativeSupport.Extensions;

namespace NativeSupport.Constants;

[ExcludeFromCodeCoverage]
public static class NativeArchitecture
{
	public const string BinariesFolder = "Binaries";

	public static string GetCurrentArchFolder => RuntimeInformation.ProcessArchitecture switch
	{
		Architecture.Arm64 => "arm64",
		Architecture.X86   => "x86",
		_                  => "x64"
	};

	public static string GetPathVar => GetCurrentOs() switch
	{
		RuntimeOs.Mac   => "DYLD_LIBRARY_PATH",
		RuntimeOs.Linux => "LD_LIBRARY_PATH",
		_               => "PATH"
	};

	public static string GetTesseractOsFolder => GetCurrentOs() switch
	{
		RuntimeOs.Mac     => "Mac",
		RuntimeOs.Linux   => "Linux",
		RuntimeOs.Windows => "Windows",
		_                 => ""
	};

	public static IEnumerable<string> GetPreferredLoadOrder => GetCurrentOs() switch
	{
		RuntimeOs.Linux => new[]
		{
			"libdl*.so", "libgcc*.so*", "libstdc++*.so*", "libjpeg*.so*", "libpng*.so*", "libtiff*.so*", "libopenjp2*.so*", "libleptonica*.so*", "libtesseract*.so*"
		},
		RuntimeOs.Mac => new[]
		{
			"libjpeg*.dylib", "libpng*.dylib", "libtiff*.dylib", "libopenjp2*.dylib", "libleptonica*.dylib", "libtesseract*.dylib"
		},
		_ => new[]
		{
			"leptonica-*.dll", "tesseract*.dll"
		}
	};
	private static RuntimeOs GetCurrentOs()
	{
		var match = EnumAdapter.TryFind<RuntimeOs>(value =>
		{
			var platform = EnumAdapter.TryGetStaticPropertyValue<RuntimeOs, OsPlatformNameAttribute, OSPlatform>(value, attribute => attribute.Name);
			return platform.HasValue && RuntimeInformation.IsOSPlatform(platform.Value);
		});

		return match ?? RuntimeOs.Other;
	}

	public static bool IsLibraryForPlatform(string path)
	{
		var ext = Path.GetExtension(path);
		return GetCurrentOs() switch
		{
			RuntimeOs.Windows => ext.Equals(".dll",   StringComparison.OrdinalIgnoreCase),
			RuntimeOs.Mac     => ext.Equals(".dylib", StringComparison.OrdinalIgnoreCase),
			_                 => ext.Equals(".so",    StringComparison.OrdinalIgnoreCase)
		};
	}

	public static string GetTesseractExecutableName()
	{
		return GetCurrentOs() switch
		{
			RuntimeOs.Windows => "tesseract.exe",
			_                 => "tesseract"
		};
	}

	public static string? GetUnixLibraryEnvVar()
	{
		return GetCurrentOs() switch
		{
			RuntimeOs.Mac   => "DYLD_LIBRARY_PATH",
			RuntimeOs.Linux => "LD_LIBRARY_PATH",
			_               => null
		};
	}
}
