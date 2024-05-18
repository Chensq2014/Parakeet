//using Exceptionless;
//using IdentityServer4.Models;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.Logging;
//using Parakeet.Net.CustomAttributes;
//using Common.Helpers;
//using Parakeet.Net.Permissions;
//using Parakeet.Net.RabbitMQModule.Core;
//using Parakeet.Net.RabbitMQModule.Producers;
//using Parakeet.Net.ROClient;
//using Parakeet.Net.ROClient.Models;
//using Parakeet.Net.ServiceGroup.JianWei.HttpApis;
//using Parakeet.Net.ServiceGroup.JianWei.HttpDtos;
//using Parakeet.Net.Users;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using Volo.Abp.MultiTenancy;
//using Common.CustomAttributes;

//namespace Parakeet.Net.Test
//{
//    //[UserCacheLock]
//    public class TestAppService : CustomerAppService, ITestAppService
//    {
//        private ICurrentTenant _currentTenant;
//        //IPasswordHasher<AppUser> 要在模块中注册
//        private readonly IPasswordHasher<AppUser> _passwordHasher;

//        private readonly IOperationTransient _transientOperation;
//        private readonly IOperationSingleton _singletonOperation;
//        private readonly IOperationScoped _scopedOperation;
//        private readonly IOperationScoped _scopedOperation2;
//        private readonly IChongqingJianWeiApi _chongqingJianWeiApi;
//        private readonly ReverseControlClient _client;

//        //private readonly IServiceProvider _serviceProvider;
//        private readonly IRabbitMQEventBusContainer _eventBusContainer;
//        #region rabbitmq测试相关

//        private static bool _isHandler = false;
//        private static bool _isBatchHandler = false;
//        private static object _locker = new object();
//        private ushort _consumerMaxBatchSize = 100;

//        private static ManualResetEventSlim _mainEvent = new ManualResetEventSlim(false);
//        //并发数
//        private int _maximalPushCount = 1000;

//        private static int _totalMessageIndex = 0;

//        private static int _sortIndex = 0;

//        private int _batchHandlerCount = 0;
//        #endregion

//        public TestAppService(
//            ICurrentTenant currentTenant,
//            IChongqingJianWeiApi chongqingJianWeiApi,
//            IOperationTransient transientOperation,
//            IOperationSingleton singletonOperation,
//            IOperationScoped scopedOperation,
//            IOperationScoped scopedOperation2,
//            IRabbitMQEventBusContainer eventBusContainer,
//            //IServiceProvider serviceProvider, 
//            IPasswordHasher<AppUser> passwordHasher)
//        {
//            _transientOperation = transientOperation;
//            _singletonOperation = singletonOperation;
//            _scopedOperation = scopedOperation;
//            _scopedOperation2 = scopedOperation2;
//            _chongqingJianWeiApi = chongqingJianWeiApi;
//            _client = new ReverseControlClient();

//            //_serviceProvider = serviceProvider;
//            _passwordHasher = passwordHasher;
//            _eventBusContainer = eventBusContainer;
//            _currentTenant = currentTenant;
//        }


//        /// <summary>
//        /// 多租户测试
//        /// </summary>
//        /// <param name="__tenantId"></param>
//        /// <returns></returns>
//        public async Task<object> GetTenantId(string __tenantId)
//        {
//            return await Task.FromResult(_currentTenant);
//        }

//        #region 测试IOperationTransient IOperationSingleton IOperationScoped

//        /// <summary>
//        /// 测试IOperationTransient IOperationSingleton IOperationScoped
//        /// </summary>
//        /// <returns></returns>
//        public async Task OnGetOperation()
//        {
//            Console.WriteLine("Transient: " + _transientOperation.OperationId);
//            Console.WriteLine("Scoped1: " + _scopedOperation.OperationId);
//            Console.WriteLine("Scoped2: " + _scopedOperation2.OperationId);
//            Console.WriteLine("Singleton: " + _singletonOperation.OperationId);
//            //_logger.Information("Transient: " + _transientOperation.OperationId);
//            //_logger.Information("Scoped: " + _scopedOperation.OperationId);
//            //_logger.Information("Singleton: " + _singletonOperation.OperationId);
//            await Task.CompletedTask;
//        }

