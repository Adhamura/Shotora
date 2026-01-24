using System.Diagnostics.CodeAnalysis;

namespace Shotora.App.Models.ItemModels;

[ExcludeFromCodeCoverage] public record TextResultItemModel(string Text, string FontFamily, double FontSize, bool IsBold, bool IsItalic);
