# Carbon.TimeScaleDb.EntityFrameworkCore

> TimescaleDB is an open-source database designed to make SQL scalable for time-series data. 
> It is engineered up from PostgreSQL and packaged as a PostgreSQL extension, 
> providing automatic partitioning across time and space (partitioning key), as well as full SQL support.[**]

[**] https://github.com/timescale/timescaledb

This package brings you essential timescale capabilities by makes you leverage the benefits of 
entity framework core without leaving your comfort zone over dbcontext, linq queries, code-first database-table migrations.

This package can simply create a code-first timescale database and table by migrating your database object and
entity configurations + some timeserie-based ready-to-use TsDb queries and free-style linq .net queries. More incoming...

Beside of all the benefits, this package also supports CQRS pattern where a read-only replica available for TimeScaleDb and does the 
read operations through read-replica, write operations through master.



## Add TimeScaleDb Support to Your Project

This package contains some other related building block packages. Those packages are the base and the abstractions and also powerful ones.
Nevertheless, this package wraps up everything and works fine for you!

| Related Package                                                                  | Required | Auto-Included |
|-----------------------------------------------------------------                 |:----:    |:----:         |
| [Carbon.TimeScaleDb](../Carbon.TimeScaleDb/README.md)                            | Yes      | Yes           |
| [Carbon.TimeSeriesDb.Abstractions](../Carbon.TimeSeriesDb.Abstractions/README.md)| Yes      | Yes           |

### Basic Usage

You can add a masstransit support with this code snippet which starts a hosted masstransit service with all the healthchecks and configuration managaments behind the scenes.
This will only builds a consumer and binds it to a queue namely *my-queue* and starts to consume it. This is the most basic 
approach ever seen.

**1. Create a database object and inherit from *TimeSerieEntityBase***

Use **[TimeSerie]** tag for a DateTime field that you want it to be timeserie indexed. This will also create an PK index with a composition of Id + Date where *Id* is auto generated field.
```csharp
    public class TimeSerieDataLog : TimeSerieEntityBase
    {
        public Guid AssetId { get; set; }
        public Guid TelemetryId { get; set; }
        public decimal Value { get; set; }
        [TimeSerie]
        public DateTime Date { get; set; }
        
        //You can have some other relations with other tables
        public Guid? TimeSerieDataLogLabelRelationId { get; set; }
        public virtual TimeSerieDataLogLabelRelation TimeSerieDataLogLabelRelation { get; set; }

    }
```
**2. Create your Entity Configuration**

You can create such an Entity Configurator by using the given base class to inherit. Simply override and implement your
configurations, then refer to the base.
```csharp
public class TimeSerieDataLogConfiguration : TimeSeriesEntityTypeConfiguration<TimeSerieDataLog>
    {
        public override void Configure(EntityTypeBuilder<TimeSerieDataLog> builder)
        {
            //Create some FK relation with other table when needed (Optional, when needed)
            builder.HasOne(x => x.TimeSerieDataLogLabelRelation).WithOne(x => x.TimeSerieDataLog).HasForeignKey<TimeSerieDataLog>(x => x.TimeSerieDataLogLabelRelationId);
            builder.HasIndex(x => x.TimeSerieDataLogLabelRelationId).IsUnique(false);
            //Jump to the base (Required)
            base.Configure(builder);
        }
    }
```

**3. Create your DBContext**

```csharp
public class TimeSerieDataLogContext : CarbonTimeScaleDbContext<TimeSerieDataLogContext>
    {
        public TimeSerieDataLogContext(DbContextOptions<TimeSerieDataLogContext> options) : base(options)
        {

        }
        public DbSet<TimeSerieDataLog> TimeSerieDataLog { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new TimeSerieDataLogConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
```

**4. Create your Repository Pattern**

Now you can create a repository by inheriting from base of EFTimeScaleDbRepository which enables a timescaledb context and useful Create, Read, Update, Delete operations.
You can add more custom methods to your repository with your own way and also you can add a repository interface.

