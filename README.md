# Sequence Generator
Thread-safe sequence generator

## Installation

### Default Provider

```
Install from Nuget:

PM> Install-Package vintorezsr.SequenceGenerator
```
Register into service collections
```C#
public void Register(IServiceCollection serviceCollection)
{
    serviceCollection.AddSequenceGenerator();
}
```

### Entity Framework Provider

```
Install from Nuget:

PM> Install-Package vintorezsr.SequenceGenerator.EntityFramework
```

Using SQL Server
```C#
serviceCollection.AddSequenceGenerator()
    .UseEntityFrameworkSequenceProvider((serviceProvider, builder) =>
    {
        builder.UseSqlServer("Data Source=localhost;User Id=sa;Password=P@ssw0rd;TrustServerCertificate=true;Encrypt=yes;");
    });
```
Using Oracle
```C#
serviceCollection.AddSequenceGenerator()
    .UseEntityFrameworkSequenceProvider((serviceProvider, builder) =>
    {
        builder.UseOracle("Server=localhost;Port=1521;User Id=sa;Password=P@ssw0rd;");
    });
```
Using PostgreSQL
```C#
serviceCollection.AddSequenceGenerator()
    .UseEntityFrameworkSequenceProvider((serviceProvider, builder) =>
    {
        builder.UseNpgsql("Server=localhost;Port=5432;User Id=sa;Password=P@ssw0rd;");
    });
```
Using SQL Lite
```C#
serviceCollection.AddSequenceGenerator()
    .UseEntityFrameworkSequenceProvider((serviceProvider, builder) =>
    {
        builder.UseSqlLite("Data Source=c:\\mydb.db;Version=3;");
    });
```
Using InMemory Database for testing purpose
```C#
serviceCollection.AddSequenceGenerator()
    .UseEntityFrameworkSequenceProvider((serviceProvider, builder) =>
    {
        builder.UseInMemoryDatabase("mydb");
    });
```

### MongoDb Provider

```
Install from Nuget:

PM> Install-Package vintorezsr.SequenceGenerator.MongoDb
```
Register into service collections
```C#
serviceCollection.AddSequenceGenerator()
    .UseMongoDbProvider("mongodb://localhost:27017", "MyDb");
```
## How to Use

### Sample Generating Sequence Number with Template
```C#
var sequenceProvider = _serviceProvider.GetService<ISequenceProvider>()!;
var sequenceGenerator = _serviceProvider.GetService<ISequenceGenerator>()!;
var sequenceTemplateKey = "sequence1";

await sequenceProvider.SaveTemplateAsync(new SequenceTemplate
{
    Key = sequenceTemplateKey,
    Template = "SEQ_{RunningNumber:000000}"
});

var sequenceNumber = await sequenceGenerator.GenerateAsync(sequenceTemplateKey);
```

### Output

```
SEQ_000001
```
