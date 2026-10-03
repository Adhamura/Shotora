using System.Text.Json;
using Shared.Interfaces.Facades;
using Shotora.App.Interfaces.Updates;
using Shotora.App.Models.Updates;

namespace Shotora.App.Services.Updates;

public class UpdateStateStore : IUpdateStateStore
{
	private readonly IFileFacade   _fileFacade;
	private readonly SemaphoreSlim _gate = new(1, 1);
	private readonly string        _path;

	public UpdateStateStore(IFileFacade fileFacade, IEnvironmentFacade environment)
	{
		_fileFacade = fileFacade;
		_path       = Path.Combine(environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Shotora", "update-state.json");
	}

	public async Task<UpdateState> LoadAsync()
	{
		await _gate.WaitAsync().ConfigureAwait(false);
		try
		{
			if (!_fileFacade.FileExists(_path))
			{
				return new UpdateState();
			}

			await using var stream = _fileFacade.OpenRead(_path);
			return await JsonSerializer.DeserializeAsync<UpdateState>(stream).ConfigureAwait(false) ?? new UpdateState();
		}
		catch (Exception)
		{
			// A corrupt or unreadable file only means the next check happens sooner.
			return new UpdateState();
		}
		finally
		{
			_gate.Release();
		}
	}

	public async Task SaveAsync(UpdateState state)
	{
		await _gate.WaitAsync().ConfigureAwait(false);
		try
		{
			_fileFacade.CreateDirectory(_fileFacade.GetDirectoryName(_path));
			var payload = JsonSerializer.SerializeToUtf8Bytes(state, new JsonSerializerOptions { WriteIndented = true });
			await _fileFacade.WriteAll(_path, payload).ConfigureAwait(false);
		}
		catch (Exception)
		{
			// Persisting updater bookkeeping is best effort.
		}
		finally
		{
			_gate.Release();
		}
	}
}
