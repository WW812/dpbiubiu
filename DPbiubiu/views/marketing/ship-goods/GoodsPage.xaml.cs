using biubiu.Domain;
using biubiu.Domain.biuMessageBox;
using biubiu.Domain.pages;
using biubiu.model;
using biubiu.model.ship_goods;
using biubiu.view_model.goods;
using biubiu.view_model.ship_goods;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// GoodsPage.xaml 的交互逻辑
    /// </summary>
    public partial class GoodsPage : UserControl
    {
        public GoodsPage()
        {
            InitializeComponent();
            //DataContext = new GoodsPageViewModel(this);
            DataContext = new ShipGoodsViewModel();
        }

        public void RefreshGoods()
        {
            /*
            try
            {
                var result = await Client.GetGoodsAsync();
                if (result.Code != 200)
                {
                    throw new Exception(result.ToString());
                }
                else
                {
                    MainArea.Children.Clear();
                    result.Data.ForEach(delegate (ShipGoods goods)
                    {
                        MaterialDesignThemes.Wpf.Card card = new MaterialDesignThemes.Wpf.Card()
                        {
                            Width = 200,
                            Padding = new Thickness(8),
                        };
                        card.SetValue(MaterialDesignThemes.Wpf.Card.BackgroundProperty, Application.Current.Resources["PrimaryHueDarkBrush"]);
                        card.SetValue(MaterialDesignThemes.Wpf.Card.ForegroundProperty, Application.Current.Resources["PrimaryHueDarkForegroundBrush"]);
                        StackPanel sp = new StackPanel();
                        sp.Children.Add(new TextBlock()
                        {
                            Text = goods.Name,
                            FontSize = 16,
                        });
                        sp.Children.Add(new TextBlock()
                        {
                            Text = goods.Price.ToString() + "元 (标准价格)",
                            FontSize = 14,
                            Margin = new Thickness(16, 4, 16, 0)
                        });
                        sp.Children.Add(new TextBlock()
                        {
                            Text = goods.RealPrice.ToString() + "元 (执行价格)",
                            FontSize = 14,
                            Margin = new Thickness(16, 4, 16, 0)
                        });
                        sp.Children.Add(new TextBlock()
                        {
                            Text = "备注",
                            FontSize = 16,
                        });
                        sp.Children.Add(new TextBlock()
                        {
                            Text = goods.Note,
                            Margin = new Thickness(16, 4, 16, 0)
                        });
                        Separator sep = new Separator();
                        sep.SetValue(Separator.StyleProperty, Application.Current.Resources["MaterialDesignLightSeparator"]);
                        sp.Children.Add(sep);
                        StackPanel sp2 = new StackPanel()
                        {
                            Margin = new Thickness(8, 0, 8, 8),
                            Orientation = Orientation.Horizontal,
                            HorizontalAlignment = HorizontalAlignment.Right,
                        };
                        ToggleButton togBtn = new ToggleButton()
                        {
                            ToolTip = "料品启用开关",
                            IsChecked = goods.Valid == 1,
                            Tag = goods.ID,
                        };
                        togBtn.SetValue(ToggleButton.StyleProperty, Application.Current.Resources["MaterialDesignSwitchLightToggleButton"]);
                        togBtn.Click += new RoutedEventHandler(ToggleButton_Click);
                        sp2.Children.Add(togBtn);
                        sp.Children.Add(sp2);
                        card.Content = sp;
                        MainArea.Children.Add(card);
                    });
                }
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message);
            }
            */
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e) => SetGoodsValid(sender as ToggleButton);

        private async void SetGoodsValid(ToggleButton tb)
        {
            tb.IsEnabled = false;  //数据传输时防止多次点击
            var goodsParam = new
            {
                ID = tb.Tag.ToString(),
                Valid = (tb.IsChecked ?? false) ? 1 : 0,
            };
            /*
            try
            {
                var result = await ModelHelper.ApiClient.ChangeGoodsValidAsync(goodsParam);
                if (result.Code != 200)
                {
                    throw new Exception(result.ToString());
                }
                else
                {
                    var str = result.Data.Name + " 已";
                    str += (tb.IsChecked ?? false) ? "启用" : "禁用";
                    SnackbarViewModel.GetInstance().PoupMessageAsync(str);
                }
            }
            catch (Exception er)
            {
                BiuMessageBoxWindows.BiuShow(er.Message,image:BiuMessageBoxImage.Error);
            }
            */
            await ModelHelper.GetInstance().GetApiDataArg(
                ModelHelper.ApiClient.ChangeGoodsValidAsync,
                goodsParam,
                delegate (DataInfo<ShipGoods> result)
                {
                    var str = result.Data.Name + " 已";
                    str += (tb.IsChecked ?? false) ? "启用" : "禁用";
                    SnackbarViewModel.GetInstance().PoupMessageAsync(str);
                });
            tb.IsEnabled = true;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //RefreshGoods();
            (DataContext as ShipGoodsViewModel).GetShipGoodsItems();
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender == null) return;
            var btn = sender as Button;
            if (btn.Tag is ShipGoods goods)
            {
                goods.IsEditing = true;
                goods.EditPrice = goods.Price;
                goods.EditRealPrice = goods.RealPrice;
                goods.EditNote = goods.Note;
            }
        }

        /// <summary>
        /// 取消保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (sender == null) return;
            var btn = sender as Button;
            if (btn.Tag is ShipGoods goods)
            {
                goods.IsEditing = false;
            }
        }

        /// <summary>
        /// 保存修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (sender == null) return;
            var btn = sender as Button;
            btn.IsEnabled = false;
            if (btn.Tag is ShipGoods goods) {
                await ModelHelper.GetInstance().GetApiDataArg(
                    ModelHelper.ApiClient.ChangeSingleGoodsAsync,
                    new { ID = goods.ID, Price = goods.EditPrice, RealPrice = goods.EditRealPrice, Note= goods.EditNote},
                    (DataInfo<ShipGoods> success)=> {
                        SnackbarViewModel.GetInstance().PoupMessageAsync("保存成功!");
                        goods.Price = success.Data.Price;
                        goods.RealPrice = success.Data.RealPrice;
                        goods.Note = success.Data.Note;
                        goods.IsEditing = false;
                    },
                    (DataInfo<ShipGoods> faild)=> {
                        SnackbarViewModel.GetInstance().PoupMessageAsync("保存失败, 请稍后重试!");
                    });
            }
            else
            {
                BiuMessageBoxWindows.BiuShow("未获得料品，请关闭重试!",image:BiuMessageBoxImage.Error);
            }
            btn.IsEnabled = true;
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            if (e.Text != "." || textBox.Text.Contains("."))
            {
                e.Handled = !RegexChecksum.IsNonnegativeReal(e.Text);
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender != null)
            {
                var textBox = sender as TextBox;
                textBox.SelectAll();
                textBox.PreviewMouseDown -= new MouseButtonEventHandler(TextBox_PreviewMouseDown);
            }
        }

        private void TextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender != null)
            {
                (sender as TextBox).Focus();
                e.Handled = true;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (string.IsNullOrWhiteSpace(textBox.Text)) textBox.Text = "0";
            textBox.PreviewMouseDown += new MouseButtonEventHandler(TextBox_PreviewMouseDown);
        }
    }
}
