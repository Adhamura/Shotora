namespace Shotora.App.Models.Updates;

/// <summary>
///     Framework-neutral description of an update discovered by Velopack.
/// </summary>
public sealed record VelopackUpdateModel(string Version, string? ReleaseNotes);