//        #endregion

//        #region Login password

//        /// <summary>
//        /// 获取加密密码字符串 需要Test权限
//        /// </summary>
//        /// <param name="password">未加密密码字符串</param>
//        /// <returns>加密密码字符串</returns>

//        [Authorize(TestPermissions.Tests.Default)]
//        public async Task<string> GetEncryptedPassword(string password)
//        {
//            var result = password?.Sha256();//idserver4 加密后的密码
//            //最终存入appUser表PasswordHash字段的密码，查看源码，这个hash还跟第一个参数Uer有关
//            result = _passwordHasher.HashPassword(new AppUser("test"), result);

//            return await Task.FromResult(result);
//        }

//        #endregion

//        #region RabbitMQTest

//        /// <summary>
//        /// 添加生产者 需要RabbitMQ权限
//        /// </summary>
//        /// <returns></returns>
//        [Authorize(TestPermissions.Tests.RabbitMQ)]
//        public async Task AddProducer()
//        {
//            _eventBusContainer.AddProducer(new ProducerAttribute("AreaName1", "Exchange1"));
//            await _eventBusContainer.AutoRegister(new[] { typeof(NetApplicationModule).Assembly });

//            var producerContainer = _eventBusContainer as IProducerContainer;//_serviceProvider.GetService<IProducerContainer>();
//            var producer = producerContainer.GetProducer("AreaName1", "Exchange1");
//            //producer.ExchangeDeclare("Exchange1");
//            await Task.CompletedTask;
//        }


//        /// <summary>
//        /// 单生产者单消费者测试
//        /// </summary>
//        /// <returns></returns>

//        [Authorize(TestPermissions.Tests.RabbitMQ)]
//        public async Task OneProducerToOneTestAppService()
//        {
//            _consumerMaxBatchSize = 1;
//            var routKey = "rout_one_test";
//            var exchange = "exchange_test";

//            var eventBus = _eventBusContainer.CreateEventBus(exchange, routKey);
//            eventBus.BindProducer<TestAppService>();
//            eventBus.AddConsumer(Handler, BatchHandler, "OneProducerToOneTestAppService");
//            eventBus.Enable();

//            //var consumerHost = await StartConsumerHost(_serviceProvider);
//            //await Task.Delay(1500);//等待消费者服务启动
//            var producer = ((IProducerContainer)_eventBusContainer).GetProducer<TestAppService>();
//            //模拟并发push
//            await Task.WhenAll(Enumerable.Range(0, _maximalPushCount).Select(x => producer.PublishAsync(Encoding.UTF8.GetBytes("producer message" + x))));
//            var stopWatch = Stopwatch.StartNew();

//            _mainEvent.Wait(5 * 60 * 1000);// 等待消费结果,期间可以观察http://localhost:15672/#/queues的队列消费情况


//            Logger.LogWarning($"_isBatchHandler:{_isBatchHandler}");

//            Logger.LogWarning($"处理{_maximalPushCount}条并发数据，总共消耗{stopWatch.ElapsedMilliseconds}毫秒");
//            //await consumerHost.StopAsync(CancellationToken.None);
//        }


//        /// <summary>
//        /// 多生产者单消费者并发批量消费测试
//        /// </summary>
//        /// <returns></returns>

//        [Authorize(TestPermissions.Tests.RabbitMQ)]
//        public async Task MultipleProducerToOneConsumerConcurrencyTest()
//        {
//            var routKey = "rout_multiple_test";
//            var exchange = "exchange_test";

//            var eventBus = _eventBusContainer.CreateEventBus(exchange, routKey);
//            eventBus.BindProducer<TestAppService>();
//            eventBus.AddConsumer(Handler, BatchHandler);
//            eventBus.Enable();

//            //var consumerHost = await StartConsumerHost(_serviceProvider);
//            //await Task.Delay(1500);//等待消费者服务启动

