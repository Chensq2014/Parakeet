using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using Common.Storage;
using Serilog;
using Volo.Abp.Modularity.PlugIns;

namespace Parakeet.Net.WebApi
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、服务注册流程_{nameof(Startup)} Start  ConfigureServices ....");
            services.AddApplication<WebApiModule>(
                options =>
                {
                    var baseDirectory = Path.GetDirectoryName(typeof(Program).Assembly.Location);
                    var path = $"{baseDirectory}/Plugins";
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    options.PlugInSources.Add(new FolderPlugInSource(path, SearchOption.AllDirectories));
                });
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、服务注册流程_{nameof(Startup)} End  ConfigureServices ....");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、中间组装件流程_{nameof(Startup)} Start  Configure ....");
            app.InitializeApplication();
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、中间组装件流程_{nameof(Startup)} End  Configure ....");
        }
    }
}
