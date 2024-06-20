using Parakeet.Net.Protos;
using Parakeet.Net.ROClient.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Parakeet.Net.ROClient
{
    /// <summary>
    /// 设备反控客户端
    /// </summary>
    public class ReverseControlClient
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public ReverseControlClient()
        {
        }

        /// <summary>
        /// 带参构造函数
        /// </summary>
        /// <param name="appId">授权Id</param>
        /// <param name="appKey">授权Key</param>
        /// <param name="appSecret">授权密钥</param>
        /// <param name="host">host主机【本机带端口】</param>
        public ReverseControlClient(string appId, string appKey, string appSecret, string host=null)
        {
            GrpcOption.Instance.Initialize(appId, appKey, appSecret,host);
        }

        /// <summary>
        /// 驼峰序列化
        /// </summary>
        private readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        #region 下发人员到设备

        /// <summary>
        /// 多设备 批量下发人员
        /// </summary>
        /// <param name="serialNos">设备编码集合</param>
        /// <param name="models">待下发人员集合</param>
        /// <returns>返回批量下发结果</returns>
        public async Task<List<ResponseWrapper>> ExecutePersonsAddCommandAsync(List<string> serialNos, List<PersonAddedModel> models)
        {
            var replies = new List<ResponseWrapper>();

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
        public async Task<List<ResponseWrapper>> ExecutePersonsAddCommandAsync(List<string> serialNos, PersonAddedModel model)
        {
            var replies = new List<ResponseWrapper>();

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
        public async Task<List<ResponseWrapper>> ExecutePersonsAddCommandAsync(string serialNo, List<PersonAddedModel> models)
        {
            var replies = new List<ResponseWrapper>();

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
        public async Task<ResponseWrapper> ExecutePersonAddCommandAsync(string serialNo, PersonAddedModel model)
        {
            if (string.IsNullOrWhiteSpace(serialNo))
            {
                return ResponseWrapper.Error("设备编码不能为空");
            }

            if (string.IsNullOrWhiteSpace(model.IdCard))
            {
                return new ResponseWrapper
                {
                    Success = false,
                    Message = "身份证编码不能为空"
                };
            }

            if (string.IsNullOrWhiteSpace(model.Name))
            {
                return new ResponseWrapper
                {
                    Success = false,
                    Message = "人员姓名不能为空"
                };
            }

            if (string.IsNullOrWhiteSpace(model.PersonnelId))
            {
                return new ResponseWrapper
                {
                    Success = false,
                    Message = "人员Id不能为空"
                };
            }

            var channel = GrpcOption.Instance.Channel;
            var client = new ReverseCommand.ReverseCommandClient(channel);

            var response = await client.ExecuteAsync(new ReverseRequest
            {
                SerialNo = serialNo,
                Command = model.CommandName,
                Body = JsonConvert.SerializeObject(model, _jsonSettings)
            }, GrpcOption.Instance.CallOptions);

            await Task.Delay(1000);

            return new ResponseWrapper()
            {
                Success = response.Success,
                Message = response.Message,
                Code = response.Code
            };
        }

        #endregion 下发人员到设备

        #region 从设备删除人员

        /// <summary>
        /// 删除人员(支持删除多条)
        /// </summary>
        /// <param name="serialNo">设备编码</param>
        /// <param name="model">待删除人员信息</param>
        /// <returns>返回删除人员结果</returns>
        public async Task<ResponseWrapper> ExecutePersonDeleteCommandAsync(string serialNo, PersonDeletedModel model)
        {
            if (string.IsNullOrWhiteSpace(serialNo))
            {
                return ResponseWrapper.Error("设备编码不能为空");
            }

            if (model.PersonnelIds == null || model.PersonnelIds.Length == 0)
            {
                return new ResponseWrapper
                {
                    Success = false,
                    Message = "人员编码不能为空"
                };
            }

            var channel = GrpcOption.Instance.Channel;
            var client = new ReverseCommand.ReverseCommandClient(channel);
            var response = await client.ExecuteAsync(new ReverseRequest
            {
                SerialNo = serialNo,
                Command = model.CommandName,
                Body = JsonConvert.SerializeObject(model, _jsonSettings)
            }, GrpcOption.Instance.CallOptions);

            return new ResponseWrapper()
            {
                Success = response.Success,
                Message = response.Message,
                Code = response.Code
            };
        }

        #endregion 从设备删除人员

        #region 人员注册

        /// <summary>
        /// 单设备批量人员注册
        /// </summary>
        /// <param name="serialNo">设备编码</param>
        /// <param name="models">待注册人员集合</param>
        /// <returns>返回批量注册人员结果</returns>
        public async Task<List<ResponseWrapper>> ExecutePersonRegisterCommandAsync(string serialNo, List<PersonRegisterModel> models)
        {
            var replies = new List<ResponseWrapper>();
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
        public async Task<ResponseWrapper> ExecutePersonRegisterCommandAsync(string serialNo, PersonRegisterModel model)
        {
            if (string.IsNullOrWhiteSpace(serialNo))
            {
                return ResponseWrapper.Error("设备编码不能为空");
            }

            if (string.IsNullOrWhiteSpace(model.IdCard))
            {
                return new ResponseWrapper
                {
                    Success = false,
                    Message = "身份证编码不能为空"
                };
            }

            if (string.IsNullOrWhiteSpace(model.Name))
            {
                return new ResponseWrapper
                {
                    Success = false,
                    Message = "人员姓名不能为空"
                };
            }

            if (string.IsNullOrWhiteSpace(model.PersonnelId))
            {
                return new ResponseWrapper
                {
                    Success = false,
                    Message = "人员Id不能为空"
                };
            }

            if (string.IsNullOrWhiteSpace(model.Photo) && string.IsNullOrWhiteSpace(model.PhotoUrl))
            {
                return new ResponseWrapper
                {
                    Success = false,
                    Message = "人员照片或Url不能为空"
                };
            }

            //注册人员含有图片等信息也等待1秒，放在真正注册逻辑中
            var channel = GrpcOption.Instance.Channel;
            var client = new ReverseCommand.ReverseCommandClient(channel);
            var response = await client.ExecuteAsync(new ReverseRequest
            {
                SerialNo = serialNo,
                Command = model.CommandName,
                Body = JsonConvert.SerializeObject(model, _jsonSettings)
            }, GrpcOption.Instance.CallOptions);

            return new ResponseWrapper
            {
                Success = response.Success,
                Message = response.Message,
                Code = response.Code
            };
        }

        #endregion 人员注册

        #region 执行基坑终端设置命令

        /// <summary>
        /// 批量执行基坑终端设置命令
        /// </summary>
        /// <param name="serialNos">设备编码集合</param>
        /// <param name="models">模型集合</param>
        /// <returns>返回批量执行结果</returns>
        public async Task<List<ResponseWrapper>> ExecuteFPTerminalSettingCommandAsync(List<string> serialNos, List<FPTerminalSetting> models)
        {
            var replies = new List<ResponseWrapper>();

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
        public async Task<ResponseWrapper> ExecuteFPTerminalSettingCommandAsync(string serialNo, FPTerminalSetting model)
        {
            if (string.IsNullOrWhiteSpace(serialNo))
            {
                return ResponseWrapper.Error("设备编码不能为空");
            }

            if (model.Type > 0x05 || model.Type < 0x01)
            {
                return new ResponseWrapper
                {
                    Success = false,
                    Message = "错误的指令类型"
                };
            }

            if (model.Sequence == 0)
            {
                return new ResponseWrapper
                {
                    Success = false,
                    Message = "流水号不能为0"
                };
            }

            switch (model.Type)
            {
                case 0x01 when string.IsNullOrWhiteSpace(model.IP):
                    return new ResponseWrapper
                    {
                        Success = false,
                        Message = "IP不能为空"
                    };

                case 0x02 when !model.CacheCycle.HasValue:
                    return new ResponseWrapper
                    {
                        Success = false,
                        Message = "缓存周期不能为空"
                    };

                case 0x03 when string.IsNullOrWhiteSpace(model.Time):
                    return new ResponseWrapper
                    {
                        Success = false,
                        Message = "校时不能为空"
                    };

                case 0x04 when !model.Frequency.HasValue:
                    return new ResponseWrapper
                    {
                        Success = false,
                        Message = "数据上报频率不能为空"
                    };

                case 0x05 when !model.WorkingMode.HasValue:
                    return new ResponseWrapper
                    {
                        Success = false,
                        Message = "设备工作模式不能为空"
                    };
            }

            //执行前等待1-3秒，放到真正执行逻辑中更好
            //await Task.Delay(new Random().Next(1000,3000));
            var channel = GrpcOption.Instance.Channel;
            var client = new ReverseCommand.ReverseCommandClient(channel);
            var response = await client.ExecuteAsync(new ReverseRequest
            {
                SerialNo = serialNo,
                Command = model.CommandName,
                Body = JsonConvert.SerializeObject(model, _jsonSettings)
            }, GrpcOption.Instance.CallOptions);

            return new ResponseWrapper()
            {
                Success = response.Success,
                Message = response.Message,
                Code = response.Code
            };
        }

        #endregion 执行基坑终端设置命令

        #region 执行基坑终端基本数据设置命令

        public async Task<ResponseWrapper> ExecuteFPTerminalBasicCommandAsync(string serialNo, FPTerminalBasic model)
        {
            if (string.IsNullOrWhiteSpace(serialNo))
            {
                return ResponseWrapper.Error("设备编码不能为空");
            }

            //如果是固定测斜仪
            if (model.Sensor == FPSensorType.FixInclino)
            {
                if (!model.Deep.HasValue || !model.Length.HasValue || !model.Span.HasValue)
                {
                    return ResponseWrapper.Error("固定测斜仪长度,深度和跨度不能为空");
                }
            }

            //如果是静力水准仪
            if (model.Sensor == FPSensorType.StaticLevel)
            {
                if (!model.Distance.HasValue)
                {
                    return ResponseWrapper.Error("与基准点之间的距离不能为空");
                }
            }

            var channel = GrpcOption.Instance.Channel;
            var client = new ReverseCommand.ReverseCommandClient(channel);
            var response = await client.ExecuteAsync(new ReverseRequest
            {
                SerialNo = serialNo,
                Command = model.CommandName,
                Body = JsonConvert.SerializeObject(model, _jsonSettings)
            }, GrpcOption.Instance.CallOptions);

            return new ResponseWrapper()
            {
                Success = response.Success,
                Message = response.Message,
                Code = response.Code
            };
        }

        public async Task<ResponseWrapper> ExecuteFPTerminalBasicCommandAsync(string serialNo, List<FPTerminalBasic> models)
        {
            if (string.IsNullOrWhiteSpace(serialNo))
            {
                return ResponseWrapper.Error("设备编码不能为空");
            }

            foreach (var model in models)
            {
                //如果是固定测斜仪
                if (model.Sensor == FPSensorType.FixInclino)
                {
                    if (!model.Deep.HasValue || !model.Length.HasValue || !model.Span.HasValue)
                    {
                        return ResponseWrapper.Error("固定测斜仪长度,深度和跨度不能为空");
                    }

                    if (models.All(m => !m.BasePoint))
                    {
                        return ResponseWrapper.Error("缺少基准点");
                    }
                }

                //如果是静力水准仪
                if (model.Sensor == FPSensorType.StaticLevel)
                {
                    if (!model.Distance.HasValue)
                    {
                        return ResponseWrapper.Error("与基准点之间的距离不能为空");
                    }

                    if (models.All(m => !m.BasePoint))
                    {
                        return ResponseWrapper.Error("缺少基准点");
                    }
                }
            }

            var collection = new FPTerminalBasicCollection()
            {
                TerminalBasics = models
            };

            var channel = GrpcOption.Instance.Channel;
            var client = new ReverseCommand.ReverseCommandClient(channel);
            var response = await client.ExecuteAsync(new ReverseRequest
            {
                SerialNo = serialNo,
                Command = collection.CommandName,
                Body = JsonConvert.SerializeObject(collection, _jsonSettings)
            }, GrpcOption.Instance.CallOptions);

            return new ResponseWrapper()
            {
                Success = response.Success,
                Message = response.Message,
                Code = response.Code
            };
        }

        public async Task<ResponseWrapper> ExecuteFPTerminalBasicDeleteCommandAsync(string serialNo, FPTerminalBasicDelete model)
        {
            if (string.IsNullOrWhiteSpace(serialNo))
            {
                return ResponseWrapper.Error("设备编码不能为空");
            }

            if (!model.TerminalId.HasValue)
            {
                return ResponseWrapper.Error("终端编号不能为空");
            }

            if (!model.PointNumber.HasValue)
            {
                return ResponseWrapper.Error("测点编号不能为空");
            }

            if (!model.Sensor.HasValue)
            {
                return ResponseWrapper.Error("传感器类型不能为空");
            }

            var channel = GrpcOption.Instance.Channel;
            var client = new ReverseCommand.ReverseCommandClient(channel);
            var response = await client.ExecuteAsync(new ReverseRequest
            {
                SerialNo = serialNo,
                Command = model.CommandName,
                Body = JsonConvert.SerializeObject(model, _jsonSettings)
            }, GrpcOption.Instance.CallOptions);

            return new ResponseWrapper()
            {
                Success = response.Success,
                Message = response.Message,
                Code = response.Code
            };
        }

        #endregion 执行基坑终端基本数据设置命令

        #region 执行塔吊基础数据设置命令

        public async Task<ResponseWrapper> ExecuteCraneBasicCommandAsync(string serialNo, CraneBasic model)
        {
            if (string.IsNullOrWhiteSpace(serialNo))
            {
                return ResponseWrapper.Error("设备编码不能为空");
            }

            if (!model.CraneId.HasValue)
            {
                return ResponseWrapper.Error("塔吊编号不能为空");
            }

            if (!model.BoomHeight.HasValue)
            {
                return ResponseWrapper.Error("塔吊高度不能为空");
            }

            if (!model.MaxLoadWeight.HasValue)
            {
                return ResponseWrapper.Error("塔吊最大吊重不能为空");
            }

            if (!model.LongArm.HasValue)
            {
                return ResponseWrapper.Error("塔吊大臂长不能为空");
            }

            if (!model.ShortArm.HasValue)
            {
                return ResponseWrapper.Error("塔吊小臂长不能为空");
            }

            var channel = GrpcOption.Instance.Channel;
            var client = new ReverseCommand.ReverseCommandClient(channel);
            var response = await client.ExecuteAsync(new ReverseRequest
            {
                SerialNo = serialNo,
                Command = model.CommandName,
                Body = JsonConvert.SerializeObject(model, _jsonSettings)
            }, GrpcOption.Instance.CallOptions);

            return new ResponseWrapper()
            {
                Success = response.Success,
                Message = response.Message,
                Code = response.Code
            };
        }

        #endregion 执行塔吊基础数据设置命令

        #region 执行升降机基础数据设置命令

        public async Task<ResponseWrapper> ExecuteLifterBasicCommandAsync(string serialNo, LifterBasic model)
        {
            if (string.IsNullOrWhiteSpace(serialNo))
            {
                return ResponseWrapper.Error("设备编码不能为空");
            }

            if (!model.LifterId.HasValue)
            {
                return ResponseWrapper.Error("升降机编号不能为空");
            }

            if (!model.MaxHeight.HasValue)
            {
                return ResponseWrapper.Error("升降机高度不能为空");
            }

            if (!model.MaxWeight.HasValue)
            {
                return ResponseWrapper.Error("升降机最大吊重不能为空");
            }

            if (!model.MaxPeopleNumber.HasValue)
            {
                return ResponseWrapper.Error("升降机最大承载人数不能为空");
            }

            if (!model.MaxTilt.HasValue)
            {
                return ResponseWrapper.Error("升降机最大倾角不能为空");
            }

            if (!model.MaxSpeed.HasValue)
            {
                return ResponseWrapper.Error("升降机最大速度不能为空");
            }

            var channel = GrpcOption.Instance.Channel;
            var client = new ReverseCommand.ReverseCommandClient(channel);
            var response = await client.ExecuteAsync(new ReverseRequest
            {
                SerialNo = serialNo,
                Command = model.CommandName,
                Body = JsonConvert.SerializeObject(model, _jsonSettings)
            }, GrpcOption.Instance.CallOptions);

            return new ResponseWrapper()
            {
                Success = response.Success,
                Message = response.Message,
                Code = response.Code
            };
        }

        #endregion 执行升降机基础数据设置命令

        #region 执行视频基础数据上传命令

        /// <summary>
        /// 执行视频基础数据上传
        /// </summary>
        /// <param name="videos"></param>
        /// <returns></returns>
        public async Task<ResponseWrapper> ExecuteVideoBasicCommandAsync(List<VideoBasic> videos)
        {
            if (videos.Count == 0)
            {
                return ResponseWrapper.Error("视频记录不能为空");
            }

            foreach (var video in videos)
            {
                if (string.IsNullOrWhiteSpace(video.SerialNo))
                {
                    return ResponseWrapper.Error("设备编码不能为空");
                }

                if (string.IsNullOrWhiteSpace(video.Source))
                {
                    return ResponseWrapper.Error("视频播放地址不能为空");
                }

                if (string.IsNullOrWhiteSpace(video.CoverUrl))
                {
                    return ResponseWrapper.Error("视频封面地址不能为空");
                }

                if (string.IsNullOrWhiteSpace(video.Name))
                {
                    return ResponseWrapper.Error("视频名称不能为空");
                }
            }

            var firstRecord = videos.FirstOrDefault();

            var channel = GrpcOption.Instance.Channel;
            var client = new ReverseCommand.ReverseCommandClient(channel);
            var response = await client.ExecuteAsync(new ReverseRequest
            {
                SerialNo = firstRecord.SerialNo,
                Command = firstRecord.CommandName,
                Body = JsonConvert.SerializeObject(videos, _jsonSettings)
            }, GrpcOption.Instance.CallOptions);

            return new ResponseWrapper()
            {
                Success = response.Success,
                Message = response.Message,
                Code = response.Code
            };
        }

        #endregion 执行视频基础数据上传命令

        #region 执行视频云台控制命令

        /// <summary>
        /// 执行视频云台控制命令
        /// </summary>
        /// <param name="serialNo"></param>
        /// <param name="setting"></param>
        /// <returns></returns>
        public async Task<ResponseWrapper> ExecuteVideoPTZCommandAsync(string serialNo, VideoDirectSetting setting)
        {
            if (string.IsNullOrWhiteSpace(serialNo))
            {
                return ResponseWrapper.Error("设备编码不能为空");
            }

            if (setting == null)
            {
                return ResponseWrapper.Error("指令不能为空");
            }

            if (setting.Parameter < 1 || setting.Parameter > 7)
            {
                return ResponseWrapper.Error("指令不在合理范围内");
            }

            var channel = GrpcOption.Instance.Channel;
            var client = new ReverseCommand.ReverseCommandClient(channel);
            var response = await client.ExecuteAsync(new ReverseRequest
            {
                SerialNo = serialNo,
                Command = setting.CommandName,
                Body = JsonConvert.SerializeObject(setting, _jsonSettings)
            }, GrpcOption.Instance.CallOptions);

            return new ResponseWrapper()
            {
                Success = response.Success,
                Message = response.Message,
                Code = response.Code
            };
        }

        #endregion 执行视频云台控制命令

        #region 执行人员定位基础数据设置命令

        /// <summary>
        /// 执行人员定位基础信息添加命令
        /// </summary>
        /// <param name="serialNo">设备编码</param>
        /// <param name="model">基础信息</param>
        /// <returns></returns>
        public async Task<ResponseWrapper> ExecutePersonLocationBasicAddedCommand(string serialNo, PersonLocationBasicAddedModel model)
        {
            if (string.IsNullOrWhiteSpace(serialNo))
            {
                return ResponseWrapper.Error("设备编码不能为空");
            }

            if (string.IsNullOrWhiteSpace(model.Name))
            {
                return ResponseWrapper.Error("姓名不能为空");
            }

            if (string.IsNullOrWhiteSpace(model.WorkerType))
            {
                return ResponseWrapper.Error("工种不能为空");
            }

            if (string.IsNullOrWhiteSpace(model.IdCard))
            {
                return ResponseWrapper.Error("身份证号码不能为空");
            }

            var channel = GrpcOption.Instance.Channel;
            var client = new ReverseCommand.ReverseCommandClient(channel);
            var response = await client.ExecuteAsync(new ReverseRequest
            {
                SerialNo = serialNo,
                Command = model.CommandName,
                Body = JsonConvert.SerializeObject(model, _jsonSettings)
            }, GrpcOption.Instance.CallOptions);

            return new ResponseWrapper()
            {
                Success = response.Success,
                Message = response.Message,
                Code = response.Code
            };
        }

        /// <summary>
        /// 执行人员定位基础信息删除命令
        /// </summary>
        /// <param name="serialNo">设备编码</param>
        /// <param name="sensorId">传感器Id(默认1)</param>
        /// <returns></returns>
        public async Task<ResponseWrapper> ExecutePersonLocationBasicDeletedCommand(string serialNo, int sensorId = 1)
        {
            if (string.IsNullOrWhiteSpace(serialNo))
            {
                return ResponseWrapper.Error("设备编码不能为空");
            }

            var model = new PersonLocationBasicDeletedModel()
            {
                SensorId = sensorId
            };

            var channel = GrpcOption.Instance.Channel;
            var client = new ReverseCommand.ReverseCommandClient(channel);
            var response = await client.ExecuteAsync(new ReverseRequest
            {
                SerialNo = serialNo,
                Command = model.CommandName,
                Body = JsonConvert.SerializeObject(model, _jsonSettings)
            }, GrpcOption.Instance.CallOptions);

            return new ResponseWrapper()
            {
                Success = response.Success,
                Message = response.Message,
                Code = response.Code
            };
        }

        #endregion 执行人员定位基础数据设置命令

        #region 执行项目地理围栏设置命令

        /// <summary>
        /// 添加地理围栏
        /// 一个项目包含多个定位设备，serialNo选择其中任意一个即可
        /// </summary>
        /// <param name="serialNo"></param>
        /// <param name="geofencing"></param>
        /// <returns></returns>
        public async Task<ResponseWrapper> ExecuteProjectGeofencingAddedCommand(string serialNo, ProjectGeofenceAddedModel geofencing)
        {
            if (string.IsNullOrWhiteSpace(serialNo))
            {
                return ResponseWrapper.Error("设备编码不能为空");
            }

            if (string.IsNullOrWhiteSpace(geofencing.Name))
            {
                return ResponseWrapper.Error("围栏名称不能为空");
            }

            if (string.IsNullOrWhiteSpace(geofencing.Code))
            {
                return ResponseWrapper.Error("围栏编码不能为空");
            }

            if (geofencing.DeviceType != DeviceType.PersonLocation && geofencing.DeviceType != DeviceType.Vehicle)
            {
                return ResponseWrapper.Error("围栏设备类型错误");
            }

            if (geofencing.CoordArray.Count < 3)
            {
                return ResponseWrapper.Error("围栏坐标不能为空");
            }

            var channel = GrpcOption.Instance.Channel;
            var client = new ReverseCommand.ReverseCommandClient(channel);
            var response = await client.ExecuteAsync(new ReverseRequest
            {
                SerialNo = serialNo,
                Command = geofencing.CommandName,
                Body = JsonConvert.SerializeObject(geofencing, _jsonSettings)
            }, GrpcOption.Instance.CallOptions);

            return new ResponseWrapper()
            {
                Success = response.Success,
                Message = response.Message,
                Code = response.Code
            };
        }

        /// <summary>
        /// 删除项目中的地理围栏
        /// 一个项目包含多个定位设备，serialNo选择其中任意一个即可
        /// </summary>
        /// <param name="serialNo"></param>
        /// <param name="geofencing"></param>
        /// <returns></returns>
        public async Task<ResponseWrapper> ExecuteProjectGeofencingDeletedCommand(string serialNo, ProjectGeofenceDeletedModel geofencing)
        {
            if (string.IsNullOrWhiteSpace(serialNo))
            {
                return ResponseWrapper.Error("设备编码不能为空");
            }

            if (string.IsNullOrWhiteSpace(geofencing.Code))
            {
                return ResponseWrapper.Error("围栏编码不能为空");
            }

            var channel = GrpcOption.Instance.Channel;
            var client = new ReverseCommand.ReverseCommandClient(channel);
            var response = await client.ExecuteAsync(new ReverseRequest
            {
                SerialNo = serialNo,
                Command = geofencing.CommandName,
                Body = JsonConvert.SerializeObject(geofencing, _jsonSettings)
            }, GrpcOption.Instance.CallOptions);

            return new ResponseWrapper()
            {
                Success = response.Success,
                Message = response.Message,
                Code = response.Code
            };
        }

        /// <summary>
        /// 添加用户到地理围栏
        /// 一个项目包含多个定位设备，serialNo选择其中任意一个即可
        /// </summary>
        /// <param name="serialNo"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ResponseWrapper> ExecuteProjectGeofencingUserAddedCommand(string serialNo, ProjectGeofenceDeviceAddedModel model)
        {
            if (string.IsNullOrWhiteSpace(serialNo))
            {
                return ResponseWrapper.Error("设备编码不能为空");
            }

            if (string.IsNullOrWhiteSpace(model.Code))
            {
                return ResponseWrapper.Error("围栏编码不能为空");
            }

            foreach (var user in model.Users)
            {
                if (string.IsNullOrWhiteSpace(user.IdCard) || string.IsNullOrWhiteSpace(user.Name))
                {
                    return ResponseWrapper.Error("用户名或身份证号码不能为空");
                }
            }

            foreach (var car in model.Cars)
            {
                if (string.IsNullOrWhiteSpace(car.CarNumber))
                {
                    return ResponseWrapper.Error("车牌号码不能为空");
                }
            }

            var channel = GrpcOption.Instance.Channel;
            var client = new ReverseCommand.ReverseCommandClient(channel);
            var response = await client.ExecuteAsync(new ReverseRequest
            {
                SerialNo = serialNo,
                Command = model.CommandName,
                Body = JsonConvert.SerializeObject(model, _jsonSettings)
            }, GrpcOption.Instance.CallOptions);

            return new ResponseWrapper()
            {
                Success = response.Success,
                Message = response.Message,
                Code = response.Code
            };
        }

        #endregion 执行人员定位地理围栏设置命令

        #region 执行车辆基础信息设置命令

        /// <summary>
        /// 执行车辆基础信息设置命令
        /// </summary>
        /// <param name="serialNo">设备编码</param>
        /// <param name="basic">基础数据</param>
        /// <returns></returns>
        public async Task<ResponseWrapper> ExecuteVehicleBasicCommand(string serialNo, VehicleBasic basic)
        {
            if (string.IsNullOrWhiteSpace(serialNo))
            {
                return ResponseWrapper.Error("设备编码不能为空");
            }

            if (string.IsNullOrWhiteSpace(basic.CarNumber))
            {
                return ResponseWrapper.Error("车辆牌照不能为空");
            }
            if (string.IsNullOrWhiteSpace(basic.DriverId))
            {
                return ResponseWrapper.Error("驾驶员Id不能为空");
            }
            if (string.IsNullOrWhiteSpace(basic.DriverName))
            {
                return ResponseWrapper.Error("驾驶员姓名不能为空");
            }
            if (string.IsNullOrWhiteSpace(basic.DriverLicense))
            {
                return ResponseWrapper.Error("驾驶员驾照不能为空");
            }
            if (!basic.Birthday.HasValue)
            {
                return ResponseWrapper.Error("驾驶员生日不能为空");
            }

            var channel = GrpcOption.Instance.Channel;
            var client = new ReverseCommand.ReverseCommandClient(channel);
            var response = await client.ExecuteAsync(new ReverseRequest
            {
                SerialNo = serialNo,
                Command = basic.CommandName,
                Body = JsonConvert.SerializeObject(basic, _jsonSettings)
            }, GrpcOption.Instance.CallOptions);

            return new ResponseWrapper()
            {
                Success = response.Success,
                Message = response.Message,
                Code = response.Code
            };
        }

        /// <summary>
        /// 执行车辆基础信息删除命令
        /// </summary>
        /// <param name="serialNo">设备编码</param>
        /// <param name="model">基础数据</param>
        /// <returns></returns>
        public async Task<ResponseWrapper> ExecuteVehicleBasicDeletedCommand(string serialNo, VehicleBasicDeleteModel model)
        {
            if (string.IsNullOrWhiteSpace(serialNo))
            {
                return ResponseWrapper.Error("设备编码不能为空");
            }

            if (Guid.Empty.Equals(model.Id))
            {
                return ResponseWrapper.Error("车辆设备基础信息Id不能为空");
            }

            var channel = GrpcOption.Instance.Channel;
            var client = new ReverseCommand.ReverseCommandClient(channel);
            var response = await client.ExecuteAsync(new ReverseRequest
            {
                SerialNo = serialNo,
                Command = model.CommandName,
                Body = JsonConvert.SerializeObject(model, _jsonSettings)
            }, GrpcOption.Instance.CallOptions);

            return new ResponseWrapper()
            {
                Success = response.Success,
                Message = response.Message,
                Code = response.Code
            };
        }

        #endregion


        #region 执行环境基础数据设置命令

        public async Task<ResponseWrapper> ExecuteEnvironmentBasicCommandAsync(EnvironmentBasic model)
        {
            if (string.IsNullOrWhiteSpace(model.FakeNo))
            {
                return ResponseWrapper.Error("设备编码不能为空");
            }

            if (!model.DeviceId.HasValue)
            {
                return ResponseWrapper.Error("设备Id不能为空");
            }

            var channel = GrpcOption.Instance.Channel;
            var client = new ReverseCommand.ReverseCommandClient(channel);
            var response = await client.ExecuteAsync(new ReverseRequest
            {
                SerialNo = model.FakeNo,
                Command = model.CommandName,
                Body = JsonConvert.SerializeObject(model, _jsonSettings)
            }, GrpcOption.Instance.CallOptions);

            return new ResponseWrapper
            {
                Success = response.Success,
                Message = response.Message,
                Code = response.Code
            };
        }

        #endregion 执行环境基础数据设置命令

        #region 执行闸机基础数据设置命令

        public async Task<ResponseWrapper> ExecuteGateBasicCommandAsync(GateBasic model)
        {
            if (string.IsNullOrWhiteSpace(model.FakeNo))
            {
                return ResponseWrapper.Error("设备编码不能为空");
            }

            if (!model.DeviceId.HasValue)
            {
                return ResponseWrapper.Error("设备Id不能为空");
            }

            var channel = GrpcOption.Instance.Channel;
            var client = new ReverseCommand.ReverseCommandClient(channel);
            var response = await client.ExecuteAsync(new ReverseRequest
            {
                SerialNo = model.FakeNo,
                Command = model.CommandName,
                Body = JsonConvert.SerializeObject(model, _jsonSettings)
            }, GrpcOption.Instance.CallOptions);

            return new ResponseWrapper
            {
                Success = response.Success,
                Message = response.Message,
                Code = response.Code
            };
        }

        #endregion 执行闸机基础数据设置命令
    }
}