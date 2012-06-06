#region License

// Copyright (c) 2011 Effort Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#endregion

using System.Data;
using System.Data.Common;
using System.Data.Metadata.Edm;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Effort.Provider
{
    public class EffortProviderManifest : DbXmlEnabledProviderManifest
    {
        private EffortVersion version;

        public EffortProviderManifest(EffortVersion version) : base(GetProviderManifest())
        {
            this.version = version;
        }

        private static XmlReader GetProviderManifest()
        {
            Assembly effortAssembly = typeof(EffortProviderManifest).Assembly;
            Stream stream = effortAssembly.GetManifestResourceStream("Effort.Provider.EffortProviderManifest.xml");

            return XmlReader.Create(stream);
        }

        protected override XmlReader GetDbInformation(string informationType)
        {
            throw new ProviderIncompatibleException();
        }

        public override TypeUsage GetEdmType(TypeUsage storeType)
        {
            string name = storeType.EdmType.Name.ToLowerInvariant();

            PrimitiveType typeKind = base.StoreTypeNameToEdmPrimitiveType[name];
            return TypeUsage.CreateDefaultTypeUsage(typeKind);
        }

        public override TypeUsage GetStoreType(TypeUsage edmType)
        {
            string name = edmType.EdmType.Name.ToLowerInvariant();

            PrimitiveType typeKind = base.StoreTypeNameToStorePrimitiveType[name];
            return TypeUsage.CreateDefaultTypeUsage(typeKind);
        }
    }
}
