
namespace Effort.Extra.Tests.Behaviours
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Machine.Specifications;

    [Behaviors]
    internal class ReturnsExistingTableBehaviour
    {
        protected static Exception thrown_exception;
        protected static string table_name;
        protected static IList<object> result;
        protected static IList<object> expected_result;
        
        It does_not_throw_an_exception = () => thrown_exception.ShouldBeNull();

        It the_table_is_not_null = () => result.ShouldNotBeNull();

        It the_table_contains_the_existing_items = () =>
        {
            expected_result.All(result.Contains).ShouldBeTrue();
            result.All(expected_result.Contains).ShouldBeTrue();
        };
    }
}