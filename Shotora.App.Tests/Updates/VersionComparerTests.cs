using Shotora.App.Models.Updates;

namespace Shotora.App.Tests.Updates;

public class VersionComparerTests
{
	[Theory]
	[InlineData("1.1.0",        "1.0.0",       true)]
	[InlineData("v1.0.1",       "1.0.0",       true)]
	[InlineData("2.0",          "1.9.9",       true)]
	[InlineData("1.0",          "1.0.0",       false)]
	[InlineData("1.0.0",        "1.0.0",       false)]
	[InlineData("0.9.0",        "1.0.0",       false)]
	[InlineData("1.0.0",        "1.0.0-beta",  true)]
	[InlineData("1.0.0-beta",   "1.0.0",       false)]
	[InlineData("1.0.0-beta.2", "1.0.0-beta.1", true)]
	[InlineData("1.0.0+abc",    "1.0.0",       false)]
	[InlineData("1.0.0-beta.10", "1.0.0-beta.9", true)]
	[InlineData("1.0.0-beta",   "1.0.0-beta.1", false)]
	[InlineData("1.0.0-rc.1",   "1.0.0-beta.5", true)]
	[InlineData("1.0.0.2",      "1.0.0.1",     true)]
	[InlineData("garbage",      "1.0.0",       false)]
	[InlineData("1.0.0",        "garbage",     false)]
	[InlineData(null,           "1.0.0",       false)]
	public void Given_Versions_When_IsNewer_Then_ComparesSemantically(string? candidate, string? current, bool expected)
	{
		Assert.Equal(expected, VersionComparer.IsNewer(candidate, current));
	}

	[Theory]
	[InlineData("v1.2",           "1.2.0")]
	[InlineData("1.2.3.4",        "1.2.3.4")]
	[InlineData("1.2.3.0",        "1.2.3")]
	[InlineData("V2.0.0-rc.1+sha", "2.0.0-rc.1")]
	[InlineData(" 3 ",            "3.0.0")]
	[InlineData("not-a-version",  "not-a-version")]
	public void Given_Text_When_Normalize_Then_ReturnsCanonicalVersion(string input, string expected)
	{
		Assert.Equal(expected, VersionComparer.Normalize(input));
	}
}
