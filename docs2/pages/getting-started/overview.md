# Overview

## Description
Effort (**E**ntity **F**ramework **F**ake **O**bjectContext **R**ealization **T**ool) is the official `In Memory` provider for Entity Framework Classic. It creates a fake or mock database that allows you to test the Business Logic Layer (BLL) without worrying about your Data Access Layer (DAL).

 - It is basically an ADO.NET provider that executes all the data operations on a lightweight in-process main memory database instead of a traditional external database. 
 - It provides some intuitive helper methods that makes this provider easy to use with ObjectContext or DbContext classes. 
 - A simple addition to existing code might be enough to create data-driven tests that can run without the presence of the external database.

The term **Transient** refers to the lifecycle of the underlying in-memory database. 

## Installing
NuGet: https://www.nuget.org/packages/Z.EntityFramework.Classic.Effort

To use Effort, you need to create a transient connection and use it for your context:

```csharp
var connection = Effort.DbConnectionFactory.CreateTransient();
var context = new EntityContext(connection));
```

## Examples

```csharp
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Windows.Forms;
using Effort;

namespace Z.Lab.EFClassic
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            var connection = DbConnectionFactory.CreateTransient();

            using (var context = new EntityContext(connection))
            {
                var list = new List<Customer>();
                for (var i = 0; i < 10; i++)
                {
                    list.Add(new Customer {Name = "ZZZ_" + i});
                }

                context.Customers.AddRange(list);
                context.SaveChanges();
            }

            using (var context = new EntityContext(connection))
            {
                var list = context.Customers.Where(x => x.ID > 3).ToList();
            }
        }

        public class EntityContext : DbContext
        {
            public EntityContext(DbConnection connection) : base(connection, false)
            {
            }

            public DbSet<Customer> Customers { get; set; }
        }

        public class Customer
        {
            public int ID { get; set; }
            public string Name { get; set; }
        }
    }
}
```

## Requirements

### Entity Framework Version
- Entity Framework 6.x
- Entity Framework 5
- Entity Framework 4

### Database Provider
- SQL Server 2008+
- SQL Azure
- SQL Compact
- Oracle

## ObjectContext Lifecycle

The owner ObjectContext (technically the DbConnection) will be using a unique database instance. If the context/connection is disposed, then the database will be disposed too. 

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
