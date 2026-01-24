using System.Diagnostics.CodeAnalysis;
using Shared.Interfaces.Adapters;
using Shared.Interfaces.Facades;

namespace Shared.Services.Adapters;

[ExcludeFromCodeCoverage]
public class HttpClientAdapter(IFileFacade fileFacade) : IHttpClientAdapter
{
	public async Task<byte[]> Get(string url, CancellationToken cancellationToken, bool configureAwait = false)
	{
		using var client = new HttpClient();
		client.Timeout = TimeSpan.FromMinutes(2);
		var getPipUri = new Uri(url);
		return await client.GetByteArrayAsync(getPipUri, cancellationToken).ConfigureAwait(configureAwait);
	}
	public async Task<bool> DownloadFileAsync(string url, string destinationPath, IProgress<double>? progress, CancellationToken cancellationToken)
	{
		using var client   = new HttpClient();
		using var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
		if (!response.IsSuccessStatusCode)
		{
			return false;
		}

		var             total        = response.Content.Headers.ContentLength ?? -1;
		await using var remoteStream = await response.Content.ReadAsStreamAsync(cancellationToken);
		await using var fileStream   = fileFacade.Create(destinationPath);
		var             buffer       = new byte[81920];
		long            totalRead    = 0;
		int             read;
		while ((read = await remoteStream.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken)) > 0)
		{
			await fileStream.WriteAsync(buffer.AsMemory(0, read), cancellationToken);
			totalRead += read;
			if (total > 0)
			{
				progress?.Report(Math.Clamp(totalRead * 100.0 / total, 0, 100));
			}
		}

		progress?.Report(100);
		return true;
	}
}
