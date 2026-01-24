namespace Shared.Interfaces.Facades;

public interface IEnvironmentFacade
{
	Environment.SpecialFolder MyPictures { get; set; }
	string                    GetFolderPath(Environment.SpecialFolder folder);
	string                    GetEnvironmentVariable(string           path);
}
