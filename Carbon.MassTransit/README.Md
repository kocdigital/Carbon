Carbon.MassTransit
===
This package is powered up by MassTransit that is a _free, open-source_ distributed application framework for .NET. MassTransit makes it easy to create applications and services that leverage message-based, 
loosely-coupled asynchronous communication for higher availability, reliability, and scalability.

This package wraps up some capabilities and presents you with an easier and maintained way. It also contains some microservice design patterns
and their ease of implementation beside of having RabbitMQ messaging.

Please refer to this website to collect more information: https://masstransit-project.com/

Check here for the [ChangeLog](Carbon.MassTransit.csproj)


Add MassTransit Support to Your Project
---------------------------
Basic Usage
-------
You can add a masstransit support with this code snippet which starts a hosted masstransit service with all the healthchecks and configuration managaments behind the scenes.
This will only builds a consumer and binds it to a queue namely *my-queue* and starts to consume it. This is the most basic 
approach ever seen.

```csharp
            services.AddMassTransitBus((x) =>
            {
                var queueName = "my-queue";

                x.AddConsumer<MyConsumer>();

                x.AddRabbitMqBus(Configuration, (provider, cfg) =>
                {
                     cfg.ReceiveEndpoint(queueName, configurator =>
                    {
                        //You can use any masstransit configurators here
                        // here is totally powered up by MassTransit itself
                        configurator.Consumer<MyConsumer>(provider);
                        //Those coming are optional
                        Retry.Incremental(3, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
                        configurator.PrefetchCount = 16;
                    });

                }
            });
```

Your Masstransit basic configuration should be given below. It only supports RabbitMQ and ServiceBus. However, ServiceBus support is now deprecated and not maintained anymore.
```json
"MassTransit": {
    "BusType": 1,
    "RabbitMq": {
      "Host": "rabbitmqhost",
      "Username": "rabbitmquser",
      "Password": "rabbitmqpassword",
      "VirtualHost": "/yourvirtualhost",
      "Port": 5672
    },
    "ServiceBus": {
      "ConnectionString": ""
    }
  },
```



And your consumer should be defined as given below just as it is done in MassTransit project. Check this URL for more information:
https://masstransit-project.com/usage/consumers.html

```csharp
class MyConsumer :
    IConsumer<ConsumedObject>
{
    public async Task Consume(ConsumeContext<ConsumedObject> context)
    {
        //Do sth.
    }
}
```

Advanced Configuration
----

More RabbitMQ specific settings can be found as below if needed. You can use each property within the configuration json file with the same name.
Check here for [RabbitMqSettings](RabbitMqSettings.cs)

Microservice Design Pattern Usages
-----
As described earlier, this package includes many useful and easy to use patterns to manage 
loosely-coupled asynchronous communication for higher availability, reliability, and scalability.

Routing Slip
---
This pattern is already implemented by MassTransit, however its usage sometimes might be cumbersome or 
requires more knowledge and proficiency about MassTransit in order to use it. Carbon eases the usage and 
abstracts you from some cumbersome configurations.

A routing slip specifies a sequence of processing steps called activities that are combined into a single transaction. As each activity completes, 
the routing slip is forwarded to the next activity in the itinerary. 
When all activities have completed, the routing slip is completed and the transaction is complete.[**]

Check this URL for more information about Routing Slip and MassTransit default approach: 

[**] https://masstransit-project.com/advanced/courier/activities.html#execute-activities

**1. Create your activities**

*Below there is an activity definition that is only a single part of an activities set*

