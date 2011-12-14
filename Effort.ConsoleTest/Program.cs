using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using EFProviderWrapperToolkit;
using Effort.Configuration;
using System.Data.Objects;

namespace Effort.ConsoleTest
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.SetBufferSize(200, 3000);
			//DatabaseAcceleratorProviderConfiguration.RegisterProvider();
			//var con = EntityConnectionWrapperUtils.CreateEntityConnectionWithWrappers("name=NorthwindEntities", DatabaseAcceleratorProviderConfiguration.ProviderInvariantName);
			var con = EntityConnectionFactory.CreateAccelerator("name=NorthwindEntities");

			using (var context = new NorthwindEntities(con))
			{
				EffortConfigurator.UpdateStatistics(context, () => context.Products);

				var employee = new Employee()
				{
					FirstName = "New Name",
					LastName = "Last"
				};

				context.Employees.AddObject(employee);
				context.SaveChanges();

				var emp = from e in context.Employees
						  where e.FirstName == "New Name"
						  select e;

				foreach (var e in emp)
				{
					Console.WriteLine("{0}: {1} {2}", e.EmployeeID, e.FirstName, e.LastName);
				}

				var id = emp.First().EmployeeID;

				var empById = context.Employees.Single(e => e.EmployeeID == 3);

				var query = from p in context.Products
							select p.QuantityPerUnit.ToUpper();

				Console.WriteLine(query.First());

				var query2 = from p in context.Products
							 select string.Concat(p.QuantityPerUnit.ToUpper(), p.QuantityPerUnit.ToLower());

				Console.WriteLine(query2.First());

				var query3 = from p in context.Products
							 where p.QuantityPerUnit.Contains("bag")
							 select p;

				Console.WriteLine(query3.First().QuantityPerUnit);
			}

			//    int price = 100;

			//    var query = from e in context.Products
			//                where e.UnitPrice > new MMDBParameter<int>("p__linq__0")
			//                select e;

			//    query.ToList();
			//}

			//{
			//    var employee = new Employee()
			//    {
			//        FirstName = "New Name",
			//        LastName = "Last"
			//    };

			//    dc.Employees.AddObject(employee);
			//    dc.SaveChanges();
			//}
			//dc.Employees.First().FirstName = "New Name";
			//var query = from p in dc.Products
			//            where p.UnitPrice > 100
			//            orderby p.UnitPrice descending
			//            select p;

			//foreach (var p in query)
			//{
			//    Console.WriteLine("{0}: {1}", p.ProductName, p.UnitPrice);
			//}

			//var query = from p in dc.Products
			//            join c in dc.Categories on p.CategoryID equals c.CategoryID into categories
			//            select new { p, categories };

			//}
			//Console.WriteLine(dc.Products.Count());

			//EffortConfigurator.CreateStatistics<Product, short?>(dc, () => dc.Products, p => p.UnitsInStock);
			//}
		}
	}
}
