using System.Diagnostics.CodeAnalysis;
using Avalonia.Threading;
using Shotora.App.Interfaces.Facades;

namespace Shotora.App.Services.Facades;

[ExcludeFromCodeCoverage]
public class DispatcherFacade : IDispatcherFacade
{
	public void Post(Action action, DispatcherPriority loaded)
	{
		Dispatcher.UIThread.Post(action, loaded);
	}
	public bool CheckAccess()
	{
		return Dispatcher.UIThread.CheckAccess();
	}
	public async Task InvokeAsync(Action action, DispatcherPriority send)
	{
		await Dispatcher.UIThread.InvokeAsync(action, send);
	}
	public async Task InvokeAsync(Action action)
	{
		await Dispatcher.UIThread.InvokeAsync(action);
	}
}
