// --------------------------------------------------------------------------------------------
// <copyright file="DbContainerParameters.cs" company="Effort Team">
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

using System;
using System.Configuration;
using System.Linq;

namespace Effort.Internal.DbManagement
{
    using Effort.DataLoaders;

    internal class DbContainerParameters
    {
        private bool? isCaseSensitive;

        public IDataLoader DataLoader { get; set; }

        public bool IsTransient { get; set; }

        public bool IsCaseSensitive
        {
            get
            {
                if (!isCaseSensitive.HasValue)
                {
                    var configValue = ConfigurationManager.AppSettings["effort:caseSensitive"];
                    // for sake of backwards compatibility, return true when app settings is not specified.
                    isCaseSensitive =
                        string.IsNullOrEmpty(configValue) ||
                        TrueValue.Any(v => string.Equals(v, configValue, StringComparison.InvariantCultureIgnoreCase));
                }
                return isCaseSensitive.Value;
            }
            set { isCaseSensitive = value; }
        }

        private static string[] TrueValue = new[] { "true", "1", "on" };

    }
}
