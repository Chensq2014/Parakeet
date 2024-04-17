using System;
using Serilog;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp.Dtos;
using Newtonsoft.Json;
using Common.Extensions;

namespace ConsoleApp.Area.Others
{
    /// <summary>
    /// 其它测试
    /// </summary>
    public static class OtherTest
    {
        /// <summary>
        /// 前缀
        /// //"http://172.16.8.122:5000";
        /// //"http://tunnel.sunbl.xyz:10001";
        /// //"http://6.6.6.151:8006";
        /// //"http://parse.spdyun.cn";
        /// </summary>
        public static readonly string Target = "http://parse.spdyun.cn";

        /// <summary>
        /// 设置路径
        /// </summary>
        public static readonly string Area = "sichuan";//"172.16.8.122:5000";

        /// <summary>
        /// 加密信息
        /// </summary>
        private static readonly KeySecret _keySecret = new KeySecret
        {
            SupplierKeyId = "440742E5-AAE9-47F1-8931-C3B0393CE421",
            SupplierKeySecret = "2VMq6793Dv0os9p0oYee4LeYSCappXGKH8ez",
            ProjectKeyId = "440742E5-AAE9-47F1-8931-C3B0393CE427",
            ProjectKeySecret = "E3mjRc0LsJ7uKxQmIcs4wgDCmnubH3JajMLk"
        };

        /// <summary>
        /// 获取实名制注册Content
        /// </summary>
        /// <returns></returns>
        public static HttpContent GetRegisterContent()
        {
            var encodePhoto = Utilities.Base64Phto.RemoveBase64ImagePrefix().UrlEncode();
            var input = new
            {
                idno = "500108199003079558",
                name = "智管渝",
                gender = 1,
                nation = "汉",
                birthday = "1989-05-23",
                address = "重庆沙坪坝",
                idissue = "重庆沙坪坝公安局",
                idperiod = "20010101-20310101",
                userType = 1,
                dev_mac = "smz-cjkjg",
                regType = 3,
                phone = "18975213456",
                inf_photo = encodePhoto,
                idphoto = encodePhoto,
                photo = encodePhoto
            };
            var list = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("idno", input.idno),
                new KeyValuePair<string, string>("name", input.name),
                new KeyValuePair<string, string>("gender", input.gender.ToString()),
                new KeyValuePair<string, string>("nation", input.nation),
                new KeyValuePair<string, string>("birthday", input.birthday),
                new KeyValuePair<string, string>("address", input.address),
                new KeyValuePair<string, string>("idissue", input.idissue),
                new KeyValuePair<string, string>("idperiod", input.idperiod),
                new KeyValuePair<string, string>("userType", input.userType.ToString()),
                new KeyValuePair<string, string>("dev_mac", input.dev_mac),
                new KeyValuePair<string, string>("regType", input.regType.ToString()),
                new KeyValuePair<string, string>("phone", input.phone),
                new KeyValuePair<string, string>("inf_photo", input.inf_photo),
                new KeyValuePair<string, string>("idphoto", input.idphoto),
                new KeyValuePair<string, string>("photo", input.photo)
            };
            AddContentHeaderParameters(input);

            var content = new FormUrlEncodedContent(list);
            content.Headers.Add("ContentType", "application/x-www-form-urlencoded");
            Log.Logger.Debug($"[成都闸机实名制]设备[{input.dev_mac}]信息数据发送");
            return content;
        }


