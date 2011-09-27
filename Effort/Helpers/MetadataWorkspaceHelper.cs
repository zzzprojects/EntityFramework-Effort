using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Metadata.Edm;

namespace Effort.Helpers
{
    internal static class MetadataWorkspaceHelper
    {
        public static EntityContainer GetEntityContainer(MetadataWorkspace workspace)
        {
            // Get the Storage Schema Definition
            ItemCollection ssdl = workspace.GetItemCollection(DataSpace.SSpace);

            EntityContainer entityContainer = ssdl.OfType<EntityContainer>().FirstOrDefault();

            if (entityContainer == null)
            {
                // Invalid SSDL
                throw new InvalidOperationException("The Storage Schema Definition does not contain any EntityContainer");
            }

            return entityContainer;
        }
    }
}