//            var producer = ((IProducerContainer)_eventBusContainer).GetProducer<TestAppService>();
//            //模拟并发push
//            await Task.WhenAll(Enumerable.Range(0, _maximalPushCount).Select(x => producer.PublishAsync(Encoding.UTF8.GetBytes("producer message" + x))));
//            var stopWatch = Stopwatch.StartNew();
//            _mainEvent.Wait(1 * 60 * 1000);//等待消费结果,期间可以观察http://localhost:15672/#/queues的队列消费情况
//            Logger.LogDebug($"处理{_maximalPushCount}条并发数据，总共消耗{stopWatch.ElapsedMilliseconds}毫秒");


//            Logger.LogDebug($"_isBatchHandler:{_isBatchHandler}");

//            ////_totalMessageIndex.ShouldBe((_maximalPushCount - 1) * (_maximalPushCount / 2));
//            //await consumerHost.StopAsync(CancellationToken.None);
//        }

//        public async Task<IHostedService> StartConsumerHost(IServiceProvider serviceProvider)
//        {
//            var hostedService = serviceProvider.GetService<IHostedService>();
//            await hostedService.StartAsync(CancellationToken.None);
//            return hostedService;
//        }

//        /// <summary>
//        /// 消费者委托
//        /// </summary>
//        /// <param name="body"></param>
//        /// <returns></returns>
//        private async Task Handler((string utf8Json, ulong tag) body)
//        {
//            try
//            {
//                _isHandler = true;
//                //模拟IO操作，如数据库写入耗时
//                //await Task.Delay(10);
//                //var message = body.utf8Json;
//                //message.ShouldBe("producer message" + _sortIndex++);

//                Logger.LogDebug($"{body.utf8Json} producer message:{_sortIndex++}");
//                if (_sortIndex == _maximalPushCount)
//                {
//                    _mainEvent.Set();
//                }
//            }
//            catch (Exception ex)
//            {
//                Logger.LogDebug($"{ex.Message}");
//                _mainEvent.Set();
//            }

//            await Task.CompletedTask;
//        }

//        /// <summary>
//        /// 消费者委托 批量
//        /// </summary>
//        /// <param name="bodyList"></param>
//        /// <returns></returns>
//        private async Task BatchHandler(List<(string utf8Json, ulong tag)> bodyList)
//        {
//            try
//            {
//                //每次批量最多处理_consumerMaxBatchSize条数据

//                Logger.LogDebug($"批量处理bodyList.Count:{bodyList.Count} 条数据 每次批量最多处理_consumerMaxBatchSize:{_consumerMaxBatchSize}条数据");

//                _batchHandlerCount++;
//                _isBatchHandler = true;

//                var messages = bodyList.Select(x => x.utf8Json).ToList();
//                foreach (var message in messages)
//                {
//                    //模拟IO操作，如数据库写入耗时
//                    //await Task.Delay(10);
//                    //Interlocked.Exchange(ref _totalMessageIndex, _totalMessageIndex + _sortIndex);
//                    //Interlocked.Add(ref _totalMessageIndex, _sortIndex);
//                    //_totalMessageIndex += _sortIndex;

//                    //确保顺序消费
//                    //message.ShouldBe("producer message" + _sortIndex);

//                    Logger.LogDebug($"producer message {_sortIndex}");
//                    Interlocked.Increment(ref _sortIndex);
//                    if (_sortIndex == _maximalPushCount)
//                    {
//                        _mainEvent.Set();
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                Logger.LogDebug($"{ex.Message}");
//                _mainEvent.Set();
//            }

//            await Task.CompletedTask;
//        }



//        #endregion

//        #region Exceptionless

