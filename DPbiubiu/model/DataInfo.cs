using biubiu.Domain.pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace biubiu.model
{
    public class DataInfo<T>
    {
        public int Code { get; set; }
        public T Data { get; set; }
        public string Msg { get; set; }
        public PageModel Page { get; set; }
        public object Obj { get; set; }

        public override string ToString()
        {
            return Msg;
        }
    }
}
