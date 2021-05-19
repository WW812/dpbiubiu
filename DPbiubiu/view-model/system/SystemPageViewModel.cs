using biubiu.model.system;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace biubiu.view_model.system
{
    public class SystemPageViewModel : INotifyPropertyChanged
    {
        public SystemPageViewModel()
        {
        }


        /// <summary>
        /// NotifyPropertyChanged事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
