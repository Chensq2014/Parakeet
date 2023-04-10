using Microsoft.Extensions.Configuration;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using QRCoder;

namespace Parakeet.Net.Helper
{
    public class VerifyCodeHelper
    {
        private static IConfiguration _appConfiguration;
        public VerifyCodeHelper(IConfiguration appConfiguration)
        {
            _appConfiguration = appConfiguration;//env.GetAppConfiguration();//
        }
        private static readonly string ImagePath = Directory.GetCurrentDirectory() + _appConfiguration["ImagePath"];

        ////也可以使用静态类中的静态变量读取上层配置文件
        //private static readonly string ImagePath = ConfigurationManager.RootPath + ConfigurationManager.Configuration["ImagePath"];

        #region 画验证码

        /// <summary>
        /// 创建一个验证码
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static Bitmap CreateVerifyCode(out string code)
        {
            //建立Bitmap对象，绘图
            Bitmap bitmap = new Bitmap(200, 60);
            Graphics graph = Graphics.FromImage(bitmap);  //在Bitmap上创建一个新的Graphics对象
            graph.FillRectangle(new SolidBrush(Color.White), 0, 0, 200, 60);
            Font font = new Font(FontFamily.GenericSerif, 48, FontStyle.Bold, GraphicsUnit.Pixel);
            Random r = new Random();
            string letters = "ABCDEFGHIJKLMNPQRSTUVWXYZ0123456789";

            StringBuilder sb = new StringBuilder();

            //添加随机的五个字母
            for (int x = 0; x < 5; x++)
            {
                string letter = letters.Substring(r.Next(0, letters.Length - 1), 1);
                sb.Append(letter);
                graph.DrawString(letter, font, new SolidBrush(Color.Black), x * 38, r.Next(0, 15));
            }
            code = sb.ToString();

            //混淆背景
            Pen linePen = new Pen(new SolidBrush(Color.Black), 2);
            for (int x = 0; x < 6; x++)
                graph.DrawLine(linePen, new Point(r.Next(0, 199), r.Next(0, 59)), new Point(r.Next(0, 199), r.Next(0, 59)));
            return bitmap;
        }

        /// <summary>
        /// 画验证码 保存路径: 网站根目录/upload/drawings/default.png
        /// </summary>
        public static void Drawing()
        {
            Bitmap bitmapobj = new Bitmap(100, 100);
            //在Bitmap上创建一个新的Graphics对象
            var graph = Graphics.FromImage(bitmapobj);
            //创建绘画对象，如Pen,Brush等
            Pen redPen = new Pen(Color.Red, 8);
            graph.Clear(Color.White);
            //绘制图形
            graph.DrawLine(redPen, 50, 20, 500, 20);
            graph.DrawEllipse(Pens.Black, new Rectangle(0, 0, 200, 100));//画椭圆
            graph.DrawArc(Pens.Black, new Rectangle(0, 0, 100, 100), 60, 180);//画弧线
            graph.DrawLine(Pens.Black, 10, 10, 100, 100);//画直线
            graph.DrawRectangle(Pens.Black, new Rectangle(0, 0, 100, 200));//画矩形
            graph.DrawString("我爱北京天安门", new Font("微软雅黑", 12), new SolidBrush(Color.Red), new PointF(10, 10));//画字符串
            if (!Directory.Exists(ImagePath))
                Directory.CreateDirectory(ImagePath);
            bitmapobj.Save(ImagePath + "default.png", ImageFormat.Png);
            //释放所有对象
            bitmapobj.Dispose();
            graph.Dispose();
        }

