using biubiu.model.goods.stock_goods;
using biubiu.view_model.stock_goods;
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

namespace biubiu.views.marketing.stock_goods
{
    /// <summary>
    /// StockGoodsPage.xaml 的交互逻辑
    /// </summary>
    public partial class StockGoodsPage : UserControl
    {
        public StockGoodsPage()
        {
            InitializeComponent();
            DataContext = new StockGoodsViewModel();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            (DataContext as StockGoodsViewModel).GetStockGoodsItems();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var datacontext = DataContext as StockGoodsViewModel;
            var btn = sender as Button;
            datacontext.ExecuteChangeValidCommand(btn.CommandParameter as StockGoods);
        }
    }
}
