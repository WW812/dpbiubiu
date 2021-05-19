using biubiu.model.user;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace biubiu.view_model.user
{
    public class UserGridViewModel : INotifyPropertyChanged
    {
        private readonly ObservableCollection<UserViewModel> _userItems;

        public UserGridViewModel(List<User> userList)
        {
            _userItems = CreateData(userList);
        }

        private static ObservableCollection<UserViewModel> CreateData(List<User> userList)
        {
            ObservableCollection<UserViewModel> observableUserViewModel = new ObservableCollection<UserViewModel>();
            userList.ForEach(delegate (User user)
            {
                //observableUserViewModel.Add(new UserViewModel { ID = user.ID, UserName = user.UserName, NickName = user.NickName, Note = user.Note });
                observableUserViewModel.Add(user.ToUserViewModel());
            });
            return observableUserViewModel;
        }

        public ObservableCollection<UserViewModel> UserItems => _userItems;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
