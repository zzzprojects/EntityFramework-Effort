// --------------------------------------------------------------------------------------------
// <copyright file="FeatureDbContext.cs" company="Effort Team">
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

namespace Effort.Test.Data.Features
{
    using System.Data.Common;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;

    public class FeatureDbContext : DbContext
    {
        public FeatureDbContext(DbConnection connection)
            : this(connection, CompiledModels.DefaultModel)
        { 
        }

        public FeatureDbContext(DbConnection connection, DbCompiledModel model) 
            : base(connection, model, true)
        {
        }

        public IDbSet<StringFieldEntity> StringFieldEntities { get; set; }

        public IDbSet<GuidKeyEntity> GuidKeyEntities { get; set; }

        public IDbSet<DateFieldEntity> DateFieldEntities { get; set; }

        public IDbSet<LargeStringFieldEntity> LargeStringFieldEntities { get; set; }

        public IDbSet<DateTimeOffsetFieldEntity> DateTimeOffsetFieldEntities { get; set; }

        public IDbSet<DateTimeFieldEntity> DateTimeFieldEntities { get; set; }

        public IDbSet<TimeFieldEntity> TimeFieldEntities { get; set; }

        public IDbSet<EnumFieldEntity> EnumFieldEntities { get; set; }

        public IDbSet<TimestampFieldEntity> TimestampFieldEntities { get; set; }

        public IDbSet<LargeTimestampFieldEntity> LargeTimestampFieldEntities { get; set; }

        public IDbSet<RequiredFieldEntity> RequiredFieldEntities { get; set; }

        public IDbSet<NumberFieldEntity> NumberFieldEntities { get; set; }

        public IDbSet<DecimalIdentityFieldEntity> DecimalIdentityFieldEntities { get; set; }

        public IDbSet<MathEntity> MathEntities { get; set; }

        public IDbSet<LargePrimaryKeyEntity> LargePrimaryKeyEntities { get; set; }

        public IDbSet<RelationEntity> RelationEntities { get; set; }

        public IDbSet<EmptyEntity> EmptyEntities { get; set; }

        public IDbSet<BinaryKeyEntity> BinaryKeyEntities { get; set; }

#if EF61
        public IDbSet<IndexedFieldEntity> IndexedFieldEntities { get; set; }

		public IDbSet<TimestampIndexedFieldEntity> TimestampIndexedFieldEntities { get; set; }
#endif
	}
}
