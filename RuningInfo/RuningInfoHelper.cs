using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RuningInfo
{
    public class RuningInfoHelper
    {
        private static string url = @"http://hb9.api.yesapi.cn/?app_key=497E3A560C6B160E9CA479D2B7E6133F&s=App.Table.Create&model_name=username&sign=B31B003DE4CD06CAA58460941A20876D&data=";
        public static void PutInfo(RuningInfoModel rim)
        {
            if (rim is null) return;
            GetInterface<RuningInfoModel>(url+$"{JsonConvert.SerializeObject(rim)}");
        }

        private static void GetInterface<T>(string url)
        {
            try
            {
                string serviceAddress = url;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(serviceAddress);
                request.Method = "GET";
                request.ContentType = "application/json";
                string retString = "";
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    string encoding = response.ContentEncoding;
                    if (encoding == null || encoding.Length < 1)
                    {
                        encoding = "UTF-8"; //默认编码  
                    }
                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding)))
                    {
                        retString = reader.ReadToEnd();
                    }
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
