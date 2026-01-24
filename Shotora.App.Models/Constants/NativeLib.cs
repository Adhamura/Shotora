using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Shotora.App.Models.Constants;

[ExcludeFromCodeCoverage]
public static class NativeLib
{
	public const  string TesseractAlias50 = "libtesseract50.so";
	public const  string LeptonicaExact   = "libleptonica-1.82.0.so";
	private const string DepJpeg          = "libjpeg.so.62";
	private const string DepPng           = "libpng16.so.16";
	private const string DepWebp          = "libwebp.so.7";
	private const string DepJbig          = "libjbig.so.0";
	private const string LibDl            = "libdl.so";
	private const string LibDl2           = "libdl.so.2";
	private const string LibDl6           = "libdl.so.6";
	private const string LibDlGcc         = "libgcc_s.so.1";
	private const string LibDlOpen        = "libopenjp2.so";
	private const string LibDlDc          = "libstdc++.so.6";
	private const string LibDlTTiff5      = "libtiff.so.5";
	private const string LibDlTTiff6      = "libtiff.so.6";

	public static readonly string[] Candidates = [TesseractAlias50, LeptonicaExact, DepJpeg, DepPng, DepWebp, DepJbig, LibDl, LibDlGcc, LibDlOpen, LibDlDc, LibDlTTiff5, LibDlTTiff6, LibDl2, LibDl6];

	public static void PreloadLibraries(string nativeDirs)
	{
		foreach (var file in Candidates)
		{
			var path = Path.Combine(nativeDirs, file);
			if (!File.Exists(path))
			{
				continue;
			}
			try
			{
				NativeLibrary.Load(path);
			}
			catch (Exception)
			{
			}
		}
	}
}
