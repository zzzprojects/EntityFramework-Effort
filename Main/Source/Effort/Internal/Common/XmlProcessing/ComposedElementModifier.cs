namespace Effort.Internal.Common.XmlProcessing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;

    internal class ComposedElementModifier : IElementModifier
    {
        private IElementSelector elementSelector;
        private IElementAttributeSelector attributeSelector;
        private IAttributeModifier attributeModifier;

        public ComposedElementModifier(IElementSelector elementSelector, IElementAttributeSelector attributeSelector, IAttributeModifier attributeModifier)
        {
            if (elementSelector == null)
            {
                throw new ArgumentNullException("elementSelector");
            }

            if (attributeSelector == null)
            {
                throw new ArgumentNullException("attributeSelector");
            }

            if (attributeModifier == null)
            {
                throw new ArgumentNullException("attributeModifier");
            }

            this.elementSelector = elementSelector;
            this.attributeSelector = attributeSelector;
            this.attributeModifier = attributeModifier;
        }

        public void Modify(XElement element, IModificationContext context)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            foreach (XElement selected in this.elementSelector.SelectElements(element))
            {
                XAttribute attribute = this.attributeSelector.SelectAttribute(selected);

                if (attribute != null)
                {
                    this.attributeModifier.Modify(attribute, context);
                }
            }

        }
    }
}
