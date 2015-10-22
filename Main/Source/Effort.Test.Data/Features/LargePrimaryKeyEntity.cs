// --------------------------------------------------------------------------------------------
// <copyright file="LargePrimaryKeyEntity.cs" company="Effort Team">
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
// ------------------------------------------------------------------------------------------

namespace Effort.Test.Data.Features
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class LargePrimaryKeyEntity
    {
        [Key]
        [Column(Order = 0)]
        public int Id0 { get; set; }

        [Key]
        [Column(Order = 1)]
        public int Id1 { get; set; }

        [Key]
        [Column(Order = 2)]
        public int Id2 { get; set; }

        [Key]
        [Column(Order = 3)]
        public int Id3 { get; set; }

        [Key]
        [Column(Order = 4)]
        public int Id4 { get; set; }

        [Key]
        [Column(Order = 5)]
        public int Id5 { get; set; }

        [Key]
        [Column(Order = 6)]
        public int Id6 { get; set; }

        [Key]
        [Column(Order = 7)]
        public int Id7 { get; set; }

        [Key]
        [Column(Order = 8)]
        public int Id8 { get; set; }

        [Key]
        [Column(Order = 9)]
        public int Id9 { get; set; }
    }
}
