using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace biubiu.Domain.pages
{
    public class PageModel 
    {
        /// <summary>
        /// 是否为第一页
        /// </summary>
        public Boolean First { get; set; }
        /// <summary>
        /// 是否为最后一页
        /// </summary>
        public Boolean Last { get; set; }
        /// <summary>
        /// 当前页码
        /// </summary>
        public int Page { get; set; }
        /// <summary>
        /// 当前页码的条数
        /// </summary>
        public int Size { get; set; }
        /// <summary>
        /// 总页码数
        /// </summary>
        public int TotalPages
        {
            get;
            set;
        }
        /// <summary>
        /// 总条数
        /// </summary>
        public int TotalSize
        {
            get;
            set;
        }

        public PageModel(int s = Config.PageSize, int p = 1)
        {
            Reset(s,p);
        }

        public void Reset(int s = Config.PageSize, int p = 1)
        {
            Size = s; 
            Page = p;
            TotalPages = 0;
            TotalSize = 0;
        }
    }
}
