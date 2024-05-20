using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using ARchGL.IOT.RoClient.Models;
#if NETSTANDARD2_1
using Grpc.Net.Client;
#endif
using Grpc.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Threading.Tasks;
using Parakeet.Net.Protos;

namespace ARchGL.IOT.RoClient
{
    /// <summary>
    /// 设备反控客户端
    /// </summary>
    public static class ReverseControlCommandClient
    {
        #region 属性字段
        /// <summary>
        /// 从服务端获取的AppId默认....  上端赋值
        /// </summary>
        public static string AppId { get; set; }

        /// <summary>
        /// 从服务端获取的AppKey默认....  上端赋值
        /// </summary>
        public static string AppKey { get; set; }

        /// <summary>
        /// 从服务端获取AppSecret 默认....  上端赋值
        /// </summary>
        public static string AppSecret { get; set; }

        /// <summary>
        /// 服务端地址 默认"https://localhost:5003" 上端可赋值(配置文件)
        /// </summary>
#if NETSTANDARD2_0
        public static string ServerUrl { get; set; } = "reverse.spdyun.cn:80";
#else
        public static string ServerUrl { get; set; } = "https://reverse.spdyun.cn";
#endif
        /// <summary>
        /// 驼峰序列化
        /// </summary>
        static readonly JsonSerializerSettings jsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

#endregion

#region 下发人员到设备
        /// <summary>
        /// 多设备 批量下发人员
        /// </summary>
        /// <param name="serialNos">设备编码集合</param>
        /// <param name="models">待下发人员集合</param>
        /// <returns>返回批量下发结果</returns>
        public static async Task<List<ReverseReply>> ExecutePersonsAddCommandAsync(List<string> serialNos, List<PersonAddedModel> models)
        {
            var replies = new List<ReverseReply>();

            foreach (var serialNo in serialNos)
            {
                replies.AddRange(await ExecutePersonsAddCommandAsync(serialNo, models));
            }
            return replies;
        }

        /// <summary>
        /// 多设备 批量下发单个人员
        /// </summary>
        /// <param name="serialNos">设备编码集合</param>
        /// <param name="model">待下发人员</param>
        /// <returns>返回批量下发结果</returns>
        public static async Task<List<ReverseReply>> ExecutePersonsAddCommandAsync(List<string> serialNos, PersonAddedModel model)
        {
            var replies = new List<ReverseReply>();

            foreach (var serialNo in serialNos)
            {
                replies.Add(await ExecutePersonAddCommandAsync(serialNo, model));
            }
            return replies;
        }

        /// <summary>
        /// 单设备 批量下发多个人员
        /// </summary>
        /// <param name="serialNo">设备编码</param>
        /// <param name="models">待下发人员集合</param>
        /// <returns>返回批量下发结果</returns>
        public static async Task<List<ReverseReply>> ExecutePersonsAddCommandAsync(string serialNo, List<PersonAddedModel> models)
        {
            var replies = new List<ReverseReply>();

            foreach (var model in models)
            {
                replies.Add(await ExecutePersonAddCommandAsync(serialNo, model));
            }
            return replies;
        }

