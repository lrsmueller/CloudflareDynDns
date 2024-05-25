using Microsoft.AspNetCore.Http;
using CloudFlare.Client.Api.Zones.DnsRecord;
using CloudFlare.Client.Enumerators;
using Microsoft.Extensions.Logging;
namespace CloudflareDynDns.Refresh;


public class DefaultRefresh : RefreshBase
{

    public readonly static string TokenParameter = "token"; //API TOKEN
    public readonly static string RecordParameter = "record";
    public readonly static string ZoneParameter = "zone";
    public readonly static string Ipv4Parameter = "ipv4";
    public readonly static string Ipv6Parameter = "ipv6";

    public DefaultRefresh(HttpRequest request, ILogger logger) : base(request, logger)
    {
    }

    protected override Dictionary<string, bool> Parameters => new Dictionary<string, bool>()
    {
        { TokenParameter, true },
        { RecordParameter, true },
        { ZoneParameter, true },
        { Ipv4Parameter, true },
        { Ipv6Parameter, false },
    };

    public override bool HasIpv6 => CheckParameter(Ipv6Parameter, false);
    public override string Ipv4 => ParseParameter(Ipv4Parameter, false);
    public override string Ipv6 => ParseParameter(Ipv6Parameter, false);
    public override string Record => ParseParameter(RecordParameter, false);

    public override string Token => ParseParameter(TokenParameter, false);
    public override string Zone => ParseParameter(ZoneParameter, false);


}

