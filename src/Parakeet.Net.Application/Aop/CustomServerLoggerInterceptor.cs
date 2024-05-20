using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Threading.Tasks;

namespace Parakeet.Net.Aop
{
    /// <summary>
    /// grpc 服务端拦截器 
    /// </summary>
    public class CustomServerLoggerInterceptor : Interceptor
    {
        private readonly ILogger<CustomServerLoggerInterceptor> _logger;

        public CustomServerLoggerInterceptor(ILogger<CustomServerLoggerInterceptor> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 简单RPC--异步式调用
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <param name="continuation"></param>
        /// <returns></returns>
        public override Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
            TRequest request,
            ServerCallContext context,
            UnaryServerMethod<TRequest, TResponse> continuation)
        {
            LogAOP<TRequest, TResponse>(MethodType.Unary, context);
            return continuation(request, context);
        }

        private void LogAOP<TRequest, TResponse>(MethodType methodType, ServerCallContext context)
            where TRequest : class
            where TResponse : class
        {
            _logger.LogInformation("****************Logger Server AOP 开始*****************");
            Log.Logger.Information($"{context.RequestHeaders[0]}---{context.Host}--{context.Method}--{context.Peer}");
            Log.Logger.Information($"Type: {methodType}. Request: {typeof(TRequest)}. Response: {typeof(TResponse)}");
            _logger.LogInformation("**************** Server AOP 结束*****************");
        }
    }
}
