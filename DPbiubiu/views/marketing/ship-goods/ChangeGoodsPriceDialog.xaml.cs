using biubiu.Domain;
using biubiu.Domain.biuMessageBox;
using biubiu.Domain.pages;
using biubiu.model;
using biubiu.model.ship_goods;
using biubiu.view_model.goods;
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

namespace biubiu.views.marketing.goods
{
    /// <summary>
    /// ChangeGoodsPriceDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ChangeGoodsPriceDialog : UserControl
    {
        //public GoodsPage GoodsPageObject;
        public string TitleStr; //参数为1是涨价，-1是降价
        public List<String> ChangeGoodsList = new List<string>(); //需要修改的料品集合

        private bool _chkAll = true; // 取消点选单个时，取消全选checkbox的选中状态

        public ChangeGoodsPriceDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 拉取料品
        /// </summary>
        /*
        private async void GetGoods()
        {
            var Client = HttpApiFactory.Create<IApi>();
            try
            {
                var result = await Client.GetGoodsAsync();
                if (result.Code != 200)
                {
                    throw new Exception(result.ToString());
                }
                else
                {
                    GoodsList.Children.Clear();
                    result.Data.ForEach(delegate (ShipGoods goods)
                    {
                        CheckBox cb = new CheckBox()
                        {
                            Margin = new Thickness(16, 4, 16, 0),
                            Content = goods.Name,
                            Tag = goods.ID,
                            FontSize = 18
                        };
                        cb.Click += new RoutedEventHandler(Goods_Click);
                        cb.SetValue(CheckBox.StyleProperty, Application.Current.Resources["MaterialDesignUserForegroundCheckBox"]);
                        GoodsList.Children.Add(cb);
                    });
                }
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message);
            }
            Client.Dispose();
        }
        */

        /// <summary>
        /// 料品checkbox 点击处理函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Goods_Click(object sender, RoutedEventArgs e)
        {
            var cb = sender as CheckBox;
            var dataContext = DataContext as ChangeGoodsPriceViewModel;
            if (cb.IsChecked ?? false)
            {
                ChangeGoodsList.Add(cb.Tag.ToString());
                if (ChangeGoodsList.Count == dataContext.GoodsItems.Count)
                {
                    SelectedAllCheckBox.IsChecked = true;
                }
            }
            else
            {
                ChangeGoodsList.Remove(cb.Tag.ToString());
                if (SelectedAllCheckBox.IsChecked == true)
                {
                    _chkAll = false;
                    SelectedAllCheckBox.IsChecked = false;
                }
            }
        }

        /// <summary>
        /// 界面载入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            TitleTxt.Text = TitleStr;
            //GetGoods();
        }

        /// <summary>
        /// 提交按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            SubmitFunctionAsync();
        }

        /// <summary>
        /// 提交处理函数
        /// </summary>
        private async void SubmitFunctionAsync()
        {
            SubmitButton.IsEnabled = false;
            var priceStr = PriceChangeRangeTextBox.Text;
            var realPriceStr = RealPriceChangeRangeTextBox.Text;
            if (TitleStr == "料品降价")
            {
                priceStr = "-" + priceStr;
                realPriceStr = "-" + realPriceStr;
            }
            var param = new
            {
                price = Double.Parse(priceStr),
                realPrice = Double.Parse(realPriceStr),
                ids = ChangeGoodsList
            };
            /*
            try
            {
                var result = await ModelHelper.ApiClient.ChangeGoodsPriceAsync(param);
                if (result.Code != 200)
                {
                    throw new Exception(result.ToString());
                }
                else
                {
                    //GoodsPageObject.RefreshGoods();
                    MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(false, this);
                }
            }
            catch (Exception er)
            {
                var msg = "";
                if (er.InnerException != null)
                    msg += er.InnerException.Message + "\n";
                msg += er.Message;
                BiuMessageBoxWindows.BiuShow(msg, image: BiuMessageBoxImage.Error);
                SubmitButton.IsEnabled = true;
            }
            */
            await ModelHelper.GetInstance().GetApiDataArg(ModelHelper.ApiClient.ChangeGoodsPriceAsync,
                param,delegate(DataInfo<object> result) {
                    Application.Current.Dispatcher.Invoke(new Action(()=> {
                        MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(false, this);
                    }));
                });
            SubmitButton.IsEnabled = true;
        }

        /// <summary>
        /// 输入框失去焦点时如果内容为空 置零
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (string.IsNullOrWhiteSpace(textBox.Text)) textBox.Text = "0";
            textBox.PreviewMouseDown += new MouseButtonEventHandler(TextBox_PreviewMouseDown);
        }

        /// <summary>
        /// 实现点击TextBox 选中文字
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender != null)
            {
                var textBox = sender as TextBox;
                textBox.SelectAll();
                textBox.PreviewMouseDown -= new MouseButtonEventHandler(TextBox_PreviewMouseDown);
            }
        }

        /// <summary>
        /// 实现点击TextBox 选中文字
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender != null)
            {
                (sender as TextBox).Focus();
                e.Handled = true;
            }
        }

        private void PriceChangeRangeTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            if (e.Text != "." || textBox.Text.Contains("."))
            {
                e.Handled = !RegexChecksum.IsNonnegativeReal(e.Text);
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (_chkAll)
                SetGoodsAllCheckBox(true);
            _chkAll = true;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_chkAll)
                SetGoodsAllCheckBox(false);
            _chkAll = true;
        }

        private void SetGoodsAllCheckBox(bool check)
        {
            var dataContext = DataContext as ChangeGoodsPriceViewModel;
            try
            {
                for (int i = 0; i < dataContext.GoodsItems.Count; i++)
                {
                    var item = GoodsItemsControl.ItemContainerGenerator.ContainerFromIndex(i);
                    CheckBox cb = Common.FindSingleVisualChildren<CheckBox>(item);
                    cb.IsChecked = check;
                    if (check)
                    {
                        ChangeGoodsList.Add(cb.Tag.ToString());
                    }
                    else
                    {
                        ChangeGoodsList.Clear();
                    }
                }
            }
            catch (Exception er)
            {
                BiuMessageBoxWindows.BiuShow(er.Message,image:BiuMessageBoxImage.Error);
            }
        }
    }
}