```csharp
    //Define your activities including an execution (happy path) and compansation (rollback) methods
    public class GatewayRegisterActivity : IActivity<IGatewayRegisterArguments, IGatewayRegisterLogs>
    {
        private readonly ILogger<GatewayRegisterActivity> _logger;

        public GatewayRegisterActivity(ILogger<GatewayRegisterActivity> logger, IGatewayService gatewayService)
        {
            _logger = logger;
        }
        
        //Unregister your gateway, rollback logic of execution part
        public async Task<CompensationResult> Compensate(CompensateContext<IGatewayRegisterLogs> context)
        {
            await _gatewayService.Unregister(context.Log.GatewayId, context.Log.TenantId, new System.Threading.CancellationToken());
            _logger.LogInformation("GatewayRegistered Compansated" + context.Log.TenantId);
            return context.Compensated();
        }

        public async Task<ExecutionResult> Execute(ExecuteContext<IGatewayRegisterArguments> context)
        {
            //Your instance that carries all the information that you produced with your business logic along with the whole process
            var instance = context.GetInstance<QuickAddTelemetryInstance, IGatewayRegisterArguments>();
            string Aei = instance.Aei, Huri = instance.Huri, Aeid = instance.CreatedAE, Name = instance.DeviceName;

            var gwRegistered = await _gatewayService.Register(context.Arguments.TenantId,
                new GatewayResourceDto()
                {
                    Aei = Aei,
                    AeId = Aeid,
                    Huri = Huri,
                    Name = Name
                });
            //Register the Gateway and get some return information and bind it to **gwRegistered** variable and use Logs object 
            //to keep gateway specific information that will be useful for compensation operations
            var registeredGateway = new GatewayRegisterLogs()
            {
                TenantId = context.Arguments.TenantId,
                GatewayId = gwRegistered.Id,
                AeId = gwRegistered.M2mAeId,
            };

            // Update your instance with the created gatewayid, so that you can use this Id in the incoming activities
            instance.GatewayId = gwRegistered.Id;

            //Successfully completes this activity by updating your instance that is going to be carried to incoming activities
            return context.CompletedWithInstanceUpdate<QuickAddTelemetryInstance, IGatewayRegisterArguments, IGatewayRegisterLogs>(registeredGateway, instance);
        }
    }
```

This is your instance which contains all the information along with the routing slip and you fill each property
while you proceed in this activity path, whereas some of them already defined at the very beginning.
```csharp
public class QuickAddTelemetryInstance : IRoutingSlipInstance
    {
        public Guid CorrelationId { get; set; }
        public Guid TelemetryId { get; set; }
        public Guid SensorId { get; set; }
        public Guid GatewayId { get; set; }
        public string CreatedCnt { get; set; }
        public string Aei { get; set; }
        public string Huri { get; set; }
        public string CreatedAE { get; set; }
        public string DeviceName { get; set; }
        public Guid TenantId { get; set; }
        public Guid AssetId { get; set; }
        public string DataColumnName { get; set; }
    }
```
**2. Register your activities and startup logic**

```csharp
            services.AddMassTransitBus((x) =>
            {
                var routingSlipCompleted = "Routing-Slip-Completed";
                var routingSlipFailed = "Routing-Slip-Failed";

                //Dependency inject your all activities
                x.AddActivitiesFromNamespaceContaining<GatewayRegisterActivity>();
                x.AddRabbitMqBus(Configuration, (provider, cfg) =>
                {
                    cfg.ConsumeRoutingSlipActivity<GatewayRegisterActivity, IGatewayRegisterArguments, IGatewayRegisterLogs>(provider, e =>
                    {
                        //You can use retry mechanism if an execution method fails
                        e.UseMessageRetry(r =>
                        {
                            r.Interval(3, TimeSpan.FromSeconds(5));
                        });
                    });

                    cfg.ConsumeRoutingSlipActivity<SensorRegisterActivity, ISensorRegisterArguments, ISensorRegisterLogs>(provider, e =>
                    {
  
                    });
                    cfg.ConsumeRoutingSlipActivity<AddTelemetryToAssetTypeActivity, IAddTelemetryToAssetTypeArguments, IAddTelemetryToAssetTypeLogs>(provider, e =>
                    {
  
                    });
                });
            });

```


RoutingSlip Completion Notifiers can be added beside of activities so that you will know where the end of story is:
```csharp
      //Those two consumers are the notifiers if your routing slip successfully completed or failed
     x.AddConsumer<RoutingSlipCompletedConsumer>();
     x.AddConsumer<RoutingSlipFailedConsumer>();
//.....
//Create two consumer that inherits RoutingSlipCompleted and Faulted Message Types from MassTransit
public class RoutingSlipCompletedConsumer : IConsumer<RoutingSlipCompleted>
//.....
public class RoutingSlipFailedConsumer : IConsumer<RoutingSlipFaulted>
```

