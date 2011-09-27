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
    public class ExpressionFixture
    {
        private QueryTestRuntime<NorthwindEntities> runtime;

        [TestInitialize]
        public void Initialize()
        {
            this.runtime = new QueryTestRuntime<NorthwindEntities>("name=NorthwindEntities");

        }

        [TestMethod]
        public void Addition()
        {
            bool result = this.runtime.Execute(

                context =>
                    from emp in context.Employees
                    where emp.EmployeeID + 3 == 4
                    select emp

            );

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Substraction()
        {
            bool result = this.runtime.Execute(

                context =>
                    from emp in context.Employees
                    where emp.EmployeeID - 3 == 4
                    select emp

            );

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Negation()
        {
            bool result = this.runtime.Execute(

                context =>
                    from emp in context.Employees
                    where - (emp.EmployeeID - 5) == 2
                    select emp

            );

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Multiply()
        {
            bool result = this.runtime.Execute(

                context =>
                    from emp in context.Employees
                    where emp.EmployeeID * 2 == 4
                    select emp

            );

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Divide()
        {
            bool result = this.runtime.Execute(

                context =>
                    from emp in context.Employees
                    where emp.EmployeeID / 2 == 2
                    select emp

            );

            Assert.IsTrue(result);
        }


        [TestMethod]
        public void ComplexArithmetic()
        {
            bool result = this.runtime.Execute(

                context =>
                    from emp in context.Employees
                    where  - (emp.EmployeeID * 3) / 2 + 5 == 1
                    select emp

            );

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Equals()
        {
            bool result = this.runtime.Execute(

                context =>
                    from emp in context.Employees
                    where emp.EmployeeID == 4
                    select emp

            );

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void GreaterThan()
        {
            bool result = this.runtime.Execute(

                context =>
                    from emp in context.Employees
                    where emp.EmployeeID > 4
                    select emp

            );

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void GreaterThanOrEquals()
        {
            bool result = this.runtime.Execute(

                context =>
                    from emp in context.Employees
                    where emp.EmployeeID >= 4
                    select emp

            );

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void SmallerThan()
        {
            bool result = this.runtime.Execute(

                context =>
                    from emp in context.Employees
                    where emp.EmployeeID < 4
                    select emp

            );

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void SmallerThanOrEquals()
        {
            bool result = this.runtime.Execute(

                context =>
                    from emp in context.Employees
                    where emp.EmployeeID <= 4
                    select emp

            );

            Assert.IsTrue(result);
        }




    }
}
