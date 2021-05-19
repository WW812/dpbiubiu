using biubiu.Domain.biuMessageBox;
using biubiu.model;
using biubiu.model.role;
using biubiu.model.user;
using biubiu.view_model.user;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WebApiClient;

namespace biubiu.views.setting.role
{
    /// <summary>
    /// EditUserDialog.xaml 的交互逻辑
    /// </summary>
    public partial class EditUserDialog : UserControl
    {
        public Role UserOfRole;
        public RolePage RolePageObject;

        public EditUserDialog()
        {
            InitializeComponent();
        }

        public async void EditUser()
        {
            /*
            Submit.IsEnabled = false;
            var userParam = new User
            {
                ID = (DataContext as UserViewModel).ID,
                NickName = NickNameText.Text,
                Note = NoteText.Text,
                PassWord = string.IsNullOrWhiteSpace(PassWordText.Text) ? null : PassWordText.Text,
            };
            try
            {
                var Result = await ModelHelper.ApiClient.EditUserAsync(userParam);
                if (Result.Code != 200)
                {
                    throw new Exception(Result.ToString());
                }
                else
                {
                    RolePageObject.GetUsers(RolePageObject.RolesListView.SelectedItem as Role);
                    MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(false,this);
                }
            }
            catch (Exception er)
            {
                BiuMessageBoxWindows.BiuShow(er.Message, image: BiuMessageBoxImage.Error);
                Submit.IsEnabled = true;
            }
            */
            Submit.IsEnabled = false;
            await ModelHelper.GetInstance().GetApiDataArg(ModelHelper.ApiClient.EditUserAsync,
                new
                {
                    ID = (DataContext as UserViewModel).ID,
                    NickName = NickNameText.Text,
                    Note = NoteText.Text,
                    PassWord = string.IsNullOrWhiteSpace(PassWordText.Text) ? null : PassWordText.Text,
                },
                delegate (DataInfo<User> result)
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        RolePageObject.GetUsers(RolePageObject.RolesListView.SelectedItem as Role);
                        MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(false, this);
                    }));
                });
            Submit.IsEnabled = true;
        }

        private void SubmitBtn_Click(object sender, RoutedEventArgs e)
        {
            if (grid.BindingGroup.CommitEdit())
                EditUser();
        }
    }
}
