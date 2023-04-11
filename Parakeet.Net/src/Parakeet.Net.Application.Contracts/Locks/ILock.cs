using System;
using System.Threading.Tasks;

namespace Parakeet.Net.Locks
{
    /// <summary>
    /// 业务锁
    /// </summary>
    public interface ILock
    {
        #region 异步锁定解锁
        
        /// <summary>
        /// 锁定，成功锁定返回true，false代表已被锁定
        /// </summary>
        /// <param name="key">锁定标识</param>
        /// <param name="expiration">锁定时间</param>
        /// <returns></returns>
        Task<bool> LockAsync(string key, TimeSpan? expiration = null);

        /// <summary>
        /// 解除锁定
        /// </summary>
        Task UnLockAsync();

        #endregion

        #region 同步锁定解锁
        
        /// <summary>
        /// 锁定，成功锁定返回true，false代表已被锁定
        /// </summary>
        /// <param name="key">锁定标识</param>
        /// <param name="expiration">锁定时间</param>
        /// <returns></returns>
        bool Lock(string key, TimeSpan? expiration = null);

        /// <summary>
        /// 解除锁定
        /// </summary>
        void UnLock();

        #endregion
    }
}
