using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudflareDynDns.Refresh
{
    public class FritzBoxRefresh : RefreshBase
    {
        public readonly static string UsernameParameter = "username";
        public readonly static string DomainParameter = "domain";

        public FritzBoxRefresh(HttpRequest request, ILogger logger) : base(request, logger)
        {
            _zone = ParseParameter(DomainParameter, false)
                .Substring(Record.Length+1);
        }

        public FritzBoxRefresh()
        {
        }

        public override bool HasIpv6 => CheckParameter(DefaultRefresh.Ipv6Parameter, false);

        public override string Ipv4 => ParseParameter(DefaultRefresh.Ipv4Parameter, false);

        public override string Ipv6 => ParseParameter(DefaultRefresh.Ipv6Parameter, false);
        public override string Record => ParseParameter(UsernameParameter, false);

        public override string Token => ParseParameter(DefaultRefresh.TokenParameter, false);
        private string _zone;
        public override string Zone => _zone;

        protected override Dictionary<string, bool> Parameters => new()
        {
            {UsernameParameter,true },
            {DomainParameter,true},
            { DefaultRefresh.TokenParameter, true },
            { DefaultRefresh.Ipv4Parameter, true },
            { DefaultRefresh.Ipv6Parameter, false },
        };
    }
}
