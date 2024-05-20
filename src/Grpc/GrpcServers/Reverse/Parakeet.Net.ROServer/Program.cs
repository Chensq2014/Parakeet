using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Net;

namespace Parakeet.Net.ROServer
{
    public class Program
    {
        public static Task Main(string[] args)
        {
            Console.Title = "IOT Reverse Server";
            Log.Logger = new LoggerConfiguration()
                //// �����ô��� Serilog ���ṩ���� ����webBuilder.UseSerilog��ʹ�������ļ��滻
                //.ReadFrom.Configuration(Configuration)
#if DEBUG
                .MinimumLevel.Debug()
#else
                .MinimumLevel.Information()
#endif
               //��������־������д,����֮��,Ŀǰ���ֻ��΢���Դ�����־���
               .MinimumLevel.Override(source: "Microsoft", minimumLevel: LogEventLevel.Information)
               .MinimumLevel.Override(source: "Microsoft.EntityFrameworkCore", minimumLevel: LogEventLevel.Error)
               //��¼�����������Ϣ --->  $"{����}message"
               .Enrich.FromLogContext()

               //��������ļ�֮ǰ�����п���̨��־ Serilog�ṩ�������ࣨSystemConsoleThemes��AnsiConsoleThemes����������ı仯  AnsiConsoleTheme.Code
               //.WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Debug, outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}",theme: SystemConsoleTheme.Colored)
               .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Verbose, outputTemplate: "ʱ��:{Timestamp: HH:mm:ss.fff} ����:{Level} ��ϸ:{Message}{NewLine}{Exception}", theme: AnsiConsoleTheme.Code)

               //The sink can write JSON output instead of plain text. CompactJsonFormatter or RenderedCompactJsonFormatter from Serilog.Formatting.Compact is recommended
               //.WriteTo.Console(new RenderedCompactJsonFormatter())

               //�ļ����ɵ���ǰ·��logs\logsyyyymmdd.txt (rollingInterval:RollingInterval.Day:���������ļ�)  ���������ļ�Serilog�ڵ�֮ǰ��������־
               .WriteTo.Async(c => c.File($"Logs{Path.DirectorySeparatorChar}log.log", restrictedToMinimumLevel: LogEventLevel.Verbose, rollingInterval: RollingInterval.Day, outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {ThreadId} {Message:lj}{NewLine}{Exception}"))
               //.WriteTo.RollingFile(@"e:\log.txt", retainedFileCountLimit: 7)//Serilog.Sink.RollingFile �Թ����ļ���֧��
               //.WriteTo.File("log-.txt", rollingInterval: RollingInterval.Day)

               //���������ĸ������ֱ������ݿ������ַ���������������������Ƿ񴴽�����͵ȼ���Serilog��Ĭ�ϴ���һЩ�С� 
               //.WriteTo.MSSqlServer("Data Source=.;Initial Catalog=parakeet;User ID=sa;Password=123456", "logs", autoCreateSqlTable: true, restrictedToMinimumLevel: LogEventLevel.Warning)

               //����Serilog Seq������ http://localhost:20210
               //.WriteTo.Seq(Environment.GetEnvironmentVariable("Serilog:SEQ_URL") ?? "http://localhost:20210")

               //�����ʼ���־
               //.WriteTo.Email(new EmailConnectionInfo()
               //{
               //    EmailSubject = "ϵͳ����,�����ٲ鿴!",//�ʼ�����
               //    FromEmail = "291***@qq.com",//����������
               //    MailServer = "smtp.qq.com",//smtp��������ַ
               //    NetworkCredentials = new NetworkCredential("291***@qq.com", "###########"),//���������ֱ��Ƿ�����������ͻ�����Ȩ��
               //    Port = ,//�˿ں�
               //    ToEmail = "183***@163.com"//�ռ���
               //})
               .CreateLogger();

            Log.Information("Start Reverse Server...");

            //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var hostBulder = CreateHostBuilder(args);
            //Build()--> BuildCommonServices
            //1��ִ������ע��ί��ConfigureServices,
            //2��ע��Ĭ�ϵ���/�����ӿ�(DiagnosticListener,DiagnosticSource,IApplicationBuilderFactory,IConfiguration,IHttpContextFactory,IMiddlewareFactory,DefaultServiceProviderFactory)
            //3��AddOptions(IOption��),AddLogging(ILogger)
            var host = hostBulder.Build();
            //host.Services.GetService<IAbpApplicationWithExternalServiceProvider>().Initialize(host.Services);
            return host.RunAsync();
        }

        // Additional configuration is required to successfully run gRPC on macOS.
        // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
#if !DEBUG
                    webBuilder.ConfigureKestrel((context, options) =>
                    {
                        //�����ļ���Ĭ��ʹ����443,����ʹ��444
                        //options.Listen(IPAddress.Any, 444, listenOptions =>
                        //{
                        //    //listenOptions.UseHttps(o => o.SslProtocols = SslProtocols.Tls12);
                        //    listenOptions.UseConnectionLogging();
                        //    listenOptions.Protocols = HttpProtocols.Http2;
                        //    listenOptions.UseHttps("Certs/reverse.spdyun.cn.pfx", "d7sd60698ta");
                        //});
                        options.Listen(IPAddress.Any, 8098, listenOptions =>
                        {
                            listenOptions.UseConnectionLogging();
                            listenOptions.Protocols = HttpProtocols.Http2;
                        });
                    });
#endif
                })
                //.ConfigureAppConfiguration(builder =>
                //{
                //    var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                //    builder.SetBasePath(Directory.GetCurrentDirectory())
                //        .AddJsonFile("appsettings.json", true, true)
                //        .AddJsonFile($"appsettings.{env}.json", true, true);
                //})
                //.ConfigureServices(collection =>
                //{
                //    //collection.AddApplication<ReverseServerModule>(options =>
                //    //{
                //    //    var baseDirectory = Path.GetDirectoryName(typeof(Program).Assembly.Location);
                //    //    var path = $"{baseDirectory}/Plugins";
                //    //    if (!Directory.Exists(path))
                //    //    {
                //    //        Directory.CreateDirectory(path);
                //    //    }
                //    //    options.PlugInSources.Add(new FolderPlugInSource(path, SearchOption.AllDirectories));
                //    //});
                //})
#if DEBUG
                .UseEnvironment(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"))
#endif
                .UseSerilog()
                .UseAutofac();
    }
}
