namespace Effort.Internal.Common.XmlProcessing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;
    using System.Data.Common;

    internal interface IElementModifier
    {
        void Modify(XElement element, IModificationContext context);
    }
}
