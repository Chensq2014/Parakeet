using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Parakeet.Net.Middleware
{
    /// <summary>
    /// 支持在返回HTML时，将返回的Stream保存到指定目录
    /// </summary>
    public class StaticPageMiddleware
    {
        private readonly RequestDelegate _next;
        private string _directoryPath;
        private bool _supportDelete;
        private bool _supportWarmup;

        public StaticPageMiddleware(RequestDelegate next, string directoryPath, bool supportDelete, bool supportWarmup)
        {
            _next = next;
            _directoryPath = directoryPath;
            _supportDelete = supportDelete;
            _supportWarmup = supportWarmup;
        }

        /// <summary>
        /// 任意HTTP请求，都要经过这个方法
        /// 抓到响应，并保存成HTML静态页
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            //context.Request.IsAjaxRequest()
            if (context.Request.Headers["X-Requested-With"].Equals("XMLHttpRequest"))
            {
                await _next(context);
            }
            else
            if (_supportDelete && context.Request.Query["ActionHeader"].Equals("Delete"))
            {
                DeleteHmtl(context.Request.Path.Value);
                context.Response.StatusCode = 200;
            }
            else if (this._supportWarmup && context.Request.Query["ActionHeader"].Equals("ClearAll"))
            {
                ClearDirectory(10);//考虑数据量
                context.Response.StatusCode = 200;
            }
            else if (context.Request.Path.Value.StartsWith("/item/"))//规则支持自定义
            {
                Console.WriteLine($"This is {nameof(StaticPageMiddleware)} InvokeAsync {context.Request.Path.Value}");

                #region context.Response.Body
                var originalStream = context.Response.Body;
                using var copyStream = new MemoryStream();
                context.Response.Body = copyStream;
                await _next(context);//后续的常规流程，正常请求响应

                copyStream.Position = 0;
                var reader = new StreamReader(copyStream);
                var content = await reader.ReadToEndAsync();
                var url = context.Request.Path.Value;

                SaveHtml(url, content);

                copyStream.Position = 0;
                await copyStream.CopyToAsync(originalStream);
                context.Response.Body = originalStream;

                #endregion
            }
            else
            {
                await _next(context);
            }
        }

        private void SaveHtml(string url, string html)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(html))
                    return;
                if (!url.EndsWith(".html"))
                    return;

                if (Directory.Exists(_directoryPath) == false)
                    Directory.CreateDirectory(_directoryPath);

                var totalPath = Path.Combine(_directoryPath, url.Split("/").Last());
                File.WriteAllText(totalPath, html);//直接覆盖
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 删除某个页面
        /// </summary>
        /// <param name="url"></param>
        private void DeleteHmtl(string url)
        {
            try
            {
                if (!url.EndsWith(".html"))
                    return;
                var totalPath = Path.Combine(_directoryPath, url.Split("/").Last());
                File.Delete(totalPath);//直接删除
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Delete {url} 异常，{ex.Message}");
            }
        }

        /// <summary>
        /// 清理文件，支持重试
        /// </summary>
        /// <param name="index">最多重试次数</param>
        private void ClearDirectory(int index)
        {
            if (index > 0)
            {
                try
                {
                    var files = Directory.GetFiles(_directoryPath);
                    foreach (var file in files)
                    {
                        File.Delete(file);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{nameof(ClearDirectory)} failed {ex.Message}");
                    ClearDirectory(index--);
                }
            }
        }
    }
}