```csharp
public class TimeScaleDataLogRepository : EFTimeScaleDbRepository<TimeSerieDataLog, TimeSerieDataLogContext>, ITimeScaleDataLogRepository<TimeSerieDataLog>
    {
        private readonly TimeSerieDataLogContext _timeSerieDataLogContext;

        public TimeScaleDataLogRepository(TimeSerieDataLogContext context) : base(context)
        {
            _timeSerieDataLogContext = context;
        }

    }


public interface ITimeScaleDataLogRepository<TEntity> : ITimeSeriesEntityRepository<TEntity>
        where TEntity : class, ITimeSeriesEntity
    {
        
    }
```

Don't forget to register your repository at startup!

```csharp
//IServiceCollection services
services.AddScoped<ITimeScaleDataLogRepository<TimeSerieDataLog>, TimeScaleDataLogRepository>();
```

**5. Register the Context and Migrate your code-first objects to TSDB**

This startup extension will check if the given database is timescale enabled and if yes, will create the necessary 
database objects with the given ones. If there is such a non-timescale table with the same name, it will convert it into timescale-based table
and migrate the data if possible.

- If you use startup just like in Dotnet 5 or backwards, or a non-minimal api in Dotnet 6 or more
```csharp
//IServiceCollection services
services.AddTimeScaleDatabaseContext<TimeSerieDataLogContext, Startup>(Configuration);
```
- If you use minimal API
```csharp
//IServiceCollection services
services.AddTimeScaleDatabaseContext<TimeSerieDataLogContext, Program>(Configuration);
```

Do the Migration:
```csharp
//IApplicationBuilder app
app.MigrateTimeScaleDatabase<TimeSerieDataLogContext>();
```

**5. Finally use a configuration section as given below**

```json
 "ConnectionStrings": {
    "TimeScaleDbConnectionString": "Server=tsdbserver;Database=tsdatalogdb_test;Port=5432;User Id=tsdbuser;Password=xxxxxx;",
  }
```

## Advanced Usage
### CQRS pattern with Read-Replica

If you have another read-replica for your master timescaledb, you can use that replica for read-purposes in order to more efficient read operations and not to
keep occupied master one.

Prior to reading this part, please make sure you have already read the Basic Usage section and implemented the neccessary operations.  

**1. Create another Read-Only Context on the top of usual one**

```csharp
public class TimeSerieDataLogReadOnlyContext : CarbonTimeScaleDbReadOnlyContext<TimeSerieDataLogReadOnlyContext>
    {
        public TimeSerieDataLogReadOnlyContext(DbContextOptions<TimeSerieDataLogReadOnlyContext> options) : base(options)
        {

        }
        public DbSet<TimeSerieDataLog> TimeSerieDataLog { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new TimeSerieDataLogConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
```

**2. Create new one or Update your existing repository to EFTimeScaleDbWithReadOnlyRepository**

If you have already a repository as it is explained in the basic usage, you can convert it into WithReadOnlyRepository

```csharp
public class TimeScaleDataLogRepository : EFTimeScaleDbWithReadOnlyRepository<TimeSerieDataLog, TimeSerieDataLogContext, TimeSerieDataLogReadOnlyContext>, ITimeScaleDataLogRepository<TimeSerieDataLog>
    {
        //Your read-write context
        private readonly TimeSerieDataLogContext _timeSerieDataLogContext;
        //Your read-only context
        private readonly TimeSerieDataLogReadOnlyContext _timeSerieDataLogReadOnlyContext;

        public TimeScaleDataLogRepository(TimeSerieDataLogContext context, TimeSerieDataLogReadOnlyContext rContext) : base(context, rContext)
        {
            _timeSerieDataLogContext = context;
            _timeSerieDataLogReadOnlyContext = rContext;
        }
    }
```
You can also add new methods as you wish. Please note that, if your method is doing read-only operations, you can use readOnly context for such operations.

**3. Change the registration way of your Context**

In the basic usage you used AddTimeScaleDatabaseContext, now change it into *AddTimeScaleDatabaseWithReadOnlyReplicaContext* to register your read-only context as well

