using Common;
using Common.Enums;
using Common.Extensions;
using Common.RabbitMQModule.Core;
using Common.TcpMudule.Interfaces;
using Common.TcpMudule.Sockets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Parakeet.Net.Caches;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Parakeet.Net.TcpHost
{
    /// <summary>
    /// TCP后台自动启动服务
    /// </summary>
    public class TcpHostService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IHostApplicationLifetime _appLifetime;
        private readonly ILogger _logger;
        private readonly ITcpServer _server;
        private readonly IServiceScope _scope;
        //private readonly ILicenseWorker _licenseWorker;
        private readonly IRabbitMQEventBusContainer _eventBusContainer;
        private readonly PacketHandlerPool _packetHandlerPool;

        public TcpHostService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _logger = serviceProvider.GetRequiredService<ILogger<TcpHostService>>();
            _appLifetime = serviceProvider.GetRequiredService<IHostApplicationLifetime>();
            _server = serviceProvider.GetRequiredService<ITcpServer>();
            _eventBusContainer = serviceProvider.GetRequiredService<IRabbitMQEventBusContainer>();
            _packetHandlerPool = serviceProvider.GetRequiredService<PacketHandlerPool>();

            //var licenseWorkerFactory = serviceProvider.GetRequiredService<ILicenseWorkerFactory>();
            //_licenseWorker = licenseWorkerFactory.CreateLicenseWorker(args);
            //_licenseWorker.OnLicensePassed += _licenseWorker_OnLicensePassed;

            _scope = serviceProvider.CreateScope();
        }


        public Task StartAsync(CancellationToken cancellationToken)
        {
            _appLifetime.ApplicationStarted.Register(OnStarted);
            _appLifetime.ApplicationStopping.Register(OnStopping);
            _appLifetime.ApplicationStopped.Register(OnStopped);

            //await _serviceProvider.WarmUpDevice();
            //await _serviceProvider.WarmUpPacketHandler();

            //tcpServer接收数据
            _server.OnReceiveCompleted += Server_OnReceiveCompleted;

            var consumers = _eventBusContainer.GetConsumers().Where(c => c.RouteKey.Equals(Magics.EQUIPMENT_REVERSE_ROUTE_KEY));
            foreach (var consumer in consumers)
            {
                consumer.OnReceived += Broadcast_OnReceived;
            }
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private void Server_OnReceiveCompleted(object sender, SocketEventArgs e)
        {
            var userToken = sender as AsyncUserToken;

            var header = new byte[] { e.Buffer[0], e.Buffer[1] }.ToHexString();

            var handlerTypes = _packetHandlerPool[header];
            if (handlerTypes == null || handlerTypes.Count == 0)
            {
                _logger.LogError($"设备{header}不存在,接收时间{DateTime.Now:yyyy-MM-dd HH:mm:ss},数据:{e.Buffer.ToHexString(true)}");
                userToken.ReceiveBuffer.Clear();
                _server.Close(userToken);
                return;
            }

            var receiveSize = userToken.ReceiveBuffer.Size;
            foreach (var handlerType in handlerTypes)
            {
                var packetHandler = _scope.ServiceProvider.Resolve<IPacketHandler>(handlerType.Handler);
                if (packetHandler != null)
                {
                    var packageStructure = packetHandler.Parse(e.Buffer, receiveSize);
                    if (packageStructure.Completed)
                    {
                        var result = packetHandler?.Handle(userToken, _server.SendAsync);
                        if (result != null)
                        {
                            if (result.Status == HandlerStatus.Break)
                            {
                                _logger.LogError($"设备数据存在问题将主动中断连接...,数据:{e.Buffer.ToHexString(true)}");
                                userToken.ReceiveBuffer.Clear();
                                _server.Close(userToken);
                                break;
                            }
                            else
                            {
                                _server.SendAsync(userToken.SendEventArgs, result.Data);
                            }
                        }
                    }
                }
                else
                {
                    _logger.LogError($"设备解析器{handlerType}不存在...,数据:{e.Buffer.ToHexString(true)}");
                    userToken.ReceiveBuffer.Clear();
                    _server.Close(userToken);
                    break;
                }
            }
        }

        //private void LicenseWorker_OnLicensePassed(object sender, LicenseEventArgs e)
        //{
        //    try
        //    {
        //        boot();
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "TCP Server 无法启动, 10 秒后将重启!");

        //        Thread.Sleep(60 * 1000);

        //        boot();
        //    }
        //}

        private void Boot()
        {
            _server.Start();
            _logger.LogInformation("启动成功...");
        }

        private void OnStarted()
        {
            //_licenseWorker.Start();
        }

        private void Broadcast_OnReceived(object sender, DeliverEventArgs e)
        {
            //Encoding.UTF8.GetString
            var bytes = e.Body.HasValue() ? Encoding.UTF8.GetBytes(e.Body) : new byte[1];
            if (bytes.Length < 2)
            {
                return;
            }

            var header = new[] { bytes[0], bytes[1] }.ToHexString();

            var handlerTypes = _packetHandlerPool[header];
            if (handlerTypes == null || handlerTypes.Count == 0)
            {
                _logger.LogError($"设备{header}不存在...");
                return;
            }

            foreach (var handlerType in handlerTypes)
            {
                var packetHandler = _scope.ServiceProvider.Resolve<IPacketHandler>(handlerType.Handler);
                if (packetHandler != null)
                {
                    var userToken = new AsyncUserToken(bytes.Length);
                    userToken.ReceiveBuffer.WriteBuffer(bytes);
                    packetHandler?.Handle(userToken, _server.SendAsync);
                }
                else
                {
                    _logger.LogError($"设备解析器{handlerType}不存在...");
                    break;
                }
            }
        }

        private void OnStopping()
        {
            _logger.LogInformation("OnStopping has been called.");

            // Perform on-stopping activities here
            _scope.Dispose();
            _server.Stop();
            _server.Dispose();
        }

        private void OnStopped()
        {
            _logger.LogInformation("OnStopped has been called.");
            // Perform post-stopped activities here
        }


    }
}
