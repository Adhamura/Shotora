using Shotora.App.Models.Enums;

namespace Shotora.App.Interfaces.Providers;

public interface IIconVisibilityProvider
{
	IReadOnlyDictionary<EditorIcon, bool> BuildVisibilityMap(IEnumerable<string>?                  hiddenIconIds);
	IEnumerable<string>                   BuildHiddenIconIds(IReadOnlyDictionary<EditorIcon, bool> visibility);
}
