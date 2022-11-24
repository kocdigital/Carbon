# Carbon.Quartz

> Quartz.NET is a full-featured, open source job scheduling system that can be used from smallest apps to large scale enterprise systems.[**]

[**] https://www.quartz-scheduler.net/

This package brings you a good wrapper and extension library for Quartz.Net which is directly compatible with any scalable, clusterable, fail-over capable and persistent enviroment such as Kubernetes.
If you have such environment, you can easily refer to this library and quickly start using Quartz.Net capabilities.

Quartz.Net, out of the box, offers all the capabilities which this package offers, however in a long-way. You need to have a
good knowledge set about the usage of Quartz.Net and know what you are doing. This package also eases the usage and gives you some one-click
of deployment enablement for the persistency context which Quartz.Net does not offer directly where it expects you to run some SQL scripts manually before deployment.

Let Carbon know about Quartz.Net, you simply use!

## Add Carbon.Quartz Support to Your Project
### Basic Usage

| Related Package                                                                  | Required | Auto-Included |
|-----------------------------------------------------------------                 |:----:    |:----:         |
| [Carbon.Quartz.Migrate.Context](../Carbon.Quartz.Migrate.Context/README.md)      | No       | No            |
| [Carbon.Quartz.Migrate.MSSQL](../Carbon.Quartz.Migrate.MSSQL/README.md)          | No       | No            |
| [Carbon.Quartz.Migrate.PostgreSQL](../Carbon.Quartz.Migrate.PostgreSQL/README.md)| No       | No            |

This basic usage introduces a non-persistent, thus not scalable Quartz.Net approach. It is recommended dev-purposes only.

**1. Register Quartz To Your Application**

Add these registrations at startup

```csharp
//IServiceCollection services
services.AddScoped<IQuartzClusterableService, QuartzService>();
services.AddQuartzScheduler(configuration, false, "MyQuartz");
```

**2. Create a Quartz Job**

In a controller method, or at startup when application is booted, you can resolve the *IQuartzClusterableService* and start using it.
```csharp

//To be resolved by DI
private readonly IQuartzClusterableService _quartzService;

//Set your scheduler for the service, scheduler name should be the same with the one you registered at startup
_quartzService.SetSchedulerId("MyQuartz");

//Create a job data to be stored in job, so that you receive this object from the context (check step 3) for each cycle
var jobData = new ExternalApiMessageCarrier()
                {
                    DataSourceId = dto.ExternalDataSourceId,
                    SchedulerId = dto.Id,
                    DataSourceName = dto.SourceName,
                    RequestBody = dto.IntegrationDetails,
                    TenantId = dto.TenantId,
                    IsStoreData = dto.IsStoreData
                };

//Create and start a job which triggers per 10 seconds
await _quartzService.AddAndStartClusterableBasicJob<DataManagerSingleJob>("MyJob", jobData, 10);
```

**3. Create a Job Executer at each cycle**

```csharp
    public class DataManagerSingleJob : IJob
    {
        private readonly ILogger<DataManagerSingleJob> _logger;
        public DataManagerSingleJob(ILogger<DataManagerSingleJob> logger,
            IConfiguration configuration)
        {
            _logger = logger;
        }
        public async Task Execute(IJobExecutionContext context)
        {
              _logger.LogInformation($" DataManagerSingleJob sending start");
              //Retrieve the stored instance object
              var messageModel = context.GetDefaultData<ExternalApiMessageCarrier>();

              //Do sth with the object
              _logger.LogInformation($"{messageModel} DataManagerSingleJob doing its job");
        }
    }
```

You are good to go, your DataManagerSingleJob will be triggered per 10 Seconds, and let you do your business logic as you build in step 3

## Advanced Usage (Recommended for production)
### Carbon.Quartz with persistent and scalable and clusterable
Let your Quartz be persistent, otherwise you cannot scale your application up. So, this part depicts
a usage of Quartz.Net in a persistent and clusterable way.

In order to use this approach, you have to add these packages to your library on the top of Carbon.Quartz package. Migrate.Context package is required, 
however MSSQL or PostgreSQL is selectable regarding of which type of database you want to use. 
If you will have both, you can add both of them. Migrations are switchable via configuration file from one to the other.

Quartz.Net, behind the scenes, uses an SQL database to enable persistency. Carbon.Quartz supports two most popular database (MSSQL and PostgreSQL).
Finally it migrates the tables to the selected database, so that you don't have to concern with anything about it.

| Related Package                                                                  | Required | Auto-Included |
|-----------------------------------------------------------------                 |:----:    |:----:         |
| [Carbon.Quartz.Migrate.Context](../Carbon.Quartz.Migrate.Context/README.md)      | Yes      | No            |
| [Carbon.Quartz.Migrate.MSSQL](../Carbon.Quartz.Migrate.MSSQL/README.md)          | Yes      | No            |
| [Carbon.Quartz.Migrate.PostgreSQL](../Carbon.Quartz.Migrate.PostgreSQL/README.md)| Yes      | No            |

**1. Register Quartz To Your Application**

*On startup =>*

Register quartz context
```csharp
services.AddQuartzContext<Program>(configuration);
```
or...
```csharp
services.AddQuartzContext<Startup>(configuration);
```
Then register the service and scheduler as you do in basic usage
```csharp
services.AddScoped<IQuartzClusterableService, QuartzService>();

//Your scheduler (api) executes 10 job at a time concurrently which is the max concurrency.
//Persistent, clusterable, scalable and failover enabled scheduler
services.AddQuartzScheduler(configuration, true, "MyQuartz", 10);
```

**2. Migrate Quartz Persistency Tables To Your Database**

*On startup =>*

This migration happens depending on your configuration.
```csharp
//IApplicationBuilder app
app.MigrateQuartz();
```
Using the configuration below:

```json
"Quartz": {
    "ConnectionStrings": {
      "DefaultConnection": "Server=yourdatabase;Database=yourquartzdb;User ID=user;Password='pass';Connect Timeout=30;",
      "ConnectionTarget": "MSSQL"
    }
  }
```

> "ConnectionTarget": "MSSQL" or "PostgreSQL"

This will automatically migrate your quartz database if not exist or ignore if exists. Now, you are persisted,
and create multiple instances of your APIs. Each API will share the load for the same scheduler id while triggering and executing the jobs, so there will be
no duplicate triggers. 

Check how persistency works in Quartz.Net: https://www.quartz-scheduler.net/documentation/quartz-3.x/tutorial/job-stores.html#ado-net-job-store-adojobstore

**3. Create a Quartz Job and job executer**

As the last but not the least, you can create your quartz job and job executer as defined in the **basic usage** section (Step 2 and 3).


## More Usage Details

QuartzService that you register to your API contains couple of useful methods.

---
```csharp
AddAndStartClusterableCustomJob<TJob>(string jobName, object jobData, ITrigger trigger) where TJob : IJob
```
This method lets you add a custom job as IJob (which is Quartz.Net specific object). You can configure your triggers customly
as it is done in Quartz.Net package. Please check the documentation.

---
```csharp
DeleteJob(string jobName)
```
Deletes a job with a name that you already added with AddAndStartClusterableJob or CustomJob

---
```csharp
ClearAllJobsExceptFor(List<string> excludingJobKeyList)
```

Clears all the jobs except for given job names. If empty list of job given, it clears all the jobs you added
for the given schedulerId.

---