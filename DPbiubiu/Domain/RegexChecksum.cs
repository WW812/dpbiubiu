using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace biubiu.Domain
{
    public class RegexChecksum
    {
        /// <summary>
        /// 是否为正实数
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsPositiveReal(string value)
        {
            return Regex.IsMatch(value, @"^(([0-9]+\.[0-9]*[1-9][0-9]*)|([0-9]*[1-9][0-9]*\.[0-9]+)|([0-9]*[1-9][0-9]*))$");
        }

        /// <summary>
        /// 是否为非负实数
        /// </summary>
        /// <returns></returns>
        public static bool IsNonnegativeReal(string value)
        {
            return Regex.IsMatch(value, @"^\d+(\.\d+)?$");
        }

        /// <summary>
        /// 是否为实数
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsReal(string value)
        {
            return Regex.IsMatch(value, @"^(-?\d+)(\.\d+)?");
        }

        /// <summary>
        /// 是否为非负整数
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNonnegativeInteger(string value)
        {
            return Regex.IsMatch(value, @"^\d+$");
        }

        /// <summary>
        /// 验证正整数
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsInteger(string value)
        {
            return Regex.IsMatch(value, @"^\+?[1-9][0-9]*$");
        }

        /// <summary>
        /// 验证是否为汉字
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsChineseCharacters(string value)
        {
            return Regex.IsMatch(value, @"^[\u4e00-\u9fa5]*$");
        }

        /// <summary>
        /// 是否为小数点后不大约两位小数的正实数
        /// </summary>
        public static bool IsNonnegativeReal2Decimal(string value)
        {
            return Regex.IsMatch(value, @"^[0-9]+(.[0-9]{0,2})?$");
        }
    }
}
