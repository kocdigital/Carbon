# Carbon.WebApplication.SolutionService

> !This package is KocDigital specific package. May not work for open-source purposes!

This package gives you the mighty power of registering your own solutions, featuresets and all the permission with its granular pieces as code-first to Platform360.

For the sake of better understanding of solution structure in Platform360, refer to this [Scientific Article](https://www.fruct.org/publications/volume-32/fruct32/files/Ayt.pdf)



## Add SolutionService Support to Your Project

This package uses Carbon.MassTransit behind the scenes.

**1. Register your service as a solution registerable service**

```csharp
//IServiceCollection services;
services.ConfigureAsSolutionService(Configuration);
```

**2. Add your Masstransit Configuration**

This package uses a saga orchestration to register your solution to the system safely. This is provided by a Message Queueing infrastructure (such as RabbitMQ)
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

**3. Create your solution as code-first**

//Define a solution with name and static guid
```csharp
public static Solution GetSolutionMigration()
        {
            return new Solution()
            {
                SolutionId = new Guid("00112233-C637-42ED-F4BE-08D7F1B05268"),
                SolutionName = "Solution1",
                Description = "Solution1 Description",
                Icon = @"data:image/png;base64,yourimage's base64 encoded string"
                Version = 2
            };
        }
```
Every time you increase the version will be detected as a modification by the system, so the solution will be re-registered. Otherwise
there will be no action.

**3. Create your featureset as code-first**

Define a featureset with all the permissions. In this example FeatureSet properties are empty. But you should fill them accordingly. 
For further detail for filling them, refer to incoming section.
```csharp
public static List<FeatureSet> GetFeatureSetMigration()
        {
            List<FeatureSet> featureSets = new List<FeatureSet>();

            featureSets.Add(
            new FeatureSet()
            {
                FeatureSetId = new Guid("99887766-F8D1-4916-BA04-469CCF50C1C3"),
                FeatureSetName = "FeatureSet1",
                Description = "FeatureSet1",
                EndpointItemPermissions = new List<EndpointItemPermission>(),

                MenuItemPermissions = new List<MenuItemPermission>(),

                UIItemPermissions = new List<UIItemPermission>(),

                PermissionGroups = new List<PermissionGroup>(),

                PermissionGroupEndpointItemPermissionRelations = new List<PermissionGroupEndpointItemPermissionRelation>(),

                PermissionGroupMenuItemPermissionRelations = new List<PermissionGroupMenuItemPermissionRelation>(),

                PermissionGroupUIItemPermissionRelations = new List<PermissionGroupUIItemPermissionRelation>(),

                Features = new List<Feature>(),

                Version = 18
            });

            return featureSets;
        }
```

Likewise the solution version, if you increase the version, the environment that is below this version will start a migration and order your permissions as you desire, otherwise it will ignore or show up an error.

**4. Register your solutions and featuresets you prepared**

```csharp
//IApplicationBuilder app
var solution = SolutionMigration.GetSolutionMigration();
var featuresets = SolutionMigration.GetFeatureSetMigration();

//First register your solution
app.RegisterAsSolution(new SolutionCreationRequest() { Solution = solution });

//And then register your featureset that is tied to your solution
foreach (var featureset in featuresets)
{
    app.RegisterAsFeatureSet(new FeatureSetCreationRequest() { FeatureSet = featureset, SolutionId = solution.SolutionId });
}
```

Once you run the code bunch above, the output in the console will likely be as given below 
(let's assume you have 1 solution with 2 featuresets)

```output
Solution has successfully submitted- SolutionId: {SolutionId}, SolutionName: {SolutionName}
FeatureSet Registration has successfully submitted for given Solution {SolutionId} FeatureSet: {FeatureSetName}
FeatureSet Registration has successfully submitted for given Solution {SolutionId} FeatureSet: {FeatureSetName}
```

In case of completing with a happy ending the output in the console will likely be as given below (Successful migration or ignored due to same version) :
```output
Solution has successfully registered- SolutionId: {SolutionId}, SolutionName: {SolutionName}
FeatureSet has successfully registered to SolutionId: {SolutionId}, FeatureSet: {FeatureSetId}
FeatureSet has successfully registered to SolutionId: {SolutionId}, FeatureSet: {FeatureSetId}
```

In case of completing with an error the output in the console will likely be as given below (Successful migration failed due to misconfiguration of your featuresets or version conflicts such as greater version in the environment than you defined in your code) :
```output
Solution registration has failed- SolutionId: {SolutionId}, SolutionName: {SolutionName}, Error: {ErrorMessage}
FeatureSet registration has failed to SolutionId: {SolutionId}, FeatureSet: {FeatureSetId}, Error: {ErrorMessage}
FeatureSet registration has failed to SolutionId: {SolutionId}, FeatureSet: {FeatureSetId}, Error: {ErrorMessage}
```


## More Details for FeatureSet
Preparing a featureset with all permissions might be a cumbersome and may create a burden on you if you are not very familiar with it.
However, there are something to be handled by care. Please read each property's description to learn what is what.

- **EndpointItemPermission (Authorization of your endpoints in your API)**
```csharp
//You can add EndpointItemPermission as much as you want
new EndpointItemPermission()
{
    //You should use your related solution's ID that is declared in previous section
    ApplicationId = new Guid("00112233-C637-42ED-F4BE-08D7F1B05268"),
    //Same with your endpoint's authorization role name
    Code = "VirtualTelemetry_ReadVirtualTelemetry",
    //Same with your endpoint's authorization role name
    Name = "VirtualTelemetry_ReadVirtualTelemetry",
    //Static but, randomly generated GUID, should not clash with other IDs
    Id = new Guid("55669933-B625-4464-71F8-08D7F03A0AED")
},
```

- **MenuItemPermissions (Your menu items that appear in the UI)**
```csharp
//You can add MenuItemPermission as much as you want
new MenuItemPermission() {
    //You should use your related solution's ID that is declared in previous section
    ApplicationId = new Guid("00112233-C637-42ED-F4BE-08D7F1B05268"),
    //Appearence order in UI
    Order = 1,
    //Should be the same as defined in UI Project in menu.json
    Code = "Asset List",
    //Should be the same as defined in UI Project in menu.json
    Name= "Asset List",
    //Static but, randomly generated GUID, should not clash with other IDs
    Id = new Guid("6B06A500-D195-4053-BFEB-8029E4FB3B43"),
    //Should be the same relative path as defined in UI Project in menu.json
    Uri = "/AssetList",
    //Should be the same meta as defined in UI Project in menu.json
    Meta="{\"path\":\"/AssetList\",\"title\":{\"en\":\"Asset Management\",\"tr\":\"Varlik Yönetimi\"},\"icon\":\"mdi-domain\",\"remote\":\"$$p360micropage$$\",\"name\":\"project-assetlist-remote\"}"
},
```

- **UIItemPermissions (Your UI items that appear in the UI pages)**
```csharp
//You can add UIItemPermission as much as you want
new UIItemPermission()
{
    //You should use your related solution's ID that is declared in previous section
    ApplicationId = new Guid("00112233-C637-42ED-F4BE-08D7F1B05268"),
    //Should be the same as defined in UI Project
    Code = "AssetUpdate",
    //Should be the same as defined in UI Project
    Name = "AssetUpdate",
    //Static but, randomly generated GUID, should not clash with other IDs
    Id = new Guid("4AFB777B-56D7-4AB6-8D31-013E1EB6A66F")
},
```
- **PermissionGroups (The group that groups your all permissions in one place)**
```csharp
new PermissionGroup()
{
    //You should use your related solution's ID that is declared in previous section
    ApplicationId = new Guid("00112233-C637-42ED-F4BE-08D7F1B05268"),
    //Read=1, Create=2, Update=3, Delete =4
    Type = (PermissionGroupType)1,
    //Name of your Permission Group that is tells all the story about grouped permissions
    Name = "Virtual Telemetry",
    //Static but, randomly generated GUID, should not clash with other IDs
    Id = new Guid("974F50BF-A30A-4299-513C-08D7F0C40890")
},
```

- **PermissionGroupEndpointItemPermissionRelations (The relationship between permission group and EndpointItemPermissions)**
```csharp
new PermissionGroupEndpointItemPermissionRelation()
{
    // The permissiongroupid that you created above
    PermissionGroupId = new Guid("974F50BF-A30A-4299-513C-08D7F0C40890"),
    //The endpoint item permissionid that you created above and you want to group in given permission gorup
    EndpointItemPermissionId = new Guid("55669933-B625-4464-71F8-08D7F03A0AED"),
    //Static but, randomly generated GUID, should not clash with other IDs
    Id = new Guid("A371ECB2-A010-4797-7996-08D7F0DC4942")
},
```

- **PermissionGroupMenuItemPermissionRelations (The relationship between permission group and MenuItemPermissions)**
```csharp
new PermissionGroupMenuItemPermissionRelation()
{
    // The permissiongroupid that you created above
    PermissionGroupId = new Guid("974F50BF-A30A-4299-513C-08D7F0C40890"),
    //The menu item permissionid that you created above and you want to group in given permission gorup
    MenuItemPermissionId = new Guid("6B06A500-D195-4053-BFEB-8029E4FB3B43"),
    //Static but, randomly generated GUID, should not clash with other IDs
    Id = new Guid("985123AA-4958-4958-4958-1234FCDC4982")
},
```

- **PermissionGroupUIItemPermissionRelations (The relationship between permission group and UIItemPermissions)**
```csharp
new PermissionGroupMenuItemPermissionRelation()
{
    // The permissiongroupid that you created above
    PermissionGroupId = new Guid("974F50BF-A30A-4299-513C-08D7F0C40890"),
    //The ui item permissionid that you created above and you want to group in given permission gorup
    UIItemPermissionId = new Guid("4AFB777B-56D7-4AB6-8D31-013E1EB6A66F"),
    //Static but, randomly generated GUID, should not clash with other IDs
    Id = new Guid("4AFB777B-56D7-4AB6-8D31-013E1EB6A661")
},
```

- **Features (Your featureset members that packs all the permission groups in one place)**
```csharp
new Feature() {
    //Static but, randomly generated GUID, should not clash with other permissions
    FeatureId = new Guid("94691F30-AA25-45E3-3A11-08D7F03075CC"),
    //Your desired FeatureName that will be a member of your featureset
    FeatureName = "Virtual Telemetry",
    //Description free-text
    Description = "Virtual Telemetry",
    //Relationship between your created permission groups and this feature, add as many as permission groups here
    ApplicationFeaturePermissionGroups = new List<ApplicationFeaturePermissionGroupRelation>()
    {
        new ApplicationFeaturePermissionGroupRelation()
        {
            // The permissiongroupid that you created above
            PermissionGroupId = new Guid("974F50BF-A30A-4299-513C-08D7F0C40890"),
            //Should be the same with your FeatureId that you defined above
            ApplicationFeatureId = new Guid("94691F30-AA25-45E3-3A11-08D7F03075CC"),
            //Static but, randomly generated GUID, should not clash with other IDs
            Id = new Guid("EF50AD63-14A0-4176-A1FD-08D7F0EAC093")
        },
        new ApplicationFeaturePermissionGroupRelation()
        {
            // The permissiongroupid that you created else
            PermissionGroupId = new Guid("88AA50BF-A30A-4299-513C-08D7F0C40890"),
            //Should be the same with your FeatureId that you defined above
            ApplicationFeatureId = new Guid("94691F30-AA25-45E3-3A11-08D7F03075CC"),
            //Static but, randomly generated GUID, should not clash with other IDs
            Id = new Guid("CCC0AD63-14A0-4176-A1FD-08D7F0EAC093")
        },
    }
},

```
