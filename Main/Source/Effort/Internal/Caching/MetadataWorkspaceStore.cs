using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Metadata.Edm;

namespace Effort.Internal.Caching
{
    internal class MetadataWorkspaceStore
    {
         private static ConcurrentCache<string, MetadataWorkspace> store;

         static MetadataWorkspaceStore()
        {
            store = new ConcurrentCache<string, MetadataWorkspace>();
        }


        public static MetadataWorkspace GetMetadataWorkspace(string metadata)
        {
            return store.Get(metadata);
        }

        public static MetadataWorkspace GetMetadataWorkspace(string metadata, Func<string, MetadataWorkspace> workspacefactorymethod)
        {
            return store.Get(metadata, () => workspacefactorymethod(metadata));
        }

        
    }
}
