// --------------------------------------------------------------------------------------------
// <copyright file="EnumFieldFixture.cs" company="Effort Team">
//     Copyright (C) 2011-2013 Effort Team
//
//     Permission is hereby granted, free of charge, to any person obtaining a copy
//     of this software and associated documentation files (the "Software"), to deal
//     in the Software without restriction, including without limitation the rights
//     to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//     copies of the Software, and to permit persons to whom the Software is
//     furnished to do so, subject to the following conditions:
//
//     The above copyright notice and this permission notice shall be included in
//     all copies or substantial portions of the Software.
//
//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//     IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//     FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//     AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//     LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//     OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//     THE SOFTWARE.
// </copyright>
// --------------------------------------------------------------------------------------------

namespace Effort.Test.Features
{
    using System.Data.Common;
    using System.Data.Entity.Infrastructure;
#if !EFOLD
    using System.Data.Entity.Core.Objects;
#else
    using System.Data.Objects;
#endif
    using System.Linq;
    using Effort.Test.Data.Features;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SoftwareApproach.TestingExtensions;

#if NET45
    [TestClass]
    public class EnumFieldFixture
    {
        private FeatureDbContext context;

        [TestInitialize]
        public void Initialize()
        {
            DbConnection connection =
                Effort.DbConnectionFactory.CreateTransient();

            this.context =
                new FeatureDbContext(
                    connection,
                    CompiledModels.GetModel<EnumFieldEntity>());
        }


        [TestMethod]
        public void EnumField_Insert()
        {
            this.context.EnumFieldEntities.Add(
                new EnumFieldEntity { Value = EnumFieldType.EnumValue1 });

            this.context.SaveChanges();
        }

        [TestMethod]
        public void EnumField_Query()
        {
            this.context.EnumFieldEntities.Add(
                new EnumFieldEntity { Value = EnumFieldType.EnumValue1 });
            this.context.SaveChanges();

            var queried = this.context.EnumFieldEntities
                .FirstOrDefault(x => x.Value == EnumFieldType.EnumValue1);

            queried.ShouldNotBeNull();
        }

        [TestMethod]
        public void EnumField_Query_ESql_Param()
        {
            var entity = new EnumFieldEntity { Value = EnumFieldType.EnumValue1 };
            this.context.EnumFieldEntities.Add(entity);
            this.context.SaveChanges();

            var adapter = this.context as IObjectContextAdapter;
            var objectContext = adapter.ObjectContext;

            var sql = "SELECT VALUE t.Id FROM EnumFieldEntities as t WHERE t.Value == @Value";
            var parameters =
                new ObjectParameter[] { 
                    new ObjectParameter("Value", EnumFieldType.EnumValue1) 
                };

            var queried = objectContext
                .CreateQuery<object>(sql, parameters)
                .FirstOrDefault();

            queried.ShouldNotBeNull();
            queried.ShouldEqual(entity.Id);
        }

        [TestMethod]
        public void EnumField_Query_Param()
        {
            this.context.EnumFieldEntities.Add(
                new EnumFieldEntity { Value = EnumFieldType.EnumValue1 });
            this.context.SaveChanges();

            var param = EnumFieldType.EnumValue1;
            var queried = this.context.EnumFieldEntities
                .FirstOrDefault(x => x.Value == param);

            queried.ShouldNotBeNull();
        }

        [TestMethod]
        public void EnumField_Delete()
        {
            var entity = new EnumFieldEntity { Value = EnumFieldType.EnumValue1 };
            this.context.EnumFieldEntities.Add(entity);
            this.context.SaveChanges();

            this.context.EnumFieldEntities.Remove(entity);
            this.context.SaveChanges();
        }

        [TestMethod]
        public void EnumField_Update()
        {
            var entity = this.context.EnumFieldEntities.Create();
            entity.Value = EnumFieldType.EnumValue1;
            this.context.EnumFieldEntities.Add(entity);
            this.context.SaveChanges();

            entity.Value = EnumFieldType.EnumValue2;
            this.context.SaveChanges();

            var queried = this.context.EnumFieldEntities
               .FirstOrDefault(x => x.Value == EnumFieldType.EnumValue2);

            queried.ShouldNotBeNull();
        }
    }
#endif
}