        /// <summary>
        /// 单设备单独下发单个人员
        /// </summary>
        /// <param name="serialNo">设备编码</param>
        /// <param name="model">待下发人员</param>
        /// <returns>返回下发结果</returns>
        public static async Task<ReverseReply> ExecutePersonAddCommandAsync(string serialNo, PersonAddedModel model)
        {
            if (string.IsNullOrWhiteSpace(serialNo))
            {
                return new ReverseReply
                {
                    Success = false,
                    Message = "设备转发编码不能为空"
                };
            }

            if (string.IsNullOrWhiteSpace(model.IdCard))
            {
                return new ReverseReply
                {
                    Success = false,
                    Message = "身份证编码不能为空"
                };
            }

            if (string.IsNullOrWhiteSpace(model.Name))
            {
                return new ReverseReply
                {
                    Success = false,
                    Message = "人员姓名不能为空"
                };
            }

            if (string.IsNullOrWhiteSpace(model.PersonnelId))
            {
                return new ReverseReply
                {
                    Success = false,
                    Message = "人员Id不能为空"
                };
            }

            var channel = CreateAuthenticatedChannel(ServerUrl);
            var client = new ReverseCommand.ReverseCommandClient(channel);

            var response = await client.ExecuteAsync(new ReverseRequest
            {
                SerialNo = serialNo,
                Command = model.CommandName,
                Body = JsonConvert.SerializeObject(model, jsonSettings)
            }, GetCallOptions());

            return response;
        }

#endregion

#region 从设备删除人员
        /// <summary>
        /// 删除人员(支持删除多条)
        /// </summary>
        /// <param name="serialNo">设备编码</param>
        /// <param name="model">待删除人员信息</param>
        /// <returns>返回删除人员结果</returns>
        public static async Task<ReverseReply> ExecutePersonDeleteCommandAsync(string serialNo, PersonDeletedModel model)
        {
            if (string.IsNullOrWhiteSpace(serialNo))
            {
                return new ReverseReply
                {
                    Success = false,
                    Message = "设备转发编码不能为空"
                };
            }

            if (model.PersonnelIds == null || model.PersonnelIds.Length == 0)
            {
                return new ReverseReply
                {
                    Success = false,
                    Message = "人员编码不能为空"
                };
            }

            var channel = CreateAuthenticatedChannel(ServerUrl);
            var client = new ReverseCommand.ReverseCommandClient(channel);
            var response = await client.ExecuteAsync(new ReverseRequest
            {
                SerialNo = serialNo,
                Command = model.CommandName,
                Body = JsonConvert.SerializeObject(model, jsonSettings)
            }, GetCallOptions());

            return response;
        }
#endregion

#region 人员注册

        /// <summary>
        /// 多设备批量人员注册
        /// </summary>
        /// <param name="serialNos">设备编码</param>
        /// <param name="models">待注册人员集合</param>
        /// <returns>返回注册人员结果</returns>
        public static async Task<List<ReverseReply>> ExecutePersonRegisterCommandAsync(List<string> serialNos, List<PersonRegisterModel> models)
        {
            var replies = new List<ReverseReply>();
            foreach (var serialNo in serialNos)
            {
                replies.AddRange(await ExecutePersonRegisterCommandAsync(serialNo, models));
            }
            return replies;
        }

        /// <summary>
        /// 多设备批量注册单个人员
        /// </summary>
        /// <param name="serialNos">设备编码</param>
        /// <param name="model">待注册人员</param>
        /// <returns>返回注册人员结果</returns>
        public static async Task<List<ReverseReply>> ExecutePersonRegisterCommandAsync(List<string> serialNos, PersonRegisterModel model)
        {
            var replies = new List<ReverseReply>();
            foreach (var serialNo in serialNos)
            {
                replies.Add(await ExecutePersonRegisterCommandAsync(serialNo, model));
            }
            return replies;
        }

        /// <summary>
        /// 单设备批量人员注册
        /// </summary>
        /// <param name="serialNo">设备编码</param>
        /// <param name="models">待注册人员集合</param>
        /// <returns>返回批量注册人员结果</returns>
        public static async Task<List<ReverseReply>> ExecutePersonRegisterCommandAsync(string serialNo, List<PersonRegisterModel> models)
        {
            var replies = new List<ReverseReply>();
            foreach (var model in models)
            {
                replies.Add(await ExecutePersonRegisterCommandAsync(serialNo, model));
            }
            return replies;
        }

