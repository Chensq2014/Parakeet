using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;

namespace Parakeet.Net.Middleware
{
    /// <summary>
    /// 中间件工厂 用于构造IMiddleware接口的中间件
    /// </summary>
    public  class ParakeetMiddlewareFactory:IMiddlewareFactory
    {
        private readonly IServiceProvider _iServiceProvider;
        private readonly ILogger _logger;

        public ParakeetMiddlewareFactory(IServiceProvider iServiceProvider, ILogger<ParakeetMiddlewareFactory> logger)
        {
            _iServiceProvider = iServiceProvider;
            _logger = logger;
        }

        /// <summary>
        /// 创建中间件
        /// </summary>
        /// <param name="middlewareType"></param>
        /// <returns></returns>
        public IMiddleware Create(Type middlewareType)
        {
            return (IMiddleware)_iServiceProvider.GetService(middlewareType);
        }

        /// <summary>
        /// 释放中间件  需要中间件继承IDisposable接口
        /// </summary>
        /// <param name="middleware"></param>
        public void Release(IMiddleware middleware)
        {
            if (middleware != null)
            {
                (middleware as IDisposable)?.Dispose();
            }
        }
    }
}
