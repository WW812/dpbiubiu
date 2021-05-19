using biubiu.Domain;
using biubiu.Domain.biuMessageBox;
using biubiu.model;
using biubiu.model.paytype;
using biubiu.view_model.paytype;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// PayTypePage.xaml 的交互逻辑
    /// </summary>
    public partial class PayTypePage : Page
    {
        public PayTypePage()
        {
            InitializeComponent();
            DataContext = new PayTypeViewModel();
        }


        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            /*
            SaveBtn.IsEnabled = false;
            if (RegexChecksum.IsReal(MoneyTextBox.Text))
            {
                if (PayTypeDataGrid.SelectedItem == null) //新增保存
                {
                    await ModelHelper.GetInstance().GetApiDataArg(
                        ModelHelper.ApiClient.AddPayTypeAsync,
                        new
                        {
                            Name = NameTextBox.Text,
                            Money = System.Convert.ToDouble(MoneyTextBox.Text),
                            Note = NoteTextBox.Text
                        });
                }
                else //修改保存
                {
                    if (PayTypeDataGrid.SelectedItem is PayType pt)
                    {
                        await ModelHelper.GetInstance().GetApiDataArg(
                            ModelHelper.ApiClient.ChangePayTypeAsync,
                            new
                            {
                                ID = pt.ID,
                                Name = NameTextBox.Text,
                                Note = NoteTextBox.Text
                            });
                    }
                }
                (DataContext as PayTypeViewModel).CurrentPage.Reset(14);
                Thread.Sleep(80);
                (DataContext as PayTypeViewModel).GetPayType();
            }
            else
            {
                BiuMessageBoxWindows.BiuShow("金额请输入数字!");
            }
            ClearInput();
            SaveBtn.IsEnabled = true;
            SaveBtn.Content = "保存新增";
            MoneyTextBox.IsEnabled = true;
            */
        }

        /// <summary>
        /// 新建账户
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            /*
            ClearInput();
            PayTypeDataGrid.SelectedItem = null;
            MoneyTextBox.IsEnabled = true;
            SaveBtn.Content = "保存新增";
            */
            var view = new AddPayTypeDialog();
            var result = await DialogHost.Show(view, "RootDialog", ClosingMoneyEventHandler);
        }

        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /*
            if (!(PayTypeDataGrid.SelectedItem is PayType pt)) return;
            NameTextBox.Text = pt.Name;
            MoneyTextBox.Text = pt.Money.ToString();
            NoteTextBox.Text = pt.Note;
            MoneyTextBox.IsEnabled = false;
            SaveBtn.Content = "保存修改";
            */
        }

        private void ClearInput()
        {
            /*
            NameTextBox.Text = "";
            MoneyTextBox.Text = "";
            NoteTextBox.Text = "";
            */
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ReGetPayType();
        }

        private async void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            if (PayTypeDataGrid.SelectedItem == null)
            {
                SnackbarViewModel.GetInstance().PoupMessageAsync("请先选择一个账户!");
            }
            else
            {
                var view = new EditPayTypeDialog {
                    pt = PayTypeDataGrid.SelectedItem as PayType
                };
                var result = await DialogHost.Show(view, "RootDialog", ClosingMoneyEventHandler);
            }
        }

        private void ClosingMoneyEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            ReGetPayType();
        }

        private void ReGetPayType()
        {
            (DataContext as PayTypeViewModel).CurrentPage.Reset(17);
            (DataContext as PayTypeViewModel).GetPayType();
        }
    }
}
