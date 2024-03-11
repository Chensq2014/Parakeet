using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Parakeet.Net.Middleware
{
    public class StreamReadMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<StreamReadMiddleware> _logger;

        public StreamReadMiddleware(RequestDelegate next, ILogger<StreamReadMiddleware> logger)
        {
            this._next = next;
            this._logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            #region 简单版本--错误的
            //#region Request
            //var requestReader = new StreamReader(context.Request.Body);
            //var requestContent = requestReader.ReadToEnd();
            //Console.WriteLine($"Request Body: {requestContent}");
            //#endregion

            //await _next(context);

            //#region Response
            //var responseReader = new StreamReader(context.Response.Body);
            //var responseContent = responseReader.ReadToEnd();
            //#endregion
            #endregion

            #region 正确的
            {
                context.Request.EnableBuffering();//允许重复读取
                var reader = new StreamReader(context.Request.Body);
                var content = await reader.ReadToEndAsync();//异步
                Console.WriteLine($"StreamRead Request Body: {content}");
                context.Request.Body.Position = 0;//重置，后续才能正确读取
            }

            {
                var responseStream = context.Response.Body;
                using var newStream = new MemoryStream();
                context.Response.Body = newStream;//将Body换成新的流，空的，，前面的是responseStream
                await _next(context);

                newStream.Position = 0;
                var responseReader = new StreamReader(newStream);
                var responseContent = await responseReader.ReadToEndAsync();
                Console.WriteLine($"Response Body: {responseContent}");

                newStream.Position = 0;
                await newStream.CopyToAsync(responseStream);
                context.Response.Body = responseStream;
            }
            #endregion
        }
    }
}
