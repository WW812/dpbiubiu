using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuningInfo
{
    public class RuningInfoModel
    {
        #region 电脑、系统相关
        /// <summary>
        /// 计算机名
        /// </summary>
        public string ComputerName { get; set; }
        /// <summary>
        /// 操作系统名称
        /// </summary>
        public string SystemName { get; set; }
        /// <summary>
        /// 系统版本号
        /// </summary>
        public string SystemVersion { get; set; }
        /// <summary>
        /// 系统位数
        /// </summary>
        public string SystemBit { get; set; }
        /// <summary>
        /// 登录机器IP
        /// </summary>
        public string ComputerIP { get; set; }
        #endregion

        #region 用户相关
        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 用户账号
        /// </summary>
        public string UserAccount { get; set; }
        /// <summary>
        /// 厂名
        /// </summary>
        public string CompanyName { get; set; }
        #endregion

        #region 错误信息
        public string ProvinceName { get; set; }
        #endregion
    }
}
