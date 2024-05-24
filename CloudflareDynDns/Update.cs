using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using CloudFlare.Client;
using CloudFlare.Client.Api.Authentication;
using CloudFlare.Client.Api.Zones.DnsRecord;
using CloudFlare.Client.Api.Result;
namespace CloudflareDynDns;

public partial class Update
{
    private readonly ILogger<Update> _logger;

    public Update(ILogger<Update> logger)
    {
        _logger = logger;
    }

    [Function(nameof(Update))]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
    {
		_logger.LogMetric("UPDATE_CALL", 1);
		var refresh = new RefreshObject(req);
        if (!refresh.Success)
        {
			_logger.LogMetric("PARAMETER_ERROR", refresh.Errors.Count);
			return new ResponseObject(refresh.Errors).Result;
        }
        
        using var CloudflareClient = new CloudFlareClient(new ApiTokenAuthentication(refresh.Token));
        var zones = await CloudflareClient.Zones.GetAsync(new CloudFlare.Client.Api.Zones.ZoneFilter() { Name = refresh.Zone });
        if (!zones.Success)
        {
			_logger.LogMetric("ZONE_ERROR", 1);
			return new ResponseObject(zones.Errors.Select(x => x.Message)).Result;
        }
        var zoneId = zones.Result.First().Id;
        
        var records = await CloudflareClient.Zones.DnsRecords.GetAsync(zoneId,new DnsRecordFilter() { Name= $"{refresh.Record}.{refresh.Zone}" });
		if (!records.Success)
		{
			_logger.LogMetric("RECORD_ERROR", 1);
			return new ResponseObject(records.Errors.Select(x=>x.Message)).Result;
		}
		
        var ipv4Result = await CloudflareClient.Zones.DnsRecords.UpdateAsync(zoneId, records.Result.First().Id, refresh.GetIpv4Entry);
        
        CloudFlareResult<DnsRecord> ipv6Result = null;
        if (refresh.HasIpv6)
        {
			ipv6Result = await CloudflareClient.Zones.DnsRecords.UpdateAsync(zoneId, records.Result.First().Id, refresh.GetIpv6Entry);
			_logger.LogMetric("IPV6_ENTRY_SYNCED", 1);
		}
        
        if(ipv4Result.Success && (ipv6Result is null || ipv6Result.Success))
        {
            _logger.LogMetric("SYNC_SUCCESS", 1);
            return ResponseObject.SuccessResponseObject.Result;

		} else
        {
			_logger.LogMetric("SYNC_ERROR", 1);
			var errors = ipv6Result is not null ? [.. ipv4Result.Errors.Select(x => x.Message), .. ipv6Result.Errors.Select(x => x.Message)] : ipv4Result.Errors.Select(x=>x.Message);

			return new ResponseObject(errors).Result;
        }

    }


}
