// --------------------------------------------------------------------------------------------
// <copyright file="ObjectContextTypeStore.cs" company="Effort Team">
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

namespace Effort.Internal.Caching
{
    using System;

    /// <summary>
    ///     Represents a cache that stores <see cref="Type"/> objects that serves as 
    ///     Effort-ready ObjectContext.
    /// </summary>
    internal class ObjectContextTypeStore
    {
        /// <summary>
        ///     Internal collection.
        /// </summary>
        private static ConcurrentCache<ObjectContextTypeKey, Type> store;

        /// <summary>
        ///     Initializes static members of the <see cref="ObjectContextTypeStore" /> class.
        /// </summary>
        static ObjectContextTypeStore()
        {
            store = new ConcurrentCache<ObjectContextTypeKey, Type>();
        }

        /// <summary>
        ///     Returns a ObjectContext type the satisfies the provided requirements. If no
        ///     such element exists the provided factory method is used to create one.
        /// </summary>
        /// <param name="entityConnectionString">
        ///     The entity connection string that identifies the database instance.
        /// </param>
        /// <param name="effortConnectionString">
        ///     The effort connection string that containes the database configuration.
        /// </param>
        /// <param name="objectContextType">
        ///     The base type that result type is derived from.
        /// </param>
        /// <param name="objectContextTypeFactoryMethod">
        ///     The factory method that instatiates the desired ObjectContext type.
        /// </param>
        /// <returns></returns>
        public static Type GetObjectContextType(
            string entityConnectionString, 
            string effortConnectionString, 
            Type objectContextType, 
            Func<Type> objectContextTypeFactoryMethod)
        {
            ObjectContextTypeKey key =
                new ObjectContextTypeKey(
                    entityConnectionString,
                    effortConnectionString,
                    objectContextType); 

            return store.Get(key, objectContextTypeFactoryMethod);
        }
    }
}
