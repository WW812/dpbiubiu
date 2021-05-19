using biubiu.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace biubiu.view_model
{
    /// <summary>
    /// 检验不可为 null、""和空白字符串
    /// </summary>
    public class NotEmptyValidationRule: ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return string.IsNullOrWhiteSpace((value ?? "").ToString())
                ? new ValidationResult(false, "不可为空")
                : ValidationResult.ValidResult;
        }
    }

    /// <summary>
    /// 是否为正实数
    /// </summary>
    public class IsPositiveRealValidationRule: ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return RegexChecksum.IsPositiveReal((value ?? "").ToString())
                ? ValidationResult.ValidResult
                : new ValidationResult(false, "请输入正数");
        }
    }

    /// <summary>
    /// 是否为实数
    /// </summary>
    public class IsRealValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return RegexChecksum.IsReal((value ?? "").ToString())
                ? ValidationResult.ValidResult
                : new ValidationResult(false, "请输入数字");
        }
    }

    /// <summary>
    /// 是否为非负实数
    /// </summary>
    public class IsNonnegativeRealValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return RegexChecksum.IsNonnegativeReal((value ?? "").ToString())
                ? ValidationResult.ValidResult
                : new ValidationResult(false, "请输入大于等于0的数字");
        }
    }



    public static class NotifyPropertyChangedExtension
    {
        public static void MutateVerbose<TField>(this INotifyPropertyChanged instance, ref TField field, TField newValue, Action<PropertyChangedEventArgs> raise, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<TField>.Default.Equals(field, newValue)) return;
            field = newValue;
            raise?.Invoke(new PropertyChangedEventArgs(propertyName));
        }
    }
}