You can use this registration extension for your execute only (without compensation) activities
```csharp
ConsumeRoutingSlipExecuteOnlyActivity<TActivity, TArguments> rather than ConsumeRoutingSlipActivity<TActivity, TArguments, TLogs>
```
**3. Build your routing slip and initiate**

```csharp

    //Inject a buscontrol or a bus to use routing slip
    private readonly IBusControl _busControl;
    public MyService(IBusControl busControl)
    {
        _busControl = busControl;
    }

    public void RoutingSlipStarter()
    {
        //Initiate a builder with a tracking number(Guid)
        var builder = new RoutingSlipBuilder(routingSlipTrackingNumber);
        
        //Create an instance for your routing slip journey
        var routingSlipInstance = new QuickAddTelemetryInstance() { CorrelationId = correlationId.Value, TenantId = tenantId };
        
        //Tweak over your instance with some implemented logic and populate
        var dataColumnName = await _service.GetColumnNameByTenantId(tenantId);
        routingSlipInstance.DataColumnName = dataColumnName;
        
        //Add your activities by also passing some arguments that can be used while in the activity
        builder.AddActivity<IDeviceCreateArguments>(busControl, new DeviceCreateArguments() { CseId = "1", Name = "gwname" });
        builder.AddActivity<ISensorCreateArguments>(busControl, new SensorCreateArguments() { Name = "sensorname", CseId = "1" });
        //Set your created instance, this is your journey logging book
        builder.SetInstance<QuickAddTelemetryInstance>(routingSlipInstance);

        //Build it and execute
        var routingSlip = builder.Build();
        await busControl.Execute(routingSlip);
    }

```

Now you are good to go! Each activity can be defined in seperate APIs, they will somehow find each other.

Request/Response Async
---

> This pattern requires Carbon.Redis to use persistent saga! Please see Carbon.Redis basic usage.

This pattern works similarly to GetResponse/Respond pattern of Masstransit. But this one uses saga orchestration behind the scenes and no timeouts are expected.
Requestor can retrieve a response from the regarding endpoint at any time by also creating a loose-coupling. This pattern also is useful for external integrations.
You implement your request logic from a common point and integrators with a specific context work in a seperate place simply listening to
requestor. Get the request with some parameters and do your integration stuff from any time and to any time and return your response. Requestor
will be looking forward to seeing your response.

> Heads up! This pattern is asynchronous. However, synchronous one will also be introduced if you keep proceed reading.

All requests and responses are handled with strings. So you can carry xml, json or raw string as you desire.

**1. Prepare your request handler consumer(Responder)**

This consumer will reside in the API that welcomes the request and handles the logic
```csharp
    public class ExternalDataConsumer : IConsumer<IRequestCarrierRequest>
    {
        private readonly IIntegrationService _integrationService;
        private readonly IDataService _dataService;
        public ExternalDataConsumer(IIntegrationService integrationService, IDataService dataService)
        {
            _integrationService = integrationService;
            _dataService = dataService;
        }
        public async Task Consume(ConsumeContext<IRequestCarrierRequest> context)
        {
            try
            {
                //Get your request body as string and deserialize into your prefered object
                var getData = JsonConvert.DeserializeObject<ExternalApiMessageCarrier>(context.Message?.RequestBody);
                //Do something with your deserialized object, maybe more deserialization if it is a json string
                var data = JsonConvert.DeserializeObject<ScheduledWeatherInfoDto>(getData.RequestBody);
                //Get the weather information from a 3rd party service
                var serviceResult = await _integrationService.GetWeatherInfo(new WeatherRequestDto()
                {
                    Lat = data?.Lat,
                    Lon = data?.Lon,
                    SubscriptionKey = data?.SubscriptionKey
                });
                //Prepare your response
                var result = new ScheduledWeatherInfoResponseDto()
                {
                    Humidity = serviceResult?.Humidity,
                    Temperature = serviceResult?.Temperature
                };
                //Serialize it into a response string as you desire
                getData.ResponseBody = JsonConvert.SerializeObject(result);
                //Send your response back to the requestor as successful
                await context.SendResponseToReqRespAsync(JsonConvert.SerializeObject(getData), StaticHelpers.ResponseCode.Ok);
            }
            catch (System.Exception ex)
            {
                //Send your response back to the requestor as failed
                await context.SendResponseToReqRespAsync(
                    JsonConvert.SerializeObject(new ExternalApiMessageCarrier()
                    {
                        ResponseBody = JsonConvert.SerializeObject(ex.Message)
                        
                    }), StaticHelpers.ResponseCode.ServerError);
            }
        }
    }
```

