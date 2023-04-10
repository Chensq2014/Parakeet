using Parakeet.Net.Dtos;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using SixLabors.Shapes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;
using Volo.Abp;

namespace Parakeet.Net.Extensions
{
    /// <summary>
    /// 图片水印，验证码
    /// </summary>
    public class ImageExtention
    {
        #region 水印图常量
        /// <summary>
        /// 水印图常量 分辨率 200*200
        /// </summary>
        public const string WatermarkBaes64String =
            "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";

        #endregion
        
        /// <summary>
        /// 画文字水印+图片水印
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static async Task<string> AddWatermarkAsync(WatermarkInputDto input)
        {
            var base64String = RemoveBase64ImagePrefix(input.Base64String);

            using (var image = Image.Load(Convert.FromBase64String(base64String)))//水印图200*200
            using (var watermarkImage = Image.Load(Convert.FromBase64String(WatermarkBaes64String)))
            {
                #region 初始化数据

                var scaleWidth = image.Width;
                var scaleHeight = image.Height;
                //var encoder = new PngEncoder { CompressionLevel = 9, BitDepth = PngBitDepth.Bit8 };//, Quantizer = new WuQuantizer(200)
                //var jpgEncoder = new JpegEncoder { Quality = 90 };
                #endregion

                #region 是否放大尺寸 至少2倍水印的宽度等比放大

                if (scaleWidth < watermarkImage.Width)
                {
                    while (scaleWidth < watermarkImage.Width)
                    {
                        scaleWidth = scaleWidth * 2;
                        scaleHeight = scaleHeight * 2;
                    }
                }

                #endregion

                #region 先把image按png压缩之后再画水印

                //var sampler = new BicubicResampler();//HermiteResampler();//new CatmullRomResampler();//new BoxResampler();//new BicubicResampler();
                image.Mutate(x => x.Resize(scaleWidth, scaleHeight, true));//.Grayscale()
                //byte[] compressResult;
                //using (var compresStream = new MemoryStream())
                //{
                //    image.SaveAsPng(compresStream, encoder);
                //    compressResult = await GetImageBytesAsync(compresStream);
                //}
                //var compressImage = Image.Load(compressResult);

                #endregion

                switch (input.WaterMarkType)
                {
                    case Enums.WaterMarkType.TextMark:
                        #region 使用字体画文字水印

                        ////"C:\\Windows\\Fonts\\arial.ttf"  //字体文件
                        ////linux字体路径：/root/.local/share/fonts/truetype/dejavu/DejaVuSansMono.ttf,
                        ////usr/share/fonts/truetype/DejaVuSansMono.ttf
                        //var fontName = new FontCollection().Install(input.FontPath);
                        var scaledFont = input.FontName.CreateFont(15, FontStyle.Italic);
                        var color = Rgba32.White;
                        image.Mutate(context =>
                        {
                            var textGraphicOptions = new TextGraphicsOptions(true)
                            {
                                HorizontalAlignment = HorizontalAlignment.Left,
                                VerticalAlignment = VerticalAlignment.Center,
                                WrapTextWidth = scaleWidth * 0.3f,
                                ColorBlendingMode = PixelColorBlendingMode.HardLight
                            };

                            //在下方画一条文字水印即可
                            var width = scaleWidth * 0.1f;
                            var height = scaleHeight * 0.2f;
                            var next = new PointF(width, height);
                            context.DrawText(textGraphicOptions, input.WatermarkText, scaledFont, color, next);

                            #region  控制next坐标点 循环设置横纵坐标画文本水印 
                            //while (width < scaleWidth * 0.7f && height < scaleHeight - scaledFont.Size * 3)
                            //{
                            //    var next = new PointF(width, height);
                            //    context.DrawText(textGraphicOptions, watermarkText, scaledFont, color, next);
                            //    width = width + scaleWidth * 0.2f;
                            //    height = height + scaleHeight * 0.2f;
                            //}
                            #endregion
                        });

                        #region 老版本使用Apply

                        //image.Mutate(x => x.Apply(img =>
                        //{
                        //    var imagWidth = img.Width;
                        //    var imgHeight = img.Height;
                        //    var color = Rgba32.White;
                        //    var textGraphicOptions = new TextGraphicsOptions(true)
                        //    {
                        //        HorizontalAlignment = HorizontalAlignment.Left,
                        //        VerticalAlignment = VerticalAlignment.Center,
                        //        WrapTextWidth = imagWidth * 0.3f,
                        //        ColorBlendingMode = PixelColorBlendingMode.HardLight
                        //    };
                        //    //循环画水印 循环设置横纵坐标 控制循环
                        //    var width = imagWidth * 0.1f;
                        //    var height = imgHeight * 0.2f;
                        //    while (width < imagWidth * 0.7f && height < imgHeight - scaledFont.Size * 3)
                        //    {
                        //        var next = new PointF(width, height);
                        //        img.Mutate(i => i.DrawText(textGraphicOptions, watermarkText, scaledFont, color, next));
                        //        width = width + imagWidth * 0.2f;
                        //        height = height + imgHeight * 0.2f;
                        //    }
                        //}));

                        #endregion

                        #endregion
                        break;
                    case Enums.WaterMarkType.ImageMark:
                        #region 在图上设置横纵坐标循环画水印
                        image.Mutate(context =>
                        {
                            var transparent = 0.1f;//透明度
                            if (input.IsCircle)
                            {
                                #region 循环画水印
                                var xIncrease = (int)(watermarkImage.Width * 0.8f);
                                var yIncrease = watermarkImage.Height;
                                for (var width = 0; width < scaleWidth; width = width + xIncrease)
                                {
                                    for (var height = 0; height < scaleHeight; height = height + yIncrease)
                                    {
                                        var position = new Point(width, height);
                                        context.DrawImage(watermarkImage, position, new GraphicsOptions(true, transparent));
                                    }
                                }
                                #endregion
                            }
                            else
                            {
                                #region 指定位置画一个水印 
                                context.DrawImage(watermarkImage, new Point(scaleWidth / 2, scaleHeight / 2), new GraphicsOptions(true, transparent));
                                #endregion
                            }
                        });

                        #region 老版本使用Apply
                        //新版本不支持Apply 以上方法画水印
                        //image.Mutate(x => x.Apply(img =>
                        //{
                        //    var imagWidth = img.Width;
                        //    var imgHeight = img.Height;
                        //    var xIncrease = (int)(watermarkImage.Width * 0.8f);
                        //    var yIncrease = watermarkImage.Height;
                        //    var transparent = 0.1f;
                        //    for (var width = 0; width < imagWidth; width = width + xIncrease)
                        //    {
                        //        for (var height = 0; height < imgHeight; height = height + yIncrease)
                        //        {
                        //            var position = new Point(width, height);
                        //            img.Mutate(i => i.DrawImage(watermarkImage, position, new GraphicsOptions(true, transparent)));
                        //        }
                        //    }
                        //}));
                        #endregion

                        #endregion
                        break;
                }

                #region 将水印图转为压缩后的png图片并转为二进制文件再转为base64字符串

                using (var stream = new MemoryStream())
                {
                    //var encoder = image.GetConfiguration().ImageFormatsManager.FindEncoder(PngFormat.Instance);
                    //compressImage.Save(stream,encoder);
                    image.SaveAsPng(stream, input.EnbEncoder);//compressImage
                    var result = await GetImageBytesAsync(stream);
                    base64String = Convert.ToBase64String(result).KeepBase64ImagePrefix();
                    input.WatermarkBase64String = base64String;
                }

                return base64String;

                #endregion
            }
        }

