# Load Data from Csv Files

## Introduction

Effort makes it possible to fill your fake database with data gathered from CSV files. Here is a sample CSV file that can be consumed by the Effort provider to fill the Products table.

{% include template-example.html title='Sample CSV File' %} 
```csharp
ProductID,ProductName,SupplierID,CategoryID,QuantityPerUnit,UnitPrice,UnitsInStock
"1","Chai","1","1","10 boxes x 20 bags","18.0000","39"
"2","Chang","1","1","24 - 12 oz bottles","19.0000","17"
```

Place all your CSV files in a favorable directory, pass the path of the directory in **CsvDataLoader** constructor to make Effort to load them.

{% include template-example.html %} 
```csharp

IDataLoader loader = new Effort.DataLoaders.CsvDataLoader(@@"D:\CsvFiles")
    
using (NorthwindEntities ctx = Effort.ObjectContextFactory.CreateTransient(loader))
{
    var products = ctx.Products.ToList();
}

```

It returns a collection that contains two products which was specified in the CSV file. 


