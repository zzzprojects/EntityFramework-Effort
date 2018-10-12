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
using System.Windows.Forms;

namespace Effort.Lab.EF6.Net40
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
                context.Users1.Add(new User1 { ColumnInt = 1 });
                context.Users1.Add(new User1 { ColumnInt = 2 });
                context.Users1.Add(new User1 { ColumnInt = 3 });
                context.Users2.Add(new User2 { ColumnInt = 1 });
                context.Users2.Add(new User2 { ColumnInt = 2 });
                context.Users2.Add(new User2 { ColumnInt = 3 });
                context.SaveChanges();
            }

            using (var context = new EntityContext(connection))
            {
                context.AspNetUserss.Add(new AspNetUsers() { Id = "1", UserName = "Test", User = context.Users.ToList() });
                context.AspNetUserss1.Add(new AspNetUsers1() { Id = "1", UserName = "Test", User = context.Users.ToList() });
                context.AspNetUserss2.Add(new AspNetUsers2() { Id = "1", UserName = "Test", User = context.Users.ToList(),
                    User1 = context.Users1.ToList(), User2 = context.Users2.ToList() });
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
                var list1 = context.AspNetUserss.Include(x => x.User).ToList();
                var list2 = context.AspNetUserss1.Include(x => x.User).ToList();
                var list3 = context.AspNetUserss2.Include(x => x.User).Include(x => x.User1).Include(x => x.User2).ToList();
            }

            using (var context = new EntityContext(connection))
            { 
                var list = context.AspNetUserss.Include(x => x.User).ToList();
                context.AspNetUserss.Add(new AspNetUsers() { Id = "2", UserName = "Test2", User = context.Users.ToList() });
                context.SaveChanges();
            }

            connection.RollbackToRestorePoint();

            using (var context = new EntityContext(connection))
            {
                var list1 = context.AspNetUserss.Include(x => x.User).ToList();
                var list2 = context.AspNetUserss1.Include(x => x.User).ToList();
                var list3 = context.AspNetUserss2.Include(x =>x.User).Include(x => x.User1).Include(x => x.User2).ToList();
            }

            using (var context = new EntityContext(connection))
            {
                var list = context.AspNetUserss.Include(x => x.User).ToList();
                context.AspNetUserss.Add(new AspNetUsers() { Id = "2", UserName = "Test2", User = context.Users.ToList() });
                context.SaveChanges();
            }

            connection.RollbackToRestorePoint();

            using (var context = new EntityContext(connection))
            {
                var list1 = context.AspNetUserss.Include(x => x.User).ToList();
                var list2 = context.AspNetUserss1.Include(x => x.User).ToList();
                var list3 = context.AspNetUserss2.Include(x => x.User).Include(x => x.User1).Include(x => x.User2).ToList();
            }
        }

        public class EntityContext : DbContext
        {
            public EntityContext(DbConnection connection) : base(connection, true)
            {
            }

            public DbSet<User> Users { get; set; }
            public DbSet<AspNetUsers> AspNetUserss { get; set; }
            public DbSet<User1> Users1 { get; set; }
            public DbSet<AspNetUsers1> AspNetUserss1 { get; set; }
            public DbSet<User2> Users2 { get; set; }
            public DbSet<AspNetUsers2> AspNetUserss2 { get; set; }



            protected override void OnModelCreating(DbModelBuilder modelBuilder)
            {

                base.OnModelCreating(modelBuilder);
            }
        }

        public class User
        {
            public int ID { get; set; }
            public int ColumnInt { get; set; }
        }


        public class AspNetUsers
        {
            public AspNetUsers()
            {
                User = new HashSet<User>();
            }

            public string Id { get; set; }
            
            [Required]
            [StringLength(maximumLength: 256)]
            public string UserName { get; set; }

            public virtual ICollection<User> User { get; set; }
        }

        public class User1
        {
            public int ID { get; set; }
            public int ColumnInt { get; set; }
        }


        public class AspNetUsers1
        {
            public AspNetUsers1()
            {
                User = new HashSet<User>();
            }

            public string Id { get; set; } 

            [Required]
            [StringLength(maximumLength: 256)]
            public string UserName { get; set; }
             

            public virtual ICollection<User> User { get; set; }
        }

        public class User2
        {
            public int ID { get; set; }
            public int ColumnInt { get; set; }
        }


        public class AspNetUsers2
        {
            public AspNetUsers2()
            {
                User = new HashSet<User>();
                User1 = new HashSet<User1>();
                User2 = new HashSet<User2>();
            }

            public string Id { get; set; }
             

            [Required]
            [StringLength(maximumLength: 256)]
            public string UserName { get; set; } 
            public virtual ICollection<User> User { get; set; }
            public virtual ICollection<User1> User1 { get; set; }
            public virtual ICollection<User2> User2 { get; set; }
        }
    }
}