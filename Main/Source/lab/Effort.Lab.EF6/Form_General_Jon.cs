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
using Effort.Provider;

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
                    db.Tests.Add(new Test() { Name = "z", Generated="z2" });
                    db.SaveChanges();

                    var list = db.Tests.AsNoTracking().ToList();
                }
            }
        }
    }

    public class EntityDbContext : DbContext
    {
        public bool IsEffort = false;

        public EntityDbContext(DbConnection dbConnection) : base(dbConnection, false)
        {
            IsEffort = dbConnection is EffortConnection;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {    
            if(IsEffort)
            {
                modelBuilder.Entity<Test>().Property(x => x.Generated).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            }           

            base.OnModelCreating(modelBuilder);
        }

        public virtual DbSet<Test> Tests { get; set; }
    }

    public class Test
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string Generated { get; set; }

    }
}