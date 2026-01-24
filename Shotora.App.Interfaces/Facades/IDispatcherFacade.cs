using Avalonia.Threading;

namespace Shotora.App.Interfaces.Facades;

public interface IDispatcherFacade
{
	void Post(Action action, DispatcherPriority loaded);
	bool CheckAccess();
	Task InvokeAsync(Action action, DispatcherPriority send);
	Task InvokeAsync(Action action);
}
