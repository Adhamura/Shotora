using System.Text;
using System.Text.RegularExpressions;

namespace Shotora.App.Models.Updates;

/// <summary>
///     Converts GitHub-flavoured Markdown release notes into readable plain text for display in a TextBlock.
/// </summary>
public static partial class ReleaseNotesFormatter
{
	public static string? ToPlainText(string? markdown)
	{
		if (string.IsNullOrWhiteSpace(markdown))
		{
			return null;
		}

		var builder       = new StringBuilder();
		var previousBlank = true;

		foreach (var rawLine in markdown.Replace("\r\n", "\n").Split('\n'))
		{
			var line = rawLine.TrimEnd();

			if (string.IsNullOrWhiteSpace(line) || line.StartsWith("<!--") || HorizontalRule().IsMatch(line))
			{
				if (!previousBlank)
				{
					builder.Append('\n');
					previousBlank = true;
				}

				continue;
			}

			line = Heading().Replace(line, string.Empty);
			line = Bullet().Replace(line, m => $"{m.Groups["indent"].Value}• ");
			line = Image().Replace(line, string.Empty);
			line = HtmlTag().Replace(line, string.Empty);
			line = Link().Replace(line, "$1");
			line = Emphasis().Replace(line, "$2");
			line = UnderscoreEmphasis().Replace(line, "$2");
			line = line.Replace("`", string.Empty);

			if (string.IsNullOrWhiteSpace(line))
			{
				continue;
			}

			builder.Append(line).Append('\n');
			previousBlank = false;
		}

		var text = builder.ToString().Trim();
		return text.Length == 0 ? null : text;
	}

	[GeneratedRegex(@"^\s{0,3}#{1,6}\s+")]
	private static partial Regex Heading();

	[GeneratedRegex(@"^(?<indent>\s*)[-*+]\s+(\[[ xX]\]\s+)?")]
	private static partial Regex Bullet();

	[GeneratedRegex(@"!\[[^\]]*\]\([^)]*\)")]
	private static partial Regex Image();

	[GeneratedRegex(@"</?[a-zA-Z][^>]*>")]
	private static partial Regex HtmlTag();

	[GeneratedRegex(@"\[([^\]]+)\]\([^)]*\)")]
	private static partial Regex Link();

	[GeneratedRegex(@"(\*\*|\*)(?=\S)(.+?)(?<=\S)\1")]
	private static partial Regex Emphasis();

	// Underscore emphasis only at word boundaries, so identifiers like my_setting_name survive.
	[GeneratedRegex(@"(?<!\w)(__|_)(?=\S)(.+?)(?<=\S)\1(?!\w)")]
	private static partial Regex UnderscoreEmphasis();

	[GeneratedRegex(@"^\s*([-*_])(\s*\1){2,}\s*$")]
	private static partial Regex HorizontalRule();
}
