using Microsoft.AspNetCore.Http;

namespace Migration.Contracts.Http;

public class CorrelationIdHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CorrelationIdHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var context = _httpContextAccessor.HttpContext;
        if (context != null && context.Items.TryGetValue("CorrelationId", out var id))
        {
            request.Headers.Add("X-Correlation-ID", id.ToString());
        }

        return await base.SendAsync(request, cancellationToken);
    }
}