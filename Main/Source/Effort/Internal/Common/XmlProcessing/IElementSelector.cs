namespace Effort.Internal.Common.XmlProcessing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;

    internal interface IElementSelector
    {
        IEnumerable<XElement> SelectElements(XElement element);
    }
}
