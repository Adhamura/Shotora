using Shotora.App.Models.Updates;

namespace Shotora.App.Interfaces.Updates;

public interface IUpdateStateStore
{
	Task<UpdateState> LoadAsync();

	Task SaveAsync(UpdateState state);
}
