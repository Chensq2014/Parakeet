using System;
using Common.Locks;
using Volo.Abp;

namespace Parakeet.Net.Locks
{
    public class UserLock : IUserLock
    {
        #region 用户锁基本信息
        /// <summary>
        /// 唯一标识key
        /// </summary>
        public const string UserLockKey = "UserLockKey-{0}-{1}";

        /// <summary>
        /// 业务锁
        /// </summary>
        public ILock Lock;

        /// <summary>
        /// 用户Id
        /// </summary>
        public Guid? UserId { get; set; }

        /// <summary>
        /// 锁定的方法
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// 是否被锁定
        /// </summary>
        public bool IsLocked { get; set; }

        /// <summary>
        /// 自动解锁时间，默认为1分钟
        /// </summary>
        public TimeSpan? Expiration { get; set; }

        #endregion

        /// <summary>
        /// 用户业务锁
        /// </summary>
        /// <param name="lock">业务锁</param>
        /// <param name="userId">用户Id</param>
        /// <param name="action">锁定的操作</param>
        /// <param name="expiration">自动解锁时间，默认为1分钟</param>
        public UserLock(ILock @lock, Guid? userId = default(Guid?), string action = null, TimeSpan? expiration = null)
        {
            Lock = @lock;
            UserId = userId;
            Action = action ?? "Any";
            Expiration = expiration ?? TimeSpan.FromMinutes(1);
            AddLock();
        }

        private void AddLock()
        {
            IsLocked = Lock.Lock(string.Format(UserLockKey, UserId, Action), Expiration);
        }

        public void Dispose()
        {
            Lock.UnLock();
        }

        public IUserLock CheckLock()
        {
            if (!IsLocked)
            {
                throw new UserFriendlyException("用户调用此接口频率太快");
            }
            return this;
        }
    }
}
