using biubiu.view_model.customer.stock_customer;
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

namespace biubiu.views.marketing.customer.stock_customer
{
    /// <summary>
    /// CreateStockCustomerDialog.xaml 的交互逻辑
    /// </summary>
    public partial class CreateStockCustomerDialog : UserControl
    {
        public CreateStockCustomerDialog()
        {
            InitializeComponent();
            DataContext = new CreateStockCustomerViewModel();
        }

        private void CreateShipCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            if (grid.BindingGroup.CommitEdit())
                (DataContext as CreateStockCustomerViewModel).SubmitCommand.Execute(this);
        }
    }
}
