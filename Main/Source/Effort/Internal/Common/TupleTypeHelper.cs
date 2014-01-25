// --------------------------------------------------------------------------------------------
// <copyright file="TupleTypeHelper.cs" company="Effort Team">
//     Copyright (C) 2011-2014 Effort Team
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

namespace Effort.Internal.Common
{
    using System;
    
    internal class TupleTypeHelper
    {
        public static readonly int LargeTupleSize = 8;

        public static Type CreateTupleType(Type[] memberTypes)
        {
            return CreateTupleType(memberTypes, 0);
        }

        private static Type CreateTupleType(Type[] memberTypes, int offset)
        {
            int memberCount = Math.Min(memberTypes.Length - offset, LargeTupleSize);

            Type[] args = new Type[memberCount];
            bool isLarge = false;

            if (LargeTupleSize <= memberCount)
            {
                isLarge = true;
                memberCount--;
            }

            for (int i = 0; i < memberCount; i++)
            {
                args[i] = memberTypes[offset + i];
            }

            if (isLarge)
            {
                // Last type is a tuple
                args[memberCount] = CreateTupleType(memberTypes, offset + memberCount);
            }

            return GetTupleType(args);
        }

        private static Type GetTupleType(params Type[] memberTypes)
        {
            Type generic = null;

            switch (memberTypes.Length)
            {
                case 1:
                    generic = typeof(Tuple<>);
                    break;
                case 2:
                    generic = typeof(Tuple<,>);
                    break;
                case 3:
                    generic = typeof(Tuple<,,>);
                    break;
                case 4:
                    generic = typeof(Tuple<,,,>);
                    break;
                case 5:
                    generic = typeof(Tuple<,,,,>);
                    break;
                case 6:
                    generic = typeof(Tuple<,,,,,>);
                    break;
                case 7:
                    generic = typeof(Tuple<,,,,,,>);
                    break;
                case 8:
                    generic = typeof(Tuple<,,,,,,,>);
                    break;
                default:
                    throw new ArgumentException("Too many members", "memberTypes");
            }

            return generic.MakeGenericType(memberTypes);
        }
    }
}
