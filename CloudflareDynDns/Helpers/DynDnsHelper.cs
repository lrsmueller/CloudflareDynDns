using CloudFlare.Client;
using CloudFlare.Client.Api.Authentication;
using CloudFlare.Client.Api.Result;
using CloudFlare.Client.Api.Zones.DnsRecord;
using CloudflareDynDns.Refresh;
using CloudflareDynDns.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudflareDynDns.Helper;

public static class DynDnsHelper
{
    public static async Task<IActionResult> UpdateDynDnsEntry(RefreshBase refresh, ILogger logger)
    {
        using var CloudflareClient = new CloudFlareClient(new ApiTokenAuthentication(refresh.Token));
        var zones = await CloudflareClient.Zones.GetAsync(new CloudFlare.Client.Api.Zones.ZoneFilter() { Name = refresh.Zone });
        if (!zones.Success)
        {
            logger.LogMetric("ZONE_ERROR", 1);
            return new DynDnsResponse(zones.Errors.Select(x => x.Message)).Result;
        }
        var zoneId = zones.Result.First().Id;

        var records = await CloudflareClient.Zones.DnsRecords.GetAsync(zoneId, new DnsRecordFilter() { Name = $"{refresh.Record}.{refresh.Zone}" });
        if (!records.Success)
        {
            logger.LogMetric("RECORD_ERROR", 1);
            return new DynDnsResponse(records.Errors.Select(x => x.Message)).Result;
        }

        var ipv4Result = await CloudflareClient.Zones.DnsRecords.UpdateAsync(zoneId, records.Result.First().Id, refresh.GetIpv4Entry);

        CloudFlareResult<DnsRecord> ipv6Result = null;
        if (refresh.HasIpv6)
        {
            ipv6Result = await CloudflareClient.Zones.DnsRecords.UpdateAsync(zoneId, records.Result.First().Id, refresh.GetIpv6Entry);
            logger.LogMetric("IPV6_ENTRY_SYNCED", 1);
        }

        if (ipv4Result.Success && (ipv6Result is null || ipv6Result.Success))
        {
            logger.LogMetric("SYNC_SUCCESS", 1);
            return DynDnsResponse.SuccessResponseObject.Result;

        }
        else
        {
            logger.LogMetric("SYNC_ERROR", 1);
            var errors = ipv6Result is not null ? [.. ipv4Result.Errors.Select(x => x.Message), .. ipv6Result.Errors.Select(x => x.Message)] : ipv4Result.Errors.Select(x => x.Message);

            return new DynDnsResponse(errors).Result;
        }
    }

}
