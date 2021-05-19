using biubiu.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace biubiu.model
{
    /// <summary>
    /// 配置类接口
    /// </summary>
    interface IConfigModelInterface
    {
        /// <summary>
        /// 写入文件
        /// </summary>
        /// <param name="iniClass"></param>
        /// <param name="section"></param>
        void WriteToFile(INIClass iniClass, string section);

        /// <summary>
        /// 从文件读取
        /// </summary>
        /// <param name="iniClass"></param>
        /// <param name="section"></param>
        void ReadFromFile(INIClass iniClass, string section);
    }
}
