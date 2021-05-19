using biubiu.view_model.customer.ship_customer;
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

namespace biubiu.views.marketing.customer.ship_customer
{
    /// <summary>
    /// CreateShipCustomerDialog.xaml 的交互逻辑
    /// </summary>
    public partial class CreateShipCustomerDialog : UserControl
    {
        public CreateShipCustomerDialog()
        {
            InitializeComponent();
            DataContext = new CreateShipCustomerViewModel();
        }

        private void CreateShipCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            if (grid.BindingGroup.CommitEdit())
                (DataContext as CreateShipCustomerViewModel).SubmitCommand.Execute(this);
        }
    }
}
