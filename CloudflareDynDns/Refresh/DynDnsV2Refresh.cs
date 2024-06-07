using Microsoft.AspNetCore.Http;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using CloudflareDynDns.Helpers;
namespace CloudflareDynDns.Refresh;

public class DynDnsV2Refresh : RefreshBase
{
    protected static readonly string HostnameParameter = "hostname";
    protected static readonly string IpAddressParameter = "myip";

    public DynDnsV2Refresh(HttpRequest request, ILogger logger) : base(request, logger)
    {
        if (Success)
        {
            _logger.LogInformation($"Creating {nameof(DynDnsV2Refresh)}");
            var authHeader = request.Headers["Authorization"];
            if (authHeader.Count <= 0 && authHeader.ToString().StartsWith("Basic"))
            {
                AddError("Missing Token/Authorization");
            }
            else
            {

                var encodedUsernamePassword = authHeader.ToString().Substring("Basic ".Length).Trim();
                var basicData = Encoding.ASCII.GetString(Convert.FromBase64String(encodedUsernamePassword));
                _token = basicData.Split(":")[1];
            }

            _hostname = request.Query.TryGetValue(HostnameParameter, out var h) ? h.ToString() : string.Empty;
            _ips = request.Query.TryGetValue(IpAddressParameter, out var i) ? i.ToString() : string.Empty;
            var splitIps = _ips.Split(",");
            if (splitIps[1].IsIpv4())
            {
                _ipv4 = splitIps[0];
            }
            else if (splitIps[1].IsIpv6())
            {
                _hasIpv6 = true;
                _ipv6 = splitIps[0];
            }
            else
            {
                AddError("Incorret IP Adress provided");
            }
            if (splitIps.Length > 1)
            {
                if (splitIps[1].IsIpv4())
                {
                    if (!_ipv4.IsNullOrWhiteSpace()) { AddError("Multiple Ipv4 provided"); }
                    else
                    {
                        _ipv4 = splitIps[1];
                    }
                }
                else if (splitIps[1].IsIpv6())
                {
                    if (HasIpv6) { AddError("Multiple Ipv6 provided"); }
                    else
                    {
                        _hasIpv6 = true;
                        _ipv6 = splitIps[1];
                    }
                }
                else
                {
                    AddError("Incorret IP Adress provided");
                }
            }

            //TODO add posibility to update full domain (example.org)
            var splitHostname = _hostname.Split(['.'], 2);
            if (splitHostname.Length != 2)
            {
                AddError("Incorret Hostname provided");
            }
            else
            {
                _zone = splitHostname[1];
                _record = splitHostname[0];
            }
        }
    }

    public DynDnsV2Refresh()
    {
    }

    public override bool HasIpv6 => _hasIpv6;

    public override string Ipv4 => _ipv4;

    public override string Ipv6 => _ipv6;

    public override string Record => _record;

    public override string Token => _token;

    public override string Zone => _zone;

    private string _ips = string.Empty;
    private string _hostname = string.Empty;
    private string _token = string.Empty;
    private string _ipv4 = string.Empty;
    private string _ipv6 = string.Empty;
    private string _zone = string.Empty;
    private string _record = string.Empty;
    private bool _hasIpv6 = false;

    protected override Dictionary<string, bool> Parameters =>new()
    {
        {HostnameParameter,true },
        {IpAddressParameter,true},
    };
}

