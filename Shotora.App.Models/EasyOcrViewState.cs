using System.Diagnostics.CodeAnalysis;

namespace Shotora.App.Models;

[ExcludeFromCodeCoverage]
public class EasyOcrViewState
{
	public bool   IsInstalled { get; init; }
	public bool   IsBroken    { get; set; }
	public double Progress    { get; init; }
	public string Status      { get; set; } = string.Empty;
}