```csharp
services.AddTimeScaleDatabaseWithReadOnlyReplicaContext<TimeSerieDataLogContext, TimeSerieDataLogReadOnlyContext, Startup>(Configuration);
```

or...

```csharp
services.AddTimeScaleDatabaseWithReadOnlyReplicaContext<TimeSerieDataLogContext, TimeSerieDataLogReadOnlyContext, Program>(Configuration);
```

**4. Extend your configuration with read-only replica information**

You can switch on/off Read Replica Enabled property. If set *false*, then read-only context will also use the master one instead of read replica.
```json
"ConnectionStrings": {
    "ReadReplicaEnabled": true,
    "TimeScaleDbConnectionString": "Server=tsdbserver;Database=tsdatalogdb_test;Port=5432;User Id=tsdbuser;Password=xxxxxx;",
    "TimeScaleDbReadOnlyConnectionString": "Server=tsdbserver_readonly;Database=tsdatalogdb_test;Port=5432;User Id=tsdbuser;Password=xxxxxx;"
  }
```

Now, you are all set and good to go!

### TimeScale-Based Functions Usage
You can use hyperfunctions or other tsdb based functions in your repository. The only thing you have to do is to define them as a free-text,
and use FromSqlRaw efcore extension.

Assume that you have a **TimeBucket** method introduced additionally to your repository and a view object *TimeSerieDataLogTimeBucketView*.

```csharp
        public async Task<List<TimeSerieDataLogTimeBucketView>> GetTelemetryByTimeBucket(AssetTelemetryAggregatedTimeBucketRequestData assetTelemetryAggregatedTimeBucketRequestData)
        {
            string ersanQuery = "";
            string daTimeBucket = assetTelemetryAggregatedTimeBucketRequestData.TimeBucketDuration + " " + assetTelemetryAggregatedTimeBucketRequestData.TimeBucketType.ToString();

            if (assetTelemetryAggregatedTimeBucketRequestData.TimeBucketType != TimeBucketType.year && assetTelemetryAggregatedTimeBucketRequestData.TimeBucketType != TimeBucketType.month)
                ersanQuery = @$"SELECT assetid , MAX(assetname) as assetname , telemetryid , MAX(telemetryname) as telemetryname , time_bucket('{daTimeBucket}', date) AS bucketdatetime, {assetTelemetryAggregatedTimeBucketRequestData.AggregationType}(value) as value
                        FROM timeseriedatalog
                        where telemetryid  = ANY(@telemetryids) and assetid = ANY(@assetids) and date < @enddate and date > @startdate and tenantid = @tenantid 
                        GROUP BY bucketdatetime, assetid, telemetryid
                        ORDER BY bucketdatetime DESC;";
            else if (assetTelemetryAggregatedTimeBucketRequestData.TimeBucketType == TimeBucketType.year || assetTelemetryAggregatedTimeBucketRequestData.TimeBucketType == TimeBucketType.month)
                ersanQuery = @$"SELECT assetid , MAX(assetname) as assetname , telemetryid , MAX(telemetryname) as telemetryname, timescaledb_experimental.time_bucket_ng('{daTimeBucket}', date) AS bucketdatetime, {assetTelemetryAggregatedTimeBucketRequestData.AggregationType}(value) as value
                        FROM timeseriedatalog
                        where telemetryid  = ANY(@telemetryids) and assetid = ANY(@assetids) and date < @enddate and date > @startdate and tenantid = @tenantid 
                        GROUP BY bucketdatetime, assetid, telemetryid
                        ORDER BY bucketdatetime DESC;";

            NpgsqlParameter start = new NpgsqlParameter("@startdate", assetTelemetryAggregatedTimeBucketRequestData.StartDate);
            NpgsqlParameter end = new NpgsqlParameter("@enddate", assetTelemetryAggregatedTimeBucketRequestData.EndDate);
            NpgsqlParameter tenantId = new NpgsqlParameter("@tenantid", assetTelemetryAggregatedTimeBucketRequestData.TenantId);
            NpgsqlParameter telemetryIds = new NpgsqlParameter("@telemetryids", NpgsqlDbType.Array | NpgsqlDbType.Uuid);
            telemetryIds.Value = assetTelemetryAggregatedTimeBucketRequestData.TelemetryIds.ToArray();

            NpgsqlParameter assetIds = new NpgsqlParameter("@assetids", NpgsqlDbType.Array | NpgsqlDbType.Uuid);
            assetIds.Value = assetTelemetryAggregatedTimeBucketRequestData.AssetIds.ToArray();

            return await _timeSerieDataLogReadOnlyContext.Set<TimeSerieDataLogTimeBucketView>().FromSqlRaw(ersanQuery, start, end, telemetryIds, tenantId, assetIds).AsNoTracking().ToListAsync();
        }

        //This is a custom object pretends to be a view object
        public class TimeSerieDataLogTimeBucketView
        {
            public Guid AssetId { get; set; }
            public string AssetName { get; set; }
            public Guid TelemetryId { get; set; }
            public string TelemetryName { get; set; }
            public DateTime BucketDateTime { get; set; }
            public double Value { get; set; }
        }
```
Define the custom view in your readonly context 

