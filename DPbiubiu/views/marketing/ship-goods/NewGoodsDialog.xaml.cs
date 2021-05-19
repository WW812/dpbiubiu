using biubiu.Domain;
using biubiu.Domain.biuMessageBox;
using biubiu.model;
using biubiu.model.ship_goods;
using biubiu.view_model.goods;
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

namespace biubiu.views.marketing.goods
{
    /// <summary>
    /// NewGoodsDialog.xaml 的交互逻辑
    /// </summary>
    public partial class NewGoodsDialog : UserControl
    {
        private List<ShipGoods> GoodsByCompositionComboBoxItemsSource = new List<ShipGoods>();

        public NewGoodsDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 为料品添加组合料品成分
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            /*
            var rate = RateTextBox.Text;
            ShipGoods selectedGoods = GoodsByCompositionComboBox.SelectedItem as ShipGoods;
            try
            {
                if (selectedGoods != null)
                {
                    if (!RegexChecksum.IsPositiveReal(rate) || double.Parse(rate) < 0 || double.Parse(rate) > 100)
                    {
                        throw new Exception("占比请输入0~100的数字");
                    }
                    else
                    {
                        var goodsComposition = new GoodsComposition
                        {
                            Goods = new ShipGoods() { ID = selectedGoods.ID, Name = selectedGoods.Name, Composition = new List<GoodsComposition>() },
                            Rate = double.Parse(rate)
                        };
            CompositionListViewItemsSource.Add(goodsComposition);
            CompositionListView.Items.Refresh();
            goodsCompositions.Add(goodsComposition);
            MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(false, AddCompositionDialog);
                        GoodsByCompositionComboBoxItemsSource.Remove(selectedGoods);
            GoodsByCompositionComboBox.Items.Refresh();
        }
                }
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message);
            }
            */
        }

        /// <summary>
        /// 从料品组合元素的listview中去除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveGoodsComposition_Click(object sender, RoutedEventArgs e)
        {
            /*
            var removeGoodsComposition = (sender as Button).CommandParameter as GoodsComposition;
            goodsCompositions.Remove(removeGoodsComposition);
            CompositionListViewItemsSource.Remove(removeGoodsComposition);
            CompositionListView.Items.Refresh();
            GoodsByCompositionComboBoxItemsSource.Add(removeGoodsComposition.Goods);
            GoodsByCompositionComboBox.Items.Refresh();
            */
        }

        /// <summary>
        /// 创建料品按钮点击函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateGoodsBtn_Click(object sender, RoutedEventArgs e)
        {
            //if (grid.BindingGroup.CommitEdit())
            //{
            if (!RegexChecksum.IsNonnegativeReal(GoodsPrice.Text) || !RegexChecksum.IsNonnegativeReal(GoodsRealPrice.Text))
            {
                BiuMessageBoxWindows.BiuShow("料品价格或执行价格填写不正确！",image:BiuMessageBoxImage.Warning);
                return;
            }
                CreateGoods();
            //}
        }

        /// <summary>
        /// 创建料品
        /// </summary>
        private async void CreateGoods()
        {
            CreateGoodsButton.IsEnabled = false;
            //var g = DataContext as GoodsViewModel;
            var goodsParam = new ShipGoods
            {
                Name = GoodsName.Text,
                Price = Convert.ToDouble(GoodsPrice.Text),
                RealPrice = Convert.ToDouble(GoodsRealPrice.Text),
                Note = RoleNote.Text,
                Valid = 1,
            };

            /*
            try
            {
                var result = await ModelHelper.ApiClient.CreateGoodsAsync(goodsParam);
                if (result.Code != 200)
                {
                    throw new Exception(result.ToString());
                }
                else
                {
                    MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(false, this);
                }
            }
            catch (Exception er)
            {
                BiuMessageBoxWindows.BiuShow(er.Message, image: BiuMessageBoxImage.Error);
                CreateGoodsButton.IsEnabled = true;
            }
            */
            await ModelHelper.GetInstance().GetApiDataArg(ModelHelper.ApiClient.CreateGoodsAsync,
                goodsParam,
                delegate(DataInfo<ShipGoods> result) {
                    Application.Current.Dispatcher.Invoke(new Action(()=> {
                        MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(false, this);
                    }));
                });
                CreateGoodsButton.IsEnabled = true;
        }

        /// <summary>
        /// 获取可以当作元素的料品列表
        /// </summary>
        private async void GetGoodsByComposition()
        {
            try
            {
                var result = await ModelHelper.ApiClient.GetGoodsAsync();
                if (result.Code != 200)
                {
                    throw new Exception(result.ToString());
                }
                else
                {
                    /*
                    foreach (ShipGoods goods in result.Data)
                    {
                        if (goods.Composition == null)
                        {
                            GoodsByCompositionComboBoxItemsSource.Add(goods);
                        }
                    }
                    GoodsByCompositionComboBox.ItemsSource = GoodsByCompositionComboBoxItemsSource;
                    */
                }
            }
            catch (Exception er)
            {
                BiuMessageBoxWindows.BiuShow(er.Message, image: BiuMessageBoxImage.Error);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            GetGoodsByComposition();
        }

        private void GoodsPrice_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            if (e.Text != "." || textBox.Text.Contains("."))
            {
                e.Handled = !RegexChecksum.IsNonnegativeReal(e.Text);
            }
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

    }
}
