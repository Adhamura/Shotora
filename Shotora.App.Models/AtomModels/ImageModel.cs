using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Shotora.App.Models.AtomModels;

[ExcludeFromCodeCoverage]
[StructLayout(LayoutKind.Sequential)]
public struct ImageModel
{
	public int    width;
	public int    height;
	public int    xoffset;
	public int    format;
	public IntPtr data;
	public int    byte_order;
	public int    bitmap_unit;
	public int    bitmap_bit_order;
	public int    bitmap_pad;
	public int    depth;
	public int    bytes_per_line;
	public int    bits_per_pixel;
	public uint   red_mask;
	public uint   green_mask;
	public uint   blue_mask;
	public IntPtr obdata;
}
