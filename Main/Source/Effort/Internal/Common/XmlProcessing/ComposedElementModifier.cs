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
        private IElementModifier elementModifier;

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

        public ComposedElementModifier(IElementSelector elementSelector, IElementModifier elementModifier)
        {
            if (elementSelector == null)
            {
                throw new ArgumentNullException("elementSelector");
            }

            if (elementModifier == null)
            {
                throw new ArgumentNullException("elementModifier");
            }

            this.elementSelector = elementSelector;
            this.elementModifier = elementModifier;
        }

        public void Modify(XElement element, IModificationContext context)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            foreach (XElement selected in this.elementSelector.SelectElements(element))
            {
                if (this.attributeSelector != null && this.attributeModifier != null)
                {
                    XAttribute attribute = this.attributeSelector.SelectAttribute(selected);

                    if (attribute != null)
                    {
                        this.attributeModifier.Modify(attribute, context);
                    }
                }

                if (this.elementModifier != null)
                {
                    this.elementModifier.Modify(selected, context);
                }
            }
        }
    }
}
