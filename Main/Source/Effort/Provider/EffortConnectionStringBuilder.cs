#region License

// Copyright (c) 2011 Effort Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#endregion


using System;
using System.Data.Common;

namespace Effort.Provider
{
    public class EffortConnectionStringBuilder : DbConnectionStringBuilder
    {
        private static readonly string Key_InstanceId = "InstanceId";
        private static readonly string Key_DataLoaderType = "DataLoaderType";
        private static readonly string Key_DataLoaderArg = "DataLoaderArg";
        private static readonly string Key_DataLoaderCached = "DataLoaderCached";

        public EffortConnectionStringBuilder()
        {

        }

        public EffortConnectionStringBuilder(string connectionString)
        {
            base.ConnectionString = connectionString;
        }


        public string InstanceId
        {
            get
            {
                if (!base.ContainsKey(Key_InstanceId))
                {
                    return string.Empty;
                }

                return base[Key_InstanceId] as string;
            }
            set
            {
                base[Key_InstanceId] = value;
            }
        }

        public Type DataLoaderType
        {
            get
            {
                if (!base.ContainsKey(Key_DataLoaderType))
                {
                    return null;
                }

                string assemblyQualifiedName = base[Key_DataLoaderType] as string;

                return Type.GetType(assemblyQualifiedName);
            }
            set
            {
                if (value == null)
                {
                    base[Key_DataLoaderType] = null;
                    return;
                }

                base[Key_DataLoaderType] = value.AssemblyQualifiedName;

                if (this.DataLoaderType != value)
                {
                    throw new InvalidOperationException("Cannot set dataloader");
                }
            }
        }

        public string DataLoaderArgument
        {
            get
            {
                if (!base.ContainsKey(Key_DataLoaderArg))
                {
                    return string.Empty;
                }

                return base[Key_DataLoaderArg] as string;
            }
            set
            {
                base[Key_DataLoaderArg] = value;
            }
        }

        public bool DataLoaderCached
        {
            get
            {
                if (!base.ContainsKey(Key_DataLoaderCached))
                {
                    return false;
                }

                return bool.Parse(base[Key_DataLoaderCached] as string);
            }
            set
            {
                base[Key_DataLoaderCached] = value.ToString();
            }
        }
    }
}
