// --------------------------------------------------------------------------------------------
// <copyright file="FileProviderFixture.cs" company="Effort Team">
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

namespace Effort.Test.DataLoaders
{
    using System;
    using System.IO;
    using Effort.DataLoaders;
    using Effort.DataLoaders.Internal;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SoftwareApproach.TestingExtensions;

    [TestClass]
    public class FileProviderFixture
    {
        [TestMethod]
        public void ResourceFileProvider_FullPath()
        {
            var path = new Uri("res://Effort.Test/Internal/Resources/");
            var provider = new ResourceFileProvider(path);

            provider.IsValid.ShouldBeTrue();

            var file = provider.GetFile("EmptyResource.txt");

            file.Exists.ShouldBeTrue();

            using (StreamReader reader = new StreamReader(file.Open()))
            {
                reader.ReadLine().ShouldEqual("Hello World!");
            }
        }
    }
}
