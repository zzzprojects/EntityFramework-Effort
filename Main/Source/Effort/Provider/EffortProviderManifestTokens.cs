// ----------------------------------------------------------------------------------
// <copyright file="EffortProviderManifestTokens.cs" company="Effort Team">
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

namespace Effort.Provider
{
    using System;

    /// <summary>
    /// Provides the supported Effort provider manifest token values.
    /// </summary>
    public static class EffortProviderManifestTokens
    {
        /// <summary>
        /// The Version1 provider manifest token.
        /// </summary>
        public const string Version1 = "1";

        /// <summary>
        /// Gets the <see cref="EffortVersion"/> enumeration value that represents the provided
        /// manifest token value.
        /// </summary>
        /// <param name="manifestToken">The value of the manifest token.</param>
        /// <returns>The <see cref="EffortVersion"/> value.</returns>
        /// <exception cref="System.NotSupportedException">The manifest token is not supported</exception>
        public static EffortVersion GetVersion(string manifestToken)
        {
            switch (manifestToken)
            {
                case Version1:
                    return EffortVersion.Version1;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
