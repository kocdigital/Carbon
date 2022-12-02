# Carbon.ElasticSearch [<img alt="Nuget (with prereleases)" src="https://img.shields.io/nuget/vpre/Carbon.ElasticSearch">](https://www.nuget.org/packages/Carbon.ElasticSearch)


> Elasticsearch is a distributed, full-text search and analytics engine for all types of data, including textual, numerical, geospatial, structured, and unstructured.[**]

[**] https://www.elastic.co/what-is/elasticsearch


This package wraps Nest library and provides handy ElasticSearch Based operations. It eases the configuration by decreasing the startup time. Provides CQRS over ElasticSearch.


## Add Carbon.ElasticSearch Support to Your Project


| Related Package																		| Required | Auto-Included |
|:----------------------------------------------------------------						|:----:    |:----:         |
| [Carbon.ElasticSearch.Abstractions](../Carbon.ElasticSearch.Abstractions/README.md)   | No       | No            |

### Basic Usage

With Carbon.ElasticSearch package you can easly use ElasticSearch in your API.
Integration steps are easy but if you want to see implementation example, you may want to look [Carbon Sample Apllication](https://github.com/kocdigital/Carbon.Sample).

#### 1. Create Your Index Mapping (Optional)

**Dynamic Mapping**: One of the most important features of Elasticsearch is that it tries to get out of your way and let you start exploring your data as quickly as possible. 
To index a document, you don’t have to first create an index, define a mapping type, and define your fields — you can just index a document and the index, type, and fields will display automatically.

But in some cases you may want to manage your index structure, in cases like those you should create mapping for your inxdex as shown below;

```csharp
	public static void SetElasticConfiguration(this IElasticSettings options)
	{
		var mappings = new List<ElasticIndexMapping>();

		mappings.Add(new ElasticIndexMapping("my_index", c =>
				c.Map<MyEntity>(m =>
						m.AutoMap().Properties(ps => ps.Keyword(s => s.Name(n => n.MyRelatedId)))
						.Properties(ps => ps.Keyword(s => s.Name(n => n.CorrelationId)))
				).Settings(x => x.Setting("mapping.total_fields.limit", "100000"))
		));

		options.SetIndexsAndAutoMappings(mappings.ToArray());
	}
```

> Note that we're adding a extension method to IElasticSettings interface!

#### 2. Create Your Repository
Carbon.ElasticSearch offers you base repository for ease of usage. You can create a reposity which derives from that base repo.

```csharp
	public interface IMyEntityElasticRepository : IElasticRepository<MyEntity>
	{
	}

	public class MyEntityElasticRepository : BaseElasticRepository<MyEntity>, IMyEntityElasticRepository
	{
		private readonly string _indexName;
		public MyEntityElasticRepository(IElasticSettings elasticSettings) : base(elasticSettings)
		{
			_indexName = "my_index";
		}

		public override string Index => _indexName;
	}
```



#### 3. Enable Elastic Search Within Startup

To enable ElasticSearch, add code snipped below to your startup or if you're using .Net 6 minimal api; your program.cs file.

```csharp
	services.AddElasticSearchPersister(Configuration, options =>
	{
		options.SetElasticConfiguration();
		options.Build();
	});

	services.AddScoped<IMyEntityElasticRepository, MyEntityElasticRepository>();

```

> We're calling our extension method "SetElasticConfiguration" here for creating mappings.

Thats it, you're ready to use ElasticSearch from your repository.