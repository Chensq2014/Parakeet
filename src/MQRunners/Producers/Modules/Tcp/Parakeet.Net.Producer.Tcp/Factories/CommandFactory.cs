using Common.Dtos;
using Common.Entities;
using Common.Enums;
using Common.Extensions;
using Common.TcpMudule.Interfaces;
using Common.TcpMudule.Services;
using Common.TcpMudule.Sockets;
using Microsoft.Extensions.Logging;
using Parakeet.Net.Caches;
using Parakeet.Net.Producer.Tcp.Commands;
using System;
using System.Text;
using System.Text.RegularExpressions;
using Volo.Abp.ObjectMapping;

namespace Parakeet.Net.Producer.Tcp.Factories
{
    /// <summary>
    /// 命令工厂
    /// </summary>
    public class CommandFactory : BaseHandlerType, ICommandFactory<EnvironmentTcpModule>
    {
        private readonly ILogger _logger;
        private readonly IObjectMapper _objectMapper;
        private readonly IServiceProvider _serviceProvider;
        private readonly DevicePool _devicePool;

        public CommandFactory(
            ILogger<CommandFactory> logger,
            IObjectMapper objectMapper,
            DevicePool devicePool,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _devicePool = devicePool;
            _serviceProvider = serviceProvider;
            _objectMapper = objectMapper;
        }
        public CommandWrapper Create(AsyncUserToken userToken, byte[] data)
        {
            try
            {
                var package = Encoding.UTF8.GetString(data);
                var mn = Regex.Match(package, "MN=(.*?);");
                var serialNo = mn.Groups[1].Value;
                var device = _devicePool[serialNo];
                if (device == null)
                {
                    _logger.LogWarning($"[合肥川达]查不到设备[{serialNo}],请检查是否入库");
                    return CommandWrapper.Break();
                }
                if (device.Type != DeviceType.Environment)
                {
                    _logger.LogWarning($"[合肥川达]设备{device.FakeNo}非川达环境设备,不做处理...");
                    return CommandWrapper.Continue(null);
                }
                var commandRegex = Regex.Match(package, "CN=(\\d+)");
                int.TryParse(commandRegex.Groups[1].Value, out int frameType);
                ICommand command;
                switch (frameType)
                {
                    case 2011:
                    // 日均值数据
                    case 2031:
                    //小时数据
                    case 2061:
                        command = _serviceProvider.Resolve<ICommand>(typeof(PacketParseCommand).FullName);
                        break;
                    //case 2111:
                    //    command = _serviceProvider.Resolve<ICommand>(typeof(PacketUploadCommand).FullName);
                    //    break;
                    case 1011:
                        command = _serviceProvider.Resolve<ICommand>(typeof(TimeSyncCommand).FullName);
                        break;
                    default:
                        command = _serviceProvider.Resolve<ICommand>(typeof(UnknownCommand).FullName);
                        break;
                }
                ////使用时 注入IObjectMapper
                command.Device = _objectMapper.Map<Device, DeviceDto>(device);
                return CommandWrapper.Normal(command);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
