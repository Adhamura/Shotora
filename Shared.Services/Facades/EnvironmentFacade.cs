using System.Diagnostics.CodeAnalysis;
using Shared.Interfaces.Facades;

namespace Shared.Services.Facades;

[ExcludeFromCodeCoverage]
public class EnvironmentFacade : IEnvironmentFacade
{
	public string GetFolderPath(Environment.SpecialFolder folder)
	{
		return Environment.GetFolderPath(folder);
	}
	public Environment.SpecialFolder MyPictures { get; set; } = Environment.SpecialFolder.MyPictures;
	public string GetEnvironmentVariable(string path)
	{
		return $"{Environment.GetEnvironmentVariable(path)}";
	}
}
