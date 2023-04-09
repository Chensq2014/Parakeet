//using Serilog;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 第三方组件账号与密钥基类
    /// </summary>
    public abstract class BaseAppOptionDto
    {
        protected BaseAppOptionDto()
        {
            //Log.Logger.Information($"{{0}}", $"{CacheKeys.LogCount++}、{nameof(BaseAppOptionDto)}");
        }

        /// <summary>
        /// 唯一表示
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 安全密钥
        /// </summary>
        public string SecurityKey { get; set; }
    }
}
