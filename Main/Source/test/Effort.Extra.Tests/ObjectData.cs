
namespace Effort.Extra.Tests
{
    using System;
    using System.Collections.Generic;
    using Effort.Extra.Tests.Behaviours;
    using Machine.Fakes;
    using Machine.Specifications;

    internal class ObjectData
    {
        public class Ctor
        {
            [Subject("ObjectData.Ctor")]
            public class when_class_is_instantiated : WithSubject<Effort.DataLoaders.ObjectData>
            {
                It generates_a_new_identifier = () => Subject.Identifier.ShouldNotEqual(Guid.Empty);
            }
        }
        
        public class Table
        {
            [Subject("ObjectData.Table")]
            public abstract class table_context : WithSubject<Effort.DataLoaders.ObjectData>
            {
                protected static Exception thrown_exception;
                protected static string table_name;
                protected static IList<object> result;

                Because of = () => thrown_exception = Catch.Exception(
                    () => result = Subject.Table<object>(table_name));
            }

            public class when_table_with_name_does_not_exist : table_context
            {
                Establish context = () =>
                {
                    table_name = "Knights_of_the_Round_Table";
                };

                Behaves_like<CreatesNewTableBehaviour> a_new_table_is_created;
            }

            public class when_table_with_name_exists : table_context
            {
                protected static IList<object> expected_result;
                
                Establish context = () =>
                {
                    table_name = "Knights_of_the_Round_Table";
                    expected_result = Subject.Table<object>(table_name);
                    expected_result.Add("Arthur, King of the Britons");
                    expected_result.Add("Sir Bedevere, the Wise");
                    expected_result.Add("Sir Lancelot, the Brave");
                    expected_result.Add("Sir Galahad, the Pure");
                    expected_result.Add("Sir Robin, the not quite as brave as Sir Lancelot");
                };

                Behaves_like<ReturnsExistingTableBehaviour> the_existing_table_is_returned;
            }

            public class when_table_with_name_exists_but_is_of_different_type : table_context
            {
                Establish context = () =>
                {
                    table_name = "Knights_of_the_Round_Table";
                    var table = Subject.Table<string>(table_name);
                    table.Add("Arthur, King of the Britons");
                    table.Add("Sir Bedevere, the Wise");
                    table.Add("Sir Lancelot, the Brave");
                    table.Add("Sir Galahad, the Pure");
                    table.Add("Sir Robin, the not quite as brave as Sir Lancelot");
                };

                It throws_an_invalid_operation_exception =
                    () => thrown_exception.ShouldBeOfExactType<InvalidOperationException>();
            }
        }
        
        public class HasTable
        {
            [Subject("ObjectData.HasTable")]
            public abstract class has_table_context : WithSubject<Effort.DataLoaders.ObjectData>
            {
                protected static Exception thrown_exception;
                protected static string table_name;
                protected static bool result;

                private Because of = () => thrown_exception = Catch.Exception(
                    () => result = Subject.HasTable(table_name));
            }

            public class when_table_name_is_null : has_table_context
            {
                Establish context = () => table_name = null;

                It throws_an_argument_null_exception =
                    () => thrown_exception.ShouldBeOfExactType<ArgumentNullException>();
            }

            public class when_table_name_is_empty_string : has_table_context
            {
                Establish context = () => table_name = String.Empty;

                It throws_an_argument_exception =
                    () => thrown_exception.ShouldBeOfExactType<ArgumentException>();
            }

            public class when_table_name_is_whitespace : has_table_context
            {
                Establish context = () => table_name = "  ";

                It throws_an_argument_exception =
                    () => thrown_exception.ShouldBeOfExactType<ArgumentException>();
            }

            public class when_table_does_not_exist : has_table_context
            {
                Establish context = () =>
                {
                    table_name = "Knights_of_the_Round_Table";
                };

                It does_not_throw_an_exception = () => thrown_exception.ShouldBeNull();

