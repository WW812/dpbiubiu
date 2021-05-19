using biubiu.Domain;
using biubiu.Domain.biuMessageBox;
using biubiu.model;
using biubiu.model.customer.ship_customer;
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

namespace biubiu.views.finance.accept
{
    /// <summary>
    /// EditAcceptDialog.xaml 的交互逻辑
    /// </summary>
    public partial class EditAcceptDialog : UserControl
    {
        public ShipCustomerMoney AcceptObject;
        public EditAcceptDialog()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            NumTextBox.Text = AcceptObject.HonourNum;
            AcceptDate.SelectedDate = Common.TimeStamp2DateTime(AcceptObject.HonourTime ?? 0);
            foreach (var item in StatusComboBox.Items)
            {
                if ((item as ComboBoxItem).Tag.ToString().Equals(AcceptObject.HonourStatus.ToString()))
                    StatusComboBox.SelectedItem = item;
            }
            NoteTextBox.Text = AcceptObject.Note;
        }

        private async void SubmitBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(NumTextBox.Text))
                {
                    BiuMessageBoxWindows.BiuShow("承兑编号不可为空!");
                    return;
                }
                if (AcceptDate.SelectedDate == null)
                {
                    BiuMessageBoxWindows.BiuShow("请选择承兑到期日!");
                    return;
                }
                    await ModelHelper.GetInstance().GetApiDataArg(
                         ModelHelper.ApiClient.EditAcceptAsync,
                         new
                         {
                             ID = AcceptObject.ID,
                             HonourNum = NumTextBox.Text,
                             HonourTime = Common.DateTime2TimeStamp(AcceptDate.SelectedDate.Value),
                             HonourStatus = System.Convert.ToInt32((StatusComboBox.SelectedItem as ComboBoxItem).Tag.ToString()),
                             Note = NoteTextBox.Text
                         },
                         delegate(DataInfo<object> result) {
                             Application.Current.Dispatcher.Invoke(new Action(()=> {
                                 MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(true,this);
                             }));
                         });
            }catch(Exception er)
            {
                BiuMessageBoxWindows.BiuShow(er.Message);
            }
        }
    }
}
