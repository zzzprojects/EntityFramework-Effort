using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Effort;
using NMemory.Data;

namespace Effort.Lab.EF6
{
   public class AfterChangeDataAnotation
    {
        public static void test()
        {
	       Effort.EntityFrameworkEffortManager.CustomManifestPath = @"testa.xml";

			AfterChangetestString.Test.testStrings();
            AfterChangetestNumber2.Test.testNumber();
            AfterChangetestNumber.Test.testNumber(); 
            AfterChangetestBit.Test.testBit();
            AfterChangetestBinary.Test.testBinary();
        }
    }
}
namespace AfterChangetestString
{
    public class Test
    {

        public static void testStrings()
        {
            var transit = DbConnectionFactory.CreateTransient();
            using (var context = new ExampleModelA(transit))
            { 
                MyEntity test = new MyEntity(); 

                test.WeekDay = (int)DayOfWeek.Monday;
                 
                test.DateTime = new DateTime(2015, 4, 5);
                test.datetimeTwo = new DateTime(2015, 4, 5);
                test.datetimesmalldate = new DateTime(2015, 4, 5);
                test.Columvarchar_max = "Columvarchar_max";
                test.Columnvarchar_max = "Columnvarchar_max";
                test.Columvarchar = "Columvarchar";
                test.Columnnvarchar = "Columnnvarchar";
                test.date = new DateTime(2015, 4, 5);
                test.time = new TimeSpan(5, 5, 5);
                test.DateTimeOffset = DateTimeOffset.MinValue;
                test.Columtext = "Columtext";
                test.chara = "chara";
                test.nchar = "nchar";
                test.xml = "xml";
                test.Columvartext = "Columvartext";
                test.Columntext = "Columntext";
                test.Columnstring = "Columnstring";
	            test.OracleXMLTYPE = "OracleXMLTYPE";
	            test.TestData = "test";

				context.MyEntities.Add(test);
                context.SaveChanges();
            }

            using (var context = new ExampleModelA(transit))
            {
                var find1 = context.MyEntities.FirstOrDefault();
            }
        }
    }





    public class ExampleModelA : DbContext
    {


        public ExampleModelA(DbConnection dbConnection) : base(dbConnection, true)
        {

        }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        public virtual DbSet<MyEntity> MyEntities { get; set; }
    }

    public class MyEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("Columvarchar_max", TypeName = "varchar(max)")]
        public string Columvarchar_max { get; set; }
        [Column("Columnvarchar_max", TypeName = "nvarchar(max)")]
        public string Columnvarchar_max { get; set; }


        [Column("Columvartext", TypeName = "text")]
        public string Columvartext { get; set; } 

        [Column("Columvarchar", TypeName = "varchar")]
        public string Columvarchar { get; set; }
        [Column("Columnvarchar", TypeName = "nvarchar")]
        public string Columnnvarchar { get; set; }

        [Column("Columntext", TypeName = "ntext")]
        public string Columntext { get; set; }

        [Column("ColumDate", TypeName = "date")]
        public DateTime date { get; set; }
        [Column("Columtime", TypeName = "time")]
        public TimeSpan time { get; set; } //https://stackoverflow.com/questions/12186044/sql-time-type-in-entity-framework-code-first
        [Column("Columdatetimeoffset", TypeName = "datetimeoffset")]
        public DateTimeOffset DateTimeOffset { get; set; }

        [Column("Columtext", TypeName = "text")]
        public string Columtext { get; set; }

        [Column("Columnnchar", TypeName = "nchar")]
        public string nchar { get; set; }

        [Column("Columnxml", TypeName = "xml")]
        public string xml { get; set; }

        [Column("Columnchar", TypeName = "char")]
        public string chara { get; set; }


        [Column("Columnstring", TypeName = "string")]
        public string Columnstring { get; set; }

        public int WeekDay { get; set; }


        [Column("DateTime", TypeName = "datetime")]
        public DateTime DateTime { get; set; }
        [Column("datetimeTwo", TypeName = "datetime2")]
        public DateTime datetimeTwo { get; set; }
        [Column("datetimesmalldate", TypeName = "smalldatetime")]
        public DateTime datetimesmalldate { get; set; }

		[Column("OracleXMLTYPE", TypeName = "XMLTYPE")]
		public string OracleXMLTYPE { get; set; }
	    [Column("TestData", TypeName = "TestData")]
	    public string TestData { get; set; }
	}

}
namespace AfterChangetestNumber2
{


    public class Test
    {

