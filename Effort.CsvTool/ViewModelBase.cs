using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace MMDB.DatabaseExport
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
