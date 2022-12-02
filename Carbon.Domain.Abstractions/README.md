# Carbon.Domain.Abstractions [<img alt="Nuget (with prereleases)" src="https://img.shields.io/nuget/vpre/Carbon.Domain.Abstractions">](https://www.nuget.org/packages/Carbon.Domain.Abstractions)

Abstraction layer of [Carbon.Domain.EntityFrameworkCore](../Carbon.Domain.EntityFrameworkCore.Extensions/README.md).

## Some Usefull Carbon.Domain.Abstractions Objects

### - IEntity
Used as base type for entities. Adds Id field
### - IInsertAuditing
Adds fields used to specify User Information and given operation time during create/insert
### - IUpdateAuditing
Adds fields used to specify User Information and given operation time during update
### - IDeleteAuditing
Adds fields used to specify User Information and given operation time during deletetion
### - IMayHaveTenant / IMustHaveTenant
If your entity needs to serve for multiple tenants you can use this interfaces. Adds a nullable or not nullable TenantId property by use as name suggest.
### - IPassivable
When your business requires to entities to be able to set as passive for some time you can use this built-in interface. Adds a boolean IsActive property.
### - ISoftDelete
Some entities should be permanently deleted, some should not be deleted but not visible or accessiable. In times when you need to keep data you can deriver your entity from ISoftDelete interface. 
If you are using ISoftDelete with [CarbonContext](https://github.com/kocdigital/Carbon/blob/master/Carbon.Domain.EntityFrameworkCore.Extensions/Contexts/ReadWrite/CarbonContext.cs) 
you don't need to implement soft delete functionality externally; CarbonContext has a built-in implementation for this.
### - IHaveOwnership
You may encounter situations where data ownership is required. In such cases, you can use the "IHaveOwnership" interface. There are three notable properties that come with this interface;
  - **OwnerType**: [OwnerType](../Carbon.Common/OwnerType.cs) property indicates the type of the owner. It can be user, organisation, role etc.
  - **OwnerId**: This property added for indicating owner. When you need to find the owner check the OwnerType and search it with this id within correct place.
  - **OrganizationId**: If you need a organisational segregation of data within same tenant you need this property. This is not indicates the owner, but it indicates the data's governantal owner. 
Let's say you create a data. Your Id should be OwnerId and let's say you're working for X Company, OrganisationId should be X Company's Id. 
So in reality yes data is your's but, in general terms the real owner is the company which you're working.