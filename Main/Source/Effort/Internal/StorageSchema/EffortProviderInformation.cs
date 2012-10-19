using Effort.Provider;
namespace Effort.Internal.StorageSchema
{
    internal class EffortProviderInformation : ProviderInformation
    {
        public EffortProviderInformation()
            : base(EffortProviderConfiguration.ProviderInvariantName, EffortProviderManifestTokens.Version1)
        {

        }
    }
}
