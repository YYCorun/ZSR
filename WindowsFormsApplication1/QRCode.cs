using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThoughtWorks.QRCode.Codec;

namespace WindowsFormsApplication1
{
    class QRCode
    {



        /// <summary>
        /// 生成QRcode二维码
        /// </summary>
        /// <param name="code">要编码的字符串</param>
        /// <param name="size">生成图片的高度</param>
        /// <returns>生成后的二维码图片</returns>
        public static Bitmap GetQRCode(string code, int size = 3)
        {
            QRCodeEncoder qrEntity = new QRCodeEncoder();

            qrEntity.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;//二维码编码方式

            qrEntity.QRCodeScale = size;//每个小方格的宽度

            qrEntity.QRCodeVersion = 5;//二维码版本号

            qrEntity.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.L;//纠错码等级

            System.Drawing.Bitmap srcimage;



            //动态调整二维码版本号,上限40，过长返回空白图片，编码后字符最大字节长度2953
            while (true)
            {
                try
                {
                    srcimage = qrEntity.Encode(code, System.Text.Encoding.UTF8);

                    break;
                }
                catch (IndexOutOfRangeException e)
                {
                    if (qrEntity.QRCodeVersion < 100)
                    {
                        qrEntity.QRCodeVersion++;
                    }
                    else
                    {
                        srcimage = new Bitmap(100, 100);
                        break;
                    }
                }
            }

            //为生成的二维码图像裁剪白边并调整为请求的高度
            return srcimage;
        }

    }
}
