using System;

namespace Parakeet.Net.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Method)]
    public class UserCacheLockAttribute : Attribute
    {
        /// <summary>
        /// 要锁定的业务方法名称
        /// </summary>
        public string OperationName { get; set; }

        /// <summary>
        /// 锁定时间，默认一分钟
        /// </summary>
        public TimeSpan Expiration { get; set; }

        /// <summary>
        /// 用户缓存业务所锁
        /// </summary>
        /// <param name="operationName">业务方法名称</param>
        /// <param name="expirationMinutes">锁定时间，默认一分钟</param>
        public UserCacheLockAttribute(string operationName = "", int expirationMinutes = 1)
        {
            OperationName = operationName;
            Expiration = TimeSpan.FromMinutes(expirationMinutes);
        }
    }
}
