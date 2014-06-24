using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Effort.Internal.Common.XmlProcessing;

namespace Effort.Internal.StorageSchema
{
    /// <summary>
    /// Removes function mapping for Insert, Update, and Delete.
    /// This is required for being able to save changes to EFFORT context
    /// based on model with defined modification function mappings.
    /// </summary>
    internal class ModificationFunctionMappingModifier : IElementModifier
    {
        public void Modify(XElement element, IModificationContext context)
        {
            var functionMappings = element.Descendants(XName.Get("ModificationFunctionMapping", "http://schemas.microsoft.com/ado/2009/11/mapping/cs")).ToList();
            foreach (var mappingElement in functionMappings)
            {
                mappingElement.Remove();
            }
        }
    }
}