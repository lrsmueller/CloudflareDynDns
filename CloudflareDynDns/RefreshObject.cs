using Microsoft.AspNetCore.Http;
using CloudFlare.Client.Api.Zones.DnsRecord;
using CloudFlare.Client.Enumerators;
namespace CloudflareDynDns;


public class RefreshObject
{

	public readonly static string TokenParameter = "token"; //API TOKEN
	public readonly static string RecordParameter = "record";
	public readonly static string ZoneParameter = "zone";
	public readonly static string Ipv4Parameter = "ipv4";
	public readonly static string Ipv6Parameter = "ipv6";

	public RefreshObject(HttpRequest request)
    {
        _request = request;
        foreach (var parameter in Parameters)
        {
            CheckParameter(parameter.Key, parameter.Value);
        }
        Success = Errors.Count <= 0;
    }

    public bool Success { get; init; }

    public bool HasIpv6 => CheckParameter(Ipv6Parameter, false);



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
	public IReadOnlyList<string> Errors => _errors;


	private List<string> _errors = new List<string>();
	private HttpRequest _request;
	private Dictionary<string, bool> Parameters = new Dictionary<string, bool>()
	{
		{ TokenParameter, true },
		{ RecordParameter, true },
		{ ZoneParameter, true },
		{ Ipv4Parameter, true },
		{ Ipv6Parameter, false },
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

