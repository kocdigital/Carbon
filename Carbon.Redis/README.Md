# Carbon.Redis

This package is powered up by multiple useful Redis libraries such as Stackexchange.Redis [1], RedLock.net [2], Microsoft.Extensions.Caching.StackExchangeRedis [3]  that provide remarkable Redis capabilities
to your projects in many aspects.

>[1] https://github.com/StackExchange/StackExchange.Redis
> 
>[2] https://github.com/samcook/RedLock.net
> 
>[3] https://github.com/dotnet/aspnetcore

This package wraps up some capabilities such as (Redis Lock, Redis PubSub, Redis Key Operations) in a single pack and presents them you with an easier and more maintainable way. It also creates some handy logics for Redis Operations which
creates ease of usage and out-of-the-box serialization switch enablement which provides you with some object conversions. Also a Redis Healthcheck will be added
automatically to your **/health** endpoint.


## Adding Redis Support to Your Project

### Basic Usage

You can add Carbon.Redis support only with StackExchange.Redis capabilities (without the other two libraries). With this package
you can have a Redis connection (ConnectionMultiplexer) in order to be used for any purpose you like. This usage gives you a good set of functionalities
and provides you with automatic heath check capability at **/health** endpoint.

This usage also is a baseline for [Carbon.MassTransit](../Carbon.MassTransit/README.Md) Saga Pattern persistency.

**1. Register Carbon.Redis to your service**

```csharp
//IServiceCollection services
services.AddRedisPersister(Configuration);
```

**2. Get to start by configuring as below**

Using the configuration given below, all the connections are managed by StackExchange.Redis, however this configuration may not contain all connection properties that StackExchange offers.
```json
"Redis": {
    "Enabled":true,
    "KeyLength": 1000,
    "EndPoints": ["redis1:6379", "redis2:6379"],
    "KeepAlive" :0,
    "AbortOnConnectFail":false,
    "ConfigurationChannel":"",
    "TieBreaker":"",
    "ConfigCheckSeconds":0,
    "CommandMap":null,
    "Password":"passwd",
    "AllowAdmin":true,
    "AsyncTimeout":11515,
    "ConnectRetry":0,
    "ConnectTimeout":1000,
    "DefaultDatabase":14,
    "InstanceName" : "YourInstance:",
  }
```
**3. Use IDatabase that is powered by StackExchange.Redis**
```csharp
        private readonly IDatabase _redisCache;
        public PolicyService(IDatabase redisCache)
        {
            _redisCache = redisCache;
        }

        public async Task YourMethod()
        {
            //IDatabase brings you all methods provided by StackExchange.Redis
            await _redisCache.StringGetAsync("MyKey");
        }

```
---
### Level-up your usage
---
Basic usage does not add a huge value indeed, it simply wraps StackExchange and unveils its power as-is. In this section, you can
increase your control on Redis and have lots of privileges including Redis Lock, Redis Pub/Sub, useful extensions and operations by 
considering infrastructural best practices (by CQRS) (Read operations prefers replicas, Write operations prefers masters)

#### Extension Methods
**1. Register Carbon.Redis with Helper Extension in your service provider**

```csharp
//IServiceCollection services
services.AddRedisPersister(Configuration).AddCarbonRedisCachingHelper();
```

**2. Get it to start by configuring as you do in Basic Usage**

On the top of basic usage configuration, you can tweak this [RedisSettings](RedisSettings.cs) class that can be used for more control.

**3. Set your serialization type anywhere you want (Startup, OnAppStart, in your Controller etc.)**

```csharp
//Defaults to BinaryFormatter which is obsolete as of Dotnet 5. So change it to Json if you need
ICarbonCacheExtensions.SetSerializationType(CarbonContentSerializationType.Json);
```

**4. Use ICarbonCache that is powered by Carbon.Redis**
```csharp
    [Serializable]
    public class TestClass
    {
        public int MyProperty { get; set; }
        public string MyPropertyStr { get; set; }
        public bool MyPropertyBool { get; set; }
        public DateTime MyPropertyDateTime { get; set; }
        public TestClass2 MyTestClass2 { get; set; }
    }

    [Serializable]
    public class TestClass2
    {
        public string MyProperty2 { get; set; }
        public string MyProperty22 { get; set; }
    }

    private readonly ICarbonCache _carbonRedisCache;
    public PolicyService(ICarbonCache carbonRedisCache)
    {
        _carbonRedisCache = carbonRedisCache;
    }
    public async Task YourMethod()
    {
        //Use ICarbonCache here
        var ts = new TestClass();
        ts.MyPropertyStr = "Test";
        ts.MyProperty = 229;
        ts.MyPropertyBool = true;
        ts.MyPropertyDateTime = DateTime.Now;
        ts.MyTestClass2 = new TestClass2() { MyProperty22 = "test22", MyProperty2 = "2" };
        //...
    }
```


**5. You are all set, now unleash the power!**

Now you can use many kinds of useful extension methods for various Redis Data Types (e.g. Redis Set, Hash etc.)

