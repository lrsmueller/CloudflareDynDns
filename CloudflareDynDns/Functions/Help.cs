using CloudflareDynDns.Refresh;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CloudflareDynDns.Functions
{
    public class Help
    {
        private readonly ILogger<Help> _logger;

        public Help(ILogger<Help> logger)
        {
            _logger = logger;
        }

        [Function(nameof(Help))]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "help")] HttpRequest req)
        {
            List<string> urls = [
                $"{req.Host}/fb/?{new FritzBoxRefresh().Url()}&code=<YOURAZUREFUNCTIONCODE>",
                $"{req.Host}/update/?{new DynDnsV2Refresh().Url()}&code=<YOURAZUREFUNCTIONCODE>",
                $"{req.Host}/?{new DefaultRefresh().Url()}&code=<YOURAZUREFUNCTIONCODE>"
            ];


            return new JsonResult(urls);
        }
    }
}
