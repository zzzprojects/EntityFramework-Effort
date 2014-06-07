// --------------------------------------------------------------------------------------------
// <copyright file="DynamicBareSchema.cs" company="Effort Team">
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

namespace Effort.Internal.DbManagement.Schema
{
    using System;
#if !EFOLD
    using System.Data.Entity.Core.Metadata.Edm;
#else
    using System.Data.Metadata.Edm;
#endif
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Threading;
    using Effort.Internal.Common;
    using Effort.Internal.TypeConversion;
    using Effort.Internal.DbManagement.Schema.Configuration;

    internal class DynamicBareSchema : BareSchemaBase
    {
        private readonly Assembly dynamicAssembly;

        public DynamicBareSchema(CanonicalContainer container, EdmTypeConverter converter)
        {
            AssemblyBuilder assembly =
                Thread.GetDomain().DefineDynamicAssembly(
                    new AssemblyName(string.Format("Effort_DynamicEntityLib({0})", Guid.NewGuid())),
                    AssemblyBuilderAccess.Run);

            ModuleBuilder entityModule = assembly.DefineDynamicModule("Entities");

            foreach (EntityInfo entity in container.GetEntities(converter))
            {
                string name = entity.TableName;
                Type type = CreateEntityType(entity, entityModule, converter);

                this.Register(name, type);
            }
            
            this.dynamicAssembly = assembly;
        }

        private static Type CreateEntityType(
            EntityInfo entity, 
            ModuleBuilder entityModule, 
            EdmTypeConverter typeConverter)
        {
            string cliTypeName = TypeHelper.NormalizeForCliTypeName(entity.TableName);

            TypeBuilder entityTypeBuilder = entityModule.DefineType(cliTypeName, TypeAttributes.Public);

            foreach (EntityPropertyInfo property in entity.Properties)
            {
                EmitHelper.AddProperty(entityTypeBuilder, property.Name, property.ClrType);
            }

            return entityTypeBuilder.CreateType();
        }
    }
}
