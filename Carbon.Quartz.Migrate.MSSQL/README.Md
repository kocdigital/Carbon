# Carbon.Quartz.Migrate.MSSQL

> Quartz.NET is a full-featured, open source job scheduling system that can be used from smallest apps to large scale enterprise systems.[**]

[**] https://www.quartz-scheduler.net/

This package brings you the capability of single code line of migration to bring persistency of Quartz.Net for MSSQL. Check
usage details from [Carbon.Quartz](../Carbon.Quartz/README.md) 

This package contains [Carbon.Quartz.Migrate.Context](../Carbon.Quartz.Migrate.Context/README.md) which brings the migration capability.

This package can be used standalone with a Quartz.Net (https://github.com/quartznet/quartznet) package without
using Carbon.Quartz package. However, Carbon.Quartz package is also strongly suggested to be used.

**1. Register Quartz Context To Your Application**

```csharp
services.AddQuartzContext<Program>(configuration);
```
or...
```csharp
services.AddQuartzContext<Startup>(configuration);
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

This will automatically migrate your quartz database if not exist or ignore if exists.

Check how persistency works in Quartz.Net: https://www.quartz-scheduler.net/documentation/quartz-3.x/tutorial/job-stores.html#ado-net-job-store-adojobstore
