
namespace Effort.Extra.Tests.Behaviours
{
    using System;
    using System.Collections.Generic;
    using Machine.Specifications;

    [Behaviors]
    internal class CreatesNewTableBehaviour
    {
        protected static Exception thrown_exception;
        protected static string table_name;
        protected static IList<object> result;

        It does_not_throw_an_exception = () => thrown_exception.ShouldBeNull();

        It the_table_is_not_null = () => result.ShouldNotBeNull();

        It the_table_is_empty = () => result.ShouldBeEmpty();
    }
}
