using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Parakeet.Net.Middleware
{
    public class StreamWriteMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<StreamWriteMiddleware> _logger;

        public StreamWriteMiddleware(RequestDelegate next, ILogger<StreamWriteMiddleware> logger)
        {
            this._next = next;
            this._logger = logger;
        }

        /// <summary>
        /// http://localhost:5726/Home/Info?name=Eleven
        /// 请求的值改不了，除非用postman请求，body里面传值才行
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            this._logger.LogWarning($"StreamWriteMiddleware Handle Request: " + context.Request.Path);

            context.Request.EnableBuffering();
            var requestStream = context.Request.Body;
            var responseStream = context.Response.Body;
            try
            {
                using var newRequest = new MemoryStream();
                context.Request.Body = newRequest; //替换request流
                using var newResponse = new MemoryStream();
                context.Response.Body = newResponse;//替换response流
                using (var reader = new StreamReader(requestStream))
                {
                    var requestBody = await reader.ReadToEndAsync();//读取原始请求流的内容
                    if (string.IsNullOrEmpty(requestBody))//为空直接走下一环节
                    {
                        await _next.Invoke(context);
                    }
                    else
                    {
                        await using var writer = new StreamWriter(newRequest);
                        await writer.WriteAsync(requestBody.ToUpper());//直接改成大写
                        await writer.FlushAsync();
                        newRequest.Position = 0;
                        context.Request.Body = newRequest;
                        await _next(context);
                    }
                }

                //获取和修改响应
                string responseBody = null;
                using (var reader = new StreamReader(newResponse))
                {
                    newResponse.Position = 0;
                    responseBody = await reader.ReadToEndAsync();
                }

                await using (var writer = new StreamWriter(responseStream))
                {
                    await writer.WriteAsync(responseBody.ToLower());//响应全部小写
                    await writer.FlushAsync();
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError($" http中间件发生错误: " + ex.ToString());
            }
            finally
            {
                context.Request.Body = requestStream;
                context.Response.Body = responseStream;
            }
        }
    }
}
