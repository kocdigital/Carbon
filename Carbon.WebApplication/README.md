# Carbon.WebApplication

Standardization for RESTful (HTTP 1.1) Web Applications. Add this library and have a working web application immediately with some most commonly used approaches for web applications such as Containerization, 
Logging, Config Management, Hosting Service, Kubernetes Enablement, OpenAPI standards (Swagger), Authorization, Authentication, Multi-tenant management, exception handling, security etc. Use this package to easily boot a fully 
equipped web application with a couple line of codes.

## Add Carbon.WebApplication Support to Your Project
### Get Started

| Out-of-the-box Support                                                           | Availability | Description |
|-----------------------------------------------------------------                 |:----:        |:----:         |
| Kubernetes-enabled (Containerization-enabled)                                    | Yes          | When base helm chart is used as in the [Carbon.Sample Helm Chart](https://github.com/kocdigital/Carbon.Sample/tree/master/helm/CarbonSample-api), it has the best harmony, it simply manages many kubernetes features in one place. Just plug and play.           |
| Logging                                                                          | Yes          | Serilog-based|
| Config Management                                                                | Yes          | Identifies Configmaps when kubernetes used, static config files can be also configured or enables a centralized configuration management via Consul|
| Multi-Hosting Type                                                               | Yes          | Kestrel or IIS|
| Open-API support                                                                 | Yes          | Swagger via Swashbuckle |
| Authorization                                                                    | Yes          | OAuth2 with Bearer Jwt Token via IdentityServer |
| Authentication                                                                   | Yes          | Multi-Tenant Management via Platform360  |
| Exception Handling                                                               | Yes          | Static or customizable exception handling available  |
| CORS                                                                             | Yes          | Configurable by Configurations  |
| Observability                                                                    | Yes          | Basic health-check enablement or Advanced when used with other infrastructural Carbon libraries  |
| Json Options for request and response                                            | Yes          | Built-in and standardized (IgnoreNullValues, StringEnumConverter, PascalCase), can be overriden when desired  |
| Request Validation                                                               | Yes          | Rules can run against Dtos from controllers. Supported via Fluent Validation  |
| Multi .Net Run Time Support                                                      | Yes          | Supports Dotnet 3.1 - 5 and 6 directly. (Complied against these versions)  |
| Hands-free MVC initialization                                                    | Yes          | Manages MVC core features on its own. Simply use carbon, and your controllers or other simple MVC settings handled automatically  |


**1a. Build your API Startup if it is non-minimal**

Design your Startup.cs as given below:
```csharp
public class Startup : CarbonStartup<Startup>
    {
        //Base your startup and use authorization and authentication as enabled
        public Startup(IConfiguration configuration, IWebHostEnvironment environment) : base(configuration, environment, true, true)
        {
        }

        public override void CustomConfigureServices(IServiceCollection services)
        {
           //Register your services with DI
            services.AddScoped<IAssetService, AssetService>();
        }

        public override void CustomConfigure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //Tweak over your app builder
            app.CreateAuthentication("RAM");
        }

        //Optional
        public override void ConfigureEndpoints(IEndpointRouteBuilder endpoints)
        {
            //Define some endpoints here if needed(such as SignalR Hub Mappings etc.)
            base.ConfigureEndpoints(endpoints);
        }
    }

```

Design your Program.cs as given below:
```csharp
public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseCarbonFeatures();
                    webBuilder.UseStartup<Startup>();
                });
    }
```

**1b. Build your API Startup if it is minimal (Introduced as of dotnet 6)**

//Design your Program.cs as given below
```csharp
var builder = WebApplication.CreateBuilder(args);

builder.AddCarbonServices((services) => 
{          
    //Register your services with DI
    services.AddScoped<IAssetService, AssetService>();

    return services;
});

var app = builder.Build();
app.AddCarbonApplication((app) =>
{            
    //Tweak over your app builder
    app.CreateAuthentication("RAM");

    return app;
});
```


**2. Configurations**

Use the base-configuration as given below

```json
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "CorsPolicy": {
    "AllowAnyMethods": true,
    "AllowAnyHeaders": true,
    "AllowAnyOrigin": true,
    "Origins": [ "http://localhost:*", "http://*:*", "http://localhost:8080" ]
  },
  "JwtSettings": {
    "Authority": "http://youridentityserver:10067",
    "RequireHttpsMetaData": false,
    "Audience": "YourScopeForAPI",
    "TokenValidationSettings": {
      "ValidIssuers": [ "http://someidentityserver:10067", "https://somesecureotheridentityserver:40067", "https://maybeanotheridentityserver.com" ]
    }
  },
"Serilog": {
    "Using": [ "Serilog.Sinks.Console", "Serilog.Sinks.Elasticsearch" ],
    "MinimumLevel": "Information",
    "WriteTo": [
      { "Name": "Console" },
      {
        "Name": "Elasticsearch",
        "Args": {
          "nodeUris": "http://elasticsearch",
          "indexFormat": "yourapi-log-{0:yyyy.MM.dd}"
        }
      }
    ],
    "Enrich": [ "FromLogContext", "WithMachineName", "WithThreadId" ],
    "Properties": { "Application": "Your.API" }
  },
 "Swagger": {
    "EndpointUrl": "http://yourswaggerUrlThatYouWantToShowUp",
    "EndpointPath": "/swagger/v1/swagger.json",
    "EndpointName": "Your API V1",
    "Documents": [
      {
        "DocumentName": "v1",
        "OpenApiInfo": {
          "Title": "Your  API",
          "Version": "v1",
          "Description": "Your API"
        },
        "Security": {
          "AuthorizationUrl": "http://youridentityserver:10067",
          "Scopes": [
            {
              "Key": "YourScopeForAPI",
              "Description": "My Scope As In the JWTSettings"
            }
          ]
        }
      }
    ]
  }
```

**3. Some other useful tweaks**

Use this Environment Variables (either in your launch profile or machine environment variables)
to get your api up and running by using appsettings.Development.json in the debug folder by running with Kestrel standalone hosting type

```
ASPNETCORE_ENVIRONMENT=Development,
CONFIGURATION_TYPE=FILE,
ENVIRONMENT_TYPE=Kestrel,
FILE_CONFIG_PATHS=appsettings.Development.json
```

Now you are good to go, Carbon will handle any standards on behalf of you that is introduced at the very beginning.

## Advanced Usage and More Details

In this section, you can find more information about advanced usage of Carbon.WebApplication to make your API more customized and fine-tuned.

### Get to know more about Environment Variables
---
Set your aspnetcore environment type. Use **Development** for development purposes (enables some advanced error handling etc.) or **Production** for production environments.
```
ASPNETCORE_ENVIRONMENT=Development
```

You can set your configuration management type as FILE which will consume any configuration json files into your API,
or as CONSUL in order to set your configurations in a centralized way (Only recommended for dev purposes). 
```
CONFIGURATION_TYPE=FILE
```

If you set your Configuration type as file, as given above, you can set which configs should be consumed by your APIs in-which the application context contains. Comma-seperated for multiple paths
```
FILE_CONFIG_PATHS=appsettings.Development.json,config/appsettings.Other.json
```

For Environment type, Kestrel will be your the best friend in any containerized environment, such as Kubernetes or Docker. Nevertheless, if you still insist on
using IIS, you can set the Environment_Type as IIS to host your API in IIS environments.
```
ENVIRONMENT_TYPE=Kestrel
```

### Get to know more about API Startups
---
#### Remove authorization and authentication from your API (insecure, not recommended!) by setting the last two parameters as (false, false)

```csharp
public class Startup : CarbonStartup<Startup>
    {
        //Base your startup and use authorization and authentication as enabled
        public Startup(IConfiguration configuration, IWebHostEnvironment environment) : base(configuration, environment, false, false)
        {
        }

      
    }

```

or for the minimal APIs, use app.AddCarbonApplication method's parameters.


```csharp
var builder = WebApplication.CreateBuilder(args);

builder.AddCarbonServices((services) => 
{          
    services.AddScoped<IAssetService, AssetService>();
    return services;
});

var app = builder.Build();
app.AddCarbonApplication((app) =>
{            
    app.CreateAuthentication("RAM");
    return app;
}, false, false);
```

#### Add custom endpoints to minimal API in Program.cs

You can add custom endpoints such as SignalR Hubs and so on in the appbuilder phase.

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.AddCarbonServices((services) => 
{          
    services.AddScoped<IAssetService, AssetService>();
    return services;
});

var app = builder.Build();
app.AddCarbonApplication((app) =>
{            
    app.CreateAuthentication("RAM");
    return app;
}, true, true,
(e) => {
    //Do your extra mappings here
    e.MapHub<SomeHub>();
    return e;
});
```

### Get to know more about Carbon-based configurations
---
#### JWTSettings

**Authority** is your prefered identity server's URL. 

**ValidIssuers** also allow you to come with tokens provided that all are issued with the given identity servers in ValidIssuers array.

**Audience** is your API scope

```json
 "JwtSettings": {
    "Authority": "http://youridentityserver:10067",
    "RequireHttpsMetaData": false,
    "Audience": "YourScopeForAPI",
    "TokenValidationSettings": {
      "ValidIssuers": [ "http://someidentityserver:10067", "https://somesecureotheridentityserver:40067", "https://maybeanotheridentityserver.com" ]
    }
  },
```
#### Swagger

**EndpointUrl** should be the same where you advertise your API
**EndpointPath** can be kept as-is
**EndpointName** should be a well-descriptive name for your API
**AuthorizationUrl** should be your identityserver URL where swagger is supposed to collect tokens
**Scopes** should contain at least your APIs scope, plus what is necessary.

```json
 "Swagger": {
    "EndpointUrl": "http://yourswaggerUrlThatYouWantToShowUp",
    "EndpointPath": "/swagger/v1/swagger.json",
    "EndpointName": "Your API V1",
    "Documents": [
      {
        "DocumentName": "v1",
        "OpenApiInfo": {
          "Title": "Your  API",
          "Version": "v1",
          "Description": "Your API"
        },
        "Security": {
          "AuthorizationUrl": "http://youridentityserver:10067",
          "Scopes": [
            {
              "Key": "YourScopeForAPI",
              "Description": "My Scope As In the JWTSettings"
            }
          ]
        }
      }
    ]
  }
```

#### Serilog for logging

[Check here](https://github.com/serilog/serilog) for more serilog configurations. Serilog in Carbon comes
with only Console support out-of-the-box. However, if you need more sinks such as elastic search, SQL, File etc.
You need to add those sinks seperately to your project as nuget packages. In the example
below, elastic search is enabled by adding the given nuget packages

> Serilog.Formatting.Elasticsearch
> 
> Serilog.Sinks.Elasticsearch


You can enrich this serilog config by other serilog-based configurations in order to have
more powerful and comprehensive logging features.

Given configuration logs the necessary logs into console and elasticsearch with the provided url in a defined index with at least information severity level

```json
"Serilog": {
    "Using": [ "Serilog.Sinks.Console", "Serilog.Sinks.Elasticsearch" ],
    "MinimumLevel": "Information",
    "WriteTo": [
      { "Name": "Console" },
      {
        "Name": "Elasticsearch",
        "Args": {
          "nodeUris": "http://elasticsearch",
          "indexFormat": "yourapi-log-{0:yyyy.MM.dd}"
        }
      }
    ],
    "Enrich": [ "FromLogContext", "WithMachineName", "WithThreadId" ],
    "Properties": { "Application": "Your.API" }
  },
```

#### Cors for security

Given cors policy is the most priviliged approach which allows any kind of CORS security considerations. 
Check more about CORS from the internet and see how you can secure your application by tweaking this section.

```json
  "CorsPolicy": {
    "AllowAnyMethods": true,
    "AllowAnyHeaders": true,
    "AllowAnyOrigin": true,
    "AllowCredentials": true,
    "Origins": [ "http://localhost:*", "http://*:*", "http://localhost:8080" ]
  },
```
