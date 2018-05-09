# Data Loaders

## Definition

Data loaders are useful components in the Effort library that were designed to help set up the initial state of a fake database.

## Types

Effort provides multiple built-in data loaders:

 - EntityDataLoader
 - CsvDataLoader
 - CacheDataLoader

They are really easy to use, all the developer has to do is to create a data loader instance and pass it to the chosen Effort factory method. 

## EntityDataLoader

**EntityDataLoader** can fetch data from an existing database by utilizing an existing Entity Framework compatible ADO.NET provider. It is initialized with an entity connection string.

{% include template-example.html %} 
```csharp
var dataLoader = new EntityDataLoader("name=MyEntities");
```

## CsvDataLoader

The purpose of **CsvDataLoader** is to read data records from CSV files. It is initialized with a path that points to a folder containing the CSV files. Each file represents the content of a database table.

{% include template-example.html %} 
```csharp
var dataLoader = new CsvDataLoader(@@"C:\path\to\files");
```

There is also a [tool](/export-data-to-csv) that helps the developers to export the data from an existing database into appropriately formatted CSV files.

## CacheDataLoader

The **CachingDataLoader** was designed to speed up the initialization process by wrapping any data loader with a cache layer. 

 - If the wrapped data loader is specified with a specific configuration the first time, the CachingDataLoader will pull the required data from the wrapped data loader. 
 - As a side effect, this data is going to be cached in the memory. 
 - If the **CachingDataLoader** was initialized to wrap the same kind of data loader with the same configuration again, then the data will be retrieved from the previously create a cache, the wrapped data loader will not be utilized.

{% include template-example.html %} 
```csharp
var wrappedDataLoader = new CsvDataLoader(@@"C:\path\to\files");
 
var dataLoader = new CachingDataLoader(wrappedDataLoader, false);
```

## Hint:

Each data loader can be used in different scenarios. We suggest using **EntityDataLoader** during interactive testing, while **CachingDataLoader** and **CsvDataLoader** combined can be really useful if they are utilized in automated tests.
