using System.Diagnostics.CodeAnalysis;

namespace Shotora.App.Models.System;

public enum ScreenshotPortalStatus
{
	Captured,
	Denied,
	Unavailable
}

[ExcludeFromCodeCoverage]
public sealed record ScreenshotPortalResult(ScreenshotPortalStatus Status, string? FilePath = null)
{
	public static ScreenshotPortalResult Unavailable { get; } = new(ScreenshotPortalStatus.Unavailable);

	public static ScreenshotPortalResult Denied { get; } = new(ScreenshotPortalStatus.Denied);
}
