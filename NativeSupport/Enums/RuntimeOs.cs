using System.Runtime.InteropServices;
using NativeSupport.Extensions;

namespace NativeSupport.Enums;

public enum RuntimeOs
{
	[OsPlatformName(nameof(OSPlatform.Windows))]
	Windows,
	[OsPlatformName(nameof(OSPlatform.Linux))]
	Linux,
	[OsPlatformName(nameof(OSPlatform.OSX))]
	Mac,
	Other
}