        public static void testNumber()
        {
            var transit = DbConnectionFactory.CreateTransient();
            using (var context = new ExampleModelA(transit))
            { 
                MyEntity test = new MyEntity();

                test.Columnnint16 = 5;
                test.Columnnsmallint = 5;
                test.Columnnint32 = 5;
                test.Columnnint = 5;
                test.Columnnint64 = 5;
                test.Columnnbigint = 5;
                test.Columnndouble = 5;
                test.Columnnfloat = 5;
                test.Columnnsingle = 5;
                test.Columnndecimal = 5;
                test.Columnnsnumeric = 5;
                test.Columnnsmoney = 5;
                test.Columnnssmallmoney = 5;
                test.Columnnreal = 5;


                context.MyEntities.Add(test);
                context.BulkSaveChanges();
            }

            using (var context = new ExampleModelA(transit))
            {
                var find1 = context.MyEntities.FirstOrDefault();
            }
        }
    }

    public class ExampleModelA : DbContext
    {


        public ExampleModelA(DbConnection dbConnection) : base(dbConnection, true)
        {

        }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        public virtual DbSet<MyEntity> MyEntities { get; set; }
    }

    public class MyEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("Columnnint16", TypeName = "int16")]
        public Int16 Columnnint16 { get; set; }
        [Column("Columnnsmallint", TypeName = "smallint")]
        public Int16 Columnnsmallint { get; set; }
        [Column("Columnnint32", TypeName = "int32")]
        public int Columnnint32 { get; set; }
        [Column("Columnnint", TypeName = "int")]
        public int Columnnint { get; set; }
        [Column("Columnnint64", TypeName = "int64")]
        public Int64 Columnnint64 { get; set; }
        [Column("Columnnbigint", TypeName = "bigint")]
        public Int64 Columnnbigint { get; set; }
        [Column("Columnndouble", TypeName = "double")]
        public double Columnndouble { get; set; }
        [Column("Columnnfloat", TypeName = "float")]
        public double Columnnfloat { get; set; }
        [Column("Columnnsingle", TypeName = "single")]
        public Single Columnnsingle { get; set; }
        [Column("Columnnreal", TypeName = "real")]
        public Single Columnnreal { get; set; }
        [Column("Columnnsdecimal", TypeName = "decimal")]
        public decimal Columnndecimal { get; set; }
        [Column("Columnnsnumeric", TypeName = "numeric")]
        public decimal Columnnsnumeric { get; set; }
        [Column("Columnnsmoney", TypeName = "money")]
        public decimal Columnnsmoney { get; set; }
        [Column("Columnnssmallmoney", TypeName = "smallmoney")]
        public decimal Columnnssmallmoney { get; set; }
    }

}
namespace AfterChangetestNumber
{


    public class Test
    {

        public static void testNumber()
        {
            var transit = DbConnectionFactory.CreateTransient();
            using (var context = new ExampleModelA(transit))
            {
               

                MyEntity test = new MyEntity();

                //test.Columnnint16 = 5;
              //  test.Columnnsmallint = 5;
             //   test.Columnnint32 = 5;
                //test.Columnnint = 5;
                //test.Columnnint64 = 5;
                //test.Columnnbigint = 5;
                //test.Columnndouble = 5;
                //test.Columnnfloat = 5;
                //test.Columnnsingle = 5;
                //test.Columnndecimal = 5;
                //test.Columnnsnumeric = 5;
                //test.Columnnsmoney = 5;
              //  test.Columnnssmallmoney = 5;
                //test.Columnnreal = 5;


                context.MyEntities.Add(test);
                context.BulkSaveChanges();
            }

            using (var context = new ExampleModelA(transit))
            {
                var find1 = context.MyEntities.FirstOrDefault();
            }
        }
    }

    public class ExampleModelA : DbContext
    {


        public ExampleModelA(DbConnection dbConnection) : base(dbConnection, true)
        {

        }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        public virtual DbSet<MyEntity> MyEntities { get; set; }
    }

    public class MyEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        //[Column("Columnnint16", TypeName = "int16")]
        //public Int16 Columnnint16 { get; set; }
     //   [Column("Columnnsmallint", TypeName = "smallint")]
      //  public Int16 Columnnsmallint { get; set; }
        //[Column("Columnnint32", TypeName = "int32")]
        //public int Columnnint32 { get; set; }
        //[Column("Columnnint", TypeName = "int")]
        //public int Columnnint { get; set; }
        //[Column("Columnnint64", TypeName = "int64")]
        //public Int64 Columnnint64 { get; set; }
        //[Column("Columnnbigint", TypeName = "bigint")]
        //public Int64 Columnnbigint { get; set; }
        //[Column("Columnndouble", TypeName = "double")]
        //public double Columnndouble { get; set; }
        //[Column("Columnnfloat", TypeName = "float")]
        //public double Columnnfloat { get; set; }
        //[Column("Columnnsingle", TypeName = "single")]
        //public Single Columnnsingle { get; set; }
        //[Column("Columnnreal", TypeName = "real")]
        //public Single Columnnreal { get; set; }
        //[Column("Columnnsdecimal", TypeName = "decimal")]
        //public decimal Columnndecimal { get; set; }
        //[Column("Columnnsnumeric", TypeName = "numeric")]
        //public decimal Columnnsnumeric { get; set; }
        //[Column("Columnnsmoney", TypeName = "money")]
        //public decimal Columnnsmoney { get; set; }
        //[Column("Columnnssmallmoney", TypeName = "smallmoney")]
        //public decimal Columnnssmallmoney { get; set; } 
    }

}
namespace AfterChangetestBit
{

