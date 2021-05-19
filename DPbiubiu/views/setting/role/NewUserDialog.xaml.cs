using biubiu.Domain.biuMessageBox;
using biubiu.model;
using biubiu.model.role;
using biubiu.model.user;
using biubiu.view_model.user;
using System;
using System.Collections.Generic;
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
    /// NewUserDialog.xaml 的交互逻辑
    /// </summary>
    public partial class NewUserDialog : UserControl
    {
        public Role UserOfRole;
        public RolePage RolePageObject;

        public NewUserDialog()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            RoleNameText.Text = UserOfRole.Name;
        }

        public async void NewUser()
        {
            /*
            NewUserBtn.IsEnabled = false;
            var userParam = new User
            {
                UserName = UserNameText.Text,
                PassWord = PassWord.Password,
                NickName = NickNameText.Text,
                RoleID = UserOfRole.ID,
                Note = NoteText.Text,
            };
            try
            {
                var Result = await ModelHelper.ApiClient.NewUserAsync(userParam);
                if (Result.Code != 200)
                {
                    throw new Exception(Result.ToString());
                }
                else
                {
                    RolePageObject.GetUsers(UserOfRole);
                    MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(false,this);
                }
            }
            catch (Exception er)
            {
                BiuMessageBoxWindows.BiuShow(er.Message, image: BiuMessageBoxImage.Error);
                NewUserBtn.IsEnabled = true;
            }
            */
            NewUserBtn.IsEnabled = false;
            await ModelHelper.GetInstance().GetApiDataArg(ModelHelper.ApiClient.NewUserAsync,
                new
                {
                    UserName = UserNameText.Text,
                    PassWord = PassWord.Password,
                    NickName = NickNameText.Text,
                    RoleID = UserOfRole.ID,
                    Note = NoteText.Text,
                },
                delegate (DataInfo<User> result)
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        RolePageObject.GetUsers(UserOfRole);
                        MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(false, this);
                    }));
                });
            NewUserBtn.IsEnabled = true;
        }

        private void NewUserBtn_Click(object sender, RoutedEventArgs e)
        {
            if (grid.BindingGroup.CommitEdit() && PassWordWaring.Text == "" && ConfirmPassWordWaring.Text == "")
                NewUser();

        }

        private void PassWord_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ValidatePassWord();
        }

        private void ConfirmPassWord_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ValidatePassWord();
        }

        /// <summary>
        /// 校验密码输入框和密码确认输入框的内容
        /// </summary>
        private void ValidatePassWord()
        {
            PassWordWaring.Text = string.IsNullOrWhiteSpace(PassWord.Password) ? "不可为空" : "";
            //ConfirmPassWordWaring.Text = string.IsNullOrWhiteSpace(ConfirmPassWord.Password) ? "不可为空" : "";
            ConfirmPassWordWaring.Text = (PassWord.Password != ConfirmPassWord.Password) ? "两次密码输入不一样" : "";
        }
    }
}