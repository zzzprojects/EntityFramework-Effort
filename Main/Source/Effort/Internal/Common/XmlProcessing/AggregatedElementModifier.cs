namespace Effort.Internal.Common.XmlProcessing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;

    internal class AggregatedElementModifier : IElementModifier
    {
        private IList<IElementModifier> modifiers;

        public AggregatedElementModifier()
        {
            this.modifiers = new List<IElementModifier>();
        }

        public void AddModifier(IElementModifier modifier)
        {
            if (modifier == null)
            {
                throw new ArgumentNullException("modifier");
            }

            this.modifiers.Add(modifier);
        }

        public void Modify(XElement root, IModificationContext context)
        {
            foreach (IElementModifier modifier in this.modifiers)
            {
                modifier.Modify(root, context);
            }
        }
    }
}
