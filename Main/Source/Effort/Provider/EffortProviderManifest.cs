// --------------------------------------------------------------------------------------------
// <copyright file="EffortProviderManifest.cs" company="Effort Team">
//     Copyright (C) 2012 Effort Team
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

namespace Effort.Provider
{
    using System;
    using System.Data.Common;
    using System.Data.Metadata.Edm;
    using System.IO;
    using System.Reflection;
    using System.Xml;

    /// <summary>
    /// Metadata interface for all CLR types types.
    /// </summary>
    public class EffortProviderManifest : DbXmlEnabledProviderManifest
    {
        private const string MetadataResource = "Effort.Provider.EffortProviderManifest.xml";
        private EffortVersion version;

        /// <summary>
        /// Initializes a new instance of the <see cref="EffortProviderManifest" /> class.
        /// </summary>
        /// <param name="version">The version of manifest metadata.</param>
        public EffortProviderManifest(EffortVersion version) : base(GetProviderManifest())
        {
            this.version = version;
        }

        /// <summary>
        /// When overridden in a derived class, this method maps the specified storage type and
        /// a set of facets for that type to an EDM type.
        /// </summary>
        /// <param name="storeType">
        /// The <see cref="T:System.Data.Metadata.Edm.TypeUsage" /> instance that describes a
        /// storage type and a set of facets for that type to be mapped to the EDM type.
        /// </param>
        /// <returns>
        /// The <see cref="T:System.Data.Metadata.Edm.TypeUsage" /> instance that describes an
        /// EDM type and a set of facets for that type.
        /// </returns>
        public override TypeUsage GetEdmType(TypeUsage storeType)
        {
            string name = storeType.EdmType.Name.ToLowerInvariant();

            PrimitiveType typeKind = this.StoreTypeNameToEdmPrimitiveType[name];
            return TypeUsage.CreateDefaultTypeUsage(typeKind);
        }

        /// <summary>
        /// When overridden in a derived class, this method maps the specified EDM type and a
        /// set of facets for that type to a storage type.
        /// </summary>
        /// <param name="edmType">
        /// The <see cref="T:System.Data.Metadata.Edm.TypeUsage" /> instance that describes the
        /// EDM type and a set of facets for that type to be mapped to a storage type.
        /// </param>
        /// <returns>
        /// The <see cref="T:System.Data.Metadata.Edm.TypeUsage" /> instance that describes a
        /// storage type and a set of facets for that type.
        /// </returns>
        public override TypeUsage GetStoreType(TypeUsage edmType)
        {
            string name = edmType.EdmType.Name.ToLowerInvariant();

            PrimitiveType typeKind = this.StoreTypeNameToStorePrimitiveType[name];
            
            return TypeUsage.CreateDefaultTypeUsage(typeKind);
        }

        /// <summary>
        /// When overridden in a derived class, this method returns provider-specific 
        /// information. This method should never return null.
        /// </summary>
        /// <param name="informationType">The type of the information to return.</param>
        /// <returns>
        /// The <see cref="T:System.Xml.XmlReader"/> object that contains the requested 
        /// information.
        /// </returns>
        protected override XmlReader GetDbInformation(string informationType)
        {
            throw new NotSupportedException();
        }

        private static XmlReader GetProviderManifest()
        {
            Assembly effortAssembly = typeof(EffortProviderManifest).Assembly;
            Stream stream = effortAssembly.GetManifestResourceStream(MetadataResource);

            return XmlReader.Create(stream);
        }
    }
}
