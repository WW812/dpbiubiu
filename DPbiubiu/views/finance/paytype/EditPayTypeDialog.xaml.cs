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
    /// EditPayTypeDialog.xaml 的交互逻辑
    /// </summary>
    public partial class EditPayTypeDialog : UserControl
    {
        public PayType pt;
        public EditPayTypeDialog()
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
            else
            {
                await ModelHelper.GetInstance().GetApiDataArg(
                       ModelHelper.ApiClient.ChangePayTypeAsync,
                       new
                       {
                           ID = pt.ID,
                           Name = NameTextBox.Text,
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

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            NameTextBox.Text = pt.Name;
            MoneyTextBox.Text = pt.Money.ToString();
            NoteTextBox.Text = pt.Note;
        }
    }
}
