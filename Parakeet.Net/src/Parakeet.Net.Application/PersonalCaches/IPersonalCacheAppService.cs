using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Parakeet.Net.Dtos;
using Volo.Abp.DependencyInjection;

namespace Parakeet.Net.PersonalCaches
{
    /// <summary>
    /// 个性化缓存
    /// </summary>
    public interface IPersonalCacheAppService : ITransientDependency
    {
        //Task<Bitmap> Verify();

        //Task<IActionResult> VerifyCode();

        Task<FileResult> GetValidCodeImage();

        Task<string> GetValidCodeBase64String();

        /// <summary>
        /// 设置缓存验证码5分钟,每次都重新设置验证码
        /// </summary>
        /// <param name="code">需要缓存的验证码</param>
        /// <returns></returns>
        Task SetCacheCode(string code);

        /// <summary>
        /// 获取缓存验证码
        /// </summary>
        /// <returns></returns>
        Task<string> GetCacheCode();

        /// <summary>
        /// 清空指定key的缓存
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task ClearCache(InputNameDto input);
    }
}
