
namespace Effort.Test.Environment.Fakes
{
    using Effort.Test.Tools;
    using System.Collections.Generic;

    internal class ResultSetComposerMock : IResultSetComposer
    {
        private int commitCounter;
        private int setValueCounter;

        private HashSet<string> currentItem;

        public ResultSetComposerMock()
        {
            this.currentItem = new HashSet<string>();
            
            this.commitCounter = 0;
            this.setValueCounter = 0;
        }

        public IResultSet ResultSet
        {
            get { return null; }
        }

        public int CommitCount
        {
            get { return this.commitCounter; }
        }

        public int SetValueCount
        {
            get { return this.setValueCounter; }
        }

        public void SetValue<T>(string name, T value)
        {
            if (this.currentItem.Add(name))
            {
                this.setValueCounter++;
            }
        }

        public void Commit()
        {
            this.commitCounter++;
            this.currentItem.Clear();
        }
    }
}
