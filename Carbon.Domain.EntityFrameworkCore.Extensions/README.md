# Carbon.Domain.EntityFrameworkCore [<img alt="Nuget (with prereleases)" src="https://img.shields.io/nuget/vpre/Carbon.Domain.EntityFrameworkCore">](https://www.nuget.org/packages/Carbon.Domain.EntityFrameworkCore)

This package contains EntityFramework-based wrappers and libraries. 
Also introduces CQRS and Repository Pattern for SQL-based infrastructures. Makes you manage your SQL context in a secure, and multi-tenant managed way.

## Carbon.Domain.EntityFrameworkCore Features

1. **Predefined Business Objects**: Carbon.Domain.EntityFrameworkCore offers some predefined business objects like; softdelete, active/passive entities, autditing, ownership etc. 
You can review those on [Carbon.Domain.Abstractions](../Carbon.Domain.Abstractions/README.md).
2. **Read Only Context**: Some cases there should be only one api which can write to db, other ones just read the data. 
For cases like that Carbon offers built-in readonly db context and when implementing api you can use that context.
3. **Multi Tenant Management**: If your entity needs to serve for multiple tenants; first you must derive your entity from [IMustHaveTenant](../Carbon.Domain.Abstractions/Entities/IMustHaveTenant.cs). 
And after that you can use TenantManaged repositories. With that you don't need to filter tenant id every time.
4. **Entity Solution Relation**: If your entities serves more than one solution, Carbon offers you a way to manage that in a built-in way. What is the "Solution" here? 
It's basically some distinct business areas like banking, e-commerce etc. that you serve. Let's assume you have an Category entity in both Solutions but you don't want to show e-commerce categories on banking solution. 
On cases like those you can specify a solution to entity.

## Add Carbon.Domain.EntityFrameworkCore to Your Project

### 1. Create an Entity and Entity Configuration

Let's assume we have an entity like below;

```csharp
	public class MyEntity : IEntity, IMustHaveTenant, IHaveOwnership<EntitySolutionRelation>
	{
		public Guid Id { get; set; }
		public string Name { get; set; }

		public Guid TenantId { get; set; } //Comes from IMustHaveTenant interface
		
		//Those below comes from IHaveOwnership interface
		public Guid OwnerId { get; set; }
		public OwnerType OwnerType { get; set; }
		public Guid OrganizationId { get; set; }
		public ICollection<EntitySolutionRelation> RelationalOwners { get; set; }
	}
```

And for this entity we need a configuration, because we're using **code first** approach. So create an entity configuration file like shown below.
```csharp
	public class MyEntityConfiguration : TenantManagedEntityConfigurationBase<MyEntity>
	{

		public override void Configure(EntityTypeBuilder<MyEntity> builder)
		{
			builder.HasKey(x => x.Id);

			builder.Property(x => x.Name)
				   .IsRequired(true)
				   .HasMaxLength(512);

			builder.Property(x => x.TenantId)
				   .IsRequired(true);

			base.Configure(builder);
		}
	}
```
> Note that our entity uses tenant based approach
so we are using [TenantManagedEntityConfigurationBase](https://github.com/kocdigital/Carbon/blob/master/Carbon.Domain.EntityFrameworkCore.Extensions/TenantManagedEntityConfigurationBase.cs) class.

### 2. Create a DB Context

We can use CarbonTenantManagedContext as base class because our entity has a tenantid and at the same time it derives from IHaveOwnership interface.

```csharp
	public class MyDBContext : CarbonTenantManagedContext<MyDBContext>
	{
		public MyDBContext(DbContextOptions options) : base(options)
		{

		}

		public virtual DbSet<MyEntity> MyEntity { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfiguration(new MyEntityConfiguration());

			//Don't forget to call base.OnModelCreating method.This is important!
			base.OnModelCreating(modelBuilder);
		}
	}
```

### 3. Register DB Context

In this step, you need to use the [Carbon.WebApplication](https://www.nuget.org/packages/Carbon.WebApplication)
& [Carbon.WebApplication.EntityFrameworkCore](https://www.nuget.org/packages/Carbon.WebApplication.EntityFrameworkCore)
packages, otherwise you have to do some operations manually.

On your Startup.cs;

```csharp
	public override void CustomConfigureServices(IServiceCollection services)
	{
		...
		services.AddDatabaseContext<MyDBContext, Startup>(Configuration);
		...
		services.ConfigureAsSolutionService(Configuration);//We need to add this because our entity is solution based
	}

	public override void CustomConfigure(IApplicationBuilder app, IWebHostEnvironment env)
	{
		...
		app.MigrateDatabase<MyDBContext>();
		...
	}
```
At this point you can access MyDBContext wherever you wanted but there is a one last step; migration!

### 4. Create a Migration

Code First needs migrations to sync with the database. And there are multiple db providers like postgresql, mssql, sqlite etc. 
So we must create migrations in a different project than the one containing our DbContext. 

#### 4.1 Create a Class Library for Provider Migrations
Create a new class library project on your solution. 

#### 4.2 Add a reference to your DbContext project

#### 4.3 Create ContextDesignFactory

```csharp
	public class MyDBContextDesignFactory : IDesignTimeDbContextFactory<MyDBContext>
	{
		public MyDBContext CreateDbContext(string[] args)
		{
			var connectionString = "your connection string";

			var migrationsAssembly = typeof(MyDBContextDesignFactory).GetTypeInfo().Assembly.GetName().Name;

			var optionsBuilder = new DbContextOptionsBuilder<MyDBContext>()
		   .UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
		   //.UseNpgsql(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));// Use tihs when using postgre sql


			return new MyDBContext(optionsBuilder.Options);
		}
	}
```

#### 4.4 Add a reference to your migrations project from the startup project.

```xml
<ItemGroup>
  <ProjectReference Include="..\WebApplication1.MSSQLMigrations\WebApplication1.MSSQLMigrations.csproj">
</ItemGroup>
```

> If this causes a circular dependency, you can update the base output path of the migrations project instead:

```xml
<PropertyGroup>
  <BaseOutputPath>..\WebApplication1\bin\</BaseOutputPath>
</PropertyGroup>
```

#### 4.5 Create migration.
If you did everything correctly, you should be able to add new migrations to the project.
```PowerShell
Add-Migration NewMigration -Project WebApplication1.MSSQLMigrations
```
> Change project parameter for other providers.
#### 4.6 Repeat these steps for other supported providers.
---

> If you have problems please check [Microsoft documentation](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/projects) about migrations.