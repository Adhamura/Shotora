using System.Diagnostics.CodeAnalysis;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using Shotora.App.Interfaces.System;

namespace Shotora.App.Services;

[ExcludeFromCodeCoverage]
public class ApplicationShutdownService : IApplicationShutdownService
{
	public void Shutdown()
	{
		Dispatcher.UIThread.Post(() =>
		{
			if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
			{
				desktop.Shutdown();
			}
		});
	}
}
