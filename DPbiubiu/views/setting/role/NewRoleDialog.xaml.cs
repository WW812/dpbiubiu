using biubiu.Domain.biuMessageBox;
using biubiu.model;
using biubiu.model.role;
using MaterialDesignThemes.Wpf;
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
    /// SampleDialog.xaml 的交互逻辑
    /// </summary>
    public partial class NewRoleDialog : UserControl
    {
        public RolePage RolePageObject;
        //public List<Permission> RoleOfPermissions = new List<Permission>();

        public NewRoleDialog()
        {
            InitializeComponent();
        }

        public async void NewRole()
        {
            /*
            NewRoleBtn.IsEnabled = false;
            var RoleParam = new Role { Name = RoleName.Text, Note = RoleNote.Text };
            try
            {
                var Result = await ModelHelper.ApiClient.NewRoleAsync(RoleParam);
                if (Result.Code != 200)
                {
                    throw new Exception(Result.ToString());
                }
                else
                {
                    RolePageObject.GetRoles();
                    NewRoleBtn.Command = MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand;
                    NewRoleBtn.Command.Execute(false);
                }
            }
            catch (Exception er)
            {
                BiuMessageBoxWindows.BiuShow(er.Message, image: BiuMessageBoxImage.Error);
                NewRoleBtn.IsEnabled = true;
            }
            */
            NewRoleBtn.IsEnabled = false;
            await ModelHelper.GetInstance().GetApiDataArg(ModelHelper.ApiClient.NewRoleAsync,
               new { Name = RoleName.Text, Note = RoleNote.Text },
               delegate(DataInfo<Role> result) {
                   Application.Current.Dispatcher.Invoke(new Action(()=> {
                       RolePageObject.GetRoles();
                       MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(false,this);
                   }));
               });
                NewRoleBtn.IsEnabled = true;
        }

        private void NewRoleBtn_Click(object sender, RoutedEventArgs e)
        {
            if (grid.BindingGroup.CommitEdit())
                NewRole();
        }

        /// <summary>
        /// 获取权限列表
        /// </summary>
        /*
        private async void GetPermissions()
        {
            var Client = HttpApiFactory.Create<IApi>();
            try
            {
                var Result = await Client.GetPermissionsAsync();
                if (Result.Code != 200)
                {
                    throw new Exception(Result.ToString());
                }
                else
                {
                    Result.Data.ForEach(delegate (Permission permission)
                    {
                        CheckBox cb = new CheckBox
                        {
                            Margin = new Thickness(16, 4, 16, 0),
                            Content = permission.Name,
                            Tag = permission
                        };
                        cb.SetValue(CheckBox.StyleProperty, Application.Current.Resources["MaterialDesignUserForegroundCheckBox"]);
                        cb.Click += new RoutedEventHandler(PermissionCheck_Click);
                        PermissionStackPanel.Children.Add(cb);
                    });
                }
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message);
            }
        }

        private void PermissionCheck_Click(object sender, RoutedEventArgs e)
        {
            var check = sender as CheckBox;
            if (check.IsChecked == true)
            {
                RoleOfPermissions.Add(check.Tag as Permission);
            }
            else
            {
                RoleOfPermissions.Remove(check.Tag as Permission); 
            }
        }
        */

        #region
        /// <summary>
        /// 数据校验发生错误时的处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /*
        private void grid_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added)
            {
                //Console.WriteLine(e.Error.ErrorContent.ToString());
            }
        }
        */
        #endregion

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //GetPermissions();
        }
    }
}
