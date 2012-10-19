namespace Effort.Internal.StorageSchema
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Data.Common;

    internal interface IProviderInformation
    {
        string InvariantName { get; }

        string ManifestToken { get; }

        DbProviderManifest Manifest { get; }
    }
}
