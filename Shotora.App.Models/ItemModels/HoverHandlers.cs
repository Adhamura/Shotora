using System.Diagnostics.CodeAnalysis;
using Avalonia.Input;

namespace Shotora.App.Models.ItemModels;

[ExcludeFromCodeCoverage]
public sealed class HoverHandlers
{
	public EventHandler<PointerEventArgs>? Enter { get; init; }
	public EventHandler<PointerEventArgs>? Exit  { get; init; }
}