        /// <summary>
        /// 获取闸机考勤
        /// </summary>
        /// <returns></returns>
        public static HttpContent GetGateContent()
        {
            var input = new
            {
                deviceCode = "zzj-gate",
                peopleIdentity = "510123197807021234",
                actionType = 1,//1入场2离场
                sampleTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
            var content = AddContentHeaderParameters(input);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            Log.Logger.Debug($"[考勤]设备[{input.deviceCode}]信息数据发送");
            return content;
        }


        /// <summary>
        /// 获取环境数据Content
        /// </summary>
        /// <returns></returns>
        public static HttpContent GetEnvironmentContent()
        {
            #region 塔吊运行数据
            var input = new
            {
                deviceCode = "zzj-environment",
                samplePm10 = 27.9,//pm10
                samplePm25 = 35.2,
                sampleNoise = 63.5,
                sampleTemp = 25.00,
                sampleWet = 0.00,
                sampleWindSpeed = 23.0,
                sampleRain = 100.01,
                sampleWindDirect = 274,
                samplePressure = 1.36,
                sampleWdText = "东",//取值范围：东、南、西、北、东南、西北、东北、西南
                sampleTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };

            #endregion

            var content = AddContentHeaderParameters(input);
            SetCraneContentHeaderParameters(content);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            Log.Logger.Debug($"[环境]设备[{input.deviceCode}]基础信息数据发送");
            return content;
        }


        /// <summary>
        /// 设置塔机ContentHeader加密参数
        /// </summary>
        /// <param name="content"></param>
        private static void SetCraneContentHeaderParameters(StringContent content)
        {
            var rCode = Utilities.GenerateRandomString(15);
            var ts = DateTime.Now.ToUnixTimeTicks();
            var keyId = $"{_keySecret?.SupplierKeyId}_{_keySecret?.ProjectKeyId}";
            var signature = Utilities.GetTokenByProject(rCode, ts, _keySecret?.ProjectKeySecret);
            content.Headers.Add("keyId", keyId);
            content.Headers.Add("ts", ts.ToString());
            content.Headers.Add("rCode", rCode);
            content.Headers.Add("signature", signature);
        }


        private static StringContent AddContentHeaderParameters(object input)
        {
            var inputJson = JsonConvert.SerializeObject(input);//TextJsonConvert.SerializeObject(input);
            //Log.Logger.Debug($"Json序列化input:");
            //Log.Logger.Debug($"{JsonConvert.DeserializeObject<object>(inputJson)}");
            Console.WriteLine($"Json序列化input:");
            Console.WriteLine($"{JsonConvert.DeserializeObject<object>(inputJson)}");
            var content = new StringContent(inputJson, Encoding.UTF8);
            return content;
        }




        #region 杭州宇泛人脸设备

        /// <summary>
        /// 设置人脸设备密码
        /// </summary>
        /// <returns></returns>
        public static HttpContent GetGateSerialNoContent()
        {
            var input = new
            {
                name = "getDeviceKey"
            };
            var content = AddContentHeaderParameters(input);
            content.Headers.Add("ContentType", "application/x-www-form-urlencoded");
            Log.Logger.Debug($"获取[考勤]设备序列号");
            return content;
        }


        /// <summary>
        /// 设置人脸设备密码
        /// </summary>
        /// <returns></returns>
        public static HttpContent SetGatePasswordContent()
        {
            var deviceCode = "84E0F424CB5F51FA";//"84E0F424213151FA";
            var input = new
            {
                //deviceCode = "84E0F424213151FA",
                oldPass = "12345678",
                newPass = "12345678"
            };
            var content = AddContentHeaderParameters(input);
            content.Headers.Add("ContentType", "application/x-www-form-urlencoded");
            Log.Logger.Debug($"[考勤]设备[{deviceCode}]密码设置");
            return content;
        }


        /// <summary>
        /// 设置设备心跳回调api
        /// </summary>
        /// <returns></returns>
        public static HttpContent SetGateHeartBeatContent()
        {
            var deviceCode = "84E0F424CB5F51FA";//"84E0F424213151FA";
            var input = new
            {
                pass = 12345678,
                url = $"{Target}/{Area}/hzyf/heartbeat"
            };
            var content = AddContentHeaderParameters(input);
            content.Headers.Add("ContentType", "application/x-www-form-urlencoded");
            Log.Logger.Debug($"[考勤]设备[{deviceCode}]设置心跳回调地址:{input.url}");
            return content;
        }


        /// <summary>
        /// 设置设备获取任务api
        /// </summary>
        /// <returns></returns>
        public static HttpContent SetGateGetTaskContent()
        {
            var deviceCode = "84E0F424CB5F51FA";//"84E0F424213151FA";
            var input = new
            {
                pass = 12345678,
                url = $"{Target}/{Area}/hzyf/getTask"
            };
            var content = AddContentHeaderParameters(input);
            content.Headers.Add("ContentType", "application/x-www-form-urlencoded");
            Log.Logger.Debug($"设置[考勤]设备[{deviceCode}]设备获取任务api:{input.url}");
            return content;
        }


        /// <summary>
        /// 设置设备获取任务回调api
        /// </summary>
        /// <returns></returns>
        public static HttpContent SetGateHandleTaskResultContent()
        {
            var deviceCode = "84E0F424CB5F51FA";//"84E0F424213151FA";
            var input = new
            {
                pass = 12345678,
                url = $"{Target}/{Area}/hzyf/Feedback"
            };
            var content = AddContentHeaderParameters(input);
            content.Headers.Add("ContentType", "application/x-www-form-urlencoded");
            Log.Logger.Debug($"设置[考勤]设备[{deviceCode}]设备获取任务回调api:{input.url}");
            return content;
        }

        /// <summary>
        /// 设置设备识别回调api
        /// </summary>
        /// <returns></returns>
        public static HttpContent SetGateIdentifyCallBackContent()
        {
            var deviceCode = "84E0F424CB5F51FA";//"84E0F424213151FA";
            var input = new
            {
                pass = 12345678,
                base64Enable = 2,
                callbackUrl = $"{Target}/{Area}/hzyf/uploadAttendance"
            };
            var content = AddContentHeaderParameters(input);
            content.Headers.Add("ContentType", "application/x-www-form-urlencoded");
            Log.Logger.Debug($"设置[考勤]设备[{deviceCode}]设备识别回调api:{input.callbackUrl}");
            return content;
        }


        /// <summary>
        /// 获取设备人员api
        /// </summary>
        /// <returns></returns>
        public static HttpContent GetPersonByPageContent()
        {
            var deviceCode = "84E0F424CB5F51FA";//"84E0F424213151FA";
            var input = new
            {
                pass = 12345678,
                personId = -1,
                length = 1000,
                index = 0
            };
            //AddContentHeaderParameters(input);
            var content = AddContentHeaderParameters(input);

            #region content

            ////var content = new StringContent(TextJsonConvert.SerializeObject(input), Encoding.UTF8, "application/json-patch+json");

            //var content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
            //{
            //    new KeyValuePair<string, string>("pass",input.pass.ToString()),
            //    new KeyValuePair<string, string>("personId",input.personId.ToString()),
            //    new KeyValuePair<string, string>("index",input.index.ToString()),
            //    new KeyValuePair<string, string>("length",input.length.ToString())
            //});
            #endregion

            content.Headers.Add("ContentType", "application/x-www-form-urlencoded");
            Log.Logger.Debug($"[考勤]设备[{deviceCode}]人员:{input.index + 1}页，top:{input.length}人");
            return content;
        }

        /// <summary>
        /// 删除设备所有人员api
        /// </summary>
        /// <returns></returns>
        public static HttpContent PersonDeleteAll()
        {
            var deviceCode = "84E0F424CB5F51FA";//"84E0F424213151FA";
            var input = new
            {
                pass = 12345678,
                id = -1
            };
            //AddContentHeaderParameters(input);
            var content = AddContentHeaderParameters(input);

            #region content

            ////var content = new StringContent(TextJsonConvert.SerializeObject(input), Encoding.UTF8, "application/json-patch+json");

            //var content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
            //{
            //    new KeyValuePair<string, string>("pass",input.pass.ToString()),
            //    new KeyValuePair<string, string>("personId",input.personId.ToString()),
            //    new KeyValuePair<string, string>("index",input.index.ToString()),
            //    new KeyValuePair<string, string>("length",input.length.ToString())
            //});
            #endregion

            content.Headers.Add("ContentType", "application/x-www-form-urlencoded");
            Log.Logger.Debug($"[考勤]设备[{deviceCode}]清空所有人员");
            return content;
        }


        #endregion


        #region 厦门会展中心一标段扬尘设备

        /// <summary>
        /// 厦门会展中心一标段考勤历史数据Content
        /// </summary>
        /// <returns></returns>
        public static HttpContent GetXiamenSectionOneGateHistoryContent()
        {
            var now = DateTime.Now;
            var input = new
            {
                pageSize = 100,
                pageIndex = 0,
                begin = now.AddDays(-1),
                end = now
            };
            //AddContentHeaderParameters(input);
            var content = AddContentHeaderParameters(input);

            #region content

            //var content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
            //{
            //    new KeyValuePair<string, string>("pageSize",input.pageSize.ToString()),
            //    new KeyValuePair<string, string>("pageIndex",input.pageIndex.ToString())
            //});
            #endregion
            content.Headers.Add("Cookie", "satoken=f9f4183d-5b7b-4c9b-bda7-2401def00833");
            //content.Headers.Add("ContentType", "application/x-www-form-urlencoded");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            Log.Logger.Debug($"厦门会展中心一标段[考勤]设备第[{input.pageIndex}页{input.pageSize}]条数据");
            return content;
        }

        /// <summary>
        /// 厦门会展中心二标段考勤历史数据Content
        /// </summary>
        /// <returns></returns>
        public static HttpContent GetXiamenSectionTwoGateHistoryContent()
        {
            var now = DateTime.Now;
            var input = new
            {
                appid = "",
                sign = "",
                beginDate = now.AddDays(-1),
                endDate = now,
                inOutType = "IN",//IN进，OUT出
                projectId = "",
                startId = 0,
                pageSize = 100
            };
            //AddContentHeaderParameters(input);
            //var content = AddContentHeaderParameters(input);

            #region content

            var content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("pageSize",input.pageSize.ToString())
            });
            #endregion
            //content.Headers.Add("Cookie", "satoken=f9f4183d-5b7b-4c9b-bda7-2401def00833");
            //content.Headers.Add("ContentType", "application/x-www-form-urlencoded");
            //content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            Log.Logger.Debug($"厦门会展中心二标段[考勤]设备 当前{input.pageSize}条数据");
            return content;
        }


        #endregion


        #region 奥思恩(扬尘)设备

        /// <summary>
        /// 奥思恩扬尘设备实时数据Content
        /// </summary>
        /// <returns></returns>
        public static HttpContent GetAoSienEnvironmentRealtimeContent()
        {
            var input = new
            {
                //SN = "2021060203100003",//2021060203100003对应的sn是 ：MjAyMTA2MDIwMzEwMDAwMw==
                SN = "MjAyMTA2MDIwMzEwMDAwMw==",//2021060203100003对应的sn是 ：MjAyMTA2MDIwMzEwMDAwMw==
                //requestPath = "realtime"//|realtime|minute|hour|day
            };
            //AddContentHeaderParameters(input);
            //var content = AddContentHeaderParameters(input);

            #region content

            var content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("SN",input.SN)
                //new KeyValuePair<string, string>("requestPath",input.requestPath)
            });
            #endregion

            //content.Headers.Add("ContentType", "application/x-www-form-urlencoded");
            Log.Logger.Debug($"奥思恩[扬尘]设备[{input.SN}]数据");
            return content;
        }

        /// <summary>
        /// 奥思恩扬尘设备历史数据Content
        /// </summary>
        /// <returns></returns>
        public static HttpContent GetAoSienEnvironmentHistoryContent()
        {
            var now = DateTime.Now;
            var input = new
            {
                requestPath = "realtime",//|realtime|minute|hour|day
                sn = "MjAyMTA2MDIwMzEwMDAwMw==",//2021060203100003对应的sn是 ：MjAyMTA2MDIwMzEwMDAwMw==
                startTime = $"{now.AddHours(-1):yyyy-MM-dd HH:mm:ss}",
                endTime = $"{now:yyyy-MM-dd HH:mm:ss}"
            };
            //AddContentHeaderParameters(input);
            var content = AddContentHeaderParameters(input);

            #region content

            //var content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
            //{
            //    new KeyValuePair<string, string>("sn",input.sn),
            //    new KeyValuePair<string, string>("requestPath",input.requestPath),
            //    new KeyValuePair<string, string>("startTime",startTime),
            //    new KeyValuePair<string, string>("endTime",endTime)
            //});
            #endregion

            //content.Headers.Add("ContentType", "application/x-www-form-urlencoded");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            Log.Logger.Debug($"奥思恩[扬尘]设备[{input.sn}]数据");
            return content;
        }

        #endregion


        #region 沙区管网

        /// <summary>
        /// 沙区管网获取设备常规属性数据Content
        /// </summary>
        /// <returns></returns>
        public static HttpContent GetShaquGuanwangDeviceInfosContent()
        {
            var input = new
            {
                //devGroup = "井盖位移",
                //devId = "06110016",
                //remark = $"xxx"
            };
            var content = AddContentHeaderParameters(input);
            #region url content

            //var content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
            //{
            //    new KeyValuePair<string, string>("sn",input.sn),
            //    new KeyValuePair<string, string>("requestPath",input.requestPath),
            //    new KeyValuePair<string, string>("startTime",startTime),
            //    new KeyValuePair<string, string>("endTime",endTime)
            //});
            //content.Headers.Add("ContentType", "application/x-www-form-urlencoded");
            #endregion

            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            Log.Logger.Debug($"沙区管网获取设备常规属性数据");
            return content;
        }

        /// <summary>
        /// 沙区管网获取设备历史数据Content
        /// </summary>
        /// <returns></returns>
        public static HttpContent GetShaquGuanwangDeviceHistoryContent()
        {
            var now = DateTime.Now;
            var input = new
            {
                startTime = now.AddDays(-1),
                endTime = now,
                rutName="井盖位移",
                isAlarm="normal",
                devId="0000000010",
                channel=20
            };
            //AddContentHeaderParameters(input);
            var content = AddContentHeaderParameters(input);

            #region content

            //var content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
            //{
            //    new KeyValuePair<string, string>("startTime",input.startTime),
            //    new KeyValuePair<string, string>("endTime",input.endTime),
            //});
            //content.Headers.Add("ContentType", "application/x-www-form-urlencoded");
            #endregion

            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            Log.Logger.Debug($"沙区管网获取设备历史数据");
            return content;
        }

        /// <summary>
        /// 沙区管网获取设备标定数据Content
        /// </summary>
        /// <returns></returns>
        public static HttpContent GetShaquGuanwangDeviceLocationContent()
        {
            var input = new
            {
                project = "跳蹬河",
                rfid = "11911110285"
            };
            //AddContentHeaderParameters(input);
            var content = AddContentHeaderParameters(input);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            Log.Logger.Debug($"沙区管网获取设备标定数据");
            return content;
        }

        #endregion

    }

}
