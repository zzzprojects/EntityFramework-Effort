// --------------------------------------------------------------------------------------------
// <copyright file="AggregatedElementModifier.cs" company="Effort Team">
//     Copyright (C) Effort Team
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

namespace Effort.Internal.Common.XmlProcessing
{
    using System;
    using System.Collections.Generic;
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
