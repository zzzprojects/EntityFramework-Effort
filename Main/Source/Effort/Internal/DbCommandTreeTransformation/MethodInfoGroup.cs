// --------------------------------------------------------------------------------------------
// <copyright file="MethodInfoGroup.cs" company="Effort Team">
//     Copyright (C) 2012-2013 Effort Team
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

namespace Effort.Internal.DbCommandTreeTransformation
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Effort.Internal.Common;

    internal sealed class MethodInfoGroup
    {
        private Dictionary<Type, FastLazy<MethodInfo>> methods;

        public MethodInfoGroup(params Tuple<Type, FastLazy<MethodInfo>>[] methods)
        {
            this.methods = new Dictionary<Type, FastLazy<MethodInfo>>();

            for (int i = 0; i < methods.Length; i++)
            {
                this.methods.Add(methods[i].Item1, methods[i].Item2);
            }
        }

        public MethodInfo this[Type type]
        {
            get
            {
                FastLazy<MethodInfo> result = null;

                if (this.methods.TryGetValue(type, out result))
                {
                    return result.Value;
                }

                return null;
            }
        }
    }
}
