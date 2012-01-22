using System;
using System.ComponentModel;

namespace Effort.CsvTool
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyChanged(string propertyName)   
      {   
            if (string.IsNullOrEmpty(propertyName))   
                throw new ArgumentNullException("propertyName");   
  
            if (PropertyChanged != null)   
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));   
       }

        #endregion
    }
}
