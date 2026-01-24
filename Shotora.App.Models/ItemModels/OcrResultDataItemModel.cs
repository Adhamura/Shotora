using System.Diagnostics.CodeAnalysis;

namespace Shotora.App.Models.ItemModels;

[ExcludeFromCodeCoverage]
public record OcrResultDataItemModel(string? Text, string? Error)
{
	public bool Success => !string.IsNullOrWhiteSpace(Text);
}
