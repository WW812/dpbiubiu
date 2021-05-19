using biubiu.model.stock_order;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace biubiu.Converter
{
    /// <summary>
    /// 反转Boolean类型
    /// </summary>
    public class ReverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            return !System.Convert.ToBoolean(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            return new NotImplementedException();
        }
    }

    /// <summary>
    /// 数字和Bool转换
    /// （1 to true ; 0 to false）
    /// </summary>
    public class IntToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value == 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Boolean)value ? 1 : 0;
        }
    }

    /// <summary>
    /// 数字和Visibility转换
    /// 0隐藏 1显示
    /// </summary>
    public class IntToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value == 0 ? Visibility.Hidden : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 0;
        }
    }
    /// <summary>
    /// 数字和Visibility转换
    /// 0显示 1隐藏
    /// </summary>
    public class ReverseIntToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value == 0 ? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 0;
        }
    }

    /// <summary>
    /// JavaScript时间戳转换为DateTime
    /// </summary>
    public class LongToDateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if ((long)value == 0) return "#";
                System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
                return startTime.AddMilliseconds((long)value);
            }
            catch (Exception)
            {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    /// <summary>
    /// 根据单据状态更改提交按钮文字
    /// </summary>
    public class OrderStatusToSubmitButtonTextConverter : IValueConverter
    {
        // parameter 0为出料 1为进料
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((string)parameter == "0")
            {
                if ((int)value == 0) return "进厂装载";
                if ((int)value == 1) return "出厂结算";
            }

            if ((string)parameter == "1")
            {
                if ((int)value == 0) return "料车进厂";
                if ((int)value == 1) return "出厂结算";
            }

            return "提交(订单状态可能有误)";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 0;
        }
    }

    /// <summary>
    /// 扣吨方式和Boolean
    /// default = false,百分比= true
    /// </summary>
    public class DeductWeightTypeToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return (int)value == 1;
            }
            catch (Exception)
            {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
   
    public class VoidDataGridRowColorConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if ((int)value == 0)
                {
                    return "Black";
                }else
                {
                    return "Red";
                }
            }
            catch (Exception)
            {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public class ShipOrderStatusToDescribeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((int)value)
            {
                case 0:
                    return "新单据";
                case 1:
                    return "已付款";
                case 2:
                    return "已交账";
                default:
                    return "状态码错误";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }


    public class StockOrderStatusToDescribeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((int)value)
            {
                case 0:
                    return "新单据";
                case 1:
                    return "已付款";
                case 2:
                    return "已交账";
                default:
                    return "状态码错误";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    /// <summary>
    /// 承兑状态码描述
    /// </summary>
    public class AcceptStatusToDescribeConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((int)value)
            {
                case 0:
                    return "未兑现";
                case 1:
                    return "已兑现";
                case 2:
                    return "已出账";
                case 3:
                    return "退还原主";
                case 4:
                    return "有问题";
                case 5:
                    return "兑换中";
                case 6:
                    return "到期承兑";
                default:
                    return "状态码错误";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
