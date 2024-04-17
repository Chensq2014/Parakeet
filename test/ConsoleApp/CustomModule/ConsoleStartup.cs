//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Net.Http;
//using System.Reflection;
//using System.Text;
//using ConsoleApp.Dtos.XiamenHuizhan;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;
//using Common.Extensions;
//using Polly;
//using Serilog;

//namespace ConsoleApp
//{
//    /// <summary>
//    /// Startup 
//    /// </summary>
//    public class ConsoleStartup
//    {
//        /// <summary>
//        /// 配置
//        /// </summary>
//        public IConfiguration Configuration { get; }

//        /// <summary>
//        /// ConsoleStartup
//        /// </summary>
//        public ConsoleStartup()
//        {
//            var builder = new ConfigurationBuilder()
//                .SetBasePath(Directory.GetCurrentDirectory())
//                .AddJsonFile("appsettings.json", true, true);

//            Configuration = builder.Build();
//        }

//        /// <summary>
//        /// ConfigureServices
//        /// </summary>
//        /// <param name="services"></param>
//        /// <returns></returns>
//        public IServiceProvider ConfigureServices(IServiceCollection services)
//        {
//            services.AddSingleton<IConfiguration>(Configuration);

//            //services.ConfigureSingleton(Configuration.GetSection("SubscriberOptions"), () => new SubscriberOptions());
//            services.AddHostedService<XiamenHuizhanBackGroundRequestService>();//厦门接口服务注册
//            services.AddLogging(builder =>
//            {
//                builder.AddConfiguration(Configuration.GetSection("Logging")).AddConsole();
//                builder.ClearProviders();
//                builder.AddSerilog();
//            });
//            //使用HuNan别名的httClient工厂来构造 HuNan的httpClient 添加重试、熔断器、超时等策略 注册 IHttpClientFactory
//            var timeout = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(10));
//            services.AddHttpClient(CustomerConsts.AppName, c =>
//                {
//                    //c.BaseAddress = new Uri("http://39.108.81.42"); //设置默认Uri:9070 端口号和api在client端设置
//                    c.DefaultRequestHeaders.Add("Accept", "application/json");
//                })
//                .AddPolicyHandler(request => timeout)
//                .AddTransientHttpErrorPolicy(p => p.RetryAsync(3));

//            services.AddGrpcClient<CustomMath.CustomMathClient>(options =>
//            {
//                //options.Address = new Uri("https://localhost:5001");
//                //options.Interceptors.Add(new CustomClientLoggerInterceptor(logger));
//            })
//            .ConfigureChannel(grpcOptions =>
//            {
//                //options.MaxReceiveMessageSize = 10 * 1024 * 1024;
//                //options.MaxSendMessageSize = 10 * 1024 * 1024;
//                //HttpClient --443 代替grpc-https://localhost:5001
//                grpcOptions.HttpClient = new HttpClient(new HttpClientHandler
//                {
//                    ServerCertificateCustomValidationCallback = (msg, cert, chain, error) => true//忽略证书
//                });
//            });



//            //List<IStartup> startups = new List<IStartup>();
//            ////加载插件
//            //foreach (var file in Directory.GetFiles(Path.Combine(AppContext.BaseDirectory, "Plugins"), "*.dll", SearchOption.AllDirectories))
//            //{
//            //    var assembly = Assembly.LoadFile(file);

//            //    var typeToRegister = assembly.GetTypes()
//            //        .FirstOrDefault(type => type.BaseType != null && typeof(IStartup).IsAssignableFrom(type));
//            //    if (typeToRegister != null)
//            //    {
//            //        startups.Add((IStartup)Activator.CreateInstance(typeToRegister, Configuration));
//            //    }
//            //}

//            //////排序后加载，否则会出现子模块无法启动的问题
//            ////startups.OrderBy(s => s.Sort);

//            //foreach(var startup in startups)
//            //{
//            //    startup.ConfigureServices(services);
//            //    //startup.Configure();
//            //}

//            //注册完毕以后，BuildServiceProvider  返回注册完毕后的实例提供器
//            ServiceProviderFactory.ServiceProvider = services.BuildServiceProvider();

//            return ServiceProviderFactory.ServiceProvider;
//        }


//        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
//        {
//            //控制台应用程序不在netcore框架里，需要手动调用
//            ILogger<Startup> logger = loggerFactory.CreateLogger<Startup>();
//            logger.LogDebug($"{nameof(Startup)}默认配置管道开始.....");
//            app.InitializeApplication();
//            Log.Logger.Debug($"{nameof(Startup)}默认配置管道结束.....");
//        }
//    }
//}
