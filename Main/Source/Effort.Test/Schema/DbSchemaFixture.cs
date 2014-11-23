// --------------------------------------------------------------------------------------------
// <copyright file="DbSchemaFixture.cs" company="Effort Team">
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
// -------------------------------------------------------------------------------------------

namespace Effort.Test.Schema
{
#if !EFOLD
    using System.Data.Entity.Core.Metadata.Edm;
#else
    using System.Data.Metadata.Edm;
#endif
    using System.IO;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Linq;
    using Effort.Internal.DbManagement;
    using Effort.Internal.DbManagement.Schema;
    using Effort.Internal.StorageSchema;
    using NUnit.Framework;

    [TestFixture]
    public class DbSchemaFixture
    {
        [Test]
        public void DbSchema_CompoundKey()
        {
            StoreItemCollection ssdl = LoadSSDL("CompoundKey"); 
 
            DbSchema schema = DbSchemaFactory.CreateDbSchema(ssdl);

            DbContainer container = new DbContainer(new DbContainerParameters());
            container.Initialize(schema);
        }

        private static StoreItemCollection LoadSSDL(string name)
        {
            string resourceName =
                string.Format(
                    "Effort.Test.Schema.Resources.DbSchema.{0}.xml",
                    name);

            Stream stream = Assembly
                .GetExecutingAssembly()
                .GetManifestResourceStream(resourceName);

            XElement ssdl = XDocument.Load(stream).Root;

            UniversalStorageSchemaModifier
                .Instance
                .Modify(ssdl, new EffortProviderInformation());

            return new StoreItemCollection(new XmlReader[] { ssdl.CreateReader() });
        }
    }
}
