using biubiu.Domain;
using biubiu.model;
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

namespace biubiu.views.setting.print
{
    /// <summary>
    /// PrintOperateControl.xaml 的交互逻辑
    /// </summary>
    public partial class PrintOperateControl : UserControl
    {
        public PrintOperateControl()
        {
            InitializeComponent();
        }


        public BillConfig SelectedBillConfig
        {
            get { return (BillConfig)GetValue(SelectedBillConfigProperty); }
            set { SetValue(SelectedBillConfigProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedBillConfigProperty =
            DependencyProperty.Register("SelectedBillConfig", typeof(BillConfig), typeof(UserControl), new PropertyMetadata());


        private void Chip_Click(object sender, RoutedEventArgs e)
        {
            var chip = sender as MaterialDesignThemes.Wpf.Chip;
            var bc = chip.Tag as BillConfig;
            SelectedGoodsChips(bc);
        }

        private void SelectedGoodsChips(BillConfig bc)
        {
            for (int i = 0; i < TemplateItemsControl.Items.Count; i++)
            {
                var titem = TemplateItemsControl.ItemContainerGenerator.ContainerFromIndex(i);
                GroupBox gb = Common.FindSingleVisualChildren<GroupBox>(titem);
                var tItems = Common.FindSingleVisualChildren<ItemsControl>(gb);
                for (int j = 0; j < tItems.Items.Count; j++)
                {
                    var item = tItems.ItemContainerGenerator.ContainerFromIndex(j);
                    MaterialDesignThemes.Wpf.Chip c = Common.FindSingleVisualChildren<MaterialDesignThemes.Wpf.Chip>(item);
                    if (bc != null && bc.TemplatePath == (c.Tag as BillConfig).TemplatePath)
                    {
                        //选中状态
                        c.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Black"));
                        c.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("White"));

                        SelectedBillConfig = c.Tag as BillConfig;
                    }
                    else
                    {
                        c.Background = new SolidColorBrush(Color.FromRgb(232, 232, 232));
                        c.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Black"));
                    }
                }
            }
        }

    }
}
