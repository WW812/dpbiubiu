using biubiu.model.role;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace biubiu.view_model.user
{
    public class UserViewModel : INotifyPropertyChanged
    {

        public UserViewModel()
        {

        }

        public UserViewModel(string role)
        {
            _userOfRole = role;
        }

        /// <summary>
        /// ID
        /// </summary>
        private string _id;
        public string ID
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
                //NotifyPropertyChanged("UserName");
                this.MutateVerbose(ref _id, value, RaisePropertyChanged());
            }
        }

        /// <summary>
        /// 用户名
        /// </summary>
        private string _userName;
        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                _userName = value;
                //NotifyPropertyChanged("UserName");
                this.MutateVerbose(ref _userName, value, RaisePropertyChanged());
            }
        }

        /// <summary>
        /// 密码
        /// </summary>
        private string _passWord;
        public string PassWord
        {
            get
            {
                return _passWord;
            }
            set
            {
                _passWord = value;
                this.MutateVerbose(ref _passWord, value, RaisePropertyChanged());
                //NotifyPropertyChanged("PassWord");
            }
        }

        private string _userOfRole;
        public string UserOfRole
        {
            get
            {
                return _userOfRole;
            }
            set
            {
                _userOfRole = value;
                this.MutateVerbose(ref _userOfRole, value, RaisePropertyChanged());
            }
        }

        private int _type;
        public int Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
                this.MutateVerbose(ref _type, value, RaisePropertyChanged());
            }
        }

        private string _nickName;
        public string NickName
        {
            get
            {
                return _nickName;
            }
            set
            {
                _nickName = value;
                this.MutateVerbose(ref _nickName, value, RaisePropertyChanged());
            }
        }

        private string _note;
        public string Note
        {
            get
            {
                return _note;
            }
            set
            {
                _note = value;
                this.MutateVerbose(ref _note, value, RaisePropertyChanged());
            }
        }

        /// <summary>
        /// NotifyPropertyChanged事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        private Action<PropertyChangedEventArgs> RaisePropertyChanged()
        {
            return args => PropertyChanged?.Invoke(this, args);
        }
        /*
        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        */
    }
}