                It the_result_is_false = () => result.ShouldBeFalse();
            }

            public class when_table_exists : has_table_context
            {
                Establish context = () =>
                {
                    table_name = "Knights_of_the_Round_Table";
                    Subject.Table<string>(table_name);
                };

                It does_not_throw_an_exception = () => thrown_exception.ShouldBeNull();

                It the_result_is_true = () => result.ShouldBeTrue();
            }
        }
        
        public class TableType
        {
            [Subject("ObjectData.TableType")]
            public abstract class table_type_context : WithSubject<Effort.DataLoaders.ObjectData>
            {
                protected static Exception thrown_exception;
                protected static string table_name;
                protected static Type result;

                Because of = () => thrown_exception = Catch.Exception(
                    () => result = Subject.TableType(table_name));
            }

            public class when_table_name_is_null : table_type_context
            {
                Establish context = () => table_name = null;

                It throws_an_argument_null_exception =
                    () => thrown_exception.ShouldBeOfExactType<ArgumentNullException>();
            }

            public class when_table_name_is_empty_string : table_type_context
            {
                Establish context = () => table_name = String.Empty;

                It throws_an_argument_exception =
                    () => thrown_exception.ShouldBeOfExactType<ArgumentException>();
            }

            public class when_table_name_is_whitespace : table_type_context
            {
                Establish context = () => table_name = "  ";

                It throws_an_argument_exception =
                    () => thrown_exception.ShouldBeOfExactType<ArgumentException>();
            }

            public class when_table_does_not_exist : table_type_context
            {
                Establish context = () => table_name = "Sir_not_appearing_in_this_test.";

                It throws_an_invalid_operation_exception =
                    () => thrown_exception.ShouldBeOfExactType<InvalidOperationException>();
            }

            public class when_table_exists : table_type_context
            {
                Establish context = () =>
                {
                    table_name = "Knights_of_the_Round_Table";
                    Subject.Table<string>(table_name);
                };

                It does_not_throw_an_exception = () => thrown_exception.ShouldBeNull();

                It the_result_is_correct = () => result.ShouldEqual(typeof(string));
            }
        }

        public class GetTable
        {
            [Subject("ObjectData.GetTable")]
            public abstract class get_table_context : WithSubject<Effort.DataLoaders.ObjectData>
            {
                protected static Exception thrown_exception;
                protected static string table_name;
                protected static object result;

                Because of = () => thrown_exception = Catch.Exception(
                    () => result = Subject.GetTable(table_name));
            }

            public class when_table_name_is_null : get_table_context
            {
                Establish context = () => table_name = null;

                It throws_an_argument_null_exception =
                    () => thrown_exception.ShouldBeOfExactType<ArgumentNullException>();
            }

            public class when_table_name_is_empty_string : get_table_context
            {
                Establish context = () => table_name = String.Empty;

                It throws_an_argument_exception =
                    () => thrown_exception.ShouldBeOfExactType<ArgumentException>();
            }

            public class when_table_name_is_whitespace : get_table_context
            {
                Establish context = () => table_name = "  ";

                It throws_an_argument_exception =
                    () => thrown_exception.ShouldBeOfExactType<ArgumentException>();
            }

            public class when_table_does_not_exist : get_table_context
            {
                Establish context = () => table_name = "Sir_not_appearing_in_this_test.";

                It does_not_throw_an_exception = () => thrown_exception.ShouldBeNull();

                It the_result_is_null = () => result.ShouldBeNull();
            }

            public class when_table_exists : get_table_context
            {
                static object expected_result;

                Establish context = () =>
                {
                    table_name = "Knights_of_the_Round_Table";
                    expected_result = Subject.Table<string>(table_name);
                };

                It does_not_throw_an_exception = () => thrown_exception.ShouldBeNull();

                It the_result_is_correct = () => result.ShouldEqual(expected_result);
            }
        }
    }
}
