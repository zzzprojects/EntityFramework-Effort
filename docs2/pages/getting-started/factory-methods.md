# Factory Methods

## Factory Methods

All the factory classes provide two kinds of factory methods.

 - CreateTransient
 - CreatePersistent

The difference between them is in the lifecycle of the underlying in-memory database bound to the endpoint. 

## CreateTransient

Creates a fake object that relies on an in-memory database instance that lives during the connection object lifecycle. If the connection object is disposed or garbage collected, then the underlying database will be garbage collected too.

The following code instantiates a DbContext object with a fake DbConnection object and adds some data to the fake database. 

Then creates another DbContext instance with a newly created fake DbConnection and tries to query the previously added data.

{% include template-example.html %} 
```csharp
using (var ctx = new PeopleDbContext(Effort.DbConnectionFactory.CreateTransient()))
{
    ctx.People.Add(new Person() { Id = 1, Name = "John Doe" });
    ctx.SaveChanges();
 
    Console.WriteLine("Test 1: First Count: {0}", ctx.People.Count());
}
 
using (var ctx = new PeopleDbContext(Effort.DbConnectionFactory.CreateTransient()))
{
    Console.WriteLine("Test 1: Second Count: {0}", ctx.People.Count());
}
```

The second DbContext instance is not able to see the entity added by the first DbContext instance.

## CreatePersistent

Creates a fake object that relies on an in-memory database instance that lives during the complete application lifecycle. This method accepts a string argument which identifies the fake database instance that the connection object is bound to.

Let's change the previous code a little bit by using **CreatePersistent** method.

{% include template-example.html %} 
```csharp
using (var ctx = new PeopleDbContext(Effort.DbConnectionFactory.CreatePersistent("1")))
{
    ctx.People.Add(new Person() { Id = 1, Name = "John Doe" });
    ctx.SaveChanges();
 
    Console.WriteLine("Test 2: First Count: {0}", ctx.People.Count());
}
 
using (var ctx = new PeopleDbContext(Effort.DbConnectionFactory.CreatePersistent("1")))
{
    Console.WriteLine("Test 2: Second Count: {0}", ctx.People.Count());
}
```

The second DbContext instance can see the newly added entity this time because the connection objects are bound to the same database instance. This is possible because they were created with the same identifier.
