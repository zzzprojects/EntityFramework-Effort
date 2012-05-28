#region License

// Copyright (c) 2011 Effort Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#endregion

using System;
using System.Linq;

namespace Effort.Internal.TypeGeneration
{
    internal class TupleTypeFactory
    {
        public static Type Create(params Type[] typeArguments)
        {
            switch (typeArguments.Length)
            {
                case 1:
                    return typeof(Tuple<>).MakeGenericType(typeArguments);
                case 2:
                    return typeof(Tuple<,>).MakeGenericType(typeArguments);
                case 3:
                    return typeof(Tuple<,,>).MakeGenericType(typeArguments);
                case 4:
                    return typeof(Tuple<,,,>).MakeGenericType(typeArguments);
                case 5:
                    return typeof(Tuple<,,,,>).MakeGenericType(typeArguments);
                case 6:
                    return typeof(Tuple<,,,,,>).MakeGenericType(typeArguments);
                case 7:
                    return typeof(Tuple<,,,,,,>).MakeGenericType(typeArguments);
                default:
                    return AnonymousTypeFactory.Create(
                        typeArguments
                        .Select((type, i) => new { 
                            Name = string.Format("Item{0}", i), 
                            Type = type })
                        .ToDictionary(x => x.Name, x => x.Type));
            }

            
        }
    }
}
