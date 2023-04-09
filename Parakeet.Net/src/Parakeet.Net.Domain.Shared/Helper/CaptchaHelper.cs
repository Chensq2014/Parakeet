using Parakeet.Net.Dtos;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace Parakeet.Net.Helper
{
    public class CaptchaHelper
    {
        const string Letters = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <param name="length">验证码长度</param>
        /// <returns></returns>
        public static string GenerateCaptchaCode(int length = 4)
        {
            var rand = new Random();
            var maxRand = Letters.Length - 1;
            var sb = new StringBuilder();

            for (var i = 0; i < length; i++)
            {
                var index = rand.Next(maxRand);
                sb.Append(Letters[index]);
            }

            return sb.ToString();
        }

        /// <summary>
        /// 生成验证码图片（越宽越高越容易识别）
        /// </summary>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="captchaCode">验证码</param>
        /// <returns></returns>
        public static CaptchaResultDto GenerateCaptchaImage(int width, int height, string captchaCode)
        {
            using (var baseMap = new Bitmap(width, height))
            using (var graph = Graphics.FromImage(baseMap))
            {
                var rand = new Random();

                graph.Clear(GetRandomLightColor());

                DrawCaptchaCode();//画验证码

                DrawDisorderLine();//画干扰线

                AdjustRippleEffect();//调整，扭曲图片

                using (var ms = new MemoryStream())
                {
                    baseMap.Save(ms, ImageFormat.Png);
                    return new CaptchaResultDto { CaptchaCode = captchaCode, CaptchaByteData = ms.ToArray(), Timestamp = DateTime.Now };
                }

                int GetFontSize(int imageWidth, int captchCodeCount)//字体大小
                {
                    var averageSize = imageWidth / captchCodeCount;
                    return Convert.ToInt32(averageSize);
                }

                Color GetRandomDeepColor()//随机颜色
                {
                    int redlow = 160, greenLow = 100, blueLow = 160;
                    return Color.FromArgb(rand.Next(redlow), rand.Next(greenLow), rand.Next(blueLow));
                }

                Color GetRandomLightColor()//随机线条
                {
                    int low = 180, high = 255;

                    var nRend = rand.Next(high) % (high - low) + low;
                    var nGreen = rand.Next(high) % (high - low) + low;
                    var nBlue = rand.Next(high) % (high - low) + low;

                    return Color.FromArgb(nRend, nGreen, nBlue);
                }

                void DrawCaptchaCode()//画验证码方法
                {
                    var fontBrush = new SolidBrush(Color.Black);
                    var fontSize = GetFontSize(width, captchaCode.Length);
                    var font = new Font(FontFamily.GenericSerif, fontSize, FontStyle.Bold, GraphicsUnit.Pixel);
                    for (int i = 0; i < captchaCode.Length; i++)
                    {
                        fontBrush.Color = GetRandomDeepColor();

                        int shiftPx = fontSize / 6;

                        float x = i * fontSize + rand.Next(-shiftPx, shiftPx) + rand.Next(-shiftPx, shiftPx);
                        int maxY = height - fontSize;
                        if (maxY < 0) maxY = 0;
                        float y = rand.Next(0, maxY);

                        graph.DrawString(captchaCode[i].ToString(), font, fontBrush, x, y);
                    }
                }

                void DrawDisorderLine()//画验干扰线方法
                {
                    var linePen = new Pen(new SolidBrush(Color.Black), 2);
                    for (int i = 0; i < rand.Next(3, 5); i++)
                    {
                        linePen.Color = GetRandomDeepColor();

                        Point startPoint = new Point(rand.Next(0, width), rand.Next(0, height));
                        Point endPoint = new Point(rand.Next(0, width), rand.Next(0, height));
                        graph.DrawLine(linePen, startPoint, endPoint);

                        Point bezierPoint1 = new Point(rand.Next(0, width), rand.Next(0, height));
                        Point bezierPoint2 = new Point(rand.Next(0, width), rand.Next(0, height));

                        graph.DrawBezier(linePen, startPoint, bezierPoint1, bezierPoint2, endPoint);
                    }
                }

                void AdjustRippleEffect()
                {
                    short nWave = 6;
                    int nWidth = baseMap.Width;
                    int nHeight = baseMap.Height;

                    var pt = new Point[nWidth, nHeight];

                    double newX, newY;
                    double xo, yo;

                    for (int x = 0; x < nWidth; ++x)
                    {
                        for (int y = 0; y < nHeight; ++y)
                        {
                            xo = ((double)nWave * Math.Sin(2.0 * 3.1415 * (float)y / 128.0));
                            yo = ((double)nWave * Math.Cos(2.0 * 3.1415 * (float)x / 128.0));

                            newX = (x + xo);
                            newY = (y + yo);

                            //if (newX > 0 && newX < nWidth)
                            //{
                            //    pt[x, y].X = newX > 0 && newX < nWidth ? (int)newX : 0;
                            //}
                            //else
                            //{
                            //    pt[x, y].X = 0;
                            //}


                            //if (newY > 0 && newY < nHeight)
                            //{
                            //    pt[x, y].Y = (int)newY;
                            //}
                            //else
                            //{
                            //    pt[x, y].Y = 0;
                            //}

                            pt[x, y].X = newX > 0 && newX < nWidth ? (int)newX : 0;
                            pt[x, y].Y = newY > 0 && newY < nHeight ? (int)newY : 0;
                        }
                    }

                    var bSrc = (Bitmap)baseMap.Clone();

                    var bitmapData = baseMap.LockBits(new Rectangle(0, 0, baseMap.Width, baseMap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                    var bmSrc = bSrc.LockBits(new Rectangle(0, 0, bSrc.Width, bSrc.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

                    int scanline = bitmapData.Stride;

                    IntPtr Scan0 = bitmapData.Scan0;
                    IntPtr SrcScan0 = bmSrc.Scan0;

                    unsafe
                    {
                        byte* p = (byte*)(void*)Scan0;
                        byte* pSrc = (byte*)(void*)SrcScan0;

                        int nOffset = bitmapData.Stride - baseMap.Width * 3;

                        int xOffset, yOffset;

                        for (int y = 0; y < nHeight; ++y)
                        {
                            for (int x = 0; x < nWidth; ++x)
                            {
                                xOffset = pt[x, y].X;
                                yOffset = pt[x, y].Y;

                                if (yOffset >= 0 && yOffset < nHeight && xOffset >= 0 && xOffset < nWidth)
                                {
                                    p[0] = pSrc[(yOffset * scanline) + (xOffset * 3)];
                                    p[1] = pSrc[(yOffset * scanline) + (xOffset * 3) + 1];
                                    p[2] = pSrc[(yOffset * scanline) + (xOffset * 3) + 2];
                                }

                                p += 3;
                            }
                            p += nOffset;
                        }
                    }

                    baseMap.UnlockBits(bitmapData);
                    bSrc.UnlockBits(bmSrc);
                    bSrc.Dispose();
                }
            }
        }
    }
}
