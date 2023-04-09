using Volo.Abp.DependencyInjection;

namespace Parakeet.Net.Cache
{
    public interface ITempFileCacheManager : ITransientDependency
    {
        /// <summary>
        /// 设置缓存二进制流
        /// </summary>
        /// <param name="token"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        Task SetFile(string token, byte[] content);

        /// <summary>
        /// 获取文件二进制流
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<byte[]> GetFile(string token);
    }
}
