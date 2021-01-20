using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
    public partial class Form_Request_TimeoutIssue : Form
    {
        public Form_Request_TimeoutIssue()
        {
            InitializeComponent();

            try
            {
                var connection = Effort.DbConnectionFactory.CreateTransient();
                //connection.SetConnectionTimeout(0);

                using (var inMemoryContext = new EntityContext(connection))
                {
                    var categories = new List<Categories>();

                    for (int i = 1; i < 5000; i++)
                    {
                        var category = new Categories()
                        {
                            CategoryID = i,
                            CategoryName = "test",
                            Products = new List<Products>()
                            {
                                new Products()
                                {
                                    ProductID = i,
                                    ProductName = "testProduct " + i,
                                    QuantityPerUnit = i.ToString(),
                                    UnitPrice = i * new Random().Next(),
                                    Discontinued = false,
                                    Suppliers = new Suppliers()
                                    {
                                        CompanyName = "testCompany " + i,
                                        SupplierID = i,
                                        Phone = "+31232435363",
                                        ContactName = "Lregdh",
                                    }
                                }
                            }
                        };

                        categories.Add(category);
                    }

                    inMemoryContext.Categories.AddRange(categories);
                    inMemoryContext.SaveChanges();
                }

                Console.WriteLine("Finished");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);

            }
        }

        public class EntityContext : DbContext
        {
            public EntityContext(DbConnection connection) : base(connection, true)
            {
            }

            public DbSet<Categories> Categories { get; set; }
            public DbSet<Products> Products { get; set; }
            public DbSet<Suppliers> Suppliers { get; set; }

            protected override void OnModelCreating(DbModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);
            }
        }

        public partial class Categories
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
            public Categories()
            {
                Products = new HashSet<Products>();
            }

            [Key]
            public int CategoryID { get; set; }

            [Required]
            [StringLength(15)]
            public string CategoryName { get; set; }

            [Column(TypeName = "ntext")]
            public string Description { get; set; }

            [Column(TypeName = "image")]
            public byte[] Picture { get; set; }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
            public virtual ICollection<Products> Products { get; set; }
        }

        public partial class Products
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
            public Products()
            {
            }

            [Key]
            public int ProductID { get; set; }

            [Required]
            [StringLength(40)]
            public string ProductName { get; set; }

            public int? SupplierID { get; set; }

            public int? CategoryID { get; set; }

            [StringLength(20)]
            public string QuantityPerUnit { get; set; }

            [Column(TypeName = "money")]
            public decimal? UnitPrice { get; set; }

            public short? UnitsInStock { get; set; }

            public short? UnitsOnOrder { get; set; }

            public short? ReorderLevel { get; set; }

            public bool Discontinued { get; set; }

            public virtual Categories Categories { get; set; }

            public virtual Suppliers Suppliers { get; set; }
        }

        public partial class Suppliers
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
            public Suppliers()
            {
                Products = new HashSet<Products>();
            }

            [Key]
            public int SupplierID { get; set; }

            [Required]
            [StringLength(40)]
            public string CompanyName { get; set; }

            [StringLength(30)]
            public string ContactName { get; set; }

            [StringLength(30)]
            public string ContactTitle { get; set; }

            [StringLength(60)]
            public string Address { get; set; }

            [StringLength(15)]
            public string City { get; set; }

            [StringLength(15)]
            public string Region { get; set; }

            [StringLength(10)]
            public string PostalCode { get; set; }

            [StringLength(15)]
            public string Country { get; set; }

            [StringLength(24)]
            public string Phone { get; set; }

            [StringLength(24)]
            public string Fax { get; set; }

            [Column(TypeName = "ntext")]
            public string HomePage { get; set; }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
            public virtual ICollection<Products> Products { get; set; }
        }
    }
}