**2. Prepare your response handler consumer(Requestor)**

This consumer will reside in the API that creates the request and welcomes the response and handles the response logic

```csharp
public class ExternalDataResponseHandlerConsumer : IConsumer<IResponseCarrier>
    {
        private readonly IExternalDataService _externalDataService;

        public ExternalDataResponseHandlerConsumer(IExternalDataService externalDataService)
        {
            _externalDataService = externalDataService;
        }
        public async Task Consume(ConsumeContext<IResponseCarrier> context)
        {
            Log.Information($"Response Received {context.Message.CorrelationId} {context.Message.ResponseCode} {context.Message.ResponseBody}");
            //Collect the response from response body as string
            string responseMessage = context.Message.ResponseBody;
            //Convert into a known object
            var messageModel = JsonConvert.DeserializeObject<ExternalApiMessageCarrier>(responseMessage);
            //Use response code if needed
            var isHealthy = context.Message.ResponseCode == Carbon.MassTransit.AsyncReqResp.StaticHelpers.ResponseCode.Ok ? true : false;
            Log.Information($"isHealty :  {isHealthy}");           

            if (isHealthy)
            {
                try
                {
                    //Create your data by the given response
                    var createdEntity = await _externalDataService.CreateAsync(new Dto.ExternalDataCreateDto()
                    {
                        ExternalDataSourceId = messageModel.DataSourceId,
                        ValueAsStr = messageModel.ResponseBody,
                        CorrelationId = context.Message.CorrelationId,
                    });
                    Log.Information($"Inserted externalData {createdEntity.Id}");
                }
                catch (Exception exp)
                {
                    Log.Error($"Update Error externalData  CorrelationId: {context.Message.CorrelationId} : {exp.Message}");
                }
            }
            

        }
    }
```

**3. Register your requestor to your API**

This will register your requestor and response handler (in startup)
```csharp
    //See Carbon.Redis package in order to make it up and running
    services.AddRedisPersister(builder.Configuration);
    services.AddAsyncRequestResponsePatternForRequestor<ExternalDataResponseHandlerConsumer>(builder.Configuration);
```

**4. Register your responder to your API**

This will register your responder and request handler (in startup)
```csharp
//Use a user-friendly and sensible destination name such as **MyIntegrationApi**, this destination name can be targeted
//by your request sender so that you request finds this responder well
services.AddAsyncRequestResponsePatternForResponder<ExternalDataConsumer>(Configuration, "MyIntegrationApi");
```
**5a. Send your request from requestor API (Async)**

Inject your buscontrol inherited from IReqRespRequestorBus and use the given method as below. Please note that this
approach is async, so that *SendRequestToReqRespAsync* won't be waiting for any response and simply continue. You can handle your response as given in the step 2.
```csharp

private readonly IReqRespRequestorBus _busControl;
public ExternalDataRequestSender(ILogger<ExternalDataRequestSender> logger,
            IReqRespRequestorBus busControl)
        {
            _logger = logger;
            _busControl = busControl;
        }

await _busControl.SendRequestToReqRespAsync(messageModelAsStr, "MyIntegrationApi"); 
```
**5b. Send your request from requestor API (Sync)**
This approach on the contrary of the previous one as a sync which awaits for a response within a given timeout.
So if, Step 5a is not working for you, you can use this one as well.

```csharp
private readonly IReqRespRequestorBus _busControl;
public ExternalDataRequestSender(ILogger<ExternalDataRequestSender> logger,
            IReqRespRequestorBus busControl)
        {
            _logger = logger;
            _busControl = busControl;
        }
//Sync awaiter for a response at most 2 minutes (Timeout)
await _reqRespRequestorBus.GetResponseFromReqRespAsync(messageModelAsStr, "MyIntegrationApi", new TimeSpan(0,2,0));
```

In order to send a response to this awaiter, you have to use the given method below just in your response handler which is also described in step 2.

