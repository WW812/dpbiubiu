using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
using System.Windows.Threading;

namespace biubiu.views.Loading
{
    /// <summary>
    /// Interaction logic for LoadingDialog.xaml
    /// Loading遮罩
    /// </summary>
    public partial class LoadingDialog : UserControl
    {
        private static LoadingDialog _loading = null;
        private static readonly string _message = "加载中，请稍后...";

        private LoadingDialog()
        {
            InitializeComponent();
        }


        public static void Show(string mes = null)
        {
            App.Current.Dispatcher.Invoke(new Action(() =>
            {
                try
                {
                    if (_loading is null) _loading = new LoadingDialog();
                    _loading.Text_Message.Text = string.IsNullOrEmpty(mes) ? _message : mes;
                    DialogHost.Show(_loading, "RootDialog");
                }
                catch (Exception) { }
            }));
        }

        public static void Close()
        {
            Thread.Sleep(200);
            App.Current?.Dispatcher.Invoke(new Action(() =>
            {
                try
                {
                    if(_loading != null) 
                        MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(false, _loading);
                }
                catch (Exception) { }
            }));
        }

    }
}
