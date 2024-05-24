using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using CloudFlare.Client;
using CloudFlare.Client.Api.Authentication;
using CloudFlare.Client.Api.Zones.DnsRecord;
using CloudFlare.Client.Enumerators;
using Microsoft.AspNetCore.Components.Web;
using CloudFlare.Client.Api.Result;
namespace CloudflareDynDns;

public class Update
{
    private readonly ILogger<Update> _logger;

    public Update(ILogger<Update> logger)
    {
        _logger = logger;
    }

    //https://dyndns.nicoo.org/?token=<pass>&record=***REMOVED***&zone=***REMOVED***&ipv4=<ipaddr>&ipv6=<ip6addr>
    [Function(nameof(Update))]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "nic/update")] HttpRequest req)
    {
        var refresh = new RefreshObject(req);
        if (!refresh.Success)
        {
            return new BadRequestObjectResult(refresh.Errors);
        }
        
        using var CloudflareClient = new CloudFlareClient(new ApiTokenAuthentication(refresh.Token));
        var zones = await CloudflareClient.Zones.GetAsync(new CloudFlare.Client.Api.Zones.ZoneFilter() { Name = refresh.Zone });
        if (!zones.Success)
        {
            return new NotFoundObjectResult(zones.Errors);
        }
        var zoneId = zones.Result.First().Id;
        var records = await CloudflareClient.Zones.DnsRecords.GetAsync(zoneId,new DnsRecordFilter() { Name= $"{refresh.Record}.{refresh.Zone}" });
		if (!records.Success)
		{
			return new NotFoundObjectResult(records.Errors);
		}
		var ipv4Result = await CloudflareClient.Zones.DnsRecords.UpdateAsync(zoneId, records.Result.First().Id, refresh.GetIpv4Entry);
        CloudFlareResult<DnsRecord> ipv6Result = null;
        if (refresh.HasIpv6)
        {
            ipv6Result = await CloudflareClient.Zones.DnsRecords.UpdateAsync(zoneId, records.Result.First().Id, refresh.GetIpv6Entry);
		}
        if(ipv4Result.Success && (ipv6Result is null || ipv6Result.Success))
        {
            return new OkObjectResult("Ip aktualisisert");
        } else
        {
            var errors = ipv6Result is not null ? [.. ipv4Result.Errors, .. ipv6Result.Errors] : ipv4Result.Errors;

			return new ConflictObjectResult(errors);
        }

    }

    public class RefreshObject
    {
        public RefreshObject(HttpRequest request)
        {
            _request = request;
            foreach (var parameter in Parameters)
            {
                CheckParameter(parameter.Key, parameter.Value);
            }


            Success = Errors.Count <= 0;
        }
        private HttpRequest _request;
        private Dictionary<string, bool> Parameters = new Dictionary<string, bool>()
        {
            { TokenParameter, true },
            { RecordParameter, true },
            { ZoneParameter, true },
            { Ipv4Parameter, true },
            { Ipv6Parameter, false },
        };

        public bool Success { get; init; }

        public bool HasIpv6 => CheckParameter(Ipv6Parameter, false);

        private List<string> _errors = new List<string>();
        public IReadOnlyList<string> Errors => _errors;

        public string Token => ParseParameter(TokenParameter, false);
		public string Record => ParseParameter(RecordParameter, false);
		public string Zone => ParseParameter(ZoneParameter, false);
		public string Ipv4 => ParseParameter(Ipv4Parameter, false);
		public string Ipv6 => ParseParameter(Ipv6Parameter, false);

		public ModifiedDnsRecord GetIpv4Entry => new()
        {
			Content = Ipv4,
			Type = DnsRecordType.A,
			Name = $"{Record}.{Zone}"
		};

		public ModifiedDnsRecord GetIpv6Entry => new()
		{
			Content = Ipv6,
			Type = DnsRecordType.Aaaa,
			Name = $"{Record}.{Zone}"
		};

		private string ParseParameter(string parameter, bool writeError = false)
        {
            
            if(CheckParameter(parameter, writeError))
            {
                return _request.Query[parameter];   
            }
            return string.Empty;
        }

        private bool CheckParameter(string parameter, bool writeError = true)
        {
            if(_request.Query.ContainsKey(parameter)) return true;
            if (writeError)
            {
				_errors.Add($"{parameter} is missing");
			}
            return false;
        }
    }
	public static string TokenParameter = "token"; //API TOKEN
    public static string RecordParameter = "record";
	public static string ZoneParameter = "zone";
	public static string Ipv4Parameter = "ipv4";
	public static string Ipv6Parameter = "ipv6";
}
