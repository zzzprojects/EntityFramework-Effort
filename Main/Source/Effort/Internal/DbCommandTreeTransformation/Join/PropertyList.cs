// ----------------------------------------------------------------------------------
// <copyright file="PropertyList.cs" company="Effort Team">
//     Copyright (C) 2012 by Effort Team
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
// ----------------------------------------------------------------------------------

namespace Effort.Internal.DbCommandTreeTransformation.Join
{
    internal class PropertyList
    {
        private object[] values;

        public PropertyList(object[] values)
        {
            this.values = values;
        }

        public object FirstProperty
        {
            get
            {
                return this.values[0];
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is PropertyList == false)
            {
                return false;
            }

            var otherList = obj as PropertyList;

            if (this.values.Length != otherList.values.Length)
            {
                return false;
            }

            for (int i = 0; i < this.values.Length; i++)
            {
                if (this.values[i] == null && otherList.values[i] == null)
                {
                    continue;
                }

                if (this.values[i] == null || otherList.values[i] == null)
                {
                    return false;
                }

                if (this.values[i].Equals(otherList.values[i]) == false)
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            int result = this.GetHashCodeFor(0);

            for (int i = 1; i < this.values.Length; i++)
            {
                result ^= this.GetHashCodeFor(i);
            }

            return result;
        }

        private int GetHashCodeFor(int index)
        {
            if (this.values[index] == null)
            {
                return 0;
            }
            else
            {
                return this.values[index].GetHashCode();
            }
        }
    }
}
