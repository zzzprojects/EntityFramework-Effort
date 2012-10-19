namespace Effort.Internal.Common.XmlProcessing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    internal interface IModificationContext
    {
        void Set<T>(string key, T element) 
            where T : class;

        T Get<T>(string key, T defaultElement) 
            where T : class;
    }
}
