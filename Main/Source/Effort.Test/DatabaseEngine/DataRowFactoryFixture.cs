// --------------------------------------------------------------------------------------------
// <copyright file="DataRowFactoryFixture.cs" company="Effort Team">
//     Copyright (C) 2011-2013 Effort Team
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

namespace Effort.Test.DatabaseEngine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Effort.Internal.TypeGeneration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SoftwareApproach.TestingExtensions;
    
    [TestClass]
    public class DataRowFactoryFixture
    {
        [TestMethod]
        public void DataRow_DataRowProperty_attribute_Index_property()
        {
            Type type = CreateType(CreateNames(8));

            foreach (PropertyInfo prop in type.GetProperties())
            {
                DataRowPropertyAttribute attr = prop
                    .GetCustomAttributes(false)
                    .OfType<DataRowPropertyAttribute>()
                    .FirstOrDefault();

                attr.ShouldNotBeNull();

                int index = int.Parse(prop.Name.Split(new char[] { '_' })[1]);

                index.ShouldEqual(attr.Index);
            }
        }

        [TestMethod]
        public void DataRow_same_definition_should_result_in_same_type()
        {
            IDictionary<string, Type> definition = CreateRichDefinition();

            Type datarow1 = DataRowFactory.Create(definition);
            Type datarow2 = DataRowFactory.Create(definition);

            datarow1.ShouldBeSameAs(datarow2);
        }

        [TestMethod]
        public void DataRow_GetValue_method()
        {
            int count = 8;
            Type type = CreateType(CreateNames(count));

            object[] args = GenerateArguments(count);

            DataRow data = Activator.CreateInstance(type, args) as DataRow;
            data.ShouldNotBeNull();

            for (int i = 0; i < count; i++)
            {
                bool b1 = data.Equals(null);
                bool b2 = data.Equals(data);
                bool b3 = data.Equals(5543);
                object stored = data.GetValue(i);

                stored.ShouldNotBeNull();
                stored.ShouldEqual(args[i]);
            }
        }

        [TestMethod]
        public void DataRow_Equal_method()
        {
            object[] args = new object[] { 1, 2, 3 };
            Type type = CreateType(CreateNames(args.Length));

            DataRow data = CreateInstance(type, args);
            data.ShouldNotBeNull();

            data.Equals(null).ShouldBeFalse();
            data.Equals(new object()).ShouldBeFalse();
            data.Equals(5).ShouldBeFalse();
            data.Equals(data).ShouldBeTrue();
            data.Equals(CreateInstance(type, args)).ShouldBeTrue();
        }

        [TestMethod]
        public void DataRow_GetHashCode_should_result_in_same_value()
        {
            object[] args = new object[] { 1, 2, 3 };
            Type type = CreateType(CreateNames(args.Length));

            DataRow data = CreateInstance(type, args);
            data.ShouldNotBeNull();

            int hash = data.GetHashCode();

            CreateInstance(type, args).GetHashCode().ShouldEqual(hash);
        }

        [TestMethod]
        public void DataRow_annotated_with_LargeDataRowAttribute_if_it_has_many_properties()
        {
            Type type = CreateType(CreateNames(LargeDataRowAttribute.LargePropertyCount));

            type.GetCustomAttributes(typeof(LargeDataRowAttribute), false).ShouldNotBeEmpty();
        }

        private static DataRow CreateInstance(Type type, object[] args)
        {
            DataRow data = Activator.CreateInstance(type, args) as DataRow;
            return data;
        }

        private static object[] GenerateArguments(int count)
        {
            object[] args = new object[count];
            Random rnd = new Random();
            for (int i = 0; i < count; i++)
            {
                args[i] = rnd.Next();
            }
            return args;
        }

        private static Type CreateType(params string[] names)
        {
            Dictionary<string, Type> props = new Dictionary<string,Type>();

            for (int i = 0; i < names.Length; i++)
            {
                props.Add(names[i], typeof(int));
            }

            return DataRowFactory.Create(props);
        }

        private static string[] CreateNames(int count)
        {
            string[] result = new string[count];

            for (int i = 0; i < count; i++)
            {
                result[i] = string.Format("Prop_{0}", i);
            }

            return result;
        }

        private static Dictionary<string, Type> CreateRichDefinition()
        {
            return new Dictionary<string, Type>() {
                    { "FirstName", typeof(string) },
                    { "Age", typeof(int) },
                    { "BirthDate", typeof(DateTime) }
                };
        }
    }
}
