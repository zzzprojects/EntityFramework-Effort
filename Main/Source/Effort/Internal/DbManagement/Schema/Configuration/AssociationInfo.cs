// --------------------------------------------------------------------------------------------
// <copyright file="AssociationInfo.cs" company="Effort Team">
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

namespace Effort.Internal.DbManagement.Schema.Configuration
{
    using System.Collections.Generic;
#if !EFOLD
    using System.Data.Entity.Core.Metadata.Edm;
#else
    using System.Data.Metadata.Edm;
#endif
    using System.Linq;

    internal class AssociationInfo
    {
        private readonly AssociationSet association;
        private readonly ReferentialConstraint constraint;

        private AssociationInfo(AssociationSet association)
        {
            this.association = association;
            this.constraint = association.ElementType.ReferentialConstraints[0];
        }

        public static bool Create(AssociationSet association, out AssociationInfo result)
        {
            if (association.ElementType.ReferentialConstraints.Count != 1)
            {
                result = null;
                return false;
            }

            result = new AssociationInfo(association);
            return true;
        }

        public EntitySet PrimaryTable
        {
            get
            {
                return this.GetTable(constraint.FromRole);
            }
        }

        public EntitySet ForeignTable
        {
            get
            {
                return this.GetTable(constraint.ToRole);
            }
        }

        public ICollection<EdmProperty> PrimaryKeyMembers
        {
            get
            {
                return this.constraint.FromProperties;
            }
        }

        public ICollection<EdmProperty> ForeignKeyMembers
        {
            get
            {
                return this.constraint.ToProperties;
            }
        }

        public bool CascadedDelete
        {
            get
            {
                return this.association
                    .AssociationSetEnds[this.constraint.FromRole.Name]
                    .CorrespondingAssociationEndMember
                    .DeleteBehavior == OperationAction.Cascade;
            }
        }

        private EntitySet GetTable(RelationshipEndMember relationEndpoint)
        {
            RefType refType = relationEndpoint.TypeUsage.EdmType as RefType;

            return this.association
                .AssociationSetEnds
                .Select(x => x.EntitySet)
                .First(x => x.ElementType == refType.ElementType);
        }
    }
}
