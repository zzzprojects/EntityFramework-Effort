# Overview

## Definition

**Effort** stands for **E**ntity **F**ramework **F**ake **O**bjectContext **R**ealization **T**ool. It is a powerful tool that enables a convenient way to create automated tests for Entity Framework based applications.

 - It is basically an ADO.NET provider that executes all the data operations on a lightweight in-process main memory database instead of a traditional external database. 
 - It provides some intuitive helper methods too that make really easy to use this provider with existing ObjectContext or DbContext classes. 
 - A simple addition to existing code might be enough to create data-driven tests that can run without the presence of the external database.

The following code returns all the categories stored in the database. 

{% include template-example.html %} 
```csharp
using(NorthwindEntities context = new NorthwindEntities())
{
    return context.Categories.ToList();
}
```

A simple modification is enough to make Entity Framework use a fake in-memory database.

{% include template-example.html %} 
```csharp
using(NorthwindEntities context = Effort.ObjectContextFactory.CreateTransient<NorthwindEntities>())
{
    return context.Categories.ToList();
}
```

The term **Transient** refers to the lifecycle of the underlying in-memory database. 

## Installing & Upgrading
Download the <a href="/download">NuGet Package</a>

## Requirements

### Entity Framework Version

- Entity Framework 6.x
- Entity Framework < 6

### Database Provider

- SQL Server 2008+
- SQL Azure
- SQL Compact
- Oracle


## ObjectContext Lifecycle

The owner ObjectContext (technically the DbConnection) will be using a unique database instance. If the context/connection is disposed, than the database will be disposed too. 

You could set the initial state of the database with Entity Framework, but Effort provides data loaders to do this more easily. The following example fetches the initial data from a real database.

{% include template-example.html %} 
```csharp
IDataLoader loader = new EntityDataLoader("name=NorthwindEntities");
 
using(NorthwindEntities context = 
    ObjectContextFactory.CreateTransient<NorthwindEntities>(loader))
{
    return context.Categories.ToList();
}
```

## CSV Tool

You can entirely dismiss the need for the external database by exporting your data tables into local CSV files. 

Effort provides a [tool](/export-data-to-csv) to export all your tables easily to CSV files and use the CSV data loader.

{% include template-example.html %} 
```csharp
IDataLoader loader = new CsvDataLoader("C:\PathOfTheCsvFiles");
 
using(NorthwindEntities context = 
    ObjectContextFactory.CreateTransient<NorthwindEntities>(loader))
{
    return context.Categories.ToList();
}
```
## Contribute

The best way to contribute is by spreading the word about the library:

 - Blog it
 - Comment it
 - Fork it
 - Star it
 - Share it
 - A **HUGE THANKS** for your help.
