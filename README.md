# CloudflareDynDns

[![Deploy to Functions](https://github.com/lrsmueller/CloudflareDynDns/actions/workflows/azure-deploy.yml/badge.svg)](https://github.com/lrsmueller/CloudflareDynDns/actions/workflows/azure-deploy.yml)
[![Publish Docker](https://github.com/lrsmueller/CloudflareDynDns/actions/workflows/docker-publish.yml/badge.svg)](https://github.com/lrsmueller/CloudflareDynDns/actions/workflows/docker-publish.yml)

Azure Functions App providing a DynDns Service 2 Custom Urls.
Support for DynDnsV2 (Telekom Speedport Smart 3+4) 

## Urls
##### Default custom url 
```https://functionsapp.local/?token=PASSWORD&record=RECORD&zone=DOMAIN&ipv4=IPV4&ipv6=IPV6&code=YOURAZUREADCODE```

##### DynDnsV2 url 
```https://USERNAME:PASSWORD@functionsapp.local/update?hostname=DOMAIN&myip=IPV4,IPV6&code=YOURAZUREADCODE```

The DynDns V2 Url uses http basic auth. however this is not checked. The password is only used as the Cloudflare Token

#### Parameters for (test.example.org)
- USERNAME: record (test)
- PASSWORD: Cloudflare API token - [Create an API token](https://developers.cloudflare.com/fundamentals/api/get-started/create-token/) with read/write access to the zone (example.org) you want to use
- DOMAIN: hostname (example.org)
- IPV4: ipv4 address 
- IPV6: ipv6 address
- YOURAZURECODE: the Function Authentication Code. Unless you turn auth to anonymous in the Code 

**Known Issue** Fritz!Box routers require the Domain to be the complete hostname [#1](https://github.com/lrsmueller/CloudflareDynDns/issues/1)

## Setup
### Setup on Azure
- Fork Repo
- Create an Azure Function App with this Repo
- (optional) Bind your custom ddns domain to the Function App
- Create a Cloudflare API Token
- Create and edit the URLs for your DDNS Client/Router 

### Run on Docker
`docker run -d -p 8080:8080 ghcr.io/lrsmueller/cloudflaredyndns:latest`

