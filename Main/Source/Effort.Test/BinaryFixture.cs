using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Effort.Test.Utils;
using Effort.Test.Data;

namespace Effort.Test
{
    [TestClass]
    public class BinaryFixture
    {
        private QueryTestRuntime<NorthwindEntities> runtime;

        [TestInitialize]
        public void Initialize()
        {
            this.runtime = new QueryTestRuntime<NorthwindEntities>("name=NorthwindEntities");

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
