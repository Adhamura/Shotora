using Shotora.App.Models.Updates;

namespace Shotora.App.Tests.Updates;

public class ReleaseNotesFormatterTests
{
	[Theory]
	[InlineData(null)]
	[InlineData("")]
	[InlineData("  \n \n")]
	[InlineData("<!-- generated -->")]
	public void Given_EmptyNotes_When_ToPlainText_Then_ReturnsNull(string? markdown)
	{
		Assert.Null(ReleaseNotesFormatter.ToPlainText(markdown));
	}

	[Fact]
	public void Given_Markdown_When_ToPlainText_Then_StripsSyntaxAndKeepsStructure()
	{
		const string markdown = "## What's new\r\n\r\n- **Faster** OCR\n* See [docs](https://x.y) for `details`\n  - nested item\n\n\n---\nThanks!";

		var text = ReleaseNotesFormatter.ToPlainText(markdown);

		Assert.Equal("What's new\n\n• Faster OCR\n• See docs for details\n  • nested item\n\nThanks!", text);
	}

	[Fact]
	public void Given_SnakeCaseIdentifier_When_ToPlainText_Then_KeepsUnderscoresInsideWords()
	{
		Assert.Equal("Fixed my_setting_name", ReleaseNotesFormatter.ToPlainText("Fixed my_setting_name"));
	}

	[Fact]
	public void Given_HtmlImages_When_ToPlainText_Then_DropsTags()
	{
		const string markdown = "Shotora v1.0.0 - Initial Release\r\n\r\n<img width=\"1929\" alt=\"image\" src=\"https://x/y\" />\r\n<b>Bold</b> text";

		Assert.Equal("Shotora v1.0.0 - Initial Release\n\nBold text", ReleaseNotesFormatter.ToPlainText(markdown));
	}
}
