using Avalonia.Controls;
using Shotora.App.Models;
using Shotora.App.Models.ItemModels;

namespace Shotora.App.Interfaces.Abstractions;

public interface ITextInputDialogService
{
	Task<TextResultItemModel?> ShowAsync(Window owner, double defaultFontSize, AnnotationItem? existing = null);
}
