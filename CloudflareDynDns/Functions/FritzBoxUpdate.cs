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

        [Function("FritzBoxUpdate")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "fb")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
