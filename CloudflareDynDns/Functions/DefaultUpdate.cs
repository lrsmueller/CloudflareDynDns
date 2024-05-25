using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using CloudFlare.Client;
using CloudFlare.Client.Api.Authentication;
using CloudFlare.Client.Api.Zones.DnsRecord;
using CloudFlare.Client.Api.Result;

using CloudflareDynDns.Refresh;
using CloudflareDynDns.Response;
using CloudflareDynDns.Helper;

namespace CloudflareDynDns.Functions;

public partial class DefaultUpdate
{
    private readonly ILogger<DefaultUpdate> _logger;

    public DefaultUpdate(ILogger<DefaultUpdate> logger)
    {
        _logger = logger;
    }

    [Function(nameof(DefaultUpdate))]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "")] HttpRequest req)
    {
        _logger.LogMetric("DEFAULT_CALL", 1);
        var refresh = new DefaultRefresh(req, _logger);
        if (!refresh.Success)
        {
            _logger.LogMetric("PARAMETER_ERROR", refresh.Errors.Count);
            return new DynDnsResponse(refresh.Errors).Result;
        }

        return await DynDnsHelper.UpdateDynDnsEntry(refresh, _logger);

    }


}
