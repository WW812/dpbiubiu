﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace biubiu.Domain
{
    public static class Encoder
    {
        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <param name="msg">信息</param>
        /// <param name="version">版本 1 ~ 40</param>
        /// <param name="pixel">像素点大小</param>
        /// <param name="icon_path">图标路径</param>
        /// <param name="icon_size">图标尺寸</param>
        /// <param name="icon_border">图标边框厚度</param>
        /// <param name="white_edge">二维码白边</param>
        /// <returns>位图</returns>
        public static Bitmap CodeIcon(string msg, int version, int pixel, string icon_path, int icon_size, int icon_border, bool white_edge)
        {
            QRCoder.QRCodeGenerator code_generator = new QRCoder.QRCodeGenerator();

            QRCoder.QRCodeData code_data = code_generator.CreateQrCode(msg, QRCoder.QRCodeGenerator.ECCLevel.M/* 这里设置容错率的一个级别 */, true, true, QRCoder.QRCodeGenerator.EciMode.Utf8, version);

            QRCoder.QRCode code = new QRCoder.QRCode(code_data);

            Bitmap icon = new Bitmap(icon_path);

            Bitmap bmp = code.GetGraphic(pixel, Color.Black, Color.White, icon, icon_size, icon_border, white_edge);

            return bmp;
        }

        public static Bitmap Code(string msg, int version, int pixel, bool white_edge)
        {
            QRCoder.QRCodeGenerator code_generator = new QRCoder.QRCodeGenerator();

            QRCoder.QRCodeData code_data = code_generator.CreateQrCode(msg, QRCoder.QRCodeGenerator.ECCLevel.M/* 这里设置容错率的一个级别 */, true, true, QRCoder.QRCodeGenerator.EciMode.Utf8, version);

            QRCoder.QRCode code = new QRCoder.QRCode(code_data);

            Bitmap bmp = code.GetGraphic(pixel, Color.Black, Color.White, white_edge);

            return bmp;
        }
    }
}
