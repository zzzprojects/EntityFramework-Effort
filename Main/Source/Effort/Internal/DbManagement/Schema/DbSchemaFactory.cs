// --------------------------------------------------------------------------------------------
// <copyright file="DbSchemaFactory.cs" company="Effort Team">
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

namespace Effort.Internal.DbManagement.Schema
{
#if !EFOLD
    using System.Data.Entity.Core.Metadata.Edm;
#else
    using System.Data.Metadata.Edm;
#endif
    using System.Linq;
    using Effort.Internal.TypeConversion;
    using Effort.Internal.DbManagement.Schema.Configuration;

    internal static class DbSchemaFactory
    {
        public static DbSchema CreateDbSchema(StoreItemCollection edmStoreSchema)
        {
            EntityContainer entityContainer =
                edmStoreSchema.GetItems<EntityContainer>().FirstOrDefault();

            EdmTypeConverter converter = new EdmTypeConverter(new DefaultTypeConverter());
            IBareSchema bareSchema = new DynamicBareSchema(entityContainer, converter);

            TableConfigurationGroup tableConfig = new TableConfigurationGroup();
            tableConfig.Register(new BareSchemaConfiguration(bareSchema));
            tableConfig.Register<PrimaryKeyConfiguration>();
            tableConfig.Register<IdentityConfiguration>();
            tableConfig.Register<GeneratedGuidConfiguration>();
            tableConfig.Register<NotNullConfiguration>();
            tableConfig.Register<VarcharLimitConfiguration>();
            tableConfig.Register<CharLimitConfiguration>();

            DbSchemaBuilder schemaBuilder = new DbSchemaBuilder();

            foreach (EntitySet entitySet in entityContainer.BaseEntitySets.OfType<EntitySet>())
            {
                EntityInfo entityInfo = new EntityInfo(entitySet, converter);
                DbTableInfoBuilder tableBuilder = new DbTableInfoBuilder();

                // Run all configurations
                tableConfig.Configure(entityInfo, tableBuilder);

                schemaBuilder.Register(tableBuilder);
            }

            RelationConfigurationGroup associationConfig = new RelationConfigurationGroup();
            associationConfig.Register<RelationConfiguration>();

            foreach (AssociationSet association in entityContainer.BaseEntitySets.OfType<AssociationSet>())
            {
                AssociationInfo associationInfo = null;

                if (!AssociationInfo.Create(association, out associationInfo))
                {
                    continue;
                }

                associationConfig.Configure(associationInfo, schemaBuilder);
            }

            return schemaBuilder.Create();
        }
    }
}
