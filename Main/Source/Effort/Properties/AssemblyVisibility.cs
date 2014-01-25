// --------------------------------------------------------------------------------------------
// <copyright file="AssemblyVisibility.cs" company="Effort Team">
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

using System.Runtime.CompilerServices;

#if SIGN_ASSEMBLY

[assembly: InternalsVisibleTo("Effort.Test, PublicKey=0024000004800000940000000602000000240000525341310004000001000100f189e6dad4bfd30224bd153091a88a16eb0505aa2e8504b9bb7e675ec47652056a56e4f8467681d7d7959a2b6653551bbf72397c316de6ba7818e799c29f319528e7e7a91b510b2a309c41393ef9b508a44655c58856e7fee994127cf91a7e75e72b294e3bee58939d8a107ff4544aa12e19e37d561860ec0627ad9c4be5aba4")]

#else

[assembly: InternalsVisibleTo("Effort.Test")]

#endif
