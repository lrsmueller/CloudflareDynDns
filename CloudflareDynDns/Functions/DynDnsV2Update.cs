using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

using CloudflareDynDns.Refresh;
using CloudflareDynDns.Response;
using CloudflareDynDns.Helper;

namespace CloudflareDynDns.Functions
{
    public class DynDnsV2Update
    {
        private readonly ILogger<DynDnsV2Update> _logger;

        public DynDnsV2Update(ILogger<DynDnsV2Update> logger)
        {
            _logger = logger;
        }

        [Function(nameof(DynDnsV2Update))]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "update")] HttpRequest req)
        {

            _logger.LogMetric("DYNDNSV2_CALL", 1);
            var refresh = new DynDnsV2Refresh(req, _logger);
            if (!refresh.Success)
            {
                _logger.LogMetric("PARAMETER_ERROR", refresh.Errors.Count);
                return new DynDnsResponse(refresh.Errors).Result;
            }
            return await DynDnsHelper.UpdateDynDnsEntry(refresh, _logger);
        }
    }
}
