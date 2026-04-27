using DataIngestorService.ConfigurationOptions;
using Microsoft.Extensions.Options;

namespace DataIngestorService.APIs.DelegatingHandlers;

public class WeakApiAuthHandler(IOptions<WeakAppOptions> options) : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        request.Headers.Add("X-API-KEY", options.Value.AuthorizationKeyValue);
        return base.SendAsync(request, cancellationToken);
    }
}
