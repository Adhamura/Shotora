namespace Shared.Interfaces.Adapters;

public interface IHttpClientAdapter
{
	Task<byte[]> Get(string               url, CancellationToken cancellationToken, bool               configureAwait = false);
	Task<bool>   DownloadFileAsync(string url, string            tempPath,          IProgress<double>? progress, CancellationToken cancellationToken);
}
