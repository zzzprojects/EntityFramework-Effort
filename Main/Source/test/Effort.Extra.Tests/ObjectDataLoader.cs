
namespace Effort.Extra.Tests
{
    using System;
    using System.Collections.Generic;
    using Effort.DataLoaders;
    using Machine.Fakes;
    using Machine.Specifications;

    internal class ObjectDataLoader
    {
        public class Ctor
        {
            [Subject("ObjectDataLoader.Ctor")]
            public abstract class ctor_context : WithFakes
            {
                protected static Exception thrown_exception;
                protected static Effort.DataLoaders.ObjectData data;
                protected static Effort.DataLoaders.ObjectDataLoader subject;

                Because of = () => thrown_exception = Catch.Exception(
                    () => subject = new Effort.DataLoaders.ObjectDataLoader(data));
            }

            public class when_data_is_null : ctor_context
            {
                Establish context = () =>
                {
                    data = null;
                };

                It throws_an_argument_null_exception =
                    () => thrown_exception.ShouldBeOfExactType<ArgumentNullException>();
            }

            public class when_data_is_valid : ctor_context
            {
                Establish context = () =>
                {
                    data = new Effort.DataLoaders.ObjectData();
                };

                It does_not_throw_an_exception = () => thrown_exception.ShouldBeNull();

                It the_data_identifier_is_assigned_to_the_argument = () =>
                    subject.Argument.ShouldEqual(data.Identifier.ToString());
            }
        }
        
        public class CreateTableDataLoaderFactory
        {
            [Subject("ObjectDataLoader.CreateTableDataLoaderFactory")]
            public abstract class create_table_data_loader_factory_context : WithSubject<Effort.DataLoaders.ObjectDataLoader>
            {
                protected static Exception thrown_exception;
                protected static Effort.DataLoaders.ObjectData data;
                protected static ITableDataLoaderFactory result;

                Establish context = () =>
                {
                    data = new Effort.DataLoaders.ObjectData();
                    Configure(x => x.For<Effort.DataLoaders.ObjectData>().Use(() => data));
                };

                Because of = () => thrown_exception = Catch.Exception(
                    () => result = Subject.CreateTableDataLoaderFactory());
            }

            public class when_argument_is_not_valid_guid : create_table_data_loader_factory_context
            {
                Establish context = () =>
                {
                    Subject.Argument = "Oh noes!";
                };

                It throws_an_invalid_operation_exception =
                    () => thrown_exception.ShouldBeOfExactType<InvalidOperationException>();
            }

            public class when_argument_does_not_exist_in_collection : create_table_data_loader_factory_context
            {
                Establish context = () =>
                {
                    Subject.Argument = Guid.NewGuid().ToString();
                };

                It throws_a_key_not_found_exception = () => 
                    thrown_exception.ShouldBeOfExactType<KeyNotFoundException>();
            }

            public class when_argument_is_valid_and_exists_in_collection : create_table_data_loader_factory_context
            {
                Establish context = () =>
                {
                    Subject.Argument = data.Identifier.ToString();
                };

                It does_not_throw_an_exception = () => thrown_exception.ShouldBeNull();

                It the_result_is_not_null = () => result.ShouldNotBeNull();
            }
        }
    }
}
