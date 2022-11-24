﻿# Carbon.HttpClient.Auth

Simple HTTP Handler for inter-microservices communication. Recommended for only purpose of in-cluster communication.
Supports oAuth2 client_credentials grant typed authorization. Both HTTP 1.1 (REST-based) and HTTP 2.0 (GRPC-based)
when [Carbon.WebApplication.Grpc.Client](../Carbon.WebApplication.Grpc.Client/README.md) used are supported.

It handles the token retreival from identity server supported by oauth2 and 
In case of having 401 status code from microservices due to token expiration or something, it refreshes the token and resends
the http request again.

[Carbon.WebApplication.Grpc.Client](../Carbon.WebApplication.Grpc.Client/README.md) uses this package as-is for the
inter-microservices authorized communication.

**1a. Simply register this httpclient (For HTTP 1.1 REST-based)**
 ```csharp
//IServiceCollection services
services.AddHttpClientAuth();
```
or..

**1b. Or register this grpchttpclient (For HTTP 2.0 GRPC-based)**
 ```csharp
//IServiceCollection services
services.AddGrpcHttpClientAuth();
```

**2. Create an authentication with the name just as it is writing in the configuration file**

This creates the token at startup by communicating with identity server and stores it until you ask for.
```csharp
//IApplicationBuilder - Let the name be 'RAM'. You can say 'Thing'. It is the identifier.
app.CreateAuthentication("RAM");
```

And use the configuration as given below.
AuthenticationType 4 is type of *client_credentials* in *oAuth2*. AuthenticationType 1, 2 or 3 is legacy and unsupported.

```json
"Authorizations":
{
    "RAM":
        {
            "AuthenticationType": 4,
            "Secret":"yoursecret",
            "Scope":"scope1 scope2",
            "ClientId":"yourclient"
        }
},
"JwtSettings": {
    "Authority": "http://youridentityurl.com",
  }
```

> http://youridentityurl.com/connect/token will be requested for the token operations as this is the default approach of IdentityServer https://github.com/IdentityServer/IdentityServer4

If you are already using Carbon.WebApplication, JwtSettings section already exists. You don't have to take care of it.


**3. Use it in your code**

When there is a scenario such as resulted with 401 Unauthorized for your request, it will refresh the token and store the new one for future references by replacing the old one and resends the
request from the stracth automatically.

```csharp
    public class ExternalService : IExternalService
    {
        // Inject your Authed Http Client Factory by DI
        private readonly AuthHttpClientFactory _clientFactory;

        public ExternalService(AuthHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }


        public async Task<bool> DoSomethingByUsingAuthClient()
        {
            try
            {
                //Get your auth client with the given name in service registrations
                var authClient = _clientFactory.CreateAuthClient("RAM");
                //Have your httpclient
                var clientForAsset = authClient.HttpClient;
                //Do whatever you want to do with your master httpclient
                clientForAsset.DefaultRequestHeaders.Accept.Clear();
                clientForAsset.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //Create a HTTP request message for the purpose of HTTP payload
                HttpRequestMessage httpRequestMessageToAsset = new HttpRequestMessage();
                httpRequestMessageToAsset.Headers.Add("tenantId", "123");
                httpRequestMessageToAsset.Method = HttpMethod.Post;

                var jStr = JsonConvert.SerializeObject(new {test = "test"});

                httpRequestMessageToAsset.Content = new StringContent(jStr, Encoding.UTF8, "application/json");

                var uribuilder = new UriBuilder("http://theapiurl");
                string daUrl = uribuilder.ToString();

                httpRequestMessageToAsset.RequestUri = new Uri(daUrl);
                //Send your request and collect the response
                var resp = await _clientFactory.SendAsync(authClient, httpRequestMessageToAsset);
                if (resp.IsSuccessStatusCode)
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception caught while notifying for subscription: {0} \n Stack trace: {1}", ex.Message, ex.StackTrace);
                throw ex;
            }
        }
    }
```

Check here also https://github.com/kocdigital/Carbon.Sample/blob/master/Carbon.Sample.API/Domain/Services/DataService.cs#L45 for
a sample in detail.

## Advanced Usage
You can use multiple instances of authorizations. In order to do that, simply CreateAuthentication more than one by defining
multiple authorizations in the config file:

```json
"Authorizations":
{
    "RAM":
        {
            "AuthenticationType": 4,
            "Secret":"yoursecret",
            "Scope":"scope1 scope2",
            "ClientId":"yourclient"
        },
     "Other":
        {
            "AuthenticationType": 4,
            "Secret":"myothersecret",
            "Scope":"scope1 scope2 scope3",
            "ClientId":"yourotherclient"
        }
},
"JwtSettings": {
    "Authority": "http://youridentityurl.com",
  }
```

Create authentication for each (RAM and Other):
```csharp
//Register it as usual
services.AddHttpClientAuth();

//IApplicationBuilder - Create two instead of one as in the basic usage
app.CreateAuthentication("RAM");
app.CreateAuthentication("Other");

```
And use whichever you want:

```csharp
    public class ExternalService : IExternalService
    {
        // Inject your Authed Http Client Factory by DI
        private readonly AuthHttpClientFactory _clientFactory;

        public ExternalService(AuthHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }


        public async Task<bool> DoSomethingByUsingAuthClient()
        {
          //Get your auth client with the given name in service registrations
          //RAM-based token and authorization will be used in this client
          var authClientForRAM = _clientFactory.CreateAuthClient("RAM");
          //Other-based token and authorization will be used in this client
          var authClientForOther = _clientFactory.CreateAuthClient("Other");            
        }
    }
```