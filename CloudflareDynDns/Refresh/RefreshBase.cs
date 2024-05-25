using CloudFlare.Client.Api.Zones.DnsRecord;
using CloudFlare.Client.Enumerators;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CloudflareDynDns.Refresh
{
    public abstract class RefreshBase
    {
        protected ILogger _logger;
        public RefreshBase(HttpRequest request, ILogger logger)
        {
            _logger = logger;
            _request = request;
            _logger.LogInformation($"Creating {nameof(RefreshBase)} Object");
            foreach (var parameter in Parameters)
            {
                CheckParameter(parameter.Key, parameter.Value);
            }
        }

        protected List<string> _errors = new List<string>();
        protected HttpRequest _request;
        protected abstract Dictionary<string, bool> Parameters { get; }

        public abstract bool HasIpv6 { get; }
        public abstract string Ipv4 { get; }
        public abstract string Ipv6 { get; }
        public abstract string Record { get; }

        public abstract string Token { get; }
        public abstract string Zone { get; }


        public IReadOnlyList<string> Errors => _errors;
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

        public bool Success => _errors.Count <= 0;

        protected bool CheckParameter(string parameter, bool writeError = true)
        {
            if (_request.Query.ContainsKey(parameter)) return true;
            if (writeError)
            {
                AddError($"{parameter} is missing");
            }
            return false;
        }
        protected string ParseParameter(string parameter, bool writeError = false)
        {

            if (CheckParameter(parameter, writeError))
            {
                return _request.Query[parameter];
            }
            return string.Empty;
        }

        protected void AddError(string message)
        {
            _logger.LogError(message);
            _logger.LogMetric("REFRESH_OBJ_ERROR", 1);
            _errors.Add($"{message}");
        }
    }
}