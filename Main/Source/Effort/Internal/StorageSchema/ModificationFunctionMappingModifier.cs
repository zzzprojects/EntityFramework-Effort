// --------------------------------------------------------------------------------------------
// <copyright file="ModificationFunctionMappingModifier.cs" company="Effort Team">
//     Copyright (C) 2011-2014 Effort Team
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

namespace Effort.Internal.StorageSchema
{
    using System.Linq;
    using System.Xml.Linq;
    using Effort.Internal.Common.XmlProcessing;

    /// <summary>
    ///		Removes function mapping for Insert, Update, and Delete. This is required for being 
    ///		able to save changes to EFFORT context based on model with defined modification 
    ///		function mappings.
    /// </summary>
    internal class ModificationFunctionMappingModifier : IElementModifier
    {
        public void Modify(XElement element, IModificationContext context)
        {
            var functionMappings = element.Descendants(
                XName.Get(
                    "ModificationFunctionMapping", 
                    "http://schemas.microsoft.com/ado/2009/11/mapping/cs"))
                .ToList();

            foreach (var mappingElement in functionMappings)
            {
                mappingElement.Remove();
            }
        }
    }
}
