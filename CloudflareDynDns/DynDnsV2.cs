using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CloudflareDynDns
{
    public class DynDnsV2
    {
        private readonly ILogger<DynDnsV2> _logger;

        public DynDnsV2(ILogger<DynDnsV2> logger)
        {
            _logger = logger;
        }

		//http://{username}:{password}@upddyndns.com/update?hostname={yourhostname}&myip={ipaddress},{ip6address}&
		//http://test:***REMOVED***@localhost:7022/api/update?hostname=***REMOVED***2.***REMOVED***&myip=217.80.70.77,2001:db8:3333:4444:5555:6666:7777:8888&
		//password -> token
		//hostname -> record.zone
		//myip -> split(,)
		[Function("DynDnsV2")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get",Route = "update")] HttpRequest req)
        {
			
			_logger.LogMetric("DYNDNSV2_CALL", 1);
			var refresh = new DynDnsV2RefreshObject(req, _logger);
			if (!refresh.Success)
			{
				_logger.LogMetric("PARAMETER_ERROR", refresh.Errors.Count);
				return new ResponseObject(refresh.Errors).Result;
			}
			return await DynDnsService.UpdateDynDnsEntry(refresh, _logger);
		}
    }
}