    public class Test
    {

        public static void testBit()
        {
            var transit = DbConnectionFactory.CreateTransient();
            using (var context = new ExampleModelA(transit))
            {
                MyEntity test = new MyEntity();



                test.Columnnboolean = false;
                test.Columnnsbit = false;
                test.Columnnuniqueidentifier = Guid.NewGuid();
                test.Columnnuniqueiguid = Guid.NewGuid();
                test.Columnnbyte = Byte.MaxValue;
                test.Columnnstinyint = Byte.MinValue;
                test.Columnnsbyte = SByte.MinValue;


                context.MyEntities.Add(test);
                context.SaveChanges();
            }

            using (var context = new ExampleModelA(transit))
            {
                var find1 = context.MyEntities.FirstOrDefault();
            }

            var transit2 = DbConnectionFactory.CreateTransient();
            using (var context = new ExampleModelA(transit2))
            {
                MyEntity test = new MyEntity();



                test.Columnnboolean = false;
                test.Columnnsbit = false;
                test.Columnnuniqueidentifier = Guid.NewGuid();
                test.Columnnuniqueiguid = Guid.NewGuid();
                test.Columnnbyte = Byte.MaxValue;
                test.Columnnstinyint = Byte.MinValue;
                test.Columnnsbyte = SByte.MinValue;

                context.MyEntities.Add(test);
               
                context.BulkSaveChanges();
            }

            using (var context = new ExampleModelA(transit2))
            {
                var find1 = context.MyEntities.FirstOrDefault();
            }
        }
    }

    public class ExampleModelA : DbContext
    {


        public ExampleModelA(DbConnection dbConnection) : base(dbConnection, true)
        {

        }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        public virtual DbSet<MyEntity> MyEntities { get; set; }
    }

    public class MyEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("Columnnboolean", TypeName = "boolean")]
        public bool Columnnboolean { get; set; }
        [Column("Columnnsbit", TypeName = "bit")]
        public bool Columnnsbit { get; set; }
        [Column("Columnnuniqueidentifier", TypeName = "uniqueidentifier")]
        public Guid Columnnuniqueidentifier { get; set; }
        [Column("Columnnuniqueiguid", TypeName = "guid")]
        public Guid Columnnuniqueiguid { get; set; }
        [Column("Columnnbyte", TypeName = "byte")]
        public byte Columnnbyte { get; set; }
        [Column("Columnnstinyint", TypeName = "tinyint")]
        public byte Columnnstinyint { get; set; }
        [Column("Columnnsbyte", TypeName = "sbyte")]
        public SByte Columnnsbyte { get; set; }


        //[Column("Columrowversion", TypeName = "rowversion")]
        ////public Timestamp rowversion { get; set; }
        //[Column("Columtimestamp", TypeName = "timestamp")]
        //public byte[] Columtimestamp { get; set; }
    }
}
namespace AfterChangetestBinary
{

    public class Test
    {

        public static void testBinary()
        {
            var transit = DbConnectionFactory.CreateTransient();
            using (var context = new ExampleModelA(transit))
            {
                MyEntity test = new MyEntity();

                test.image = new byte[] { 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20 }; // valeur mit dedans, mais très mal indiquer 
                test.binary = new byte[] { 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20 }; // valeur mit dedans, mais très mal indiquer 
                test.binary2 = new byte[] { 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20 }; // valeur mit dedans, mais très mal indiquer 
                test.binary_varbinary = new byte[] { 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20 }; // valeur mit dedans, mais très mal indiquer 
                context.MyEntities.Add(test);
                context.BulkSaveChanges();
            }

            using (var context = new ExampleModelA(transit))
            {
                var find1 = context.MyEntities.FirstOrDefault();
            }
        }
    }

    public class ExampleModelA : DbContext
    {


        public ExampleModelA(DbConnection dbConnection) : base(dbConnection, true)
        {

        }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        public virtual DbSet<MyEntity> MyEntities { get; set; }
    }

    public class MyEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("image", TypeName = "image")]
        public byte[] image { get; set; }


        [Column("binary", TypeName = "binary")]
        public byte[] binary { get; set; }


        [Column("varbinarie", TypeName = "varbinary")]
        public byte[] binary_varbinary { get; set; }

        [Column("binary2", TypeName = "varbinary(MAX)")]
        public byte[] binary2 { get; set; }

    }
} 
