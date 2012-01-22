using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Effort.Configuration;
using Effort.Demo;

namespace Effort.AcceleratorModeDemo
{
	class Program
	{
		static void Main(string[] args)
		{
			ConsoleHelper.Init();

			//gyorsitotarazott kapcsolat letrehozasa
			var con = EntityConnectionFactory.CreateAccelerator("name=NorthwindEntities");

			// kontextus letrehozasa a kapcsolattal
			using (var context = new NorthwindEntities(con))
			{
				//EffortConfigurator.UpdateStatistics(context, () => context.Products);

				string firstname = "First";

				// uj entitas letrehozasa
				var employee = new Employee()
				{
					FirstName = firstname,
					LastName = "Last",
					City = "Old City"
				};

				// entitas beszurasa az adatbazisba
				context.Employees.AddObject(employee);
				context.SaveChanges();

				ConsoleHelper.Print("Az elso kapcsolatnyitasnal letrejott az adatbazis es beszurasra kerult az uj entitas.");

				// a lekerdezes mar az MMDB-bol tortenik
				var emp = (from e in context.Employees
						   where e.FirstName == firstname
						   select e).ToList();

				foreach (var e in emp)
				{
					Console.WriteLine("{0}: {1} {2}", e.EmployeeID, e.FirstName, e.LastName);
				}

				ConsoleHelper.Print("A lekerdezest az MMDB vegezte el, fent lathato a leforditott LINQ lekerdezes");

				// lekerdezes primary key alapjan
				// az MMDB hasznalni fogja az elsodleges kulcson automatikus letrejovo unique indexet
				var empById = context.Employees.Single(e => e.EmployeeID == employee.EmployeeID);

				ConsoleHelper.Print("Az MMDB talalt egy indexet az elsodleges kulcson, igy azt hasznalta. A lekerdezest parameteresen forditotta.");

				empById.City = "New City";
				context.SaveChanges();

				empById = context.Employees.Single(e => e.EmployeeID == employee.EmployeeID);
				Console.WriteLine("City property value: {0}", empById.City);

				ConsoleHelper.Print("Egy tulajdonsag modositasa, majd az entitas ujboli lekerese" + Environment.NewLine +
					"Latszik, hogy nem tortent lekerdezes forditas (nincs 'Expression compiled' naplobejegyzes), mert a gyorsitotarban megtalalta ezt a lekerdezesfat ('Transformcache used')");
			}
		}
		
	}
}
