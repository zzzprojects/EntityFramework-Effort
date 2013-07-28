// --------------------------------------------------------------------------------------------
// <copyright file="DynamicBareSchema.cs" company="Effort Team">
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
    using System;
    using System.Data.Metadata.Edm;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Threading;
    using Effort.Internal.Common;
    using Effort.Internal.TypeConversion;

    internal class DynamicBareSchema : BareSchemaBase
    {
        private readonly Assembly dynamicAssembly;

        public DynamicBareSchema(EntityContainer entityContainer, EdmTypeConverter converter)
        {
            AssemblyBuilder assembly =
                Thread.GetDomain().DefineDynamicAssembly(
                    new AssemblyName(string.Format("Effort_DynamicEntityLib({0})", Guid.NewGuid())),
                    AssemblyBuilderAccess.Run);

            ModuleBuilder entityModule = assembly.DefineDynamicModule("Entities");

            foreach (EntitySet entitySet in entityContainer.BaseEntitySets.OfType<EntitySet>())
            {
                EntityType entityType = entitySet.ElementType;

                string name = entitySet.GetTableName();
                Type type = CreateEntityType(entitySet, entityModule, converter);

                this.Register(name, type);
            }
            
            this.dynamicAssembly = assembly;
        }

        private static Type CreateEntityType(
            EntitySet entitySet, 
            ModuleBuilder entityModule, 
            EdmTypeConverter typeConverter)
        {
            EntityType entityType = entitySet.ElementType;
            string cliTypeName = TypeHelper.NormalizeForCliTypeName(entitySet.GetTableName());

            TypeBuilder entityTypeBuilder = entityModule.DefineType(cliTypeName, TypeAttributes.Public);

            foreach (EdmProperty property in entityType.Properties)
            {
                string memberName = property.GetColumnName();
                Type memberType = typeConverter.Convert(property.TypeUsage);

                EmitHelper.AddProperty(entityTypeBuilder, memberName, memberType);
            }

            return entityTypeBuilder.CreateType();
        }
    }
}
