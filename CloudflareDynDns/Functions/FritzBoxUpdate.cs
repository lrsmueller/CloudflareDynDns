using CloudflareDynDns.Helper;
using CloudflareDynDns.Refresh;
using CloudflareDynDns.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CloudflareDynDns.Functions
{
    public class FritzBoxUpdate
    {
        private readonly ILogger<FritzBoxUpdate> _logger;

        public FritzBoxUpdate(ILogger<FritzBoxUpdate> logger)
        {
            _logger = logger;
        }

        [Function(nameof(FritzBoxUpdate))]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "fb")] HttpRequest req)
        {
            _logger.LogMetric("FB_CALL", 1);
            var refresh = new FritzBoxRefresh(req, _logger);
            if (!refresh.Success)
            {
                _logger.LogMetric("PARAMETER_ERROR", refresh.Errors.Count);
                return new DynDnsResponse(refresh.Errors).Result;
            }
            return await DynDnsHelper.UpdateDynDnsEntry(refresh, _logger);
        }
    }
}
