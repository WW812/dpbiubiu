using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using biubiu.Domain;

namespace biubiu.model
{
    /// <summary>
    /// 票据设置对象类
    /// </summary>
    public class BillConfig: IConfigModelInterface, INotifyPropertyChanged
    {
        /// <summary>
        /// 是否指定打印机
        /// </summary>
        private bool _isAppoint = false;
        public bool IsAppoint
        {
            get { return _isAppoint; }
            set { _isAppoint = value;
                NotifyPropertyChanged("IsAppoint");
            }
        }

        /// <summary>
        /// 指定打印机名字
        /// </summary>
        private string _appointPrinterName = "";
        public string ApponitPrinterName
        {
            get { return _appointPrinterName; }
            set
            {
                _appointPrinterName = value;
                NotifyPropertyChanged("ApponitPrinterName");
            }
        }

        /// <summary>
        /// 头部文字后缀
        /// </summary>
        private string _titleSuffix = "";
        public string TitleSuffix
        {
            get { return _titleSuffix; }
            set { _titleSuffix = value;
                NotifyPropertyChanged("TitleSuffix");
            }
        }

        /// <summary>
        /// 底部文字
        /// </summary>
        private string _footerWord = "";
        public string FooterWord
        {
            get { return _footerWord; }
            set { _footerWord = value;
                NotifyPropertyChanged("FooterWord");
            }
        }

        /// <summary>
        /// 打印次数
        /// </summary>
        private int _printTimes = 0;
        public int PrintTimes
        {
            get { return _printTimes; }
            set { _printTimes = value;
                NotifyPropertyChanged("PrintTimes");
            }
        }

        /// <summary>
        /// 模板名称
        /// </summary>
        private string _templateName = "";// = "宽80mm_默认";
        public string TemplateName
        {
            get { return _templateName; }
            set { _templateName = value;
            NotifyPropertyChanged("TemplateName"); }
        }

        /// <summary>
        /// 模板地址
        /// </summary>
        /// <param name="iniClass"></param>
        /// <param name="section"></param>
        private string _templatePath = "";// = "./template/Ship/热敏小票/宽80mm_默认.rdlc";
        public string TemplatePath
        {
            get { return _templatePath; }
            set { _templatePath = value;
                NotifyPropertyChanged("TemplatePath");
            }
        }

        public double AdjustWeight { get; set; } // 打印矫正毛重数值


        public void WriteToFile(INIClass iniClass, string section)
        {
            iniClass.IniWriteValue(section, "IsAppoint",IsAppoint.ToString());
            iniClass.IniWriteValue(section, "ApponitPrinterName", ApponitPrinterName.ToString());
            iniClass.IniWriteValue(section, "TitleSuffix", TitleSuffix.ToString());
            iniClass.IniWriteValue(section, "FooterWord", FooterWord.ToString());
            iniClass.IniWriteValue(section, "PrintTimes", PrintTimes.ToString());
            iniClass.IniWriteValue(section, "TemplateName", TemplateName.ToString());
            iniClass.IniWriteValue(section, "TemplatePath",TemplatePath.ToString());
            iniClass.IniWriteValue(section, "AdjustWeight", AdjustWeight.ToString());
        }

        public void ReadFromFile(INIClass iniClass, string section)
        {
            IsAppoint = "True".Equals(iniClass.IniReadValue(section, "IsAppoint"));
            ApponitPrinterName = iniClass.IniReadValue(section, "ApponitPrinterName");
            TitleSuffix = iniClass.IniReadValue(section, "TitleSuffix");
            FooterWord = iniClass.IniReadValue(section, "FooterWord");
            var times = iniClass.IniReadValue(section, "PrintTimes");
            PrintTimes = Int32.Parse(string.IsNullOrEmpty(times) ? "0" : times);
            TemplateName = iniClass.IniReadValue(section, "TemplateName");
            TemplatePath = iniClass.IniReadValue(section, "TemplatePath");
            var adjWeight = iniClass.IniReadValue(section, "AdjustWeight");
            AdjustWeight = Double.Parse(string.IsNullOrEmpty(adjWeight) ? "0.0" : adjWeight);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
