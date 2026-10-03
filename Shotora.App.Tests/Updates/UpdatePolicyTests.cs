using Shotora.App.Models.Updates;

namespace Shotora.App.Tests.Updates;

public class UpdatePolicyTests
{
	private static readonly DateTimeOffset Now = new(2026, 10, 3, 12, 0, 0, TimeSpan.Zero);

	[Fact]
	public void Given_AutoCheckDisabled_When_ShouldRunAutomaticCheck_Then_False()
	{
		Assert.False(UpdatePolicy.ShouldRunAutomaticCheck(false, null, Now));
	}

	[Fact]
	public void Given_NeverChecked_When_ShouldRunAutomaticCheck_Then_True()
	{
		Assert.True(UpdatePolicy.ShouldRunAutomaticCheck(true, null, Now));
	}

	[Theory]
	[InlineData(1,   false)]
	[InlineData(23,  false)]
	[InlineData(24,  true)]
	[InlineData(100, true)]
	[InlineData(-5,  true)]
	public void Given_LastCheck_When_ShouldRunAutomaticCheck_Then_RespectsInterval(int hoursAgo, bool expected)
	{
		Assert.Equal(expected, UpdatePolicy.ShouldRunAutomaticCheck(true, Now.AddHours(-hoursAgo), Now));
	}

	[Theory]
	[InlineData(null,     false, true)]
	[InlineData("1.2.0",  false, false)]
	[InlineData("v1.2.0", false, false)]
	[InlineData("1.1.0",  false, true)]
	[InlineData("1.2.0",  true,  true)]
	public void Given_AvailableUpdate_When_ShouldPromptUser_Then_HonoursSkippedVersion(string? skipped, bool userInitiated, bool expected)
	{
		var result = new UpdateCheckResult(UpdateStatus.UpdateAvailable, "1.0.0", "1.2.0");

		Assert.Equal(expected, UpdatePolicy.ShouldPromptUser(result, skipped, userInitiated));
	}

	[Theory]
	[InlineData(true,  true)]
	[InlineData(false, false)]
	public void Given_NoUpdate_When_ShouldPromptUser_Then_OnlyForManualChecks(bool userInitiated, bool expected)
	{
		var result = UpdateCheckResult.UpToDate("1.0.0", "1.0.0", null);

		Assert.Equal(expected, UpdatePolicy.ShouldPromptUser(result, null, userInitiated));
	}
}
