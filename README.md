# CloudflareDynDns

Azure Functions App providing a DynDns Service 2 Custom Urls.
Support for DynDnsV2 (Telecom Smart 3+4) 

### Urls
##### Default custom url 
```https://functionsapp.local/?token=PASSWORD&record=RECORD&zone=DOMAIN&ipv4=IPV4&ipv6=IPV6&code=YOURAZUREADCODE```

##### DynDnsV2 url 
```https://USERNAME:PASSWORD@functionsapp.local/update?hostname=DOMAIN&myip=IPV4,IPV6&code=YOURAZUREADCODE```

The DynDns V2 Url uses http basic auth. however this is not checked. The password is only used as the Cloudflare Token

#### Parameters for (test.example.org)
- USERNAME: record (test)
- PASSWORD: Cloudflare Api Token (TODO link)
- DOMAIN: hostname (example.org)
- IPV4: ipv4 address 
- IPV6: ipv6 address
- YOURAZURECODE: the Function Authentication Code. Unless you turn auth to anonymous in the Code 

### Setup
- Fork Repo
- Create an Azure Function App with this Repo
- (optional) Bind your custom ddns domain to the Function App
- Create a Cloudflare API Token
- Create and edit the URLs for your DDNS Client/Router 

### ToDo
- [ ] Route which provides a HTML page to easily create your URLs, 
- [ ] Rework DynDnsV2 to User USERNAME as RECORD and hostname without split 
- [ ] Support for Root URL (example.org)
