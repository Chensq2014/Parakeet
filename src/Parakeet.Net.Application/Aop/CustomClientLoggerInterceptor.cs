using Common.Storage;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;
using Serilog;
using System;

namespace Parakeet.Net.Aop
{
    /// <summary>
    /// grpc 客户端拦截器 
    /// </summary>
    public class CustomClientLoggerInterceptor : Interceptor
    {
        private readonly Lazy<ILogger<CustomClientLoggerInterceptor>> _logger;

        public CustomClientLoggerInterceptor(Lazy<ILogger<CustomClientLoggerInterceptor>> logger)
        {
            _logger = logger;
        }

        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
            TRequest request,
            ClientInterceptorContext<TRequest, TResponse> context,
            AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            this.LogAOP(context.Method);
            return continuation(request, context);
        }


        private void LogAOP<TRequest, TResponse>(Method<TRequest, TResponse> method)
            where TRequest : class
            where TResponse : class
        {
            _logger.Value.LogDebug($"{{0}}", $"{CacheKeys.LogCount++}、****************Client AOP 开始*****************");
            Log.Logger.Information($"{method.Name}---{method.FullName}--{method.ServiceName}");
            Log.Logger.Information($"Type: {method.Type}. Request: {typeof(TRequest)}. Response: {typeof(TResponse)}");
            _logger.Value.LogDebug($"{{0}}", $"{CacheKeys.LogCount++}、****************Client AOP 结束*****************");
        }
    }
}
