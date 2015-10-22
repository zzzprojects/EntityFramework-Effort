// --------------------------------------------------------------------------------------------
// <copyright file="EffortException.cs" company="Effort Team">
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

namespace Effort.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    ///     Represents errors that occur in the Effort library.
    /// </summary>
    [Serializable]
    public class EffortException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EffortException"/> class.
        /// </summary>
        public EffortException() 
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EffortException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public EffortException(string message) : base(message) 
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EffortException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="inner">The inner exception.</param>
        public EffortException(string message, Exception inner) : base(message, inner) 
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EffortException"/> class.
        /// </summary>
        /// <param name="info">
        ///     The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> 
        ///     that holds the serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        ///     The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that 
        ///     contains contextual information about the source or destination.
        /// </param>
        protected EffortException(SerializationInfo info, StreamingContext context)
            : base(info, context) 
        { 
        }
    }
}
