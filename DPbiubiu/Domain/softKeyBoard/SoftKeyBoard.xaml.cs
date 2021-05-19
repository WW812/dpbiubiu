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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace biubiu.Domain.softKeyBoard
{
    /// <summary>
    /// SoftKeyBoard.xaml 的交互逻辑
    /// </summary>
    public partial class SoftKeyBoard : UserControl
    {
        public SoftKeyBoard()
        {
            InitializeComponent();
            BtnItems.ItemsSource = new ObservableCollection<string> {
                 "0","1","2","3","4","5","6","7","8","9",
                 "鲁", "冀", "豫", "晋", "蒙", "陕", "辽", "黑", "京", "津",
    "皖", "新", "苏", "浙", "赣", "鄂", "桂", "甘", "湘", "渝",
    "吉", "闽", "贵", "粤", "青", "藏", "川", "宁", "沪", "云",
     "A", "B", "C", "D", "E", "F", "G", "H", "I", "J",
    "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T",
    "U", "V", "W", "X", "Y", "Z"
            };
        }
    }
}
