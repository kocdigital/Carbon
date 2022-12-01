# Carbon.ConsoleApplication

Standardization for Console Applications. Add this library and have a working console application immediately with some most commonly used approaches for console applications 
such as Containerization, Logging, Config Management, Hosting Service etc.

## Add Carbon.ConsoleApplication Support to Your Project
### Get Started

| Out-of-the-box Support                                                           | Availability | Description |
|:----------------------------------------------------------------                 |:----:        |:----        |
| Kubernetes-enabled (Containerization-enabled)                                    | Yes          | When base helm chart is used as in the [Carbon.Sample Helm Chart](https://github.com/kocdigital/Carbon.Sample/tree/master/helm/CarbonSample-api), it has the best harmony, it simply manages many kubernetes features in one place. Just plug and play.           |
| Logging                                                                          | Yes          | Serilog-based|
| Config Management                                                                | Yes          | Identifies Configmaps when kubernetes used, static config files can be also configured or enables a centralized configuration management via Consul|

### 1. Configuring Main Method

For using Carbon features you must enable Carbon using Carbon.ConsoleApplication package as shown below;

> Simply create a host builder and call Carbon functions

```csharp
    public static async Task Main(string[] args)
    {
        var host = new HostBuilder()
        .AddCarbonServices<Program>((hostContext, services) =>
        {
            ....
        })
        .UseServiceProvider(app =>
        {
            ....
        });

        .....

        await host.RunConsoleAsync();
    }
```

For details about method descriptions you can use [Api Documentation](https://kocdigital.github.io/Carbon/api/Carbon.ConsoleApplication.html).

### 2. Configurations

Use the base-configuration as given below

```json
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
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
  }
```
### 3. Some other useful tweaks

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

In this section, you can find more information about advanced usage of Carbon.ConsoleApplication to make your API more customized and fine-tuned.

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

For Environment type, Kestrel will be your the best friend in any containerized environment, such as Kubernetes or Docker.
```
ENVIRONMENT_TYPE=Kestrel
```