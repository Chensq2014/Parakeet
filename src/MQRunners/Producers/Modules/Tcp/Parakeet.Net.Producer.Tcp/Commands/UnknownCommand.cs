using Common.Helpers;
using Common.TcpMudule.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace Parakeet.Net.Producer.Tcp.Commands
{
    /// <summary>
    /// 未知设备命令 【出现时需要查阅文档询问设备厂家】
    /// </summary>
    public class UnknownCommand : BaseCommand
    {
        private readonly ILogger _logger;

        public UnknownCommand(ILogger<UnknownCommand> logger)
        {
            _logger = logger;
        }

        public override byte[] Execute(byte[] request, Action<SocketAsyncEventArgs, byte[]> action)
        {
            try
            {
                _logger.LogWarning($"[环境监测]{Device.FakeNo}未知命令...");
                var packet = Encoding.UTF8.GetString(request);
                _logger.LogInformation("原始数据:" + packet);
                var mnRegex = Regex.Match(packet, "MN=(.*?);");
                var mn = mnRegex.Groups[1].Value;

                var result = $"ST=91;CN=9111;PW=123456;MN={mn};CP=&&code=2&&";
                _logger.LogInformation("解析结果：" + result);

                return Encoding.ASCII.GetBytes("##0055" + result + CRC16Helper.ToCRC16(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Encoding.ASCII.GetBytes("##0055ST=91;CN=9111;PW=123456;MN=;CP=&&code=2&&4100");
            }
        }
    }
}
