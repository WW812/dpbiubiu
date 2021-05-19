using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace biubiu.view_model.user
{
    public class UserValidateInSetter : ValidationRule
    {
        public string fieldName { get; set; }
        public UserValidateInSetter()
        {

        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                switch (fieldName)
                {
                    case "UserName":
                        if (string.IsNullOrWhiteSpace(value.ToString()))
                        {
                            return new ValidationResult(false, "请输入用户名");
                        }
                        break;
                    case "PassWord":
                        if (string.IsNullOrWhiteSpace(value.ToString()))
                        {
                            return new ValidationResult(false, "请输入密码");
                        }
                        break;
                    default:
                        break;
                }

                return ValidationResult.ValidResult;
            }
            catch (Exception e)
            {
                return new ValidationResult(false, e.Message);
            }
        }
    }
}
