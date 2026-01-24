using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Shotora.App.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
	public static void AddServicesByConvention(this IServiceCollection services, Assembly assembly, ServiceLifetime lifetime = ServiceLifetime.Singleton)
	{
		var types = assembly
			.GetTypes()
			.Where(t => t is
			{
				IsClass: true, IsAbstract: false, IsPublic: true
			});

		foreach (var type in types)
		{
			var interfaceType = type.GetInterface($"I{type.Name}");
			if (interfaceType == null)
			{
				continue;
			}

			if (services.Any(sd => sd.ServiceType == interfaceType))
			{
				continue;
			}

			services.Add(new ServiceDescriptor(interfaceType, type, lifetime));
		}
	}
}
