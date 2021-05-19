using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using wow;

namespace biubiu.Domain
{
    /// <summary>
    /// 公用函数类
    /// </summary>
    public class Common
    {
        /// <summary>
        /// 深拷贝的反射实现
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T DeepCopy<T>(T obj)
        {
            //如果是字符串或值类型则直接返回
            if (obj == null || obj is string || obj.GetType().IsValueType) return obj;

            object retval = Activator.CreateInstance(obj.GetType());
            FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            FieldInfo[] baseFields = obj.GetType().BaseType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            foreach (FieldInfo field in fields)
            {
                try { field.SetValue(retval, DeepCopy(field.GetValue(obj))); }
                catch { }
            }
            foreach (FieldInfo field in baseFields)
            { //获取父类字段
                try { field.SetValue(retval, DeepCopy(field.GetValue(obj))); }
                catch { }
            }
            return (T)retval;
        }

        /// <summary>
        /// 获取ItemsCotrol的子控件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parentObj"></param>
        /// <returns></returns>
        public static T FindSingleVisualChildren<T>(DependencyObject parentObj) where T : DependencyObject
        {
            T result = null;
            if (parentObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parentObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(parentObj, i);
                    if (child != null && child is T)
                    {
                        result = child as T;
                        break;
                    }
                    result = FindSingleVisualChildren<T>(child);
                    if (result != null)
                        break;
                }
            }
            return result;
        }

        /// <summary>
        /// 格式化double类型 
        /// 保留小数点后两位，四舍六入五成双原则
        /// </summary>
        /// <param name="number"></param>
        /// <param name="range">小数点后保留位数</param>
        /// <returns></returns>
        public static double Double2DecimalCalculate(double number, int range = 2)
        {
            return double.Parse(decimal.Round(decimal.Parse(number.ToString()), range).ToString());
        }

        /// <summary>
        /// dateTime 转换为时间戳  javascript 时间戳
        /// </summary>
        /// <returns></returns>
        public static long? DateTime2TimeStamp(DateTime? dt)
        {
            if (dt == null) return null;
            else
            {
                return (long)((DateTime)dt - Config.StartTime).TotalMilliseconds;
            }
        }

        /// <summary>
        /// javascript 时间戳 转换为dateTime 
        /// </summary>
        /// <param name="stamp"></param>
        /// <returns></returns>
        public static DateTime TimeStamp2DateTime(long stamp)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            return startTime.AddMilliseconds(stamp);
        }

        /// <summary>
        /// 判断Scrollbar是否到达底部
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsVerticalScrollBarAtButtom(ScrollViewer s)
        {
            double dVer = s.VerticalOffset;
            double dViewport = s.ViewportHeight;
            double dExtent = s.ExtentHeight;

            bool isAtButtom;
            if (dVer != 0)
            {
                if (dVer + dViewport == dExtent)
                {
                    isAtButtom = true;
                }
                else
                {
                    isAtButtom = false;
                }
            }
            else
            {
                isAtButtom = false;
            }
            if (s.VerticalScrollBarVisibility == ScrollBarVisibility.Disabled
                || s.VerticalScrollBarVisibility == ScrollBarVisibility.Hidden)
            {
                isAtButtom = true;
            }
            return isAtButtom;
        }

        /// <summary>
        /// 厘米转换为英寸
        /// </summary>
        public static double CentimeterConvertToInch(double cm)
        {
            return Double2DecimalCalculate(cm * 0.39370, 3);
        }

        /// 无损压缩图片  
        /// <param name="sFile">原图片</param>  
        /// <param name="dFile">压缩后保存位置</param>  
        /// <param name="dHeight">高度</param>  
        /// <param name="dWidth"></param>  
        /// <param name="flag">压缩质量(数字越小压缩率越高) 1-100</param>  
        /// <returns></returns>  
        public static bool GetPicThumbnail(string sFile, string dFile, int dHeight, int dWidth, int flag)
        {
            if (!File.Exists(sFile))
                return false;
            System.Drawing.Image iSource = System.Drawing.Image.FromFile(sFile);
            Bitmap ob = null;
            try
            {

                ImageFormat tFormat = iSource.RawFormat;

                //按比例缩放
                System.Drawing.Size tem_size = new System.Drawing.Size(iSource.Width, iSource.Height);
                int sW;
                int sH;
                if (tem_size.Width > dHeight || tem_size.Width > dWidth)
                {
                    if ((tem_size.Width * dHeight) > (tem_size.Width * dWidth))
                    {
                        sW = dWidth;
                        sH = (int)(dWidth * tem_size.Height / tem_size.Width);
                    }
                    else
                    {
                        sH = dHeight;
                        sW = (int)(tem_size.Width * dHeight / tem_size.Height);
                    }
                }
                else
                {
                    sW = (int)tem_size.Width;
                    sH = (int)tem_size.Height;
                }

                ob = new Bitmap(dWidth, dHeight);
                Graphics g = Graphics.FromImage(ob);


                g.Clear(System.Drawing.Color.WhiteSmoke);
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                g.DrawImage(iSource, new Rectangle((dWidth - sW) / 2, (dHeight - sH) / 2, sW, sH), 0, 0, iSource.Width, iSource.Height, GraphicsUnit.Pixel);

                g.Dispose();
                //以下代码为保存图片时，设置压缩质量  
                EncoderParameters ep = new EncoderParameters();
                long[] qy = new long[1];
                qy[0] = flag;//设置压缩的比例1-100  
                EncoderParameter eParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qy);
                ep.Param[0] = eParam;
                ImageCodecInfo[] arrayICI = ImageCodecInfo.GetImageEncoders();
                ImageCodecInfo jpegICIinfo = null;
                for (int x = 0; x < arrayICI.Length; x++)
                {
                    if (arrayICI[x].FormatDescription.Equals("JPEG"))
                    {
                        jpegICIinfo = arrayICI[x];
                        break;
                    }
                }
                if (jpegICIinfo != null)
                {
                    ob.Save(dFile, jpegICIinfo, ep);//dFile是压缩后的新路径  
                }
                else
                {
                    ob.Save(dFile, tFormat);
                }
                iSource.Dispose();
                ob.Dispose();
                return true;
            }
            catch
            {
                iSource.Dispose();
                ob?.Dispose();
                return false;
            }

        }

        /// <summary>
        /// 获取文件的MD5值
        /// </summary>
        /// <param name="filePath"></param>
        public static string GetMD5ByFile(string filePath)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            try
            {
                FileStream file = new FileStream(filePath, FileMode.Open);
                byte[] retval = md5.ComputeHash(file);
                file.Close();

                StringBuilder sc = new StringBuilder();
                for (int i = 0; i < retval.Length; i++)
                {
                    sc.Append(retval[i].ToString("x2"));
                }
                return sc.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message + filePath.GetHashCode() + ex.GetHashCode();
            }
        }

        /// <summary>
        /// 获取字符串的MD5值
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetMD5ByString(string str)
        {
            if (str == null)
            {
                return null;
            }

            MD5 md5Hash = MD5.Create();

            // 将输入字符串转换为字节数组并计算哈希数据
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(str));

            // 创建一个 Stringbuilder 来收集字节并创建字符串
            StringBuilder sBuilder = new StringBuilder();

            // 循环遍历哈希数据的每一个字节并格式化为十六进制字符串
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // 返回十六进制字符串
            return sBuilder.ToString();
        }

        /// <summary>
        /// 获取一个UUID
        /// </summary>
        /// <returns></returns>
        public static string GetUUID()
        {
            Guid guid = Guid.NewGuid();
            string str = guid.ToString();
            return str;
        }
    }

}
