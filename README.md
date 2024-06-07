# CloudflareDynDns

[![Deploy to Azure Functions](https://github.com/lrsmueller/CloudflareDynDns/actions/workflows/main_f3k-cloudflare-dyndns-func.yml/badge.svg)](https://github.com/lrsmueller/CloudflareDynDns/actions/workflows/main_f3k-cloudflare-dyndns-func.yml)
[![Publish Docker](https://github.com/lrsmueller/CloudflareDynDns/actions/workflows/docker-publish.yml/badge.svg)](https://github.com/lrsmueller/CloudflareDynDns/actions/workflows/docker-publish.yml)

Azure Functions App providing a DynDns Service 2 Custom Urls.
Support for DynDnsV2 (Telekom Speedport Smart 3+4) 

## Urls
##### Default custom url 
```https://functionsapp.local/?token=<TOKEN>&record=<RECORD>&zone=<ZONE>&ipv4=<IPV4>&ipv6=<IPV6>&code=<YOURAZUREFUNCTIONCODE>```

##### DynDnsV2 url 
```https://USERNAME:PASSWORD@functionsapp.local/update/?hostname=<HOSTNAME>&myip=<MYIP>&code=<YOURAZUREFUNCTIONCODE>```

The DynDns V2 Url uses http basic auth. however this is not checked. The password is only used as the Cloudflare Token

##### FritzBox rul
```https://functionsapp.local/fb/?username=<USERNAME>&domain=<DOMAIN>&token=<TOKEN>&ipv4=<IPV4>&ipv6=<IPV6>&code=<YOURAZUREFUNCTIONCODE>```

#### Parameters for (test.example.org)
- USERNAME: record (test)
- PASSWORD: Cloudflare API token - [Create an API token](https://developers.cloudflare.com/fundamentals/api/get-started/create-token/) with read/write access to the zone (example.org) you want to use
- DOMAIN: hostname (example.org)
- IPV4: ipv4 address 
- IPV6: ipv6 address
- YOURAZURECODE: the Function Authentication Code. Unless you turn auth to anonymous in the Code 

## Setup
### Setup on Azure
- Fork Repo
- Create an Azure Function App with this Repo
- (optional) Bind your custom ddns domain to the Function App
- Create a Cloudflare API Token
- Create and edit the URLs for your DDNS Client/Router 

### Run on Docker
`docker run -d -p 8080:8080 ghcr.io/lrsmueller/cloudflaredyndns:latest`