//        /// <summary>
//        /// Exceptionless 测试
//        /// </summary>
//        /// <returns></returns>
//        public async Task ExceptionlessTest()
//        {
//            try
//            {
//                throw new ApplicationException("产生一条错误消息1");
//            }
//            catch (Exception ex)
//            {
//                ex.ToExceptionless()
//                    // 为事件设定一个编号，以便于你搜索 
//                    .SetReferenceId(Guid.NewGuid().ToString("N"))
//                    //// 添加一个不包含CreditCardNumber属性的对象信息
//                    //.AddObject(order, "Order", excludedPropertyNames: new[] {"CreditCardNumber"}, maxDepth: 2)
//                    //// 设置一个名为"Quote"的编号
//                    //.SetProperty("Quote", 123)
//                    //// 添加一个名为“Order”的标签
//                    //.AddTags("Order")
//                    //// 标记为关键异常
//                    //.MarkAsCritical()
//                    //// 设置一个地理位置坐标
//                    //.SetGeo(43.595089, -88.444602)
//                    //// 设置触发异常的用户信息
//                    //.SetUserIdentity(user.Id, user.FullName)
//                    //// 设置触发用户的一些描述
//                    //.SetUserDescription(user.EmailAddress, "I tried creating an order from my saved quote.")
//                    // 发送事件
//                    .Submit();
//                throw new ApplicationException("产生一条错误消息2");
//            }
//            finally
//            {
//                await Task.CompletedTask;
//            }
//        }

//        #endregion

//        #region 其它未在接口中的测试

//        /// <summary>
//        /// 从第三方平台人员信息
//        /// </summary>
//        /// <returns></returns>
//        public async Task<JianWeiResult<IList<ProjectWorkerDto>>> GetProjectWorker()
//        {
//            return await _chongqingJianWeiApi.GetProjectWorker(0);
//        }


//        /// <summary>
//        /// ROClientTest
//        /// </summary>
//        /// <returns></returns>
//        public async Task<ResponseWrapper> ROClientTest()
//        {
//            var serialNo = "50010810050005";//"51010710050001";//"50011910050002"
//            var replies = await _client.ExecutePersonDeleteCommandAsync(serialNo, new PersonDeletedModel
//            {
//                PersonnelIds = new string[1] { "dff02b5daaa045ed8494af2704b1713f" }
//            });
//            return replies;
//        }

//        #endregion

//        #region EnvironmentHelper

//        /// <summary>
//        /// 环境变量 测试
//        /// </summary>
//        /// <returns></returns>
//        public async Task EnvironmentShowTest()
//        {
//            try
//            {
//                Logger.LogDebug($"DatabaseType:{EnvironmentHelper.DatabaseType}");
//                Logger.LogDebug($"DatabaseConnectionString:{EnvironmentHelper.DatabaseConnectionString}");
//                Logger.LogDebug($"RootPath:{EnvironmentHelper.RootPath}");
//                Logger.LogDebug($"ApplicationData:{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}");
//                Logger.LogDebug($"LocalApplicationData:{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}");
//                Logger.LogDebug($"EnumAssemblyNames:{EnvironmentHelper.EnumAssemblyNames}");
//                Logger.LogDebug($"DesKey:{EnvironmentHelper.DesKey}");
//                Logger.LogDebug($"GetEnvironmentVariables:{Environment.GetEnvironmentVariables()}");
//                Logger.LogDebug($"GetLogicalDrives:{Environment.GetLogicalDrives()}");
//                Logger.LogDebug($"GetEnvironmentVariables:{Environment.GetEnvironmentVariable("CONNECTIONITEMS")}");
//            }
//            catch (Exception ex)
//            {
//                throw new ApplicationException($"产生一条错误消息{ex.Message}");
//            }
//            finally
//            {
//                await Task.CompletedTask;
//            }
//        }

//        #endregion

//        #region Weixin

//        /// <summary>
//        /// 企业微信测试接口
//        /// </summary>
//        /// <returns></returns>
//        [UserCacheLock]
//        public async Task WeixinLoginQrCode()
//        {
//            //https://open.work.weixin.qq.com/wwopen/sso/qrConnect?appid=CORPID&agentid=AGENTID&redirect_uri=REDIRECT_URI&state=STATE
//            await Task.CompletedTask;
//        }

//        /// <summary>
//        /// 企业微信测试接口
//        /// </summary>
//        /// <returns></returns>
//        public async Task WeixinLogin()
//        {
//            await Task.CompletedTask;
//        }

//        #endregion

//    }
//}
