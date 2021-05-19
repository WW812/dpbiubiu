using biubiu.Domain.biuMessageBox;
using biubiu.Domain.pages;
using biubiu.model;
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

namespace biubiu.views.setting.role
{
    /// <summary>
    /// ChangePasswordDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ChangePasswordDialog : UserControl
    {
        public ChangePasswordDialog()
        {
            InitializeComponent();
        }

        private async void Submit_Click(object sender, RoutedEventArgs e)
        {
            if (OldPassWordText.Password.Equals("") || NewPassWordText.Password.Equals("") || ConformPassWordText.Password.Equals(""))
            {
                BiuMessageBoxWindows.BiuShow("原密码、新密码和确认密码不可为空！");
                return;
            }
            if (!NewPassWordText.Password.Equals(ConformPassWordText.Password))
            {
                BiuMessageBoxWindows.BiuShow("新密码和确认密码输入不一致!");
                return;
            }
            SubmitBtn.IsEnabled = false;
            await ModelHelper.GetInstance().GetApiDataArg(
               ModelHelper.ApiClient.ChangePasswordAsync,
               new { password = NewPassWordText.Password, oldPassword = OldPassWordText.Password },
               delegate (DataInfo<object> result)
               {
                   Dispatcher.Invoke(new Action(() =>
                   {
                       MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(true, this);
                   }));
               });
            SubmitBtn.IsEnabled = true;
        }
    }
}
