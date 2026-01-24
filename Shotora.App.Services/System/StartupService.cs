using System.Diagnostics;
using System.Text;
using Microsoft.Win32;
using NativeSupport.Enums;
using Shared.Interfaces.Facades;
using Shotora.App.Interfaces.System;

namespace Shotora.App.Services.System;

public class StartupService(IFileFacade fileFacade, IEnvironmentFacade environment, IProcessSystemService processSystemService) : IStartupService
{
	private const string AppName = "Shotora";

	public void SetRunOnStartup(bool enabled)
	{
		var currentOs = processSystemService.GetCurrentOs();
		if (currentOs == RuntimeOs.Windows)
		{
			SetRunOnStartupWindows(enabled);
		}
		else if (currentOs == RuntimeOs.Mac)
		{
			SetRunOnStartupMacOS(enabled);
		}
		else if (currentOs == RuntimeOs.Linux)
		{
			SetRunOnStartupLinux(enabled);
		}
	}

	private static void SetRunOnStartupWindows(bool enabled)
	{
		try
		{
			using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
			if (key == null)
			{
				return;
			}

			var exePath = GetExecutablePath();
			if (enabled)
			{
				key.SetValue(AppName, exePath, RegistryValueKind.String);
			}
			else
			{
				key.DeleteValue(AppName, false);
			}
		}
		catch
		{
		}
	}

	private void SetRunOnStartupMacOS(bool enabled)
	{
		try
		{
			var homeDir         = environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
			var launchAgentsDir = Path.Combine(homeDir,         "Library", "LaunchAgents");
			var plistPath       = Path.Combine(launchAgentsDir, $"com.{AppName.ToLowerInvariant()}.plist");

			if (enabled)
			{
				fileFacade.CreateDirectory(launchAgentsDir);
				var exePath      = GetExecutablePath();
				var plistContent = GenerateMacOSPlist(exePath);
				var plistBytes   = Encoding.UTF8.GetBytes(plistContent);
				fileFacade.WriteAll(plistPath, plistBytes, CancellationToken.None).GetAwaiter().GetResult();
			}
			else
			{
				if (fileFacade.FileExists(plistPath))
				{
					fileFacade.TryDelete(plistPath);
				}
			}
		}
		catch
		{
		}
	}

	private void SetRunOnStartupLinux(bool enabled)
	{
		try
		{
			var configDir   = Path.Combine(environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config", "autostart");
			var desktopPath = Path.Combine(configDir,                                                        $"{AppName.ToLowerInvariant()}.desktop");

			if (enabled)
			{
				fileFacade.CreateDirectory(configDir);
				var exePath        = GetExecutablePath();
				var desktopContent = GenerateLinuxDesktop(exePath);
				var desktopBytes   = Encoding.UTF8.GetBytes(desktopContent);
				fileFacade.WriteAll(desktopPath, desktopBytes, CancellationToken.None).GetAwaiter().GetResult();
			}
			else
			{
				if (fileFacade.FileExists(desktopPath))
				{
					fileFacade.TryDelete(desktopPath);
				}
			}
		}
		catch
		{
		}
	}

	private static string GetExecutablePath()
	{
		var exePath = Environment.ProcessPath;
		if (!string.IsNullOrWhiteSpace(exePath))
		{
			return exePath;
		}
		exePath = Process.GetCurrentProcess().MainModule?.FileName;
		return !string.IsNullOrWhiteSpace(exePath) ? exePath : AppContext.BaseDirectory;
	}

	private static string GenerateMacOSPlist(string exePath)
	{
		return $"""
				<?xml version="1.0" encoding="UTF-8"?>
				<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
				<plist version="1.0">
				<dict>
					<key>Label</key>
					<string>com.{AppName.ToLowerInvariant()}</string>
					<key>ProgramArguments</key>
					<array>
						<string>{exePath}</string>
					</array>
					<key>RunAtLoad</key>
					<true/>
				</dict>
				</plist>
				""";
	}

	private static string GenerateLinuxDesktop(string exePath)
	{
		return $"""
				[Desktop Entry]
				Type=Application
				Name={AppName}
				Exec={exePath}
				Hidden=false
				NoDisplay=false
				X-GNOME-Autostart-enabled=true
				""";
	}
}
