# Fake ObjectContext Instance

## Factory Classes

The Effort library provides multiple factory classes that developers can choose from as per their requirements, and all of them can serve well in different scenarios.

 - DbConnectionFactory
 - EntityConnectionFactory
 - ObjectContextFactory

### Purpose

Factory Classes are capable of creating a different type of fake data endpoints, such as DbConnection, EntityConnection, and ObjectContext. 

 - Fake DbConnection objects are ideal for using them in the Code First programming approach. 
 - The purpose of fake EntityConnection objects is to utilize them in the Database First and Model First techniques. 
 - Instantiate ObjectContext objects by passing them as a constructor argument.

In Entity Framework, when you query data to retrieve all the products from the database. Behind the scenes, a connection is established with the appropriate database server, and an SQL command is executed.

{% include template-example.html %} 
```csharp
using (NorthwindEntities ctx = new NorthwindEntities())
{
    return ctx.Products.ToList();
}
```

The ObjectContextFactory class provides helper methods that can create ObjectContext instances that behave differently.

**Effort** makes it possible to eliminate this communication and run every required operation by the current process. 

{% include template-example.html %} 
```csharp
using (NorthwindEntities ctx = Effort.ObjectContextFactory.CreateTransient<NorthwindEntities>())
{
    return ctx.Products.ToList();
}
```

If you execute the above code, there will be no communication with your external database server. Instead, the data operations will be performed by an in-process lightweight database.
