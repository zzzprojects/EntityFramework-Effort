// --------------------------------------------------------------------------------------------
// <copyright file="EffortConnectionFixture.cs" company="Effort Team">
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
// -------------------------------------------------------------------------------------------

namespace Effort.Test
{
    using System;
    using Effort.Provider;
    using Effort.Test.Internal.Fakes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SoftwareApproach.TestingExtensions;
    
    [TestClass]
    public class EffortConnectionFixture
    {
        [TestMethod]
        public void transient_EffortConnection_should_unregister_container()
        {
            EffortConnectionMock conn = new EffortConnectionMock();
            conn.MarkAsPrimaryTransient();

            conn.Dispose();

            conn.IsUnregisterContainerCalled.ShouldBeTrue();
        }

        [TestMethod]
        public void transient_EffortConnection_should_unregister_container2()
        {
            EffortConnectionMock conn = new EffortConnectionMock();
            conn.ConnectionString = "IsTransient=True";

            conn.Dispose();

            conn.IsUnregisterContainerCalled.ShouldBeTrue();
        }

        [TestMethod]
        public void transient_EffortConnection_should_normalize_connectionstring()
        {
            EffortConnection conn = new EffortConnection();
            conn.ConnectionString = "IsTransient=True";

            var builder = new EffortConnectionStringBuilder(conn.ConnectionString);

            // The IsTransient flag should be removed
            builder.IsTransient.ShouldBeFalse();
            Guid value;
            // The InstanceId should be set as a guid
            Guid.TryParse(builder.InstanceId, out value).ShouldBeTrue();
        }
    }
}
