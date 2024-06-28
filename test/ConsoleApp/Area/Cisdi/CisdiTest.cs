using ConsoleApp.Dtos;
using Common.Extensions;
using System;
using Serilog;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace ConsoleApp.Area.Cisdi
{
    /// <summary>
    /// 中冶塞迪测试
    /// </summary>
    public static class CisdiTest
    {
        public static string OrganizationId = "科技(重庆)有限公司";
        public static string ClassId = "现场班组";
        public static string UserId = "001";

        /// <summary>
        /// 测试环境 加密信息
        /// </summary>
        private static KeySecret _keySecret = new KeySecret
        {
            SupplierKeyId = "496e092b-17f0-4b67-a961-56abb54fed4c",
            SupplierKeySecret = "bhBDJc448Ul0LS6cGkZPObIDqtQQtjKCLI23",
            ProjectKeyId = "854e82a217bc41eaa44351e086624634",
            ProjectKeySecret = "HKmyboKblHoKE5x5ezmzqPMsYHsedpaCNv8v",
            DeviceCode = "qzdevicecodetest001"
        };

        ///// <summary>
        ///// 正式环境 加密信息
        ///// </summary>
        //private static readonly KeySecret _keySecret = new KeySecret
        //{
        //    SupplierKeyId = "496e092b-17f0-4b67-a961-56abb54fed4c",
        //    SupplierKeySecret = "bhBDJc448Ul0LS6cGkZPObIDqtQQtjKCLI23",
        //    ProjectKeySecret = "HKmyboKblHoKE5x5ezmzqPMsYHsedpaCNv8v",
        //    ProjectKeyId = "daa5b4a93dc5421abfcacf34f870ba04",//正式环境项目Id
        //    DeviceCode = "Y0028096400005"//扬尘设备
        //};

        /// <summary>
        /// 添加组织机构 
        /// </summary>
        /// <returns></returns>
        public static HttpContent AddOrganization()
        {
            var dataList = new List<object>
            {
                new
                {
                    projectId = _keySecret?.ProjectKeyId,
                    receiveId = OrganizationId,
                    //receiveParentId = organizationId,
                    name = OrganizationId,
                    type = 1.ToString()//1: 公司 2:班组

                }
    };
            var input = new { dataList };
            var content = GetContent(input);
            Log.Logger.Debug($"[中冶赛迪]添加组织机构[{OrganizationId}]数据发送");
            return content;
        }

        /// <summary>
        /// 添加组织机构下的班组信息
        /// </summary>
        /// <returns></returns>
        public static HttpContent AddClass()
        {
            var dataList = new List<object>
            {
                new
                {
                    projectId = _keySecret?.ProjectKeyId,
                    receiveId = ClassId,
                    receiveParentId = OrganizationId,
                    name = ClassId,
                    type = 2.ToString()//1: 公司 2:班组
                }
            };
            var input = new { dataList };
            var content = GetContent(input);
            Log.Logger.Debug($"[中冶赛迪]添加组织机构：[{OrganizationId}]班组：[{ClassId}]数据发送");
            return content;
        }


        /// <summary>
        /// 现场人员新增 (实名制采集)门禁
        /// </summary>
        /// <returns></returns>
        public static HttpContent GetWorkerRegisterContent()
        {
            var photo = Utilities.Base64Phto.RemoveBase64ImagePrefix();//.UrlEncode();
            var dataList = new List<object>();
            var parents = new List<object>();
            var reciveGroup = new
            {
                receiveId = ClassId,
                isMain = 0.ToString(),
            };
            parents.Add(reciveGroup);
            var item = new
            {
                projectId = _keySecret?.ProjectKeyId,
                code = _keySecret?.DeviceCode,
                name = UserId,
                idType = 1.ToString(),//身份证
                idNumber = "500108199003071150",
                nation = "汉",
                sex = 1.ToString(),
                age = 25.ToString(),
                workTypeName = "1",
                linkmanName = UserId,
                linkmanPhone = "18975213456",
                birthday = "1989-05-23",
                laborGuard = UserId,
                province = "四川",
                city = "成都",
                town = "天府新城",
                address = "地址",
                headIcon = photo,//无需 base64头部标识 System.Web.HttpUtility.UrlEncode(data.Data.Photo)
                //marital=data.Data.Name,
                //politicsType=3,
                //isJoin=0,
                //education=0,
                hasBadMedicalHistory = 0.ToString(),
                recordTime = DateTime.Now.ToString("yyyy-MM-dd"),//HH:mm:ss
                parents = parents,//班组信息 数组
                nature = 1.ToString()
            };
            dataList.Add(item);
            var input = new { dataList };

            var content = GetContent(input);

            Log.Logger.Debug($"[中冶赛迪考勤]设备[{UserId}]注册数据发送");
            return content;
        }



        /// <summary>
        /// 考勤现场人员删除 Content
        /// </summary>
        /// <returns></returns>
        public static HttpContent GetWorkerDeleteContent()
        {
            var photo = Utilities.Base64Phto.RemoveBase64ImagePrefix();//.UrlEncode();
            var dataList = new List<object>();
            var user = new
            {
                projectId = _keySecret?.ProjectKeyId,
                laborGuard = UserId,
            };
            dataList.Add(user);
            var input = new { dataList };
            var content = GetContent(input);

            Log.Logger.Debug($"[中冶赛迪考勤]设备[{UserId}]人员删除数据发送");
            return content;
        }


        /// <summary>
        /// 考勤 Content
        /// </summary>
        /// <returns></returns>
        public static HttpContent GetGateContent()
        {
            var photo = Utilities.Base64Phto.RemoveBase64ImagePrefix();//.UrlEncode();
            var dataList = new List<object>();
            var item = new
            {
                laborGuard = UserId,
                code = _keySecret?.DeviceCode,
                inOrOut = 0.ToString(),
                recordTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                img = photo
            };
            dataList.Add(item);
            var input = new
            {
                projectId = _keySecret?.ProjectKeyId,
                code = _keySecret?.DeviceCode,
                dataList = dataList
            };
            var content = GetContent(input);

            Log.Logger.Debug($"[中冶赛迪考勤]设备[{input.code}]数据发送");
            return content;
        }


        /// <summary>
        /// 扬尘 Content
        /// </summary>
        /// <returns></returns>
        public static HttpContent GetEnvironmentContent()
        {
            var environments = new List<object>();
            var environment = new
            {
                device = _keySecret.DeviceCode,
                temperature = "25.2",
                humidity = "25.2",
                pm25 = "25.2",
                pm10 = "25.2",
                noise = "25.2",
                windSpeed = "25.2",
                windDirection = "25.2",
                pressure = "25.3"
            };
            environments.Add(environment);
            var input = new
            {
                code = _keySecret.ProjectKeyId,
                recordTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),//data.Data.Records.FirstOrDefault()?.RecordTime.ToString("yyyy-MM-dd HH:mm:ss"),
                data = environments
            };
            var content = GetContent(input);

            Log.Logger.Debug($"[湖南扬尘]设备[{environment.device}]数据发送");
            return content;
        }


        /// <summary>
        /// 塔吊 Content
        /// </summary>
        /// <returns></returns>
        public static HttpContent GetCraneContent()
        {
            var cranes = new List<object>();
            var crane = new
            {
                device = _keySecret.DeviceCode,
                weight = 120,
                windSpeed = 120,
                height = 120,
                ranges = 120,
                rotation = 15,
                bevelAngle = 12,
                moment = 123,
                cliverHeight = 42,
                operatorId = UserId,//操作人员Id 从数据库中查?
                stateStaff = 0,//人员在岗状态：0在岗；1 离岗
                fall = 1.2,//item.Ratio
            };
            cranes.Add(crane);

            var input = new
            {
                code = _keySecret.ProjectKeyId,
                recordTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),//data.Data.Records.FirstOrDefault()?.RecordTime.ToString("yyyy-MM-dd HH:mm:ss"),
                data = cranes
            };
            var content = GetContent(input);

            Log.Logger.Debug($"[中冶赛迪塔吊]设备[{crane.device}]数据发送");
            return content;
        }



        /// <summary>
        /// 塔吊攀爬 Content
        /// </summary>
        /// <returns></returns>
        public static HttpContent GetCraneWorkCycleContent()
        {
            var cranes = new List<object>();
            var crane = new
            {
                device = _keySecret.DeviceCode,
                operatorId = UserId,
                acpower = 112.ToString(),
                acalarm = 0.ToString(),
                state = 1.ToString(),
                speed = 123.ToString(),
                pull = 123.ToString(),
                height = 123.ToString(),
                dayload = 123.ToString(),
                remainpower = "",//剩余电量
                dcpower = "",
                dcalarm = "",
                dayoverload = "",//过载次数
                daycharge = "",//充电次数
                daybatalarm = "",//电池警告次数
                speel = "" //在岗标识
            };
            cranes.Add(crane);
            var input = new
            {
                code = _keySecret.ProjectKeyId,
                recordTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),//data.Data.Records.FirstOrDefault()?.RecordTime.ToString("yyyy-MM-dd HH:mm:ss"),
                data = cranes
            };
            var content = GetContent(input);

            Log.Logger.Debug($"[中冶赛迪塔吊]设备[{crane.device}]攀爬数据发送");
            return content;
        }





        /// <summary>
        /// 升降机 Content
        /// </summary>
        /// <returns></returns>
        public static HttpContent GetLifterContent()
        {
            var lifters = new List<object>();
            var lifter = new
            {
                device = _keySecret.DeviceCode,
                cload = 123.ToString(),
                cperson = (string)null,
                speed = 123.5,
                height = 123,
                state = "",
                operatorId = UserId,
                stateStaff = 0,//人员在岗状态：0在岗；1 离岗
            };
            lifters.Add(lifter);
            var input = new
            {
                code = _keySecret.ProjectKeyId,
                recordTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),//data.Data.Records.FirstOrDefault()?.RecordTime.ToString("yyyy-MM-dd HH:mm:ss"),
                data = lifters
            };
            var content = GetContent(input);

            Log.Logger.Debug($"[中冶赛迪升降机]设备[{lifter.device}]数据发送");
            return content;
        }

        /// <summary>
        /// 获取视频Content 
        /// </summary>
        /// <returns></returns>
        public static HttpContent GetVideoContent()
        {
            var videos = new List<object>();
            var video = new
            {
                device = _keySecret?.DeviceCode,
                //这是人员的接口
                //macId = Guid.NewGuid(),//基站标识
                //macName = "基站名称",//基站名称
                //hatId = "安全帽标识",//安全帽标识
                //stateHat = 0,//带帽状态：0:带帽； 1：脱帽
                //stateStaff = 0,//人员状态：0:正常；1：异常
                //remainBattery = 0.5,//剩余电量，百分比
                //enterSignal = 0//进出区域标识：0： 进入区域； 1：离开区域
            };
            videos.Add(video);
            var input = new
            {
                code = _keySecret?.ProjectKeyId,
                recordTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),//data.Data.Records.FirstOrDefault()?.RecordTime.ToString("yyyy-MM-dd HH:mm:ss"),
                generalInfo = videos
            };
            var content = GetContent(input);

            Log.Logger.Debug($"[中冶赛迪视频]设备[{video.device}]数据发送");
            return content;
        }


        private static StringContent GetContent(object input)
        {
            Log.Logger.Debug($"Json序列化input:");
            Log.Logger.Debug($"{TextJsonConvert.DeserializeObject<object>(TextJsonConvert.SerializeObject(input))}");
            var content = new StringContent(TextJsonConvert.SerializeObject(input), Encoding.UTF8);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return content;
        }


    }

}
