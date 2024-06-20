using Common;
using Common.Dtos;
using Common.Encrypts;
using Common.Enums;
using Common.Extensions;
using Common.Helpers;
using Common.RabbitMQModule.Producers;
using Common.TcpMudule.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace Parakeet.Net.Producer.Tcp.Commands
{
    /// <summary>
    /// 扬尘包解析命令
    /// </summary>
    public class PacketParseCommand : BaseCommand
    {
        private readonly ILogger _logger;
        private readonly IProducer _publisher;

        public PacketParseCommand(IProducerContainer producerContainer, ILogger<PacketParseCommand> logger)
        {
            _logger = logger;
            _publisher = producerContainer.GetProducer(Magics.CHONGQING + "." + DeviceType.Environment.ToInt(), nameof(EnvironmentTcpModule));
        }

        public override byte[] Execute(byte[] request, Action<SocketAsyncEventArgs, byte[]> action)
        {
            _logger.LogInformation($"[环境监测]{Device.FakeNo}开始执行数据解析命令...");

            var package = Encoding.UTF8.GetString(request);

            _logger.LogInformation($"[环境监测]{Device.FakeNo}解析原始结果为{package}...");

            var wrapper = new WrapperData<EnvironmentBaseDto>();

            try
            {
                var environment = new EnvironmentBaseDto{Id = Guid.NewGuid()};
                environment.Device = Device;
                environment.DeviceId = Device.Id;
                var recordTime = Regex.Match(package, "DataTime=(\\d+);").Groups[1].Value;
                environment.RecordTime = DateTime.ParseExact(recordTime, $"yyyyMMddHHmmss", CultureInfo.CurrentCulture);

                var b03 = Regex.Match(package, "B03-Avg=(.*?),");
                if (b03.Success)
                {
                    environment.Noise = decimal.Parse(b03.Groups[1].Value);
                }

                var pm25 = Regex.Match(package, "PM25-Avg=(.*?),");
                if (pm25.Success)
                {
                    environment.PM2P5 = decimal.Parse(pm25.Groups[1].Value);
                }

                var pm10 = Regex.Match(package, "PM10-Avg=(.*?),");
                if (pm10.Success)
                {
                    environment.PM10 = decimal.Parse(pm10.Groups[1].Value);
                }

                var w01 = Regex.Match(package, "W01-Avg=(.*?),");
                if (w01.Success)
                {
                    environment.WindDirection = decimal.Parse(w01.Groups[1].Value);
                }

                var w02 = Regex.Match(package, "W02-Avg=(.*?),");
                if (w02.Success)
                {
                    environment.WindSpeed = decimal.Parse(w02.Groups[1].Value);
                }

                var t01 = Regex.Match(package, "T01-Avg=(.*?),");
                if (t01.Success)
                {
                    environment.Temperature = decimal.Parse(t01.Groups[1].Value);
                }

                var h01 = Regex.Match(package, "H01-Avg=(.*?),");
                if (h01.Success)
                {
                    environment.Humidity = decimal.Parse(h01.Groups[1].Value);
                }

                var p01 = Regex.Match(package, "P01-Avg=(.*?),");
                if (p01.Success)
                {
                    environment.Pressure = decimal.Parse(p01.Groups[1].Value);
                }

                var r01 = Regex.Match(package, "R01-Avg=(.*?),");
                if (r01.Success)
                {
                    environment.Rainfall = decimal.Parse(r01.Groups[1].Value);
                }

                wrapper.Data = environment;

                //验证MD5
                var sk = Regex.Match(package, "SK=(.*?);");
                var cp = Regex.Match(package, "&&(.*?)&&");
                var key = MD5Encrypt.Encrypt(Device.Key + cp.Groups[1].Value,0).ToUpper();

                if (!sk.Groups[1].Value.Equals(key, StringComparison.CurrentCultureIgnoreCase))
                {
                    _logger.LogWarning($"[环境监测]{Device.FakeNo}验证MD5错误,该数据将被丢弃...");
                    wrapper.Success = false;
                    wrapper.Code = 1;
                    return Handler(wrapper);
                }

                //验证CRC
                var length = int.Parse(package.Substring(2, 4));
                var crc = package.Substring(length + 6, 4);
                var crcEncrypt = CRC16Helper.ToCRC16(package.Substring(6, length));

                if (!crc.Equals(crcEncrypt))
                {
                    _logger.LogWarning($"[环境监测]{Device.FakeNo}验证CRC错误,该数据将被丢弃...");
                    wrapper.Success = false;
                    wrapper.Code = 1;
                    return Handler(wrapper);
                }

                return Handler(wrapper);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[环境监测]{Device.FakeNo}数据解析异常-{ex.Message}");
            }

            wrapper.Success = false;
            wrapper.Code = 2;

            return Handler(wrapper);
        }



        private byte[] Handler(WrapperData<EnvironmentBaseDto> wrapper)
        {
            var result = $"ST=91;CN=9111;PW=123456;MN={wrapper?.Data?.Device.SerialNo};CP=&&code={wrapper.Code}&&";

            try
            {
                _publisher.PublishAsync(wrapper, "500000.1001.record");

                var heartbeat = new WrapperData<HeartbeatDto>
                {
                    Data = new HeartbeatDto
                    {
                        DeviceId = wrapper.Data.DeviceId,
                        Device = wrapper.Data.Device,
                        RecordTime = wrapper.Data.RecordTime
                    }
                };

                _publisher.PublishAsync(heartbeat, "500000.1001.heartbeat");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

            var back = "##0055" + result + CRC16Helper.ToCRC16(result) + "\r\n";
            _logger.LogInformation($"[环境监测]{Device.FakeNo}解析结果：" + back);

            return Encoding.ASCII.GetBytes(back);
        }
    }
}
