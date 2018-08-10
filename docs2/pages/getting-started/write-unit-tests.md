# Write Unit Tests

## Introduction

This tutorial shows a fundamental approach to unit testing, and we are going to test a very simple data component.

### Product Service Class
```csharp

public class ProductService : IProductService
{
    public NorthwindEntities Context { set; get; }

    public Product GetProduct(int id)
    {
        return Context.Products.FirstOrDefault(p => p.ProductID == id);
    }

    public void DeleteProduct(Product product)
    {
        Context.Products.Attach(product);

        Context.Products.DeleteObject(product);
        Context.SaveChanges();
    }
}

```

The ProductService class provides two simple operations: 

 - Querying a product entity based on Id
 - Deleting a product entity from the database.

The initializer method creates the fake object context for each unit test.

{% include template-example.html title='Initialize Fake Object' %} 
```csharp

private NorthwindEntities context;

[TestInitialize]
public void Initialize()
{
    IDataLoader loader = new CsvDataLoader(Settings.TestDataPath);
    this.context =  CreateTransient<NorthwindEntities>(loader);
}
```

The first test method verifies if an entity exist with a specific Id.

### ProductExist Test Method
```csharp
[TestMethod]
public void ProductExist()
{
    // Arrange
    ProductService service = new ProductService { Context = context };

    // Act
    var result = service.GetProduct(1);

    // Assert
    Assert.IsNotNull(result);
}

```

The test is straightforward; the data component is created and requested to query the Product entity whose ID equals 1. 

The second test method verifies if the data component can remove an entity from the database.

### DeleteProduct Test Method
```csharp
[TestMethod]
public void DeleteProduct()
{
    // Arrange
    ProductService service = new ProductService { Context = context };

    Product product = new Product();
    product.ProductID = 1;

    // Act
    service.DeleteProduct(product);

    // Assert
    product = service.GetProduct(1);

    Assert.IsNull(product);
}

```

The data component is initialized like in the previous test and requested to delete the Product entity whose Id is 1. Then the component is requested to retrieve the same entity. The test pass if the entity does not exist.

If the initial data contains the requested record, the tests will pass regardless of the order of execution. This is possible because every test starts consistently with the same initial data. The data manipulation done by a test does not affect the data context of another test. 
