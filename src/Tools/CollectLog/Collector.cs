using CollectLog.EntityFramworkCore;
using Common;
using Common.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using Serilog;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Text.Json;
using System.Timers;

namespace CollectLog
{
    public class Collector : ServiceBase
    {
        int LastReadLine = -1;
        int ErrorLastReadLine = -1;
        IConfigurationRoot Configuration;
        string FilePath;
        string ErrorFilePath;
        long ReadInterval = 60000;
        string Conn;
        string LogApi;
        DateTime? LastReadTime = null;
        CollectlogDbContext DbContext;

        public System.Timers.Timer Timer { get; set; }

        private FileSystemWatcher _watcher;
        PhysicalFileProvider _phyFileProvider;

        private static readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);
        public Collector()
        {
            Configuration = BuildConfiguration();
            FilePath = Configuration["FilePath"];
            ErrorFilePath = Configuration["ErrorFilePath"];
            Conn = Configuration["ConnectionString"];
            LogApi = Configuration["LogApi"];
            long.TryParse(Configuration["ReadInterval"], out ReadInterval);


            var dbOptions = new DbContextOptionsBuilder<CollectlogDbContext>()
                .UseNpgsql(Conn, options =>
                {
                    options.EnableRetryOnFailure();
                    options.CommandTimeout(CommonConsts.CommandTimeOut);
                })
                .Options;
            DbContext = new CollectlogDbContext(dbOptions);

            _watcher = new FileSystemWatcher
            {
                Path = Path.GetDirectoryName(FilePath),
                Filter = Path.GetFileName(FilePath),
                NotifyFilter = NotifyFilters.LastWrite
            };
            _watcher.Changed += OnFileChanged;//使用watcher 监听文件change事件
            _watcher.EnableRaisingEvents = true;

            ////文件change 还可以改为timer的方式轮询
            //Timer =new System.Timers.Timer();
            //Timer.Elapsed += OnTimerElapsed;
            //Timer.Enabled = true;
            //Timer.Start();


            ////使用_phyFileProvider
            //_phyFileProvider = new PhysicalFileProvider(FilePath);
            //ChangeToken.OnChange(
            //    changeTokenProducer: () => _phyFileProvider.Watch(FilePath),
            //    changeTokenConsumer: () => { OnFileChanged(null, null); });



        }


        protected override void OnStart(string[] args)
        {
            Start(args);
            base.OnStart(args);
            Log.Logger.Information($"服务启动...");
        }

        protected override void OnStop()
        {
            base.OnStop();
            Log.Logger.Information($"服务停止...");
        }

        public async void Start(string[] args)
        {
            try
            {
                Log.Logger.Information($"服务启动，开始扫描读取文件：{FilePath}");
                OnFileChanged(null, null);
            }
            catch (Exception ex)
            {
                Log.Logger.Information($"服务启动扫描读取文件：{FilePath}change内容失败{ex.Message}_{ex.StackTrace}");
            }
            finally
            {
                Log.Logger.Information($"日志收集服务启动成功");
            }
        }


        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                Log.Logger.Information($"定时扫描：{FilePath}");
                OnFileChanged(null, null);
            }
            catch (Exception ex)
            {
                Log.Logger.Information($"扫描读取文件：{FilePath}change内容失败{ex.Message}_{ex.StackTrace}");
            }
            finally
            {
                Log.Logger.Information($"本地扫描读取完毕");
            }
        }

        public async void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            Log.Logger.Information($"日志文件changer事件触发");
            try
            {

                await _semaphoreSlim.WaitAsync();

                using var stream = new FileStream(e?.FullPath ?? $@"{FilePath}", FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                using var sr = new StreamReader(stream, Encoding.GetEncoding("GB2312"));
                var lineList = new List<string>();
                var logInput = new RevieveLogInputDto();
                var isReadEnd = false;
                var lineIndex = 0;
                if (LastReadTime == null || LastReadTime < DateTime.Now.Date)
                {
                    var item = new LastReadItem { Date = DateTime.Now.Date, LastReadLine = -1 };
                    await LoadOrWriteLastReadInfo(item);
                    LastReadTime = item.Date;
                    LastReadLine = item.LastReadLine;
                }

                while (!isReadEnd)
                {
                    var line = sr.ReadLine();
                    if (line != null)
                    {
                        //处理line逻辑 ....
                        lineList.Add(line);
                    }
                    else
                    {
                        isReadEnd = true;
                    }
                }

                foreach (var line in lineList)
                {
                    //处理line 逻辑 准备 logInput数据
                }

                if (await SendLogsAsync(logInput))
                {
                    var item = new LastReadItem { Date = DateTime.Now.Date, LastReadLine = LastReadLine };
                    LastReadTime = item.Date;
                    await LoadOrWriteLastReadInfo(item, true);
                }
                else
                {
                    LastReadTime = null;
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"发生异常:{ex.Message}_{ex.StackTrace}");
            }
            finally
            {
                _semaphoreSlim.Release();
            }

        }


        public IConfigurationRoot BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                .AddJsonFile("appsettings.json", optional: false, true)
                .AddJsonFile("cache.json", optional: false, true);

            return builder.Build();
        }




        public async Task LoadOrWriteLastReadInfo(LastReadItem input, bool isWrite = false)
        {
            if (isWrite)
            {
                var cacheJsonPath = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\cache.json";
                Log.Logger.Information($"当前读取行数:{input.LastReadLine} 写入{cacheJsonPath}...");
                await File.WriteAllTextAsync(cacheJsonPath, JsonSerializer.Serialize(new
                {
                    LastReadItem = input
                }));
            }
            else
            {
                var date = DateTime.Parse(Configuration.GetSection("LastReadItem").GetSection("Date").Value);
                if (date.Date == input.Date.Date)
                {
                    input.LastReadLine = int.Parse(Configuration.GetSection("LastReadItem").GetSection("LastReadLine").Value);
                }
                else
                {
                    input.Date = date > input.Date ? date : input.Date;
                    input.LastReadLine = -1;
                }

                Log.Logger.Information($"读取配置文件LastReadItem 信息:{input.Date:yyyy/MM/dd}_{input.LastReadLine}");
            }
        }




        public async Task<bool> SendLogsAsync(RevieveLogInputDto input)
        {
            var isSuccess = false;
            try
            {
                if (input.Rows.Any())
                {
                    var httpClient = new HttpClient();
                    var content = new StringContent(JsonSerializer.Serialize(input), Encoding.UTF8);
                    var url = LogApi;
                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                    var response = await httpClient.PostAsync(url, content);
                    isSuccess = response.IsSuccessStatusCode;
                    if (isSuccess)
                    {
                        Log.Logger.Information($"请求{url}成功!");
                    }
                    else
                    {
                        try
                        {
                            var msg = await response?.Content?.ReadAsStringAsync();
                            Log.Logger.Error($"请求{url}失败!{msg}");
                        }
                        catch (Exception ex)
                        {
                            Log.Logger.Error($"请求{url}失败!{ex.Message}_{ex.StackTrace}");
                        }
                    }

                }
                else
                {
                    Log.Logger.Error($"未收集到日志，无请求");
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                Log.Logger.Error($"请求{LogApi}发生异常:{ex.Message}_{ex.StackTrace}");
            }
            return isSuccess;
        }


    }
}
