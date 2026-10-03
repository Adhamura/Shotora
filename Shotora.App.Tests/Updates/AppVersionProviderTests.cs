using Shotora.App.Services.Providers;

namespace Shotora.App.Tests.Updates;

public class AppVersionProviderTests
{
	[Fact]
	public void Given_InstalledVersion_When_Resolve_Then_PrefersInstalledVersion()
	{
		Assert.Equal("2.3.4", AppVersionProvider.Resolve("2.3.4", typeof(AppVersionProviderTests).Assembly));
	}

	[Fact]
	public void Given_NoInstalledVersion_When_Resolve_Then_UsesAssemblyVersion()
	{
		var version = AppVersionProvider.Resolve(null, typeof(AppVersionProviderTests).Assembly);

		Assert.Matches(@"^\d+\.\d+\.\d+", version);
		Assert.DoesNotContain("+", version);
	}
}