        /// <summary>
        /// 单设备单人员注册
        /// </summary>
        /// <param name="serialNo">设备编码</param>
        /// <param name="model">待注册人员信息</param>
        /// <returns>返回注册人员结果</returns>
        public static async Task<ReverseReply> ExecutePersonRegisterCommandAsync(string serialNo, PersonRegisterModel model)
        {
            if (string.IsNullOrWhiteSpace(serialNo))
            {
                return new ReverseReply
                {
                    Success = false,
                    Message = "设备转发编码不能为空"
                };
            }

            if (string.IsNullOrWhiteSpace(model.IdCard))
            {
                return new ReverseReply
                {
                    Success = false,
                    Message = "身份证编码不能为空"
                };
            }

            if (string.IsNullOrWhiteSpace(model.Name))
            {
                return new ReverseReply
                {
                    Success = false,
                    Message = "人员姓名不能为空"
                };
            }

            if (string.IsNullOrWhiteSpace(model.PersonnelId))
            {
                return new ReverseReply
                {
                    Success = false,
                    Message = "人员Id不能为空"
                };
            }

            if (string.IsNullOrWhiteSpace(model.Photo) && string.IsNullOrWhiteSpace(model.PhotoUrl))
            {
                return new ReverseReply
                {
                    Success = false,
                    Message = "人员照片或Url不能为空"
                };
            }

            //注册人员含有图片等信息也等待1秒，放在真正注册逻辑中
            //await Task.Delay(1000);
            var channel = CreateAuthenticatedChannel(ServerUrl);
            var client = new ReverseCommand.ReverseCommandClient(channel);
            var response = await client.ExecuteAsync(new ReverseRequest
            {
                SerialNo = serialNo,
                Command = model.CommandName,
                Body = JsonConvert.SerializeObject(model, jsonSettings)
            }, GetCallOptions());
            return response;
        }

#endregion

#region 执行基坑终端设置命令

        /// <summary>
        /// 批量执行基坑终端设置命令
        /// </summary>
        /// <param name="serialNos">设备编码集合</param>
        /// <param name="models">模型集合</param>
        /// <returns>返回批量执行结果</returns>
        public static async Task<List<ReverseReply>> ExecuteFPTerminalSettingCommandAsync(List<string> serialNos,
            List<FoundationpitTerminalSetting> models)
        {
            var replies = new List<ReverseReply>();

            foreach (var serialNo in serialNos)
            {
                foreach (var model in models)
                {
                    replies.Add(await ExecuteFPTerminalSettingCommandAsync(serialNo, model));
                }
            }
            return replies;
        }

        /// <summary>
        /// 执行基坑终端设置命令
        /// </summary>
        /// <param name="serialNo">设备编码</param>
        /// <param name="model">模型</param>
        /// <returns>返回执行结果</returns>
        public static async Task<ReverseReply> ExecuteFPTerminalSettingCommandAsync(string serialNo, FoundationpitTerminalSetting model)
        {
            if (model.Type > 0x05 || model.Type < 0x01)
            {
                return new ReverseReply
                {
                    Success = false,
                    Message = "错误的指令类型"
                };
            }

            if (model.Sequence == 0)
            {
                return new ReverseReply
                {
                    Success = false,
                    Message = "流水号不能为0"
                };
            }

            switch (model.Type)
            {
                case 0x01 when string.IsNullOrWhiteSpace(model.IP):
                    return new ReverseReply
                    {
                        Success = false,
                        Message = "IP不能为空"
                    };
                case 0x02 when !model.CacheCycle.HasValue:
                    return new ReverseReply
                    {
                        Success = false,
                        Message = "缓存周期不能为空"
                    };
                case 0x03 when string.IsNullOrWhiteSpace(model.Time):
                    return new ReverseReply
                    {
                        Success = false,
                        Message = "校时不能为空"
                    };
                case 0x04 when !model.Frequency.HasValue:
                    return new ReverseReply
                    {
                        Success = false,
                        Message = "数据上报频率不能为空"
                    };
                case 0x05 when !model.WorkingMode.HasValue:
                    return new ReverseReply
                    {
                        Success = false,
                        Message = "设备工作模式不能为空"
                    };
            }

            //执行前等待1-3秒，放到真正执行逻辑中更好
            //await Task.Delay(new Random().Next(1000,3000));
            var channel = CreateAuthenticatedChannel(ServerUrl);
            var client = new ReverseCommand.ReverseCommandClient(channel);
            var response = await client.ExecuteAsync(new ReverseRequest
            {
                SerialNo = serialNo,
                Command = model.CommandName,
                Body = JsonConvert.SerializeObject(model, jsonSettings)
            }, GetCallOptions());

            return response;
        }
#endregion

#region 执行基坑终端基本数据设置命令