> As this is a read-only query, this is the most appropriate place to define it. However, defining in your usual context is also fine!

```csharp
public class TimeSerieDataLogReadOnlyContext : CarbonTimeScaleDbReadOnlyContext<TimeSerieDataLogReadOnlyContext>
    {
        public TimeSerieDataLogReadOnlyContext(DbContextOptions<TimeSerieDataLogReadOnlyContext> options) : base(options)
        {

        }
        public DbSet<TimeSerieDataLog> TimeSerieDataLog { get; set; }

        //Add this view to your context
        public DbSet<TimeSerieDataLogTimeBucketView> TimeSerieDataLogTimeBucketView { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new TimeSerieDataLogConfiguration());

            //Tell Entity Configurator not to do with this database object at timescale-side
            modelBuilder.Entity<TimeSerieDataLogTimeBucketView>(entity =>
            {
                entity.HasNoKey();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
```
---

### Non-TimeScaleDb Tables (Raw PostgreSQL Tables)
It is possible to create non-timescaledb tables, in other words, raw (usual) postgre tables. As this package is also powered by PostgreSQL EF core packages
you have a combined and mighty ruling capability. Those tables and database objects are also migrated within the same migration session.

```csharp
public class TimeSerieDataLogContext : CarbonTimeScaleDbContext<TimeSerieDataLogContext>
    {
        public TimeSerieDataLogContext(DbContextOptions<TimeSerieDataLogContext> options) : base(options)
        {

        }
        //Tsdb based table
        public DbSet<TimeSerieDataLog> TimeSerieDataLog { get; set; }
        //non-tsdb, pure postgresql relation table
        public DbSet<TimeSerieDataLogLabelRelation> TimeSerieDataLogLabelRelation { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new TimeSerieDataLogConfiguration());
            modelBuilder.ApplyConfiguration(new TimeSerieDataLogLabelRelationConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }

   //Not a timescaledb table
   public class TimeSerieDataLogLabelRelation : IEntity
    {
        public Guid Id { get; set; }
        public virtual TimeSerieDataLog TimeSerieDataLog { get; set; }
        public virtual ICollection<TimeSerieDataLogLabel> TimeSerieDataLogLabel { get; set; }

    }

    public class TimeSerieDataLogLabelRelationConfiguration : IEntityTypeConfiguration<TimeSerieDataLogLabelRelation>
    {
        public void Configure(EntityTypeBuilder<TimeSerieDataLogLabelRelation> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.TimeSerieDataLog).WithOne(x => x.TimeSerieDataLogLabelRelation).HasForeignKey<TimeSerieDataLog>(x => x.TimeSerieDataLogLabelRelationId);
            builder.HasMany(x => x.TimeSerieDataLogLabel).WithOne(x => x.TimeSerieDataLogLabelRelation).HasForeignKey(x => x.DataLogLabelRelationId);

        }
    }
```

