using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media;
using Microsoft.Extensions.DependencyInjection;
using Shared.Services.Facades;
using Shotora.App.Extensions;
using Shotora.App.Interfaces.Builders;
using Shotora.App.Interfaces.ViewModels;
using Shotora.App.Models.ViewModels;
using Shotora.App.Services;
using Shotora.App.Services.Builders;
using Shotora.App.Services.System;
using Shotora.App.Services.ViewModels;
using Shotora.App.Views;
using Velopack;

namespace Shotora.App;

[ExcludeFromCodeCoverage]
internal static class Program
{
	[STAThread]
	public static void Main(string[] args)
	{
		InitializeDiagnostics(args);

		try
		{
			VelopackApp.Build().Run();
		}
		catch (Exception)
		{
		}

		var services = BuildServiceProvider();

		BuildAvaloniaApp(services).StartWithClassicDesktopLifetime(args);
	}

	public static AppBuilder BuildAvaloniaApp(ServiceProvider services)
	{
		var builder = AppBuilder.Configure(services.GetRequiredService<App>)
			.UsePlatformDetect()
			.With(new FontManagerOptions
			{
				DefaultFamilyName = "avares://Shotora.App/Content/KazukiReiwa-Medium.ttf#KazukiReiwa"
			})
			.LogToTrace();

		if (OperatingSystem.IsLinux())
		{
			builder = builder.With(new X11PlatformOptions
			{
				RenderingMode           = [X11RenderingMode.Software],
				EnableSessionManagement = false,
				UseDBusMenu             = false,
				UseDBusFilePicker       = false
			});
		}

		return builder;
	}

	public static AppBuilder BuildAvaloniaApp()
	{
		var services = BuildServiceProvider();
		return BuildAvaloniaApp(services);
	}

	private static void InitializeDiagnostics(string[] args)
	{
		if (!Trace.Listeners.OfType<ConsoleTraceListener>().Any())
		{
			Trace.Listeners.Add(new ConsoleTraceListener());
		}

		AppDomain.CurrentDomain.UnhandledException += (_, e) =>
		{
			Log($"Unhandled exception (terminating={e.IsTerminating}): {e.ExceptionObject}");
		};

		TaskScheduler.UnobservedTaskException += (_, e) =>
		{
			Log($"Unobserved task exception: {e.Exception}");
		};
	}

	private static void Log(string message)
	{
		var timestamp = DateTime.UtcNow.ToString("O");
		Console.WriteLine($"[{timestamp}] {message}");
	}

	private static ServiceProvider BuildServiceProvider()
	{
		var services = new ServiceCollection();

		services.AddServicesByConvention(typeof(SettingsSystemService).Assembly);
		services.AddServicesByConvention(typeof(TextInputDialogService).Assembly);
		services.AddServicesByConvention(typeof(FileFacade).Assembly);

		services.AddSingleton<IPathBuilder, PathBuilder>();
		services.AddSingleton<MainWindowViewModel>();
		services.AddTransient<MainWindow>();
		services.AddTransient<SettingsWindow>();
		services.AddTransient<OverlayWindow>();
		services.AddTransient<AboutWindow>();
		services.AddTransient<AboutViewModel>();

		services.AddSingleton<Func<OverlayWindow>>(sp => sp.GetRequiredService<OverlayWindow>);
		services.AddSingleton<Func<SettingsWindow>>(sp => sp.GetRequiredService<SettingsWindow>);
		services.AddSingleton<Func<ISettingsViewModelController>>(sp => sp.GetRequiredService<ISettingsViewModelController>);
		services.AddSingleton<Func<MainWindow>>(sp => sp.GetRequiredService<MainWindow>);
		services.AddSingleton<Func<AboutWindow>>(sp => sp.GetRequiredService<AboutWindow>);
		services.AddSingleton<Func<AboutViewModel>>(sp => sp.GetRequiredService<AboutViewModel>);

		services.AddSingleton<App>();

		return services.BuildServiceProvider();
	}
}
