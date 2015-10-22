// --------------------------------------------------------------------------------------------
// <copyright file="ConstraintFactories.cs" company="Effort Team">
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

namespace Effort.Internal.DbManagement.Schema.Constraints
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Effort.Internal.Common;
    using NMemory.Common;

    internal static class ConstraintFactories
    {
        public static object NotNull(MemberInfo member)
        {
            return Create(typeof(NotNullableConstraintFactory<,>), member);
        }

        public static object GeneratedGuid(MemberInfo member)
        {
            return Create(typeof(GeneratedGuidConstraintFactory<>), member);
        }

        public static object CharLimit(MemberInfo member, int maxLength)
        {
            return Create(typeof(CharLimitConstraintFactory<>), member, maxLength);
        }

        public static object VarCharLimit(MemberInfo member, int maxLength)
        {
            return Create(typeof(VarCharLimitConstraintFactory<>), member, maxLength);
        }

        private static object Create(Type factory, MemberInfo member, params object[] args)
        {
            IEntityMemberInfo entityMember = CreateEntityMemberInfo(member);

            return CreateFactory(factory, entityMember, args);
        }

        private static IEntityMemberInfo CreateEntityMemberInfo(MemberInfo member)
        {
            Type entityType = member.DeclaringType;
            Type memberType = TypeHelper.GetMemberType(member);

            Type memberInfoType = typeof(DefaultEntityMemberInfo<,>)
                .MakeGenericType(entityType, memberType);

            IEntityMemberInfo result = Activator.CreateInstance(memberInfoType, member) 
                as IEntityMemberInfo;

            if (result == null)
            {
                throw new InvalidOperationException("Failed to create member info");
            }

            return result;
        }

        private static object CreateFactory(
            Type factoryType, 
            IEntityMemberInfo member, 
            params object[] args)
        {
            int generics = factoryType.GetGenericArguments().Length;

            switch (generics)
            {
                case 1:
                    factoryType = factoryType.MakeGenericType(member.EntityType);
                    break;
                case 2:
                    factoryType = factoryType.MakeGenericType(member.EntityType, member.MemberType);
                    break;
                default:
                    throw new InvalidOperationException("Invalid factory type");
            }

            object[] ctorArgs = new object[args.Length + 1];
            ctorArgs[0] = member;
            Array.Copy(args, 0, ctorArgs, 1, args.Length);

            ConstructorInfo ctor = factoryType.GetConstructors().Single();
            return ctor.Invoke(ctorArgs);
        }
    }
}
