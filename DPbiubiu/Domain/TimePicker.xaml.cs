using MaterialDesignThemes.Wpf;
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

namespace biubiu.Domain
{
    /// <summary>
    /// TimePicker.xaml 的交互逻辑
    /// </summary>
    public partial class TimePicker : UserControl
    {
        private bool _textChangeTrigger = true;

        public static readonly DependencyProperty SelectedTimeProperty =
            DependencyProperty.Register("SelectedTime", typeof(DateTime?), typeof(TimePicker),
            new FrameworkPropertyMetadata(new DateTime(), new PropertyChangedCallback(SelectedTimePropertyChangedCallback)));

        private static void SelectedTimePropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs arg)
        {
            if (sender != null && sender is TimePicker)
            {
                TimePicker tp = sender as TimePicker;
                //clock.OnTimeUpdated((DateTime)arg.OldValue, (DateTime)arg.NewValue);
                if (tp.SelectedTime != null && tp._textChangeTrigger)
                {
                    tp.HourTextBox.Text = tp.SelectedTime.Value.Hour.ToString();
                    tp.MinuteTextBox.Text = tp.SelectedTime.Value.Minute.ToString();
                }
                else
                {
                    tp.SelectedTime = new DateTime();
                }
                tp._textChangeTrigger = true;
            }
        }

        public DateTime? SelectedTime
        {
            get
            {
                return (DateTime?)this.GetValue(SelectedTimeProperty);
            }
            set
            {
                this.SetValue(SelectedTimeProperty, value);
            }
        }


        public TimePicker()
        {
            InitializeComponent();
        }

        private void PackIcon_MouseEnter(object sender, MouseEventArgs e)
        {
            (sender as PackIcon).Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Black"));
        }

        private void PackIcon_MouseLeave(object sender, MouseEventArgs e)
        {
            (sender as PackIcon).Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF616161"));
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textbox = (sender as TextBox);
            if (string.IsNullOrWhiteSpace(textbox.Text))
            {
                textbox.Text = "00";
            }
            SelectedTime = DateTime.Today + TimeSpan.FromHours(System.Convert.ToInt32(HourTextBox.Text)) + TimeSpan.FromMinutes(System.Convert.ToInt32(MinuteTextBox.Text));
            textbox.PreviewMouseDown += new MouseButtonEventHandler(TextBox_PreviewMouseDown);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            switch ((sender as Button).Name)
            {
                case "HourPlusBtn":
                    if (System.Convert.ToInt32(HourTextBox.Text) < 23)
                        SelectedTime += TimeSpan.FromHours(1);
                    else
                        SelectedTime -= TimeSpan.FromHours(SelectedTime.Value.Hour);
                    break;
                case "HourMinusBtn":
                    if (System.Convert.ToInt32(HourTextBox.Text) > 0)
                        SelectedTime -= TimeSpan.FromHours(1);
                    else
                        SelectedTime = DateTime.Today + TimeSpan.FromMinutes(SelectedTime.Value.Minute) + TimeSpan.FromHours(23);
                    break;
                case "MinutePlusBtn":
                    if (System.Convert.ToInt32(MinuteTextBox.Text) < 59)
                        SelectedTime += TimeSpan.FromMinutes(1);
                    else
                        SelectedTime -= TimeSpan.FromMinutes(SelectedTime.Value.Minute);
                    break;
                case "MinuteMinusBtn":
                    if (System.Convert.ToInt32(MinuteTextBox.Text) > 0)
                        SelectedTime -= TimeSpan.FromMinutes(1);
                    else
                        SelectedTime = DateTime.Today + TimeSpan.FromHours(SelectedTime.Value.Hour) + TimeSpan.FromMinutes(59);
                    break;
            }
        }

        private void HourTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            e.Handled = !(RegexChecksum.IsNonnegativeInteger(e.Text));
        }

        private void MinuteTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            e.Handled = !(RegexChecksum.IsNonnegativeInteger(e.Text));
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textbox = sender as TextBox;
            try
            {
                if (textbox.Name.Equals("HourTextBox"))
                {
                    if (System.Convert.ToInt32(textbox.Text) > 23)
                        textbox.Text = "23";
                    if (System.Convert.ToInt32(textbox.Text) < 0)
                        textbox.Text = "0";
                }
                if (textbox.Name.Equals("MinuteTextBox"))
                {
                    if (System.Convert.ToInt32(textbox.Text) > 59)
                        textbox.Text = "59";
                    if (System.Convert.ToInt32(textbox.Text) < 0)
                        textbox.Text = "0";
                }
            }
            catch
            {
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            HourTextBox.TextChanged += new TextChangedEventHandler(TextBox_TextChanged);
            MinuteTextBox.TextChanged += new TextChangedEventHandler(TextBox_TextChanged);
        }

        private void TextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender != null)
            {
                (sender as TextBox).Focus();
                e.Handled = true;
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
    }
}
