// --------------------------------------------------------------------------------------------
// <copyright file="ResultSetComposerMock.cs" company="Effort Team">
//     Copyright (C) 2012 Effort Team
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

namespace Effort.Test.Internal.Fakes
{
    using System.Collections.Generic;
    using Effort.Test.Internal.ResultSets;

    internal class ResultSetComposerMock : IResultSetComposer
    {
        private int commitCounter;
        private int setValueCounter;

        private HashSet<string> currentItem;

        public ResultSetComposerMock()
        {
            this.currentItem = new HashSet<string>();
            
            this.commitCounter = 0;
            this.setValueCounter = 0;
        }

        public IResultSet ResultSet
        {
            get { return null; }
        }

        public int CommitCount
        {
            get { return this.commitCounter; }
        }

        public int SetValueCount
        {
            get { return this.setValueCounter; }
        }

        public void SetValue<T>(string name, T value)
        {
            if (this.currentItem.Add(name))
            {
                this.setValueCounter++;
            }
        }

        public void Commit()
        {
            this.commitCounter++;
            this.currentItem.Clear();
        }
    }
}
