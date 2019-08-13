using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Effort.DataLoaders;

namespace Effort.Lab.EF6
{
    public partial class Form_General_Jon : Form
    {
        public Form_General_Jon()
        {
            InitializeComponent();


            using (var effortConnection = Effort.DbConnectionFactory.CreateTransient())
            {
                using (var db = new EntityDbContext(effortConnection))
                {
                    db.Database.CreateIfNotExists();
                    effortConnection.IsCaseSensitive = false;
                    db.Tests.Add(new Test {Name = "aa"});
                    db.SaveChanges();
                    // Simple case-insensitive equality works
                    var results1 = db.Tests.Where(p => p.Name == "AA").ToList();
                    Console.WriteLine($"results1 count = {results1.Count}");
                    // However case-insensitive StartsWith does not work (works with SQL server)
                    var results2 = db.Tests.Where(p => p.Name.StartsWith("A")).ToList();
                    Console.WriteLine($"results2 count = {results2.Count}");
                    // StartsWith will work as long as it can be case-sensitive.
                    var results3 = db.Tests.Where(p => p.Name.StartsWith("a")).ToList();
                    Console.WriteLine($"results3 count = {results3.Count}");
                    Console.ReadLine();
                }
            }
        }
    }

    public class EntityDbContext : DbContext
    {
        public EntityDbContext(DbConnection dbConnection) : base(dbConnection, false)
        {
        }

        public virtual DbSet<Test> Tests { get; set; }
    }

    public class Test
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}