        public static async Task<ReverseReply> ExecuteFPTerminalBasicCommandAsync(string serialNo, FoundationpitTerminalBasic model)
        {
            if (!model.Sensor.HasValue)
            {
                return new ReverseReply
                {
                    Success = false,
                    Message = "基坑传感器类型不能为空"
                };
            }
            //执行前等待1-3秒，放到真正执行逻辑中更好
            //await Task.Delay(new Random().Next(1000,3000));
            var channel = CreateAuthenticatedChannel(ServerUrl);
            var client = new ReverseCommand.ReverseCommandClient(channel);
            var response = await client.ExecuteAsync(new ReverseRequest
            {
                SerialNo = serialNo,
                Command = model.CommandName,
                Body = JsonConvert.SerializeObject(model, jsonSettings)
            }, GetCallOptions());
            return response;
        }

#endregion

#region 私有方法

        /// <summary>
        /// 构建含有 Authorization token的 Channel
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        private static ChannelBase CreateAuthenticatedChannel(string address)
        {
#if NETSTANDARD2_0
            return new Channel(address, ChannelCredentials.Insecure, new List<ChannelOption>() { new ChannelOption("grpc.enable_http_proxy", 0) });
#else
            var channelCredentials = ChannelCredentials.Create(new SslCredentials(), GetCallCredentials());
            return GrpcChannel.ForAddress(address, new GrpcChannelOptions
            {
                Credentials = channelCredentials
            });
#endif
        }

        /// <summary>
        /// 获取Credentials
        /// </summary>
        /// <returns></returns>
        private static CallCredentials GetCallCredentials()
        {
            return CallCredentials.FromInterceptor((context, metadata) =>
            {
                var unixTimeStamp = DateTime.Now.ToUnixTimestamp();
                var token = GenerateToken(AppId, AppKey, AppSecret, unixTimeStamp);
                if (!string.IsNullOrWhiteSpace(token))
                {
                    metadata.Add("AppId", $"{AppId}");
                    metadata.Add("AppKey", $"{AppKey}");
                    metadata.Add("AppToken", $"{token}");
                    metadata.Add("TimeStamp", $"{unixTimeStamp}");
                }
                return Task.CompletedTask;
            });
        }

        /// <summary>
        /// 生成token
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="appKey"></param>
        /// <param name="appSecret"></param>
        /// <param name="unixTimeStamp"></param>
        /// <returns></returns>
        private static string GenerateToken(string appId, string appKey, string appSecret, long unixTimeStamp)
        {
            var plainText = $"a={appId}&s={appSecret}&t={unixTimeStamp}&v=9";

            using (var mac = new HMACSHA1(Encoding.UTF8.GetBytes(appKey)))
            {
                var hash = mac.ComputeHash(Encoding.UTF8.GetBytes(plainText));
                var pText = Encoding.UTF8.GetBytes(plainText);
                var all = new byte[hash.Length + pText.Length];
                Array.Copy(hash, 0, all, 0, hash.Length);
                Array.Copy(pText, 0, all, hash.Length, pText.Length);
                return Convert.ToBase64String(all);
            }
        }

        private static CallOptions GetCallOptions()
        {
#if NETSTANDARD2_0
            var unixTimeStamp = (DateTime.Now.Ticks - 621355968000000000) / 10000000;
            var token = GenerateToken(AppId, AppKey, AppSecret, unixTimeStamp);

            var metadata = new Metadata();
            metadata.Add("AppId", $"{AppId}");
            metadata.Add("AppKey", $"{AppKey}");
            metadata.Add("AppToken", $"{token}");
            metadata.Add("TimeStamp", $"{unixTimeStamp}");

            return new CallOptions(headers: metadata);
#else 
            return new CallOptions();
#endif
        }

        #endregion

        /// <summary>
        /// 返回10位时间戳(s) Timestamp
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static long ToUnixTimestamp(this DateTime target)
        {
            //Ticks：此属性的值表示自 0001 年 1 月 1 日凌晨 00:00:00 以来已经过的时间的以 100 纳秒为间隔的间隔数(1毫秒有10000个计时周期)。
            //从 0000年00月00日00：00：00 - 1970年01月01日00：00：00的刻度值(毫秒)1970 × 365 × 24 × 60 × 60 × 1000 × 10000 大概等于 621355968000000000
            //以上表达式的意思是要取得从1970/01/01 00:00:00 到现在经过的毫秒数了
            return (target.Ticks - TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local).Ticks) / 10000 / 1000;
        }
    }
}