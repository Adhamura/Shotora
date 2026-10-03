using System.Text;
using Moq;
using Shared.Interfaces.Adapters;
using Shotora.App.Models.Updates;
using Shotora.App.Services.Updates;

namespace Shotora.App.Tests.Updates;

public class GitHubReleaseClientTests
{
	private readonly Mock<IHttpClientAdapter> _http = new(MockBehavior.Strict);
	private readonly GitHubReleaseClient      _sut;

	public GitHubReleaseClientTests()
	{
		_sut = new GitHubReleaseClient(_http.Object);
	}

	[Fact]
	public async Task Given_LatestReleaseJson_When_GetLatestReleaseAsync_Then_ParsesRelease()
	{
		const string json = """{"tag_name":"v1.2.0","name":"Shotora 1.2","body":"Notes","html_url":"https://github.com/Adhamura/Shotora/releases/tag/v1.2.0","prerelease":false,"draft":false}""";
		_http.Setup(h => h.Get(UpdateConstants.LatestReleaseApiUrl, It.IsAny<CancellationToken>(), false))
			.ReturnsAsync(Encoding.UTF8.GetBytes(json));

		var release = await _sut.GetLatestReleaseAsync(CancellationToken.None);

		Assert.NotNull(release);
		Assert.Equal("v1.2.0", release.TagName);
		Assert.Equal("Notes",  release.Body);
		Assert.Equal("https://github.com/Adhamura/Shotora/releases/tag/v1.2.0", release.HtmlUrl);
	}

	[Theory]
	[InlineData("""{"tag_name":"v2.0.0-beta","prerelease":true}""")]
	[InlineData("""{"tag_name":"v2.0.0","draft":true}""")]
	[InlineData("""{"name":"no tag"}""")]
	[InlineData("""[]""")]
	[InlineData("")]
	public void Given_UnusablePayload_When_Parse_Then_ReturnsNull(string json)
	{
		Assert.Null(GitHubReleaseClient.Parse(Encoding.UTF8.GetBytes(json)));
	}

	[Fact]
	public void Given_MissingHtmlUrl_When_Parse_Then_FallsBackToLatestReleasePage()
	{
		var release = GitHubReleaseClient.Parse(Encoding.UTF8.GetBytes("""{"tag_name":"1.0"}"""));

		Assert.NotNull(release);
		Assert.Equal(UpdateConstants.LatestReleaseUrl, release.HtmlUrl);
	}
}
