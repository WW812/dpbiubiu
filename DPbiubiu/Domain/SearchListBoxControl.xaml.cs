using biubiu.Domain.biuMessageBox;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace biubiu.Domain
{
    /// <summary>
    /// SearchListBoxControl.xaml 的交互逻辑
    /// </summary>
    public partial class SearchListBoxControl : UserControl
    {

        public SearchListBoxControl()
        {
            InitializeComponent();
        }

        private void AccountTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            AccountPopup.IsOpen = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AccountPopup.IsOpen = true;
        }

        private void AccountTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            AccountPopup.IsOpen = false;
        }

        private void AccountListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /*
            try
            {
                if (AccountListBox.SelectedItem == null) return;
                AccountTextBox.Text = AccountListBox.SelectedItem.ToString();
                AccountPopup.IsOpen = false;
            }
            catch (Exception er)
            {
                BiuMessageBoxWindows.BiuShow(er.Message);
            }
            */
        }

        private void AccountTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!AccountListBox.Items.IsEmpty)
                AccountPopup.IsOpen = true;
        }

        private void AccountTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            /*
            if(e.Key == Key.Down)
            {
                if(!AccountListBox.Items.IsEmpty)
                {
                    AccountPopup.IsOpen = true;
                    AccountListBox.SelectedIndex = 0;
                }
            }
            */
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            AccountPopup.Width = Width;
            AccountTextBox.Width = Width;
            AccountListBox.Width = Width;
            AccountTextBox.FontSize = FontSize;
            AccountListBox.FontSize = FontSize;
        }
    }
}