        /// <summary>
        /// 根据内存流返回二进制字节数组
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static async Task<byte[]> GetImageBytesAsync(MemoryStream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            var result = new byte[stream.Length];
            await stream.ReadAsync(result, 0, result.Length);
            return result;
        }

        /// <summary>
        /// 去掉Base64Image字符串前缀(扩展名)
        /// </summary>
        /// <param name="base64String"></param>
        /// <returns></returns>
        public static string RemoveBase64ImagePrefix(string base64String)
        {
            base64String = base64String.Split(',')[1];
            base64String = base64String.Replace(" ", "+");
            var mod4 = base64String.Length % 4;
            if (mod4 > 0)
            {
                base64String += new string('=', 4 - mod4);
            }
            return base64String;
        }

        /// <summary>
        /// 检查是否传递的是png图片的base64String
        /// </summary>
        /// <param name="base64String"></param>
        /// <returns></returns>
        public static bool CheckIsPngFromBase64String(string base64String)
        {
            if (string.IsNullOrEmpty(base64String) || base64String.Split(',').Length == 1)
            {
                return true;
            }
            var isPng = base64String.Contains("data:image/png;base64", StringComparison.OrdinalIgnoreCase);
            if (!isPng)
            {
                throw new UserFriendlyException("只能上传png格式图片");
            }
            return true;
        }

