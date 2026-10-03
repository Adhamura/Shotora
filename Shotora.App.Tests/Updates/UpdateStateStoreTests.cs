using System.Text;
using Moq;
using Shared.Interfaces.Facades;
using Shotora.App.Models.Updates;
using Shotora.App.Services.Updates;

namespace Shotora.App.Tests.Updates;

public class UpdateStateStoreTests
{
	private readonly Mock<IEnvironmentFacade> _environment = new();
	private readonly Mock<IFileFacade>        _files       = new();
	private readonly string                   _path        = Path.Combine("appdata", "Shotora", "update-state.json");
	private readonly UpdateStateStore         _sut;

	public UpdateStateStoreTests()
	{
		_environment.Setup(e => e.GetFolderPath(Environment.SpecialFolder.ApplicationData)).Returns("appdata");
		_sut = new UpdateStateStore(_files.Object, _environment.Object);
	}

	[Fact]
	public async Task Given_NoFile_When_Load_Then_ReturnsEmptyState()
	{
		_files.Setup(f => f.FileExists(_path)).Returns(false);

		var state = await _sut.LoadAsync();

		Assert.Null(state.LastCheckUtc);
		Assert.Null(state.SkippedVersion);
	}

	[Fact]
	public async Task Given_State_When_SaveThenLoad_Then_RoundTrips()
	{
		byte[]? written = null;
		_files.Setup(f => f.GetDirectoryName(_path)).Returns(Path.GetDirectoryName(_path));
		_files.Setup(f => f.WriteAll(_path, It.IsAny<byte[]>())).Callback<string, byte[]>((_, b) => written = b).Returns(Task.CompletedTask);

		var saved = new UpdateState { LastCheckUtc = new DateTimeOffset(2026, 10, 3, 12, 0, 0, TimeSpan.Zero), SkippedVersion = "1.2.0" };
		await _sut.SaveAsync(saved);

		Assert.NotNull(written);
		_files.Setup(f => f.FileExists(_path)).Returns(true);
		_files.Setup(f => f.OpenRead(_path)).Returns(() => new MemoryStream(written!));

		var loaded = await _sut.LoadAsync();

		Assert.Equal(saved.LastCheckUtc, loaded.LastCheckUtc);
		Assert.Equal("1.2.0",            loaded.SkippedVersion);
	}

	[Fact]
	public async Task Given_CorruptFile_When_Load_Then_ReturnsEmptyState()
	{
		_files.Setup(f => f.FileExists(_path)).Returns(true);
		_files.Setup(f => f.OpenRead(_path)).Returns(() => new MemoryStream(Encoding.UTF8.GetBytes("{not json")));

		var state = await _sut.LoadAsync();

		Assert.Null(state.LastCheckUtc);
	}
}
