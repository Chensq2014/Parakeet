using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Parakeet.Net.TcpHost
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            Console.Title = "Parakeet IOT Platform Tcp Parse Service";

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
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

            Log.Information("Start Tcp Server...");

            var hostBuilder = CreateHostBuilder(args);
            var host = hostBuilder.Build();
            await host.RunAsync();

            Log.Error("after host.RunAsync ��Something Wrong Happend��...");
            return 0;
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .UseSerilog()
                .UseAutofac();
    }
}
