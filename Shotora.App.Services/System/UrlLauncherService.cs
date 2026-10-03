using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Shotora.App.Interfaces.System;

namespace Shotora.App.Services.System;

[ExcludeFromCodeCoverage]
public class UrlLauncherService : IUrlLauncherService
{
	public bool Open(string url)
	{
		if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) ||
		    uri.Scheme is not ("http" or "https" or "mailto"))
		{
			return false;
		}

		try
		{
			if (OperatingSystem.IsWindows())
			{
				Process.Start(new ProcessStartInfo(uri.AbsoluteUri) { UseShellExecute = true });
			}
			else
			{
				var opener = OperatingSystem.IsMacOS() ? "open" : "xdg-open";
				Process.Start(new ProcessStartInfo(opener) { ArgumentList = { uri.AbsoluteUri }, UseShellExecute = false });
			}

			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}
}
