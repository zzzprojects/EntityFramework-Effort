namespace Effort.Internal.Common.XmlProcessing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;

    internal class SelfElementSelector : IElementSelector
    {
        public IEnumerable<XElement> SelectElements(XElement element)
        {
            yield return element;
        }
    }
}
