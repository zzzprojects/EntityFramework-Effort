using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Effort.CodeFirst;
using Effort.Demo;

namespace Effort.CodeFirstDemo
{
	class Program
	{
		static void Main(string[] args)
		{
			ConsoleHelper.Init();

			CodeFirstEmulatorProviderConfiguration.RegisterProvider();
			using (var db = new NorthwindContext())
			{
				Console.WriteLine("{0} categories found.", db.Categories.Count());

				foreach (var cat in db.Categories)
				{
					Console.WriteLine(cat.Name);
				}

				// Use Find to locate the Food category 
				var food = db.Categories.Find("FOOD");
				if (food == null)
				{
					food = new Category { CategoryId = "FOOD", Name = "Foods" };
					db.Categories.Add(food);
				}

				// Create a new Food product 
				Console.Write("Please enter a name for a new food: ");
				var productName = Console.ReadLine();

				var product = new Product { Name = productName, Category = food };
				db.Products.Add(product);

				int recordsAffected = db.SaveChanges();

				Console.WriteLine(
					"Saved {0} entities to the database.",
					recordsAffected);

				// Query for all Food products using LINQ 
				var allFoods = from p in db.Products
							   where p.Category.CategoryId == "FOOD"
							   orderby p.Name
							   select p;

				Console.WriteLine("All foods in database:");
				foreach (var item in allFoods)
				{
					Console.WriteLine("{1} - {0}", item.Name, item.Category.Name);
				}

				Console.WriteLine("Press any key to exit.");
				Console.ReadKey();
			}
		}
	}
}
