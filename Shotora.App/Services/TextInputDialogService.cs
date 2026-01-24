using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Avalonia.Controls;
using Shotora.App.Interfaces.Abstractions;
using Shotora.App.Models;
using Shotora.App.Models.ItemModels;
using Shotora.App.Views;

namespace Shotora.App.Services;

[ExcludeFromCodeCoverage]
public class TextInputDialogService : ITextInputDialogService
{
	public async Task<TextResultItemModel?> ShowAsync(Window owner, double defaultFontSize, AnnotationItem? existing = null)
	{
		var dialog = existing == null
			? new TextInputWindow(fontSize: defaultFontSize)
			: new TextInputWindow(existing.Text,
				existing.FontFamily,
				existing.FontSize > 0 ? existing.FontSize : defaultFontSize,
				existing.Bold,
				existing.Italic);

		var result = await dialog.ShowDialog<bool>(owner);
		if (!result || string.IsNullOrWhiteSpace(dialog.TextValue))
		{
			return null;
		}

		return new TextResultItemModel(
			dialog.TextValue,
			dialog.FontFamilyValue,
			dialog.FontSizeValue,
			dialog.IsBold,
			dialog.IsItalic);
	}
}
