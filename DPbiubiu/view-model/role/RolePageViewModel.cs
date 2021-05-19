using biubiu.Domain;
using biubiu.Domain.biuMessageBox;
using biubiu.model.role;
using biubiu.model.user;
using biubiu.view_model.user;
using biubiu.views.setting.role;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WebApiClient;

namespace biubiu.view_model.role
{
    public class RolePageViewModel : INotifyPropertyChanged
    {
        //private ObservableCollection<PermissionViewModel> _permissionItems;
        public RolePage RolePageObject { get; set; }

        public RolePageViewModel()
        {
        }

        public RolePageViewModel(RolePage rp)
        {
            RolePageObject = rp;
        }

        public Role CurrentRole => RolePageObject.RolesListView.SelectedItem as Role;

        public ICommand RunDialogCommand => new AnotherCommandImplementation(ExecuteRunDialog);
        public ICommand RunNewUserDialogCommand => new AnotherCommandImplementation(ExecuteRunNewUserDialog);
        public ICommand RunEditUserDialogCommand => new AnotherCommandImplementation(ExecuteRunEditUserDialog);

        private async void ExecuteRunDialog(object o)
        {
            //let's set up a little MVVM, cos that's what the cool kids are doing:
            var view = new NewRoleDialog
            {
                RolePageObject = RolePageObject,
                DataContext = new NewRoleDialogViewModel()
            };

            //show the dialog
            var result = await DialogHost.Show(view, "RootDialog", ClosingEventHandler);


            //check the result...
        }

        private async void ExecuteRunNewUserDialog(object o)
        {
            if (o == null)
            {
                BiuMessageBoxWindows.BiuShow("请先选择角色",image:BiuMessageBoxImage.Warning);
                return;
            }
            //let's set up a little MVVM, cos that's what the cool kids are doing:
            var view = new NewUserDialog()
            {
                UserOfRole = (o as Role),
                RolePageObject = RolePageObject,
                DataContext = new UserViewModel()
            };

            //show the dialog
            var result = await DialogHost.Show(view, "RootDialog", ClosingEventHandler);
        }

        private async void ExecuteRunEditUserDialog(object o)
        {
            if (o == null)
            {
                BiuMessageBoxWindows.BiuShow("参数错误，传入值为null!",image:BiuMessageBoxImage.Error);
                return;
            }
            var view = new EditUserDialog
            {
                RolePageObject = RolePageObject,
                DataContext = (o as UserViewModel),
            };

            var result = await DialogHost.Show(view, "RootDialog", ClosingEventHandler);
        }

        /// <summary>
        /// 关闭窗口，可以刷新列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void ClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            Console.WriteLine("You can intercept the closing event, and cancel here.");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
