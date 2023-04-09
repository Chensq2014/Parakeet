using Parakeet.Net.Enums;
using Parakeet.Net.Extensions;
using System;
using System.DrawingCore;
using System.DrawingCore.Drawing2D;
using System.DrawingCore.Imaging;
using System.DrawingCore.Text;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Parakeet.Net.Helper
{
    /// <summary>
    /// ImageHelper 绘制验证码 图片等比/不等比缩放 
    /// </summary>
    public class ImageHelper
    {
        ////const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        //const string Chars = "小鹦鹉工作室";
        //#region 静态成员

        //static readonly Font[] FontArr = new Font[] { new Font(FontFamily.GenericMonospace, 60.0f), new Font(FontFamily.GenericSansSerif, 60.0f), new Font(FontFamily.GenericSerif, 60.0f) };
        //static float minAngle = -15.0f, maxAngle = 15.0f;
        //static int minTrans = 40, maxTrans = 60;
        //static int minScale = 10, maxScale = 100;
        //public static Random Rand = new Random();

        //#endregion

        #region 成员函数

        //public static string GetRandText()
        //{
        //    var length = Rand.Next(1, 5);
        //    return new string(Enumerable.Repeat(Chars, length).Select(s => s[Rand.Next(s.Length)]).ToArray());
        //}

        //public static Font GetRandFont()
        //{
        //    return FontArr[Rand.Next(0, 3)];
        //}

        //public static float GetRandRotation()
        //{
        //    return Rand.Next((int)minAngle, (int)maxAngle);
        //}

        //public static float GetRandScale()
        //{
        //    return Rand.Next(minScale, maxScale) / 100.0f;
        //}

        //public static int GetRandTransparency()
        //{
        //    return 255 * Rand.Next(minTrans, maxTrans) / 100;
        //}

        //public static Color GetRandColor()
        //{
        //    return Color.FromArgb(GetRandTransparency(), Rand.Next(0, 255), Rand.Next(0, 255), Rand.Next(0, 255));
        //}

        #endregion
        //绘图的原理很简单：Bitmap就像一张画布，Graphics如同画图的手，把Pen或Brush等绘图对象画在Bitmap这张画布上

        /// <summary>
        /// 创建一个验证码 常用 
        /// </summary>
        /// <param name="code">返回code</param>
        /// <returns>返回bitmap文件</returns>
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


        #region 图片压缩
        /// <summary>
        /// 按比例缩放,图片不会变形，会优先满足原图和最大长宽比例最高的一项
        /// </summary>
        /// <param name="oldPath"></param>
        /// <param name="newPath"></param>
        /// <param name="maxWidth"></param>
        /// <param name="maxHeight"></param>
        public static void CompressPercent(string oldPath, string newPath, int maxWidth, int maxHeight)
        {
            var sourceImg = Image.FromFile(oldPath);
            var newWidth = (decimal)maxWidth;
            var newHeight = (decimal)maxHeight;
            var percentWidth = (decimal)sourceImg.Width > maxWidth ? maxWidth : (decimal)sourceImg.Width;

            if (sourceImg.Height * percentWidth / sourceImg.Width > maxHeight)
            {
                newHeight = maxHeight;
                newWidth = (decimal)maxHeight / sourceImg.Height * sourceImg.Width;
            }
            else
            {
                newWidth = percentWidth;
                newHeight = (percentWidth / sourceImg.Width) * sourceImg.Height;
            }
            var bitmap = new Bitmap((int)newWidth, (int)newHeight);
            var graph = Graphics.FromImage(bitmap);
            graph.InterpolationMode = InterpolationMode.High;
            graph.SmoothingMode = SmoothingMode.HighQuality;
            graph.Clear(Color.Transparent);
            graph.DrawImage(sourceImg, new Rectangle(0, 0, (int)newWidth, (int)newHeight), new Rectangle(0, 0, sourceImg.Width, sourceImg.Height), GraphicsUnit.Pixel);
            sourceImg.Dispose();
            graph.Dispose();
            bitmap.Save(newPath, ImageFormat.Jpeg);
            bitmap.Dispose();
        }

        /// <summary>
        /// 按照指定大小对图片进行缩放，可能会图片变形
        /// </summary>
        /// <param name="oldPath"></param>
        /// <param name="newPath"></param>
        /// <param name="newWidth"></param>
        /// <param name="newHeight"></param>
        public static void ImageChangeBySize(string oldPath, string newPath, int newWidth, int newHeight)
        {
            var sourceImg = Image.FromFile(oldPath);
            var bitmap = new Bitmap(newWidth, newHeight);
            var graph = Graphics.FromImage(bitmap);
            graph.InterpolationMode = InterpolationMode.High;
            graph.SmoothingMode = SmoothingMode.HighQuality;
            graph.Clear(Color.Transparent);
            graph.DrawImage(sourceImg, new Rectangle(0, 0, newWidth, newHeight), new Rectangle(0, 0, sourceImg.Width, sourceImg.Height), GraphicsUnit.Pixel);
            sourceImg.Dispose();
            graph.Dispose();
            bitmap.Save(newPath, ImageFormat.Png);
            bitmap.Dispose();
        }

        /// <summary>
        /// 等比压缩图片，返回固定大小并居中
        /// </summary>
        /// <param name="mg"></param>
        /// <param name="newSize"></param>
        /// <returns></returns>
        public static Image ResizeImage(Image mg, Size newSize)
        {
            double ratio;//压缩比
            if ((mg.Width / Convert.ToDouble(newSize.Width)) > (mg.Height / Convert.ToDouble(newSize.Height)))
                ratio = Convert.ToDouble(mg.Width) / Convert.ToDouble(newSize.Width);
            else
                ratio = Convert.ToDouble(mg.Height) / Convert.ToDouble(newSize.Height);
            var myHeight = (int)Math.Ceiling(mg.Height / ratio);
            var myWidth = (int)Math.Ceiling(mg.Width / ratio);
            Image bp = new Bitmap(newSize.Width, newSize.Height);
            var x = (newSize.Width - myWidth) / 2;
            var y = (newSize.Height - myHeight) / 2;
            Graphics g = Graphics.FromImage(bp);
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.InterpolationMode = InterpolationMode.High;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            Rectangle rect = new Rectangle(x, y, myWidth, myHeight);
            g.DrawImage(mg, rect, 0, 0, mg.Width, mg.Height, GraphicsUnit.Pixel);
            mg.Dispose();
            return bp;
        }

        #endregion

        #region 给图片加水印
        // 给图片加水印
        /// <summary>
        ///     添加水印(分图片水印与文字水印两种)
        /// </summary>
        /// <param name="base64String">原图片base64string</param>
        /// <param name="wmtType">要添加的水印的类型</param>
        /// <param name="sWaterMarkContent">
        ///     水印内容，若添加文字水印，此即为要添加的文字；
        ///     若要添加图片水印，此为水印图片的base64String
        /// </param>
        /// <param name="position"></param>
        /// <param name="transparence"></param>
        public static async Task<string> AddWaterMarkAsync(string base64String,
            WaterMarkType wmtType, string sWaterMarkContent,
            WatermarkPosition position, float transparence = 1.0f)
        {
            var image = GetBitmapImageFromBase64(base64String);
            //最好这里压缩 定义矩形
            var scale = image.Width > 1500 ? 3 : image.Width < 500 ? 1 : 2;
            image = scale > 1 ? ResizeImage(image, new Size(image.Width / scale, image.Height / scale)) : image;
            //image = ImageManipulator.Resize(image, 1f / scale);//
            //准备画板
            var g = Graphics.FromImage(image);
            g.SmoothingMode = SmoothingMode.HighQuality;//SmoothingMode.HighSpeed;//
            //g.CompositingQuality = CompositingQuality.HighSpeed;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.InterpolationMode = InterpolationMode.High;//InterpolationMode.Low;//
            g.DrawImage(image, 0, 0, image.Width, image.Height);
            Image waterImage = null;
            switch (wmtType)
            {
                case WaterMarkType.TextMark:
                    //文字转图片水印
                    waterImage = GenerateRandomTextImage(sWaterMarkContent);
                    break;
                case WaterMarkType.ImageMark:
                    //图片水印
                    waterImage = GetBitmapImageFromBase64(sWaterMarkContent);
                    break;
            }
            AddWatermarkImage(g, waterImage, position, image.Width, image.Height, transparence);
            //将画了水印的bitmap转为base64String返回保存到数据库
            var strbase64 = await GetBase64FromImageAsync(image);
            image.Dispose();
            g.Dispose();
            return strbase64;
        }

        /// <summary>
        /// 在原图上添加满屏水印
        /// </summary>
        /// <param name="source"></param>
        /// <param name="watermark"></param>
        /// <param name="pos"></param>
        /// <param name="fill"></param>
        public static void AddWatermark(Image source, Image watermark, Point pos, bool fill)
        {
            using (watermark)
            using (Graphics imageGraphics = Graphics.FromImage(source))
            using (Brush watermarkBrush = new TextureBrush(watermark))
            {
                if (fill)
                    imageGraphics.FillRectangle(watermarkBrush, new Rectangle(new Point(0, 0), source.Size));
                else
                    imageGraphics.DrawImage(watermark, pos);
            }
        }
        static Image GenerateRandomTextImage(string waterText, bool bgcolor = false)
        {
            var font = new Font(FontFamily.GenericMonospace, 30.0f);//GetRandFont();
            //var scale = 1;//GetRandScale();
            var angle = -15;//GetRandRotation();
            if (string.IsNullOrWhiteSpace(waterText))
            {
                waterText = "智管云资料管理系统"; //GetRandText();
            }
            //var transParent = 255 *25 / 100;//GetRandTransparency()
            var img = Generate(waterText, font, Color.FromArgb(255, 100, 255, 255), bgcolor ? Color.FromArgb(100, 200, 214, 224) : Color.Transparent);
            //img = ImageManipulator.Resize(img, scale);
            img = ImageManipulator.Rotate(img, angle);
            return img;
        }
        public static Image Generate(string text, Font font, Color textColor, Color backColor)
        {
            //first, create a dummy bitmap just to get a graphics object
            Image img = new Bitmap(1, 1);
            Graphics drawing = Graphics.FromImage(img);

            //measure the string to see how big the image needs to be
            SizeF textSize = drawing.MeasureString(text, font);

            //free up the dummy image and old graphics object
            img.Dispose();
            drawing.Dispose();

            //create a new image of the right size
            img = new Bitmap((int)textSize.Width, (int)textSize.Height);

            drawing = Graphics.FromImage(img);
            drawing.SmoothingMode = SmoothingMode.HighQuality;

            //paint the background
            drawing.Clear(backColor);

            //create a brush for the text
            Brush textBrush = new SolidBrush(textColor);

            drawing.DrawString(text, font, textBrush, 0, 0);

            drawing.Save();

            textBrush.Dispose();
            drawing.Dispose();

            return img;
        }
        /// <summary>
        ///     加水印文字
        /// </summary>
        /// <param name="picture">imge 对象</param>
        /// <param name="watermarkText">水印文字内容</param>
        /// <param name="watermarkPosition">水印位置</param>
        /// <param name="width">被加水印图片的宽</param>
        /// <param name="height">被加水印图片的高</param>
        /// <param name="transparence">透明度</param>
        private static void AddWatermarkText(Graphics picture, string watermarkText,
                    WatermarkPosition watermarkPosition, int width, int height, float transparence)
        {
            // 确定水印文字的字体大小
            int[] sizes = { 128, 64, 32, 30, 28, 26, 24, 22, 20, 18, 16, 14, 12, 10, 8, 6, 4 };
            Font crFont = null;
            var crSize = new SizeF();
            foreach (var t in sizes)
            {
                crFont = new Font("Arial Black", t, FontStyle.Bold);
                crSize = picture.MeasureString(watermarkText, crFont);
                if ((ushort)crSize.Width < (ushort)width)
                {
                    break;
                }
            }
            // 生成水印图片（将文字写到图片中）
            var floatBmp = new Bitmap((int)crSize.Width + 3,
                (int)crSize.Height + 3, PixelFormat.Format32bppArgb);
            var fg = Graphics.FromImage(floatBmp);
            var pt = new PointF(0, 0);
            // 画阴影文字
            Brush transparentBrush0 = new SolidBrush(Color.FromArgb(255, Color.Black));
            Brush transparentBrush1 = new SolidBrush(Color.FromArgb(255, Color.Black));
            fg.DrawString(watermarkText, crFont, transparentBrush0, pt.X, pt.Y + 1);
            fg.DrawString(watermarkText, crFont, transparentBrush0, pt.X + 1, pt.Y);
            fg.DrawString(watermarkText, crFont, transparentBrush1, pt.X + 1, pt.Y + 1);
            fg.DrawString(watermarkText, crFont, transparentBrush1, pt.X, pt.Y + 2);
            fg.DrawString(watermarkText, crFont, transparentBrush1, pt.X + 2, pt.Y);
            transparentBrush0.Dispose();
            transparentBrush1.Dispose();
            // 画文字
            fg.SmoothingMode = SmoothingMode.HighQuality;
            fg.DrawString(watermarkText,
                crFont, new SolidBrush(Color.White),
                pt.X, pt.Y, StringFormat.GenericDefault);
            // 保存刚才的操作
            fg.Save();
            fg.Dispose();
            // floatBmp.Save("d://WebSite//DIGITALKM//ttt.jpg");

            // 将水印图片加到原图中
            AddWatermarkImage(picture, new Bitmap(floatBmp), watermarkPosition, width, height, transparence);
        }

        /// <summary>
        ///     加水印图片
        /// </summary>
        /// <param name="picture">imge 对象</param>
        /// <param name="watermark">Image对象（以此图片为水印）</param>
        /// <param name="watermarkPosition">水印位置</param>
        /// <param name="width">被加水印图片的宽</param>
        /// <param name="height">被加水印图片的高</param>
        /// <param name="transparence">水印透明度</param>
        private static void AddWatermarkImage(Graphics picture, Image watermark,
             WatermarkPosition watermarkPosition, int width, int height, float transparence)
        {
            if (transparence < 0.0f || transparence > 1.0f) throw new ArgumentException("透明度值只能在0.0f~1.0f之间");
            var imageAttributes = new ImageAttributes();
            var colorMap = new ColorMap
            {
                OldColor = Color.FromArgb(255, 0, 255, 0),
                NewColor = Color.FromArgb(0, 0, 0, 0)
            };
            ColorMap[] remapTable = { colorMap };
            imageAttributes.SetRemapTable(remapTable, ColorAdjustType.Bitmap);
            //设置透明度
            float[][] colorMatrixElements =
            {
                new[] {1.0f, 0.0f, 0.0f, 0.0f, 0.0f},
                new[] {0.0f, 1.0f, 0.0f, 0.0f, 0.0f},
                new[] {0.0f, 0.0f, 1.0f, 0.0f, 0.0f},
                new[] {0.0f, 0.0f, 0.0f, transparence, 0.0f},
                new[] {0.0f, 0.0f, 0.0f, 0.0f, 1.0f}
            };

            var colorMatrix = new ColorMatrix(colorMatrixElements);

            imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.SkipGrays, ColorAdjustType.Bitmap);
            var xpos = 0;
            var ypos = 0;
            var watermarkWidth = 0;
            var watermarkHeight = 0;
            double bl;
            //计算水印图片的比率
            //取背景的1/2宽度来比较
            if ((width > watermark.Width * 2) && (height > watermark.Height * 2))
            {
                bl = 1;
            }
            else if ((width > watermark.Width * 2) && (height < watermark.Height * 2))
            {
                bl = Convert.ToDouble(height / 2) / Convert.ToDouble(watermark.Height);
            }
            else if ((width < watermark.Width * 2) && (height > watermark.Height * 2))
            {
                bl = Convert.ToDouble(width / 2) / Convert.ToDouble(watermark.Width);
            }
            else
            {
                if (width * watermark.Height > height * watermark.Width)
                {
                    bl = Convert.ToDouble(height / 2) / Convert.ToDouble(watermark.Height);
                }
                else
                {
                    bl = Convert.ToDouble(width / 2) / Convert.ToDouble(watermark.Width);
                }
            }
            watermarkWidth = Convert.ToInt32(watermark.Width * bl);
            watermarkHeight = Convert.ToInt32(watermark.Height * bl);
            switch (watermarkPosition)
            {
                case WatermarkPosition.LeftTop:
                    xpos = 10;
                    ypos = 10;
                    break;
                case WatermarkPosition.RightTop:
                    xpos = width - watermarkWidth - 10;
                    ypos = 10;
                    break;
                case WatermarkPosition.RigthBottom:
                    xpos = width - watermarkWidth - 10;
                    ypos = height - watermarkHeight - 10;
                    break;
                case WatermarkPosition.LeftBottom:
                    xpos = 10;
                    ypos = height - watermarkHeight - 10;
                    break;
            }
            picture.DrawImage(
                watermark,
                new Rectangle(xpos, ypos, watermarkWidth, watermarkHeight),
                0,
                0,
                watermark.Width,
                watermark.Height,
                GraphicsUnit.Pixel,
                imageAttributes);
            watermark.Dispose();
            imageAttributes.Dispose();
        }


        /// <summary>
        ///     生成: 原图绘制水印的 System.Drawing.Bitmap 对象
        /// </summary>
        /// <param name="sBitmap">原图 System.Drawing.Bitmap 对象: System.Drawing.Bitmap</param>
        /// <param name="wBitmap">水印 System.Drawing.Bitmap 对象: System.Drawing.Bitmap</param>
        /// <param name="position">
        ///     枚举 Uinatlex.ToolBox.ImageManager.WatermarkPosition :
        ///     Uinatlex.ToolBox.ImageManager.WatermarkPosition
        /// </param>
        /// <param name="margin">水印边距: int</param>
        /// <returns>返回: 原图绘制水印的 System.Drawing.Bitmap 对象 System.Drawing.Bitmap</returns>
        public static Bitmap CreateWatermark(Bitmap sBitmap, Bitmap wBitmap, WatermarkPosition position, int margin)
        {
            var graphics = Graphics.FromImage(sBitmap);
            graphics.DrawImage(wBitmap,
                GetWatermarkRectangle(position, sBitmap.Width, sBitmap.Height, wBitmap.Width, wBitmap.Height, margin));
            graphics.Dispose();
            wBitmap.Dispose();
            return sBitmap;
        }
        /// <summary>
        ///     获取: 枚举 Uinatlex.ToolBox.ImageManager.WatermarkPosition 对应的 System.Drawing.Rectangle 对象
        /// </summary>
        /// <param name="positon">
        ///     枚举 Uinatlex.ToolBox.ImageManager.WatermarkPosition:
        ///     Uinatlex.ToolBox.ImageManager.WatermarkPosition
        /// </param>
        /// <param name="oX">原图宽度: int</param>
        /// <param name="oY">原图高度: int</param>
        /// <param name="x">水印宽度: int</param>
        /// <param name="y">水印高度: int</param>
        /// <param name="i">边距: int</param>
        /// <returns>
        ///     返回: 枚举 Uinatlex.ToolBox.ImageManager.WatermarkPosition 对应的 System.Drawing.Rectangle 对象:
        ///     System.Drawing.Rectangle
        /// </returns>
        private static Rectangle GetWatermarkRectangle(WatermarkPosition positon, int oX, int oY, int x, int y, int i)
        {
            switch (positon)
            {
                case WatermarkPosition.LeftTop:
                    return new Rectangle(i, i, x, y);
                case WatermarkPosition.Left:
                    return new Rectangle(i, (oY - y) / 2, x, y);
                case WatermarkPosition.LeftBottom:
                    return new Rectangle(i, oY - y - i, x, y);
                case WatermarkPosition.Top:
                    return new Rectangle((oX - x) / 2, i, x, y);
                case WatermarkPosition.Center:
                    return new Rectangle((oX - x) / 2, (oY - y) / 2, x, y);
                case WatermarkPosition.Bottom:
                    return new Rectangle((oX - x) / 2, oY - y - i, x, y);
                case WatermarkPosition.RightTop:
                    return new Rectangle(oX - x - i, i, x, y);
                case WatermarkPosition.RightCenter:
                    return new Rectangle(oX - x - i, (oY - y) / 2, x, y);
                default:
                    return new Rectangle(oX - x - i, oY - y - i, x, y);
            }
        }

        #endregion

        #region base64转二进制再转Bitmap Bitmap再转二进制再转base64

        /// <summary>
        /// base64转二进制再转Bitmap
        /// </summary>
        /// <param name="base64String"></param>
        /// <returns></returns>
        public static Image GetBitmapImageFromBase64(string base64String)
        {
            base64String = base64String.Split(',')[1];
            base64String = base64String.Replace(" ", "+");
            var mod4 = base64String.Length % 4;
            if (mod4 > 0)
            {
                base64String += new string('=', 4 - mod4);
            }
            var ms = new MemoryStream(Convert.FromBase64String(base64String));
            var image = Image.FromStream(ms);//new Bitmap(ms);
            return image;
        }
        /// <summary>
        /// bitmap图片转Base64
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static async Task<string> GetBase64FromImageAsync(Image image)
        {
            var strbaser64 = "";
            try
            {
                //Bitmap bmp = new Bitmap(imagefile);              
                var ms = new MemoryStream();
                image.Save(ms, ImageFormat.Png);
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                await ms.ReadAsync(arr, 0, (int)ms.Length);
                strbaser64 = Convert.ToBase64String(arr).KeepBase64ImagePrefix();
                ms.Close();
            }
            catch (Exception)
            {
                throw new Exception("图片转换为base64字符串出错!");
            }
            return strbaser64;
        }

        /// <summary>
        /// 图片
        /// </summary>
        /// <param name="imagefile"></param>
        /// <returns></returns>
        public static string GetBase64FromImage(string imagefile)
        {
            string strbaser64 = "";
            try
            {
                Bitmap bmp = new Bitmap(imagefile);
                MemoryStream ms = new MemoryStream();
                bmp.Save(ms, ImageFormat.Jpeg);
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                ms.Close();
                strbaser64 = Convert.ToBase64String(arr).KeepBase64ImagePrefix();
            }
            catch (Exception)
            {
                throw new Exception("Something wrong during convert!");
            }
            return strbaser64;
        }

        #endregion


    }
}