        /// <summary>
        ///  画验证码 保存路径: 网站根目录/upload/drawings/verif.png
        /// </summary>
        public static void VerificationCode()
        {
            Bitmap bitmapobj = new Bitmap(300, 300);
            //在Bitmap上创建一个新的Graphics对象
            var graph = Graphics.FromImage(bitmapobj);
            graph.DrawRectangle(Pens.Black, new Rectangle(0, 0, 150, 50));//画矩形
            graph.FillRectangle(Brushes.White, new Rectangle(1, 1, 149, 49));
            graph.DrawArc(Pens.Blue, new Rectangle(10, 10, 140, 10), 150, 90);//干扰线
            string[] arrStr = new string[] { "我", "们", "孝", "行", "白", "到", "国", "中", "来", "真" };
            var random = new Random();
            int i;
            for (int j = 0; j < 4; j++)
            {
                i = random.Next(10);
                graph.DrawString(arrStr[i], new Font("微软雅黑", 15), Brushes.Red, new PointF(j * 30, 10));
            }
            bitmapobj.Save(Path.Combine(ImagePath, "verif.png"), ImageFormat.Png);
            bitmapobj.Dispose();
            graph.Dispose();
        }

        #endregion

        #region base64转二进制再转Bitmap Bitmap再转二进制再转base64

        /// <summary>
        /// base64转二进制再转Bitmap
        /// </summary>
        /// <param name="base64String"></param>
        /// <returns></returns>
        public static Bitmap GetBitmapImageFromBase64(string base64String)
        {
            base64String = base64String.Split(',')[1];
            base64String = base64String.Replace(" ", "+");
            var mod4 = base64String.Length % 4;
            if (mod4 > 0)
            {
                base64String += new string('=', 4 - mod4);
            }
            var ms = new MemoryStream(Convert.FromBase64String(base64String));
            var bitmap = new Bitmap(ms);
            return bitmap;
        }

        /// <summary>
        /// bitmap图片转Base64
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static async Task<string> GetBase64FromImageAsync(Bitmap bmp)
        {
            var ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Png);
            var arr = new byte[ms.Length];
            ms.Position = 0;
            await ms.ReadAsync(arr, 0, (int)ms.Length);
            ms.Close();
            return Convert.ToBase64String(arr);
        }

        #endregion

        #region 二维码 通过VS中的【NUGET】下载并引用QRCode (QRCoder.dll)   这个dll不能用core的drawing 等出新版本之后再启用

        /// <summary>
        /// 二维码 通过VS中的【NUGET】下载并引用QRCode (QRCoder.dll)
        /// </summary>
        /// <param name="url">生成二维码的内容</param>
        /// <returns>base64String png</returns>
        public static async Task<string> GetQrCodeImage(string url)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
            QRCode qrcode = new QRCode(qrCodeData);

            // qrcode.GetGraphic 方法可参考最下发“补充说明”
            var qrCodeImage = qrcode.GetGraphic(5, Color.Black, Color.White, null, 15, 6, false);
            return await GetBase64FromImageAsync(qrCodeImage);
            //MemoryStream ms = new MemoryStream();
            //qrCodeImage.Save(ms, ImageFormat.Png);

            // 如果想保存图片 可使用  qrCodeImage.Save(filePath);

            //// 响应类型
            //context.Response.ContentType = "image/Jpeg";
            ////输出字符流
            //context.Response.BinaryWrite(ms.ToArray());


            /* GetGraphic方法参数说明
                public Bitmap GetGraphic(int pixelsPerModule, Color darkColor, Color lightColor, Bitmap icon = null, int iconSizePercent = 15, int iconBorderWidth = 6, bool drawQuietZones = true)
            * 
                int pixelsPerModule:生成二维码图片的像素大小 ，我这里设置的是5 
            * 
                Color darkColor：暗色   一般设置为Color.Black 黑色
            * 
                Color lightColor:亮色   一般设置为Color.White  白色
            * 
                Bitmap icon :二维码 水印图标 例如：Bitmap icon = new Bitmap(context.Server.MapPath("~/images/zs.png")); 默认为NULL ，加上这个二维码中间会显示一个图标
            * 
                int iconSizePercent： 水印图标的大小比例 ，可根据自己的喜好设置 
            * 
                int iconBorderWidth： 水印图标的边框
            * 
                bool drawQuietZones:静止区，位于二维码某一边的空白边界,用来阻止读者获取与正在浏览的二维码无关的信息 即是否绘画二维码的空白边框区域 默认为true
            */
        }

        #endregion

    }
}