```csharp
    
    public async Task YourMethod()
    {
       //...   
        
       //Converts your flat object into hash key-value pairs within a single redis hash key, you can retrieve single or multiple fields later
       await _carbonRedisCache.HashSetAsObject<TestClass>("MyObjectInHash", ts);
       //Gets your entire object that is converted into hash key-value pairs within a single redis hash key.
       TestClass myTsFromRedis = await _carbonRedisCache.HashGetAsObject<TestClass>("MyObjectInHash");
       //Gets your entire object with the given fields that is converted into hash key-value pairs within a single redis hash key.
       TestClass myPartlyTsFromRedis = await _carbonRedisCache.HashGetAsObject<TestClass>("MyObjectInHash", new string[] { nameof(ts.MyPropertyStr), nameof(ts.MyPropertyDateTime) });
        
       //Gets your entire object with the given fields that is converted into hash key-value pairs within a single redis hash key, if key does not exist, it tries to fill with the given function
       TestClass getOrSetObj = await _carbonRedisCache.HashGetAsObject<TestClass>("MyObjectInHashFromSql", () =>
            {
                TestClass tsFromDb = GetFromSQLDatabase();
                return Task.FromResult(tsFromDb);
            });

       //Use SetTTL extension method after Set operations to set an expiry time in Redis
       await _carbonRedisCache.HashSetAsObject<TestClass>("MyTTLedHashObject", ts).SetTTL(new TimeSpan(0, 1, 0));
       // Or providing a key whenever you want to set TTL
       await _carbonRedisCache.SetTTL("MyTTLedHashObject", new TimeSpan(0, 1, 0));


       //Set a hash in redis with the given dictionary where each key will be another hash field in the same hash key
       Dictionary<String, String> dd = new Dictionary<String, String>();
       dd.Add("field1", "yolo");
       dd.Add("field2", "bolo");
       dd.Add("field3", "dolo");
       await _carbonRedisCache.HashSet<String>("MyHashFields", dd);
       //Gets your entire information in a dictionary with the given fields within a single redis hash key.
       Dictionary<String, String> onlySelectedFields = await _carbonRedisCache.HashGet<String>("MyHashFields", new string[] { "field1", "field2" });
       //Gets your entire information in a dictionary for all fields within a single redis hash key.
       Dictionary<String, String> allFields = await _carbonRedisCache.HashGet<String>("MyHashFields");
       //Gets the selected field from entire dictionary that is converted into hash key-value pairs within a single redis hash key.
       String singleField = await _carbonRedisCache.HashGet<String>("MyHashFields", "field1");

       //Add an object to a Redis Set within the given key (One by one)
       await _carbonRedisCache.SetAdd("myset", "yolo");
       await _carbonRedisCache.SetAdd("myset", "bolo");
       await _carbonRedisCache.SetAdd("myset", "dolo");
       await _carbonRedisCache.SetAdd("myset2", "folo");
       await _carbonRedisCache.SetAdd("myset2", "yolo");
       await _carbonRedisCache.SetAdd("myset2", "bolo");
       await _carbonRedisCache.SetAdd("myset2", "zolo");
       //Add multiple set members at a time
       await _carbonRedisCache.SetAdd("myset3", new string[] { "yolo", "golo" });
       //Get all the members in Redis Set
       List<String> allMembersOfMySet = await _carbonRedisCache.SetGetMembers<String>("myset");
       //Get Members of a Redis Set by applying set operations such as Union, Intersect, Difference for multiple keys
       List<String> intersectionMembersOfMySetAndMySet2 = await _carbonRedisCache.SetMultiGetMembers<String>(StackExchange.Redis.SetOperation.Intersect, new string[] { "myset", "myset2" });
       //Removes given members in a Redis Set
       await _carbonRedisCache.SetRemove<String>("myset", new string[] { "yolo", "bolo" });

       //Simply gets your entire object as single that is converted into single hash field within that redis hash key, if key does not exist, it tries to fill with the given function
       var getOrSetHash = await _carbonRedisCache.HashSingleGet<String>("SingleDataOfHash", () =>
            {
                return Task.FromResult("OnlyThisDataWillBeStored");
            });
       //Simple set operation which is inherited from IDistributedCache interface, all the set logic is managed by Microsoft.Extensions.Caching.Distributed with TTL and absolute or sliding expiration time as you wish
       await _carbonRedisCache.SetAsync("MyAnotherKey", ts, new CancellationToken(), new TimeSpan(0, 1, 0), true);

       //Removes any type of data from Redis with the given key
       await _carbonRedisCache.RemoveAsync("AnyKey");


    }
```

#### RedisLock for Distributed Lock

> Distributed locks are useful for ensuring only one process is using a particular resource at any given time (even if the processes are running on different machines).
> https://github.com/samcook/RedLock.net


```csharp
    private readonly ICarbonCache _carbonRedisCache;
    private readonly RedLockFactory _redlockFactory;

    public PolicyService(ICarbonCache carbonRedisCache)
    {
        _carbonRedisCache = carbonRedisCache;
        _redlockFactory = _carbonRedisCache.GetRedLockFactory();

    }
    public async Task YourMethod()
    {
        //Locks the given 'AnIdentifierToLock' identifier and make them others wait for 30 seconds, where redis key will expire 30 seconds later as well, and awaiter side will try to unlock per 2 seconds.
        using (var redlock = await _redlockFactory.CreateLockAsync("AnIdentifierToLock", TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(2)))
              {
                  if (redlock.IsAcquired)
                  {
                      //implement your distributed locked logic here, so that ensure any other instance of your API will wait this operation to end before it starts
                  }
              }
    }
```

#### Redis Pub/Sub for Notifications
To be filled later...

---
#### Accessing Stack.Exchange Based Objects (Server, Database etc.)
Access server object to execute server-based commands
```csharp
//Provides configuration controls of a redis server
IServer server = _carbonRedisCache.GetServer();
```

Access to database object to unveil all the redis operations via IDatabase
```csharp
IDatabase database = _carbonRedisCache.GetDatabase();
```

Get the current database number in use
```csharp
int dbnumber = _carbonRedisCache.GetRedisDatabaseNumber();
```

Get the current instance name
```csharp
string instancename = _carbonRedisCache.GetInstanceName();
```
