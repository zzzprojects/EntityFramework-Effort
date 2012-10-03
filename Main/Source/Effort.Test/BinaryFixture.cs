using System.Linq;
using Effort.Test.Data;
using Effort.Test.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Effort.Test.Data.Northwind;

namespace Effort.Test
{
    [TestClass]
    public class BinaryFixture
    {
        private QueryTestRuntime<NorthwindObjectContext> runtime;

        [TestInitialize]
        public void Initialize()
        {
            this.runtime = new QueryTestRuntime<NorthwindObjectContext>(NorthwindObjectContext.DefaultConnectionString);

        }


        [TestMethod]
        public void BinaryResult()
        {
            bool result = this.runtime.Execute(

                context =>
                    from emp in context.Employees
                    select emp.Photo

            );

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void BinaryConstant()
        {
            bool result = this.runtime.Execute(

                context =>
                    from emp in context.Employees
                    select new { Photo = emp.Photo, Dummy = new byte[] { 1, 2, 3 } }
            );
        }

        [TestMethod]
        public void BinaryParameter()
        {
            byte[] binary = new byte[] { 1, 2, 3 };

            bool result = this.runtime.Execute(

                context =>
                    from emp in context.Employees
                    select new { Photo = emp.Photo, Dummy = binary }
            );
        }
    }
}
