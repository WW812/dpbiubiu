using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace biubiu.Domain.biuMessageBox
{
    /// <summary>
    /// BiuMessageBoxWindows.xaml 的交互逻辑
    /// </summary>
    public partial class BiuMessageBoxWindows : Window
    {
        private readonly int _msgFontSizeUpper = 36;
        private readonly int _msgFontSizeLower = 18;
        private readonly int _fontSizeFeed = 3;  // 值越小，字体越大
        private BiuMessageBoxResult _result;
        private static readonly Dictionary<string, BiuMessageBoxButton> _pool = new Dictionary<string,BiuMessageBoxButton>();

        private BiuMessageBoxWindows()
        {
            InitializeComponent();
        }

        public static BiuMessageBoxResult BiuShow(string messageText, BiuMessageBoxButton button = BiuMessageBoxButton.OK, BiuMessageBoxImage image = BiuMessageBoxImage.Information, string caption = "提示", Window owner = null)
        {
            BiuMessageBoxResult r = BiuMessageBoxResult.None;
            try
            {
                if (messageText is null) messageText = string.Empty;
                var _poolKey = messageText + caption;
                if (_pool.ContainsKey(_poolKey)) return r;
                _pool.Add(_poolKey, button);

                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    var biuMsgBox = new BiuMessageBoxWindows
                    {
                        Title = caption
                    };
                    biuMsgBox.MessageText.Text = messageText;
                    biuMsgBox.MessageText.FontSize = biuMsgBox.GetMessageFontSizeByMessageLength(messageText.Length);
                    biuMsgBox.SetDisplayButton(button);
                    biuMsgBox.SetDisplayImage(image);
                    biuMsgBox.IBiuShow(owner, _poolKey);
                    r = biuMsgBox._result;
                }));
            }
            catch (Exception )
            {
                _pool?.Clear();
            }
            return r;
        }

        /// <summary>
        /// 根据字数获取最佳FontSize
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        private int GetMessageFontSizeByMessageLength(int count)
        {
            if (count < 6)
                return _msgFontSizeUpper;
            var s = _msgFontSizeUpper - count / _fontSizeFeed - count % _fontSizeFeed;
            return s < _msgFontSizeLower ? _msgFontSizeLower : s;
        }

        /// <summary>
        /// 根据BiuMessageBoxImage 设置显示图标
        /// </summary>
        /// <param name="image"></param>
        private void SetDisplayImage(BiuMessageBoxImage image)
        {
            switch (image)
            {
                case BiuMessageBoxImage.Error:
                    Icon = new BitmapImage(new Uri("./images/error.ico", UriKind.Relative));
                    MessageImage.Source = new BitmapImage(new Uri("/biubiu;component/Resources/error.png", UriKind.Relative));
                    break;
                case BiuMessageBoxImage.Question:
                    Icon = new BitmapImage(new Uri("./images/question.ico", UriKind.Relative));
                    MessageImage.Source = new BitmapImage(new Uri("/biubiu;component/Resources/question.png", UriKind.Relative));
                    break;
                case BiuMessageBoxImage.Warning:
                    Icon = new BitmapImage(new Uri("./images/warning.ico", UriKind.Relative));
                    MessageImage.Source = new BitmapImage(new Uri("/biubiu;component/Resources/warning.png", UriKind.Relative));
                    break;
                case BiuMessageBoxImage.Information:
                    Icon = new BitmapImage(new Uri("./images/information.ico", UriKind.Relative));
                    MessageImage.Source = new BitmapImage(new Uri("/biubiu;component/Resources/information.png", UriKind.Relative));
                    break;
                case BiuMessageBoxImage.None:
                default:
                    Icon = new BitmapImage(new Uri("./images/none.ico", UriKind.Relative));
                    MessageImage.Source = new BitmapImage(new Uri("/biubiu;component/Resources/none.png", UriKind.Relative));
                    break;
            }

        }

        /// <summary>
        /// 根据BiuMessageBoxButton 设置显示的按钮
        /// </summary>
        /// <param name="button"></param>
        private void SetDisplayButton(BiuMessageBoxButton button)
        {
            switch (button)
            {
                case BiuMessageBoxButton.OKCancel:
                    ButtonsStackPanel.Children.Add(IGetButton("确定", BiuMessageBoxResult.OK));
                    ButtonsStackPanel.Children.Add(IGetButton("取消", BiuMessageBoxResult.Cancel));
                    break;
                case BiuMessageBoxButton.YesNo:
                    ButtonsStackPanel.Children.Add(IGetButton("是", BiuMessageBoxResult.Yes));
                    ButtonsStackPanel.Children.Add(IGetButton("否", BiuMessageBoxResult.No));
                    break;
                case BiuMessageBoxButton.YesNoCancel:
                    ButtonsStackPanel.Children.Add(IGetButton("是", BiuMessageBoxResult.Yes));
                    ButtonsStackPanel.Children.Add(IGetButton("否", BiuMessageBoxResult.No));
                    ButtonsStackPanel.Children.Add(IGetButton("取消", BiuMessageBoxResult.Cancel));
                    break;
                default:
                    ButtonsStackPanel.Children.Add(IGetButton("确定", BiuMessageBoxResult.OK));
                    break;
            }
        }

        /// <summary>
        /// 创建一个按钮
        /// </summary>
        /// <param name="name"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private Button IGetButton(string content, BiuMessageBoxResult result)
        {
            Button btn = new Button
            {
                Content = content,
                Tag = result,
                Margin = new Thickness(4),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 18,
            };
            btn.Click += new RoutedEventHandler(Button_Click);
            return btn;
        }

        private void IBiuShow(Window owner, string poolKey)
        {
            Owner = owner ?? Application.Current.MainWindow;
            Owner.Opacity = 0.5;
            ShowDialog();
            Owner.Opacity = 1;
            if (_pool.ContainsKey(poolKey)) _pool.Remove(poolKey);
            //return _result;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _result = (BiuMessageBoxResult)(sender as Button).Tag;
            Close();
        }
    }
}
