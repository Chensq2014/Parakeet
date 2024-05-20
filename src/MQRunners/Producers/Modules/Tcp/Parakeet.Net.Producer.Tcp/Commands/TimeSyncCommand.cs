using Common.Extensions;
using Common.Helpers;
using Common.TcpMudule.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Sockets;
using System.Text;

namespace Parakeet.Net.Producer.Tcp.Commands
{
    /// <summary>
    /// 时间同步用
    /// </summary>
    public class TimeSyncCommand : BaseCommand
    {
        private readonly ILogger _logger;

        public TimeSyncCommand(ILogger<PacketParseCommand> logger)
        {
            _logger = logger;
        }

        public override byte[] Execute(byte[] request, Action<SocketAsyncEventArgs, byte[]> action)
        {
            try
            {
                _logger.LogInformation($"[环境监测]{Device.FakeNo}开始执行校时命令...");
                _logger.LogInformation($"[环境监测]{Device.FakeNo}校时命令码[{Encoding.UTF8.GetString(request)}]...");
                var result = $"ST=52;CN=1012;PW=123456;MN={Device.SerialNo};Flag=3;CP=&&SystemTime={DateTime.Now:yyyyMMddHHmmss};&&";
                var bits = Encoding.ASCII.GetBytes($"##{result.Length.ToString().PadLeft(4, '0')}" + result + CRC16Helper.ToCRC16(result) + "\r\n");

                _logger.LogInformation($"[环境监测]{Device.FakeNo}校时命令返回码[{bits.ToHexString(true)}]...");
                return bits;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[环境监测]设备{Device.FakeNo}校时失败错误信息:{ex.Message}");
                return request;
            }
        }
    }
}