```csharp
public class ExternalDataResponseHandlerConsumer : IConsumer<IResponseCarrier>
    {
        private readonly IExternalDataService _externalDataService;

        public ExternalDataResponseHandlerConsumer(IExternalDataService externalDataService)
        {
            _externalDataService = externalDataService;
        }
        public async Task Consume(ConsumeContext<IResponseCarrier> context)
        {
            //...Implement your all logic as is given in the step 2, and use this method as the last
            //This will respond to your requet awaiter.
            await context.RespondToReqRespAsync(context.Message.ResponseBody);
        }
    }
```

Use Routing Slip in Request/Response Async pattern
------
As described above, routing slip including all activities are async operation which is not awaited where you initiate. For example, you
wanted to do a chain of implementations such as sensor registering by triggering a controller method, however you want to await a response from
your routing slip, so that you return the created object information to triggerer user via given controller method. In this case, 
you can initiate a routing slip from your Request Handler.

You basically register as given in the previous section, select your pattern (sync/async) as desired.

Now you can start a routing slip session directly from your request handler. 

> Please note that, your routing slip or other sessions should be correlated with the same correlationId.


```csharp
    public class ExternalDataConsumer : IConsumer<IRequestCarrierRequest>
    {
        private readonly IIntegrationService _integrationService;
        private readonly IDataService _dataService;
        public ExternalDataConsumer(IIntegrationService integrationService, IDataService dataService)
        {
            _integrationService = integrationService;
            _dataService = dataService;
        }
        public async Task Consume(ConsumeContext<IRequestCarrierRequest> context)
        {
                //Get your request body as string and deserialize into your prefered object
                var getData = JsonConvert.DeserializeObject<ExternalApiMessageCarrier>(context.Message?.RequestBody);
                //Create and build your routing slip as described in the previous sections and return your build routing slip object by using the same correlationid
                RoutingSlip yourAlreadyBuiltRoutingSlip = _dataService.CreateAndBuildYourRoutingSlip(context.CorrelationId);
                //You can execute your built routing slip object like this
                await context.Execute(yourAlreadyBuiltRoutingSlip);

                //This time do not response to your requestor unless your routing slip ends
                //await context.SendResponseToReqRespAsync(JsonConvert.SerializeObject(getData), StaticHelpers.ResponseCode.Ok);
        }
    }
```

Here we consumed our request and started a routing slip session and did not send response to our requestor, so our requestor still pending response
Remember we registered routing slip completed consumers where your routing slip hits at the very end.
```csharp
     x.AddConsumer<RoutingSlipCompletedConsumer>();
     x.AddConsumer<RoutingSlipFailedConsumer>();
```
In both consumer returns your response to the requestor as given below. If your routing slip failed, you can send response as failed status.

```csharp
    public class RoutingSlipCompletedConsumer : IConsumer<RoutingSlipCompleted>
    {

        public RoutingSlipCompletedConsumer()
        {

        }

        public async Task Consume(ConsumeContext<RoutingSlipCompleted> context)
        {
            if (context.Message.Variables.TryGetValue(IRoutingSlipBuilder.InstanceName, out var inst))
            {
                //Collect your latest instance from routingslip and response to your requestor directly from this point
                await context.SendResponseToReqRespAsync(inst.ToString());
            }
        }

    }

```

In your response consumer, handle your routing slip instance response, do your logic and take necessary action. If you started
to the journey with sync(5b) method by GetResponseFromReqRespAsync, then use RespondToReqRespAsync at the end of your logic.

```csharp
public class ExternalDataResponseHandlerConsumer : IConsumer<IResponseCarrier>
    {

        public ExternalDataResponseHandlerConsumer(IExternalDataService externalDataService)
        {
        }
        public async Task Consume(ConsumeContext<IResponseCarrier> context)
        {
            Log.Information($"Response Received {context.Message.CorrelationId} {context.Message.ResponseCode} {context.Message.ResponseBody}");
            //Collect the response from response body as string
            string responseMessage = context.Message.ResponseBody;
            //Do your response logic, Response Body contains your instance from routing slip...
            
            //If you are using synchronous(5b) approach of ReqRespAsync pattern, respond to your awaiting requestor
            //If you are using asynchronous(5a), then omit this line
            await context.RespondToReqRespAsync(context.Message.ResponseBody);
        }
            

        
    }
```

