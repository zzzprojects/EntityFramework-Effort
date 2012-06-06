using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace Effort.Provider
{
    /// <summary>
    /// Represents a collection of parameters associated with a EffortCommand.
    /// </summary>
    public sealed class EffortParameterCollection : DbParameterCollection
    {
        private object syncRoot;
        private List<EffortParameter> internalCollection;

        public EffortParameterCollection()
        {
            this.syncRoot = new object();
            this.internalCollection = new List<EffortParameter>();
        }

        public override int Add(object value)
        {
            EffortParameter parameter = value as EffortParameter;

            if (parameter == null)
            {
                throw new ArgumentException("The provided parammeter object is incompatible");
            }

            this.internalCollection.Add(parameter);
            return this.internalCollection.Count - 1;
        }

        public override void AddRange(Array values)
        {
            List<EffortParameter> parameters = new List<EffortParameter>();

            foreach (object value in values)
            {
                EffortParameter parameter = value as EffortParameter;

                if (parameter == null)
                {
                    throw new ArgumentException("The provided parammeter object is incompatible");
                }

                parameters.Add(parameter);
            }

            this.internalCollection.AddRange(parameters);
        }

        public override void Clear()
        {
            this.internalCollection.Clear();
        }

        public override bool Contains(string value)
        {
            return this.internalCollection.Any(p => p.ParameterName == value);
        }

        public override bool Contains(object value)
        {
            return this.internalCollection.Contains(value as EffortParameter);
        }

        public override void CopyTo(Array array, int index)
        {
            for (int i = 0; i < this.internalCollection.Count; i++)
            {
                array.SetValue(this.internalCollection[i], index + i);
            }
        }

        /// <summary>
        /// Returns the number of elements in the EffortParameterCollection
        /// </summary>
        public override int Count
        {
            get 
            {
                return this.internalCollection.Count;
            }
        }

        public override IEnumerator GetEnumerator()
        {
            return this.internalCollection.GetEnumerator();
        }

        protected override DbParameter GetParameter(string parameterName)
        {
            return this.internalCollection.FirstOrDefault(p => p.ParameterName == parameterName);
        }

        protected override DbParameter GetParameter(int index)
        {
            return this.internalCollection[index];
        }

        public override int IndexOf(string parameterName)
        {
            for (int i = 0; i < this.internalCollection.Count; i++)
            {
                if (this.internalCollection[i].ParameterName == parameterName)
                {
                    return i;
                }
            }

            return -1;
        }

        public override int IndexOf(object value)
        {
            return this.internalCollection.IndexOf(value as EffortParameter);
        }

        public override void Insert(int index, object value)
        {
            EffortParameter parameter = value as EffortParameter;

            if (parameter == null)
            {
                throw new ArgumentException("The provided parammeter object is incompatible");
            }

            this.internalCollection.Insert(index, parameter);
        }

        public override bool IsFixedSize
        {
            get { return false; }
        }

        public override bool IsReadOnly
        {
            get { return false; }
        }

        public override bool IsSynchronized
        {
            get { return false; }
        }

        public override void Remove(object value)
        {
            this.internalCollection.Remove(value as EffortParameter);
        }

        public override void RemoveAt(string parameterName)
        {
            EffortParameter parameter = this.internalCollection.FirstOrDefault(p => p.ParameterName == parameterName);

            this.internalCollection.Remove(parameter);
        }

        public override void RemoveAt(int index)
        {
            this.internalCollection.RemoveAt(index);
        }

        protected override void SetParameter(string parameterName, DbParameter value)
        {
        
        }

        protected override void SetParameter(int index, DbParameter value)
        {
        
        }

        public override object SyncRoot
        {
            get { return this.syncRoot; }
        }
    }
}
