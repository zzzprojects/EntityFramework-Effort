namespace Effort.Internal.StorageSchema
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;

    internal static class StorageSchemaNamespaces
    {
        public static readonly XNamespace V1 = "http://schemas.microsoft.com/ado/2006/04/edm/ssdl";
        public static readonly XNamespace V2 = "http://schemas.microsoft.com/ado/2009/02/edm/ssdl";
        public static readonly XNamespace V3 = "http://schemas.microsoft.com/ado/2009/11/edm/ssdl";
    }
}
