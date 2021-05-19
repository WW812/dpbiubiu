using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace biubiu.view_model.role
{
    public class PermissionViewModel : INotifyPropertyChanged
    {
        private string _id;
        private string _name;
        private bool _isSelected;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                this.MutateVerbose(ref _name, value, RaisePropertyChanged());
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected == value) return;
                _isSelected = value;
                this.MutateVerbose(ref _isSelected,value,RaisePropertyChanged());
            }
        }

        public string ID
        {
            get { return _id; }
            set
            {
                _id = value;
                this.MutateVerbose(ref _id, value, RaisePropertyChanged());
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private Action<PropertyChangedEventArgs> RaisePropertyChanged()
        {
            return args => PropertyChanged?.Invoke(this, args);
        }
    }
}
