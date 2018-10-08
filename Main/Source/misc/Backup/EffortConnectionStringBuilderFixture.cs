// --------------------------------------------------------------------------------------------
// <copyright file="EffortConnectionStringBuilderFixture.cs" company="Effort Team">
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

namespace Effort.Test
{
    using Effort.Provider;
    using NUnit.Framework;
    using System;

    [TestFixture]
    public class EffortConnectionStringBuilderFixture
    {
        [Test]
        public void EffortConnectionStringBuilder_InstanceId()
        {
            var writer = new EffortConnectionStringBuilder();
            writer.InstanceId = "InstanceId";

            var reader = new EffortConnectionStringBuilder(writer.ConnectionString);

            Assert.AreEqual("InstanceId", reader.InstanceId);
        }

        [Test]
        public void EffortConnectionStringBuilder_DataLoaderArgument()
        {
            var writer = new EffortConnectionStringBuilder();
            writer.DataLoaderArgument = "LoaderArgument";

            var reader = new EffortConnectionStringBuilder(writer.ConnectionString);

            Assert.AreEqual("LoaderArgument", reader.DataLoaderArgument);
        }

        [Test]
        public void EffortConnectionStringBuilder_DataLoaderType()
        {
            var writer = new EffortConnectionStringBuilder();
            writer.DataLoaderType = typeof(Effort.DataLoaders.EmptyDataLoader);

            var reader = new EffortConnectionStringBuilder(writer.ConnectionString);

            Assert.AreEqual(typeof(Effort.DataLoaders.EmptyDataLoader), reader.DataLoaderType);
        }

        [Test]
        public void EffortConnectionStringBuilder_IsTransient()
        {
            var writer = new EffortConnectionStringBuilder();
            writer.IsTransient = true;

            var reader = new EffortConnectionStringBuilder(writer.ConnectionString);

            Assert.IsTrue(reader.IsTransient);
        }

        [Test]
        public void EffortConnectionStringBuilder_Normalize()
        {
            var builder = new EffortConnectionStringBuilder();
            builder.IsTransient = true;

            builder.Normalize();

            Assert.IsFalse(builder.IsTransient);
            Guid value;
            Assert.IsTrue(Guid.TryParse(builder.InstanceId, out value));
        }
    }
}
