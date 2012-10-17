namespace Effort.Internal.StorageSchema
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Data.Common;
    using Effort.Internal.Common;

    internal class ProviderInformation : IProviderInformation
    {
        private string invariantName;
        private string manifestToken;
        private DbProviderManifest manifest;

        public ProviderInformation(string invariantName, string manifestToken)
        {
            this.invariantName = invariantName;
            this.manifestToken = manifestToken;
            this.manifest = ProviderHelper.GetProviderManifest(invariantName, manifestToken);
        }

        public string InvariantName
        {
            get { return this.invariantName; }
        }

        public string ManifestToken
        {
            get { return this.manifestToken; }
        }

        public DbProviderManifest Manifest
        {
            get { return this.manifest; }
        }
    }
}
