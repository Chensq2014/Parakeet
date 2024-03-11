using Common.Dtos;
using Common.Extensions;
using Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Volo.Abp.Caching;

namespace Parakeet.Net.PersonalCaches
{
    /// <summary>
    /// 个性化缓存 统一几种管理
    /// </summary>
    [Description("个性化缓存")]
    public class PersonalCacheAppService : CustomerAppService, IPersonalCacheAppService
    {
        private readonly IDistributedCache<string> _cacheManager;
        private readonly IHttpContextAccessor _httpContext;
        //private readonly DemoConfig _demoConfig;
        public PersonalCacheAppService(
            IHttpContextAccessor httpContext,
            //IOptionsMonitor<DemoConfig> demoConfig,
            IDistributedCache<string> cacheManager)
        {
            _httpContext = httpContext;
            _cacheManager = cacheManager;
            //_demoConfig = demoConfig.CurrentValue;
        }

        #region 使用windows ImageHelper画验证码 返回文件与bitmap

        ///// <summary>
        ///// 获取图片验证码(缓存验证码5分钟有效)
        ///// </summary>
        ///// <returns></returns>
        //public async Task<Bitmap> Verify()
        //{
        //    var image = ImageHelper.CreateVerifyCode(out string code);
        //    await _cacheManager.SetAsync("CheckCode", code);
        //    //var strbase64 = await ImageHelper.GetBase64FromImageAsync(image);
        //    return image;
        //}

        ///// <summary>
        ///// 获取图片验证码gif文件(缓存验证码5分钟有效)
        ///// </summary>
        ///// <returns></returns>
        //public async Task<IActionResult> VerifyCode()
        //{
        //    var bitmap = VerifyCodeHelper.CreateVerifyCode(out string code);
        //    await _cacheManager.GetCache(NetConsts.PersonalCaches).SetAsync("CheckCode", code);
        //    MemoryStream stream = new MemoryStream();
        //    bitmap.Save(stream, ImageFormat.Gif);//png/gif
        //    return FileExtension.FileContentResult(stream.ToArray(), "image/gif");
        //}

        #endregion

        #region SixLabors.ImageSharp 画验证码 返回文件与base64

        /// <summary>
        /// Get获取验证码图片
        /// </summary>
        /// <returns></returns>
        public async Task<FileResult> GetValidCodeImage()
        {
            var input = new ValidCodeImageInputDto();
            await SetCacheCode(input.Code);
            var bytes = ImageExtention.GetValidCodeBytes(input);
            return FileContentResult(bytes, "image/png");
        }

        /// <summary>
        /// Get获取验证码图片Base64String
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetValidCodeBase64String()
        {
            var input = new ValidCodeImageInputDto();
            await SetCacheCode(input.Code);
            return ImageExtention.GetValidCodeString(input);
        }

        /// <summary>
        /// 设置缓存验证码5分钟,每次都重新设置验证码
        /// </summary>
        /// <param name="code">需要缓存的验证码</param>
        /// <returns></returns>
        public async Task SetCacheCode(string code)
        {
            var cacheKey = $"{nameof(PersonalCacheAppService)}_VerifyCode";
            await _cacheManager.SetAsync(cacheKey, code, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });
        }

        /// <summary>
        /// 获取缓存验证码
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetCacheCode()
        {
            var cacheKey = $"{nameof(PersonalCacheAppService)}_VerifyCode";
            return await _cacheManager.GetAsync(cacheKey);
        }

        /// <summary>
        /// 清空指定key的缓存
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task ClearCache(InputNameDto input)
        {
            await _cacheManager.RemoveAsync(input.Name);
        }
        #endregion


        #region 搬移baseController中的文件扩展方法
        /// <summary>
        /// Returns a file with the specified <paramref name="fileContents" /> as content (<see cref="F:Microsoft.AspNetCore.Http.StatusCodes.Status200OK" />), the
        /// specified <paramref name="contentType" /> as the Content-Type and the specified <paramref name="fileDownloadName" /> as the suggested file name.
        /// This supports range requests (<see cref="F:Microsoft.AspNetCore.Http.StatusCodes.Status206PartialContent" /> or
        /// <see cref="F:Microsoft.AspNetCore.Http.StatusCodes.Status416RangeNotSatisfiable" /> if the range is not satisfiable).
        /// </summary>
        /// <param name="fileContents">The file contents.</param>
        /// <param name="contentType">The Content-Type of the file.</param>
        /// <param name="fileDownloadName">The suggested file name.</param>
        /// <returns>The created <see cref="T:Microsoft.AspNetCore.Mvc.FileContentResult" /> for the response.</returns>
        [NonAction]
        public static FileContentResult FileContentResult(byte[] fileContents, string contentType, string fileDownloadName = null)
        {
            FileContentResult fileContentResult = new FileContentResult(fileContents, contentType);
            fileContentResult.FileDownloadName = fileDownloadName;
            return fileContentResult;
        }

        /// <summary>
        /// Returns the file specified by <paramref name="physicalPath" /> (<see cref="F:Microsoft.AspNetCore.Http.StatusCodes.Status200OK" />) with the
        /// specified <paramref name="contentType" /> as the Content-Type and the
        /// specified <paramref name="fileDownloadName" /> as the suggested file name.
        /// This supports range requests (<see cref="F:Microsoft.AspNetCore.Http.StatusCodes.Status206PartialContent" /> or
        /// <see cref="F:Microsoft.AspNetCore.Http.StatusCodes.Status416RangeNotSatisfiable" /> if the range is not satisfiable).
        /// </summary>
        /// <param name="physicalPath">The path to the file. The path must be an absolute path.</param>
        /// <param name="contentType">The Content-Type of the file.</param>
        /// <param name="fileDownloadName">The suggested file name.</param>
        /// <returns>The created <see cref="T:Microsoft.AspNetCore.Mvc.PhysicalFileResult" /> for the response.</returns>
        [NonAction]
        public static PhysicalFileResult PhysicalFile(string physicalPath, string contentType, string fileDownloadName)
        {
            PhysicalFileResult physicalFileResult = new PhysicalFileResult(physicalPath, contentType);
            physicalFileResult.FileDownloadName = fileDownloadName;
            return physicalFileResult;
        }

        #endregion
    }
}
