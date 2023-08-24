using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.IO;
using Parakeet.Net.Web;
using Serilog;

namespace Parakeet.Net.Extentions
{
    /// <summary>
    /// 添加xml注释文件资源扩展
    /// </summary>
    public static class SwaggerGenOptionsExtention
    {
        /// <summary>
        /// 自定义IncludeXmlComments 将指定xml文件加入到swagger注释
        /// </summary>
        /// <param name="options"></param>
        public static SwaggerGenOptions IncludeXmlCommentFiles(this SwaggerGenOptions options)
        {
            var baseDirectory = Path.GetDirectoryName(typeof(Program).Assembly.Location);
            var directory = new DirectoryInfo(baseDirectory);
            foreach (var item in directory.GetFiles("*.xml"))
            {
                Log.Logger.Information($"加载xml文件 :{item.FullName}....");
                options.IncludeXmlComments(item.FullName);
            }
            return options;
        }
    }
}