        #region 画验证码

        /// <summary>
        /// 画点+画字=验证码   返回验证码byte[]  
        /// </summary>
        /// <param name="input">验证码及图片参数</param>
        /// <returns>验证码byte[]</returns>
        public static byte[] GetValidCodeBytes(ValidCodeImageInputDto input)
        {
            byte[] bytes;
            try
            {
                var dianWith = 1; //点宽度
                var xSpace = 10;  //点与点之间x坐标间隔
                var ySpace = 5;    //y坐标间隔
                var textLenth = input.Code.Length;  //文字长度
                var maxX = input.XLength / textLenth; //每个文字最大x宽度
                var prevWenZiX = 0; //前面一个文字的x坐标
                var size = 16;//字体大小

                //字体
                var font = new Font(input.FontName, size);  //字体

                //点坐标
                var listPath = new List<IPath>();
                for (var i = 0; i < input.XLength / xSpace; i++)
                {
                    for (var j = 0; j < input.YLength / ySpace; j++)
                    {
                        var position = new Vector2(i * xSpace, j * ySpace);
                        var linerLine = new LinearLineSegment(position, position);
                        var shapesPath = new SixLabors.Shapes.Path(linerLine);
                        listPath.Add(shapesPath);
                    }
                }

                //画图
                using (Image<Rgba32> image = new Image<Rgba32>(input.XLength, input.YLength))   //画布大小
                {
                    image.Mutate(x =>
                    {
                        //逐个画字
                        for (int i = 0; i < textLenth; i++)
                        {
                            //当前的要输出的字
                            var text = input.Code.Substring(i, 1);

                            //文字坐标
                            var textPoint = new Vector2();
                            var maxXX = prevWenZiX + (maxX - size);
                            textPoint.X = new Random().Next(prevWenZiX, maxXX);
                            textPoint.Y = new Random().Next(0, input.YLength - size);

                            prevWenZiX = Convert.ToInt32(Math.Floor(textPoint.X)) + size;

                            //画字
                            x.DrawText(
                                   text,   //文字内容
                                   font,
                                   i % 2 > 0 ? Rgba32.HotPink : Rgba32.Red,
                                   textPoint);
                        }

                        //画点 
                        x.BackgroundColor(Rgba32.WhiteSmoke).   //画布背景
                                     Draw(
                                     Pens.Dot(Rgba32.HotPink, dianWith),   //大小
                                     new PathCollection(listPath)  //坐标集合
                                 );
                    });
                    using (MemoryStream stream = new MemoryStream())
                    {
                        image.SaveAsPng(stream);
                        bytes = stream.GetBuffer();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
            return bytes;
        }

        /// <summary>
        /// 画验证码 并返回验证码base64字符串
        /// </summary>
        /// <param name="input">验证码及图片参数</param>
        /// <returns>验证码base64字符串</returns>
        public static string GetValidCodeString(ValidCodeImageInputDto input)
        {
            var bytes = GetValidCodeBytes(input);
            input.CodeBase64String = Convert.ToBase64String(bytes).KeepBase64ImagePrefix();
            return input.CodeBase64String;
        }

        #endregion
    }
}
