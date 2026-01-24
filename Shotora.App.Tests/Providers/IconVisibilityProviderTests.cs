using Shotora.App.Models.Enums;
using Shotora.App.Services.Providers;

namespace Shotora.App.Tests.Providers;

public class IconVisibilityProviderTests
{
	private readonly IconVisibilityProvider _sut = new();

	[Theory]
	[InlineData(null, new EditorIcon[0])]
	[InlineData(new[]
	{
		null, "", "   "
	},          new EditorIcon[0])]
	[InlineData(new[]
	{
		"undo"
	}, new[]
	{
		EditorIcon.Undo
	})]
	[InlineData(new[]
	{
		" Pen ", "text", "UNDO"
	}, new[]
	{
		EditorIcon.Pen, EditorIcon.Text, EditorIcon.Undo
	})]
	[InlineData(new[]
	{
		"NonExisting", "Upload"
	}, new[]
	{
		EditorIcon.Upload
	})]
	public void Given_HiddenIds_When_BuildVisibilityMap_Then_ReturnsVisibilityForEachIcon(IEnumerable<string?>? hiddenIds, EditorIcon[] expectedHidden)
	{
		var normalizedHiddenIds = hiddenIds?.Select(id => id ?? null!);
		var result              = _sut.BuildVisibilityMap(normalizedHiddenIds);

		Assert.Equal(Enum.GetValues<EditorIcon>().Length, result.Count);

		foreach (var icon in Enum.GetValues<EditorIcon>())
		{
			Assert.True(result.ContainsKey(icon));
			var shouldBeVisible = !Enumerable.Contains(expectedHidden, icon);
			Assert.Equal(shouldBeVisible, result[icon]);
		}
	}

	[Fact]
	public void Given_VisibilityMap_When_BuildHiddenIconIds_Then_ReturnsIdsForIconsMarkedInvisible()
	{
		var mutableVisibility = _sut.BuildVisibilityMap(null).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
		mutableVisibility[EditorIcon.Text]   = false;
		mutableVisibility[EditorIcon.Blur]   = false;
		mutableVisibility[EditorIcon.Upload] = false;

		var hiddenIds = _sut.BuildHiddenIconIds(mutableVisibility).ToArray();

		Assert.Equal(new[]
		{
			"Text", "Blur", "Upload"
		}, hiddenIds);
	}
}
