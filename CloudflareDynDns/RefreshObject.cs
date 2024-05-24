using Microsoft.AspNetCore.Http;
using CloudFlare.Client.Api.Zones.DnsRecord;
using CloudFlare.Client.Enumerators;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
namespace CloudflareDynDns;


public class RefreshObject : RefreshObjectBase
{

	public readonly static string TokenParameter = "token"; //API TOKEN
	public readonly static string RecordParameter = "record";
	public readonly static string ZoneParameter = "zone";
	public readonly static string Ipv4Parameter = "ipv4";
	public readonly static string Ipv6Parameter = "ipv6";

	public RefreshObject(HttpRequest request, ILogger logger) : base(request, logger)
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

public class DynDnsV2RefreshObject : RefreshObjectBase
{
	protected static readonly string HostnameParameter = "hostname";
	protected static readonly string IpAddressParameter = "myip";

	public DynDnsV2RefreshObject(HttpRequest request, ILogger logger) : base(request, logger)
	{
		if(Success)
		{
			_logger.LogInformation($"Creating {nameof(DynDnsV2RefreshObject)}");
			var authHeader = request.Headers["Authorization"];
			if (authHeader.Count<=0 && authHeader.ToString().StartsWith("Basic")) {
				AddError("Missing Token/Authorization");
			} else {
			
				var encodedUsernamePassword = authHeader.ToString().Substring("Basic ".Length).Trim();
				var basicData = Encoding.ASCII.GetString(Convert.FromBase64String(encodedUsernamePassword));
				_token = basicData.Split(":")[1];
			}

			_hostname = request.Query.TryGetValue(HostnameParameter, out var h) ? h.ToString() : string.Empty;
			_ips = request.Query.TryGetValue(IpAddressParameter, out var i) ? i.ToString() : string.Empty;
			var splitIps = _ips.Split(",");
			if(IsIpv4(splitIps[0]))
			{
				_ipv4 = splitIps[0];
			} else if (IsIpv6(splitIps[0]))
			{
				_hasIpv6 = true;
				_ipv6 = splitIps[0];
			} else
			{
				AddError("Incorret IP Adress provided");
			}
			if(splitIps.Length > 1)
			{
				if (IsIpv4(splitIps[1]))
				{
					if(_ipv4 == null || _ipv4 == string.Empty) { AddError("Multiple Ipv4 provided"); } 
					else
					{
						_ipv4 = splitIps[1];
					}
				}
				else if (IsIpv6(splitIps[1]))
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
			if(splitHostname.Length != 2) 
			{
				AddError("Incorret Hostname provided");
			} else
			{
				_zone = splitHostname[1];
				_record = splitHostname[0];
			}
		}
	}

	public override bool HasIpv6 => _hasIpv6;

	public override string Ipv4 => _ipv4;

	public override string Ipv6 => _ipv6;

	public override string Record => _record;

	public override string Token => _token;

	public override string Zone => _zone;

	private string _ips;
	private string _hostname;
	private string _token;
	private string _ipv4;
	private string _ipv6;
	private string _zone;
	private string _record;
	private bool _hasIpv6;

	protected override Dictionary<string, bool> Parameters => new()
	{
		{HostnameParameter,true },
		{IpAddressParameter,true},
	};

	private bool IsIpv4(string ip)
	{
		var ipv4Pattern = @"^((25[0-5]|(2[0-4]|1\d|[1-9]|)\d)\.?\b){4}$";
		return Regex.IsMatch(ip, ipv4Pattern);
	}

	private bool IsIpv6(string ip)
	{
		var ipv6Pattern = @"(([0-9a-fA-F]{1,4}:){7,7}[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,7}:|([0-9a-fA-F]{1,4}:){1,6}:[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,5}(:[0-9a-fA-F]{1,4}){1,2}|([0-9a-fA-F]{1,4}:){1,4}(:[0-9a-fA-F]{1,4}){1,3}|([0-9a-fA-F]{1,4}:){1,3}(:[0-9a-fA-F]{1,4}){1,4}|([0-9a-fA-F]{1,4}:){1,2}(:[0-9a-fA-F]{1,4}){1,5}|[0-9a-fA-F]{1,4}:((:[0-9a-fA-F]{1,4}){1,6})|:((:[0-9a-fA-F]{1,4}){1,7}|:)|fe80:(:[0-9a-fA-F]{0,4}){0,4}%[0-9a-zA-Z]{1,}|::(ffff(:0{1,4}){0,1}:){0,1}((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])\.){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])|([0-9a-fA-F]{1,4}:){1,4}:((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])\.){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9]))";
		return Regex.IsMatch(ip, ipv6Pattern);
	}
}

