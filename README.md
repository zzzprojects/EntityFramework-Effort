##Overview##

Effort is a powerful tool that enables a convenient way to create automated tests for Entity Framework based applications. 

It is basically an ADO.NET provider that executes all the data operations on a lightweight in-process main memory database instead of a traditional external database. It provides some intuitive helper methods too that make really easy to use this provider with existing ObjectContext or DbContext classes. A simple addition to existing code might be enough to create data driven tests that can run without the presence of the external database.

##Getting Started##
###ApplicationDbContext from ASP.net template###
```csharp
// Example ApplicationDbContext.cs
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
 // Add this ctor overload to pass in the Effort DbConnection
 public ApplicationDbContext(DbConnection existingConnection, bool contextOwnsConnection = true)
  : base(existingConnection, contextOwnsConnection)
  {
  }
}

// Example test class
[Test]
public class MyTests
{
  public void MyTest()
  {
    var db = new ApplicationDbContext(DbConnectionFactory.CreateTransient());
    db.Database.CreateIfNotExists(); // Important, or you will get a MigrationsException
    
    ApplicationUser user = db.Users.Add(new ApplicationUser("testuser"));
    db.SaveChanges();

    // Act
    var retrievedUser = new MyUsersDal(db).GetUserByUserName("testuser");
    
    // Assert
    Assert.NotNull(retrievedUser);
  }
}
//
```
Nugets
* Effort.EF6 1.1.4
* Entity Framework 6.1.2
* Microsoft.AspNet.Identity.Core 2.2.0 (for older versions, see [workarounds](https://effort.codeplex.com/discussions/470433))

###

##Resources##
 * [Download library](http://effort.codeplex.com/releases)
 * [Install NuGet package](https://effort.codeplex.com/wikipage?title=NuGet%20Packages)
 * [News](https://effort.codeplex.com/wikipage?title=News)
 * [FAQ](https://effort.codeplex.com/wikipage?title=FAQ)
 * [Tutorials](https://effort.codeplex.com/wikipage?title=Tutorials)
 * [Contribution](https://effort.codeplex.com/wikipage?title=Contribution)
 * [Nightly builds](http://development.flamich.net/oss-nightly/)
 * [CodePlex home](https://effort.codeplex.com/)
