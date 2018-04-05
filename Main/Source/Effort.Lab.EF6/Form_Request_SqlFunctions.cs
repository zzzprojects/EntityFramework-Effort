using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.SqlServer;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Effort.Lab.EF6
{
    public partial class Form_Request_SqlFunctions : Form
    {
        public Form_Request_SqlFunctions()
        {
            InitializeComponent();

            var context = new ExampleModel(DbConnectionFactory.CreateTransient());
            DateTime monday = new DateTime(2018, 4, 2);
            MyEntity test = new MyEntity { Id = 1, Name = "Example", WeekDay = (int)DayOfWeek.Monday };

            context.MyEntities.Add(test);
            context.SaveChanges();

            var count = context.MyEntities.Count();


            var find = context.MyEntities.Where(x => monday < EntityFunctions.AddDays(monday, 1)).FirstOrDefault();

        }

        public class ExampleModel : DbContext
        {
            public ExampleModel(DbConnection dbConnection) : base(dbConnection, true)
            {

            }

            // Add a DbSet for each entity type that you want to include in your model. For more information 
            // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

            public virtual DbSet<MyEntity> MyEntities { get; set; }
        }

        public class MyEntity
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int WeekDay { get; set; }
        }
    }
}
