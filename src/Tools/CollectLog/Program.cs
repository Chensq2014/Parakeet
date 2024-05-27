using Common.Extensions;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System.Reflection;
using System.ServiceProcess;

namespace CollectLog
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = SerialLogConfig(new LoggerConfiguration()).CreateLogger();

            ServiceBase.Run(new Collector());
        }

        /// <summary>
        /// SerialLog配置
        /// </summary>
        /// <param name="loggerConfiguration"></param>
        /// <returns></returns>
        private static LoggerConfiguration SerialLogConfig(LoggerConfiguration loggerConfiguration)
        {

            var dir = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}logs";
            FileExtension.CreateFolderIfNeeded(dir);
            var logPath = $@"{dir}{Path.DirectorySeparatorChar}log.log";

            return loggerConfiguration
                //// 将配置传给 Serilog 的提供程序 已在webBuilder.UseSerilog中使用配置文件替换
                //.ReadFrom.Configuration(Configuration)
#if DEBUG
                .MinimumLevel.Debug()
                //.MinimumLevel.Error()
#else
                //.MinimumLevel.Information()
                //.MinimumLevel.Error()
                .MinimumLevel.Fatal()
#endif
                //对其他日志进行重写,除此之外,目前框架只有微软自带的日志组件
                //.MinimumLevel.Override(source: "Microsoft", minimumLevel: LogEventLevel.Information)
                //.MinimumLevel.Override(source: "Microsoft", minimumLevel: LogEventLevel.Error)
                //.MinimumLevel.Override(source: "Microsoft.EntityFrameworkCore", minimumLevel: LogEventLevel.Error)
                .MinimumLevel.Override(source: "Microsoft", minimumLevel: LogEventLevel.Fatal)
                .MinimumLevel.Override(source: "Microsoft.EntityFrameworkCore", minimumLevel: LogEventLevel.Fatal)
                //记录相关上下文信息 --->  $"{变量}message"
                .Enrich.FromLogContext()
#if DEBUG

                //添加配置文件之前的所有控制台日志 Serilog提供了两个类（SystemConsoleThemes和AnsiConsoleThemes）用于主题的变化  AnsiConsoleTheme.Code
                //.WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Debug, outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}",theme: SystemConsoleTheme.Colored)
                .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Verbose,
                    outputTemplate: "时间:{Timestamp: HH:mm:ss.fff} 级别:{Level} 详细:{Message}{NewLine}{Exception}",
                    theme: AnsiConsoleTheme.Code)

                //The sink can write JSON output instead of plain text. CompactJsonFormatter or RenderedCompactJsonFormatter from Serilog.Formatting.Compact is recommended
                //.WriteTo.Console(new RenderedCompactJsonFormatter())

                //文件生成到当前路径logs\logsyyyymmdd.txt (rollingInterval:RollingInterval.Day:按天生成文件)  加载配置文件Serilog节点之前的所有日志
                .WriteTo.Async(c => c.File($"Logs{Path.DirectorySeparatorChar}log.log",
                    restrictedToMinimumLevel: LogEventLevel.Verbose, rollingInterval: RollingInterval.Day,
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {ThreadId} {Message:lj}{NewLine}{Exception}"))
                //.WriteTo.RollingFile(@"e:\log.txt", retainedFileCountLimit: 7)//Serilog.Sink.RollingFile 对滚动文件的支持
                //.WriteTo.File("log-.txt", rollingInterval: RollingInterval.Day)

                //从左至右四个参数分别是数据库连接字符串、表名、如果表不存在是否创建、最低等级。Serilog会默认创建一些列。 
                //.WriteTo.MSSqlServer("Data Source=.;Initial Catalog=parakeet;User ID=sa;Password=123456", "logs", autoCreateSqlTable: true, restrictedToMinimumLevel: LogEventLevel.Warning)

                //配置Serilog Seq接收器 http://localhost:20210
                //.WriteTo.Seq(Environment.GetEnvironmentVariable("Serilog:SEQ_URL") ?? "http://localhost:20210")

                //发送邮件日志
                //.WriteTo.Email(new EmailConnectionInfo()
                //{
                //    EmailSubject = "系统警告,请速速查看!",//邮件标题
                //    FromEmail = "291***@qq.com",//发件人邮箱
                //    MailServer = "smtp.qq.com",//smtp服务器地址
                //    NetworkCredentials = new NetworkCredential("291***@qq.com", "###########"),//两个参数分别是发件人邮箱与客户端授权码
                //    Port = ,//端口号
                //    ToEmail = "183***@163.com"//收件人
                //})
#else
                //.WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Fatal,
                //    outputTemplate: "时间:{Timestamp: HH:mm:ss.fff} 级别:{Level} 详细:{Message}{NewLine}{Exception}",
                //    theme: AnsiConsoleTheme.Code)

                .WriteTo.Async(c => c.File($"{logPath}",
                    restrictedToMinimumLevel: LogEventLevel.Information, rollingInterval: RollingInterval.Day,
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {ThreadId} {Message:lj}{NewLine}{Exception}"))
#endif
                ;
        }
    }
}
