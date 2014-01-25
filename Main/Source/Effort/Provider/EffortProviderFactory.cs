// --------------------------------------------------------------------------------------------
// <copyright file="EffortProviderFactory.cs" company="Effort Team">
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

namespace Effort.Provider
{
    using System;
    using System.Data;
    using System.Data.Common;
#if !EFOLD
    using System.Data.Entity.Core.Common;
#endif

    /// <summary>
    ///     Represents a set of methods for creating instances of the 
    /// <see cref="N:Effort.Provider"/> provider's implementation of the data source classes.
    /// </summary>
    public class EffortProviderFactory : DbProviderFactory, IServiceProvider
    {
        /// <summary>
        ///     Provides a singleton instance of the <see cref="EffortProviderFactory"/> class.
        /// </summary>
        public static readonly EffortProviderFactory Instance = new EffortProviderFactory();

        /// <summary>
        ///     Prevents a default instance of the <see cref="EffortProviderFactory" /> class
        ///     from being created.
        /// </summary>
        private EffortProviderFactory()
        {
        }

        /// <summary>
        ///     Returns a new instance of the <see cref="T:EffortConnection" /> class.
        /// </summary>
        /// <returns>
        ///     A new instance of <see cref="T:EffortConnection" />.
        /// </returns>
        public override DbConnection CreateConnection()
        {
            return new EffortConnection();
        }

        /// <summary>
        ///     Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">
        ///     An object that specifies the type of service object to get.
        /// </param>
        /// <returns>
        ///     A service object of type <paramref name="serviceType" />.-or- null if there is
        ///     no service object of type <paramref name="serviceType" />.
        /// </returns>
        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(DbProviderServices))
            {
                return EffortProviderServices.Instance;
            }

            return null;
        }
    }
}
