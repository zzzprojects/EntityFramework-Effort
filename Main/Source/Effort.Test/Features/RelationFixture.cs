// --------------------------------------------------------------------------------------------
// <copyright file="RelationFixture.cs" company="Effort Team">
//     Copyright (C) 2011-2014 Effort Team
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
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using Effort.Test.Data.Features;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SoftwareApproach.TestingExtensions;

    [TestClass]
    public class RelationFixture
    {
        private DbCompiledModel model;
        private FeatureDbContext context;


        [TestInitialize]
        public void Initialize()
        {
            var connection = Effort.DbConnectionFactory.CreateTransient();

            this.model = CompiledModels.GetModel<RelationEntity, EmptyEntity>();
            this.context = new FeatureDbContext(connection, this.model);
        }

        [TestCleanup]
        public void Cleanup()
        {
            this.context.Dispose();
        }

        public IDbSet<RelationEntity> RelationEntities
        {
            get
            {
                return this.context.RelationEntities;
            }
        }

        [TestMethod]
        public void OptionalInclude()
        {
            this.RelationEntities.Add(
                new RelationEntity()
                {
                    RequiredRelation = new EmptyEntity(),
                    OptionalRelation = new EmptyEntity()
                });

            this.context.SaveChanges();

            var res = this.RelationEntities
                .Include(x => x.OptionalRelation)
                .FirstOrDefault();

            res.ShouldNotBeNull();
            res.OptionalRelation.ShouldNotBeNull();
        }

        [TestMethod]
        public void RequiredInclude()
        {
            this.RelationEntities.Add(
                new RelationEntity()
                {
                    RequiredRelation = new EmptyEntity()
                });

            this.context.SaveChanges();

            var res = this.RelationEntities
                .Include(x => x.RequiredRelation)
                .FirstOrDefault();

            res.ShouldNotBeNull();
            res.RequiredRelation.ShouldNotBeNull();
        }

        [TestMethod]
        public void CascadedRequiredInclude()
        {
            this.RelationEntities.Add(
                new RelationEntity()
                {
                    RequiredRelation = new EmptyEntity(),
                    CascadedRelation =
                        new RelationEntity()
                        {
                            RequiredRelation = new EmptyEntity()
                        }
                });

            this.context.SaveChanges();

            var res = this.RelationEntities
                .Include(x => x.CascadedRelation.RequiredRelation)
                .AsEnumerable()
                .Where(x => x.CascadedRelation != null)
                .FirstOrDefault();

            res.ShouldNotBeNull();
            res.CascadedRelation.ShouldNotBeNull();
            res.CascadedRelation.RequiredRelation.ShouldNotBeNull();
        }

        [TestMethod]
        public void CascadedDelete()
        {
            this.RelationEntities.Add(
                new RelationEntity
                {
                    RequiredRelation = new EmptyEntity()
                });

            this.context.SaveChanges();

            // If the entity was removed in the current context, EF would remove the 
            // referrenced entity automatically
            // If a new context is created, the database would perform the cascaded delete
            var newContext = 
                new FeatureDbContext(this.context.Database.Connection, this.model);
            
            var empty = newContext.EmptyEntities.Single();

            newContext.EmptyEntities.Remove(empty);
            newContext.SaveChanges();
        }
    }
}
