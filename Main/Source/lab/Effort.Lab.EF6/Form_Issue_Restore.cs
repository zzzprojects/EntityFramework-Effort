using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Effort.Lab.EF6
{
    public partial class Form_Issue_Restore : Form
    {
        public Form_Issue_Restore()
        {
            InitializeComponent();
            var connection = Effort.DbConnectionFactory.CreateTransient();


            // SEED
            using (var context = new EntityContext(connection))
            {
                context.Users.Add(new User { ColumnInt = 1 });
                context.Users.Add(new User { ColumnInt = 2 });
                context.Users.Add(new User { ColumnInt = 3 });
                context.SaveChanges();
            }

            using (var context = new EntityContext(connection))
            {
                context.AspNetUserss.Add(new AspNetUsers() { Id = "1", UserName = "Test", User = context.Users.ToList() });
                context.SaveChanges();

            }


            // TEST
            using (var context = new EntityContext(connection))
            {

                var list = context.AspNetUserss.Include(x => x.User).ToList();
            }

            connection.CreateRestorePoint(); 

            using (var context = new EntityContext(connection))
            {

                var list = context.AspNetUserss.Include(x => x.User).ToList();
                context.AspNetUserss.Add(new AspNetUsers() { Id = "2", UserName = "Test2", User = context.Users.ToList() });
                context.SaveChanges();


            }

            connection.RollbackToRestorePoint();

            using (var context = new EntityContext(connection))
            {
                var list3 = context.AspNetUserss.Include(x => x.User).ToList();
            }
        }

        public class EntityContext : DbContext
        {
            public EntityContext(DbConnection connection) : base(connection, true)
            {
            }

            public DbSet<AspNetUsers> AspNetUserss { get; set; }
            public DbSet<User> Users { get; set; }


            protected override void OnModelCreating(DbModelBuilder modelBuilder)
            { 

                base.OnModelCreating(modelBuilder);
            }
        }



        public class AspNetUsers
        {
            public AspNetUsers()
            {
                User = new HashSet<User>();
            }

            public string Id { get; set; }

            [StringLength(maximumLength: 256)]
            public string Email { get; set; }

            public bool EmailConfirmed { get; set; }

            public string PasswordHash { get; set; }

            public string SecurityStamp { get; set; }

            public string PhoneNumber { get; set; }

            public bool PhoneNumberConfirmed { get; set; }

            public bool TwoFactorEnabled { get; set; }

            public DateTime? LockoutEndDateUtc { get; set; }

            public bool LockoutEnabled { get; set; }

            public int AccessFailedCount { get; set; }

            [Required]
            [StringLength(maximumLength: 256)]
            public string UserName { get; set; }

            [StringLength(maximumLength: 256)]
            public string UserNameProposed { get; set; }

            [StringLength(maximumLength: 256)]
            public string EmailProposed { get; set; }

            public virtual ICollection<User> User { get; set; }
        }


        public class User
        {
            public int ID { get; set; }
            public int ColumnInt { get; set; }
        }

    }
}