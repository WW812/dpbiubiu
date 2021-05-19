using biubiu.Domain;
using biubiu.Domain.biuMessageBox;
using biubiu.model;
using biubiu.model.paytype;
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

namespace biubiu.views.finance.paytype
{
    /// <summary>
    /// AddPayTypeDialog.xaml 的交互逻辑
    /// </summary>
    public partial class AddPayTypeDialog : UserControl
    {
        public AddPayTypeDialog()
        {
            InitializeComponent();
        }

        private async void SubmitBtn_Click(object sender, RoutedEventArgs e)
        {
            SubmitBtn.IsEnabled = false;
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                BiuMessageBoxWindows.BiuShow("账户名称不可为空!");
            }
            else if (!RegexChecksum.IsReal(MoneyTextBox.Text))
            {
                BiuMessageBoxWindows.BiuShow("金额请输入数字!");
            }
            else
            {
                await ModelHelper.GetInstance().GetApiDataArg(
                       ModelHelper.ApiClient.AddPayTypeAsync,
                       new
                       {
                           Name = NameTextBox.Text,
                           Money = System.Convert.ToDouble(MoneyTextBox.Text),
                           Note = NoteTextBox.Text
                       },
                       delegate (DataInfo<PayType> result) {
                           Application.Current.Dispatcher.Invoke(new Action(() => {
                               MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(true, this);
                           }));
                       });
            }
            SubmitBtn.IsEnabled = true;
        }
    }
}
