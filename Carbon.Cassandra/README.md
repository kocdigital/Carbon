# Carbon.Cassandra [<img alt="Nuget (with prereleases)" src="https://img.shields.io/nuget/vpre/Carbon.Cassandra">](https://www.nuget.org/packages/Carbon.Cassandra)

> Cassandra is an open source NoSQL distributed database trusted by thousands of companies for scalability and high availability without compromising performance. Linear scalability and proven fault-tolerance on commodity hardware or cloud infrastructure make it the perfect platform for mission-critical data.[**]
[**] https://cassandra.apache.org/_/

This package brings you a good wrapper and extension library for CassandraCsharpDriver which is directly compatible with any scalable, clusterable, fail-over capable and persistent enviroment.
If you have such environment, you can easily refer to this library and quickly start using CassandraCsharpDriver capabilities.

CassandraCsharpDriver, out of the box, offers all the capabilities which this package offers, however in a long-way. You need to have a good knowledge set about the usage of Cassandra, CassandraCsharpDriver and know what you are doing.

Let Carbon know about CassandraCsharpDriver, you simply use!

## Add Cassandra Support to Your Project


| Related Package                                                                  | Required | Auto-Included |
|-----------------------------------------------------------------                 |:----:    |:----:         |
| [Carbon.Cassandra.Abstractions](../Carbon.Cassandra.Abstractions/README.md)      | No       | No            |

### Basic Usage

You can add a Cassandra support with this code snippet which binds cassandra settings from config and injects 
[ICassandraSessionFactory](https://github.com/kocdigital/Carbon/blob/master/Carbon.Cassandra.Abstractions/ICassandraSessionFactory.cs),
[ICassandraPersisterSettings](https://github.com/kocdigital/Carbon/blob/master/Carbon.Cassandra.Abstractions/ICassandraPersisterSettings.cs) classes for usage.

At first step, you need a mapping class for your entites like below;

```csharp
    public class CassandraMappings : Mappings
    {
        public CassandraMappings()
        {
            For<MyEntity>().TableName("my_entity")
            .PartitionKey(x => x.Name)
            .Column(x => x.Name, n => n.WithName("name"))
            .Column(x => x.Id, n => n.WithName("id"))
            .Column(x => x.IsDeleted, n => n.WithName("is_deleted"));            
        }
    }
```

And when you have that kind of mapping you can basically add code snipped below to your startup.cs/program.cs file.

```csharp
    services.AddCassandraPersister(hostContext.Configuration, options =>
    {
        options.SetMapping(new CassandraMappings());
        options.Build();
    });    
```

Your basic cassandra configuration should be like below.
```json
"Cassandra": {
    "EndPoints": [ "cassandrahost" ],
    "Username": "cassandrauser",
    "Password": "cassandrapassword",
    "Port": "9042",
    "Keyspace": "yourkeyspace"
},
```

So, when you want to use cassandra you need an [ICassandraSessionFactory](https://github.com/kocdigital/Carbon/blob/master/Carbon.Cassandra.Abstractions/ICassandraSessionFactory.cs) instance. 
You can directly use it on your service or your repository(preferred).

> Note that this repository derives from [BaseCassandraRepository<T>](https://github.com/kocdigital/Carbon/blob/master/Carbon.Cassandra/BaseCassandraRepository.cs) which already implements crud methods.
```csharp
    public class MyEntityCassandraRepository : BaseCassandraRepository<MyEntity>
    {
        private readonly ICassandraPersisterSettings _cassandraSetting;
        public MyEntityCassandraRepository(ICassandraSessionFactory sessionFactory, ICassandraPersisterSettings cassandraSetting) : base(sessionFactory)
        {
            _cassandraSetting = cassandraSetting;
        }
        public override string Keyspace => _cassandraSetting.Keyspace;
    }
```

### Advanced Configuration

On a basic; you can read & write from already defined keyspaces and tables. But from domain driven design point; sharing a domain object/table is undesirable. 
Your application is the responsible one from all the domain objects including tables & keyspaces. So it must create them if they are not exist.

With this code examples below, you can initialize your db.

#### Create a DB Initializer Class

There should be settings class which derives from [CassandraPersisterSettings](https://github.com/kocdigital/Carbon/blob/master/Carbon.Cassandra/CassandraPersisterSettings.cs) 
or if you need some customizations [ICassandraPersisterSettings](https://github.com/kocdigital/Carbon/blob/master/Carbon.Cassandra.Abstractions/ICassandraPersisterSettings.cs).

```csharp
    public class CassandraSettings : CassandraPersisterSettings
    {
    }
```

After that you can create your initialization class like below.

```csharp
    public interface ICassandraDatabaseInitializer
    {
        void Init();
    }
    public class CassandraDatabaseInitializer : ICassandraDatabaseInitializer
    {
        private readonly ICassandraSessionFactory _sessionFactory;
        private readonly CassandraPersisterSettings _cassandraSettings;
        public CassandraDatabaseInitializer(ICassandraSessionFactory sessionFactory, IOptions<CassandraSettings> cassandraSettings)
        {
            _sessionFactory = sessionFactory;
            _cassandraSettings = cassandraSettings.Value;
        }
        public void Init()
        {
            var session = _sessionFactory.GetSessionFromDefaultCluster();
            session.CreateKeySpaceIfNotExists(_cassandraSettings.Keyspace);
            session.CreateTableIfNotExists<MyEntity>(_cassandraSettings.Keyspace);
        }
    }
```

#### Use DB Initializer

First; inject your database initializer class

```csharp
    services.Configure<CassandraSettings>(hostContext.Configuration.GetSection("Cassandra"));
    services.AddTransient<ICassandraDatabaseInitializer, CassandraDatabaseInitializer>();
```

After that call init method of database initializer. You can call it on Configure step or your desired place.

```csharp
    using (var serviceScope = app.GetService<IServiceScopeFactory>().CreateScope())
    {
        serviceScope.ServiceProvider.GetService<ICassandraDatabaseInitializer>().Init();
    }
```