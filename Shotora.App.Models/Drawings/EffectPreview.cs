using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;

namespace Shotora.App.Models.Drawings;

[ExcludeFromCodeCoverage] public readonly record struct EffectPreview(Grid Grid, Image Image, Rectangle Border);
