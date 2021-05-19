using ADSDK.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RFIDReader
{
    public class SystemPub
    {
        #region 程序集属性访问器
        public static string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public static string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public static string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public static string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public static string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public static string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }
        #endregion


        public static ADRcp ADRcp = null;


        /// <summary>
        /// Impinj Ant Count
        /// </summary>
        public static string AntCurrentListString = "";
        public static List<int> AntCurrentListInt
        {
            get
            {
                string[] strAntArray = AntCurrentListString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                List<int> lstAnt = new List<int>();
                for (int i = 0; i < strAntArray.Length; i++)
                {
                    try
                    {
                        lstAnt.Add(Convert.ToInt32(strAntArray[i]));
                    }
                    catch { }
                }
                return lstAnt;
            }
        }

        public static List<int> GetAntList()
        {
            string[] strAntArray = AntCurrentListString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            List<int> lstAnt = new List<int>();
            for (int i = 0; i < strAntArray.Length; i++)
            {
                try
                {
                    lstAnt.Add(Convert.ToInt32(strAntArray[i]));
                }
                catch { }
            }
            if (lstAnt.Count == 0) lstAnt.Add(1);
            return lstAnt;
        }
    }
}
