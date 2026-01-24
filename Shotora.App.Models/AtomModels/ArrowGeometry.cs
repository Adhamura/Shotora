using System.Diagnostics.CodeAnalysis;
using Avalonia;

namespace Shotora.App.Models.AtomModels;

[ExcludeFromCodeCoverage]
public readonly struct ArrowGeometry(Point start, Point tip, Point lineEnd, Point left, Point right, bool isValid)
{
	public Point Start   { get; } = start;
	public Point Tip     { get; } = tip;
	public Point LineEnd { get; } = lineEnd;
	public Point Left    { get; } = left;
	public Point Right   { get; } = right;
	public bool  IsValid { get; } = isValid;
}
