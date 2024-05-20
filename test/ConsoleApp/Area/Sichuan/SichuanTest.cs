using System;
using Serilog;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp.Dtos;
using ConsoleApp.HttpClients;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Common.Encrypts;
using Common.Extensions;

namespace ConsoleApp.Area.Sichuan
{
    /// <summary>
    /// 四川省厅测试
    /// </summary>
    public static class SichuanTest
    {
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

        #region 获取token的账号密钥 接口地址等信息

        /// <summary>
        /// Account
        /// </summary>
        public static string Account { get; set; } = "1d3ed1";

        /// <summary>
        /// Password
        /// </summary>
        public static string Password { get; set; } = "cd3a19e68";

        /// <summary>
        /// AppKey
        /// </summary>
        public static string AppKey { get; set; } = "eba3ff98-941c-4ef2-b90b-89b7cf1d8b04";

        /// <summary>
        /// AppSecret
        /// </summary>
        public static string AppSecret { get; set; } = "02DF4EC29C2B4BF290B8B29223EEA087";

        /// <summary>
        /// 管理员身份登录
        /// </summary>
        public static string OrgCode { get; set; }

        /// <summary>
        /// 软件服务商统一社会信用代码
        /// </summary>
        public static string UnitCode = "915001060891260711";
        /// <summary>
        /// 重庆筑智建科技(重庆)科技有限公司
        /// </summary>
        public static string ProjectName = "重庆筑智建科技(重庆)科技有限公司";

        /// <summary>
        /// Host
        /// </summary>
        public static string Host = "http://202.61.90.35";

        /// <summary>
        /// Port
        /// </summary>
        public static string Port = "8010";

        /// <summary>
        /// TokenApi
        /// </summary>
        public static string TokenApi = $@"/SMZ3/api/security/login";


        #endregion

        #region 获取token

        /// <summary>
        /// 获取token
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="apiUrl"></param>
        /// <returns></returns>
        public static async Task<SichuanTokenResultDto> GetAccessToken([CanBeNull] string baseUrl = null, [CanBeNull] string apiUrl = null)
        {
            try
            {
                //_keySecret.ProjectKeyId = "f80d4bce-403b-44fa-966d-d743412366ff";//appKey
                //_keySecret.ProjectKeySecret = "e3ea8a10-564f-4c19-acb0-8d233ddd4c18";//appSecret
                var tokenResult = await HttpClientPool.GetSichuanAccessToken(AppKey, AppSecret, Account, Password, baseUrl ?? $"{Host}:{Port}", apiUrl ?? TokenApi);
                return tokenResult;
            }
            catch (Exception e)
            {
                Console.WriteLine($"获取token错误：{e.Message}");
                throw;
            }
        }

        #endregion


        /// <summary>
        /// 4.1.1	项目基本信息（表名：ProjectInfo）Content
        /// </summary>
        /// <returns></returns>
        public static HttpContent GetProjectInfoContent()
        {
            #region 数据结构
            var project = new
            {
                Code = _keySecret.ProjectKeyId,//项目编码 必填 关联项目基本信息
                PrjCode = UnitCode,//项目代码 NVARCHAR(512) 非必填
                Name = ProjectName, //企业名称 必填
                Description = ProjectName, ////项目简介 NVARCHAR(1000)  O
                //1   房屋建筑工程
                //2   市政工程
                //99  其他
                Category = 1, ////项目类别    INT M   详见数据字典表项目类别
                //1   新建
                //2   改建
                //3   扩建
                ConstructType = 1,//建设性质 //  INT M   详见数据字典表建设性质
                //1   政府投资
                //2   企业投资
                //3   其他投资
                InvestType = 1, //投资类型// INT M   详见数据字典表投资类型
                AreaCode = "510000",//项目所在地 //   NVARCHAR(6) M   详见数据字典表行政区划
                Address = "建设地址",//建设地址 // NVARCHAR(200)   M
                BuildingArea = 120000,//总面积 //   DECIMAL(18, 2)   M   单位：平方米
                BuildingLength = 120000,//总长度  //  DECIMAL(18, 2)   O   单位：米
                Invest = 120000,//总投资 //  DECIMAL(18, 4)   M   单位：万元
                Scale = "项目规模", //项目规模//   NVARCHAR(200)   M
                StartDate = $"{DateTime.Now:yyyy-MM-dd}",//开工日期 //   DATE    M   精确到天, //格式：yyyy - MM - dd
                Lng = 1,//经度 // DECIMAL(18, 15)  O   WGS84经度
                Lat = 1,//纬度  // DECIMAL(18, 15)  O   WGS84纬度
                ThirdPartyProjectCode = "第三方项目编码", //第三方项目编码  NVARCHAR(50)    O   第三方系统为项目创建的编码，同一个系统不能重复编码
            };
            #endregion

            var data = new List<object> { project };
            var input = new
            {
                type = "项目基本信息",
                data
            };
            var content = AddContentHeaderParameters(input);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            //content.Headers.Add("ContentType", "application/json");
            Console.WriteLine($"[四川]项目基本信息[{project.Name}]信息数据发送");
            return content;
        }


        /// <summary>
        /// 4.1.2	项目状态信息（表名：ProjectStatusInfo）Content
        /// </summary>
        /// <returns></returns>
        public static HttpContent GetProjectStatusInfoContent()
        {
            #region 数据结构
            var project = new
            {
                Code = _keySecret.ProjectKeyId,//项目编码 必填 关联项目基本信息
                //1   筹备
                //2   立项
                //3   在建
                //4   完工
                //5   停工
                PrjStatus = 1,//项目状态 必填 M	详见数据字典表项目状态
                StatusChangeDate = $"{DateTime.Now:yyyy-MM-dd}",// DATE    M   精确到天 格式：yyyy - MM - dd
            };
            #endregion

            var data = new List<object> { project };
            var input = new
            {
                type = "项目状态信息",
                data
            };
            var content = AddContentHeaderParameters(input);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            //content.Headers.Add("ContentType", "application/json");
            Console.WriteLine($"[四川]项目状态信息[{project.PrjStatus}]信息数据发送");
            return content;
        }

        /// <summary>
        /// 4.1.3	施工许可证信息（表名：ProjectBuilderLicense）Content
        /// </summary>
        /// <returns></returns>
        public static HttpContent GetProjectBuilderLicenseContent()
        {
            #region 数据结构
            var project = new
            {
                Code = _keySecret.ProjectKeyId,//项目编码 必填 关联项目基本信息
                PrjName = ProjectName,//工程名称	NVARCHAR(200)	M	
                SafetyNo = "安监备案号",//安监备案号   NVARCHAR(40)    M
                BuilderLicenseNum = "施工许可证号",//施工许可证号  NVARCHAR(50)    M
                OrganName = "发证机关",//发证机关   NVARCHAR(50)    M
                OrganDATE = $"{DateTime.Now:yyyy-MM-dd}",//发证日期 DATE    M  
            };
            #endregion

            var data = new List<object> { project };
            var input = new
            {
                type = "施工许可证信息",
                data
            };
            var content = AddContentHeaderParameters(input);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            //content.Headers.Add("ContentType", "application/json");
            Console.WriteLine($"[四川]施工许可证信息[{project.PrjName}]信息数据发送");
            return content;
        }


        /// <summary>
        ///4.1.4	竣工验收信息（表名：TBProjectFinishCheckInfo）Content
        /// </summary>
        /// <returns></returns>
        public static HttpContent GetTBProjectFinishCheckInfoContent()
        {
            #region 数据结构
            var project = new
            {
                Code = _keySecret.ProjectKeyId,//项目编码 必填 关联项目基本信息
                PrjName = ProjectName,//工程名称	NVARCHAR(200)	M	
                PrjFinishCheckNum = "竣工验收编号",//竣工验收编号   NVARCHAR(40)    M
                OrganDATE = $"{DateTime.Now:yyyy-MM-dd}",//竣工验收日期 DATE    M  
            };
            #endregion

            var data = new List<object> { project };
            var input = new
            {
                type = "竣工验收信息",
                data
            };
            var content = AddContentHeaderParameters(input);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            //content.Headers.Add("ContentType", "application/json");
            Console.WriteLine($"[四川]竣工验收信息[{project.PrjName}]信息数据发送");
            return content;
        }

        /// <summary>
        ///4.2.1	参建单位信息（表名：ProjectCorpInfo）Content
        /// </summary>
        /// <returns></returns>
        public static HttpContent GetProjectCorpInfoContent()
        {
            #region 数据结构
            var project = new
            {
                Code = _keySecret.ProjectKeyId,//项目编码 必填 关联项目基本信息
                CorpName = ProjectName,//企业名称	NVARCHAR(200)	M	
                AreaCode = "510000",//企业注册地区   NVARCHAR(6) M   详见数据字典表行政区划
                CorpCode = UnitCode,//统一社会信用代码    VARCHAR(18) M
                RegisterDate = $"{DateTime.Now:yyyy-MM-dd}",//注册日期 DATE    M 
                //1   专业分包
                //2   设备分包
                //3   材料分包
                //4   后勤服务
                //5   特殊设备
                //6   劳务分包
                //7   监理单位
                //8   建设单位
                //9   总承包单位
                //10  勘察单位
                //11  设计单位
                //12  其它
                CorpType = 1//参建类型    INT M   详见数据字典表参建类型
            };
            #endregion

            var data = new List<object> { project };
            var input = new
            {
                type = "参建单位信息",
                data
            };
            var content = AddContentHeaderParameters(input);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            //content.Headers.Add("ContentType", "application/json");
            Console.WriteLine($"[四川]参建单位信息[{project.CorpName}]信息数据发送");
            return content;
        }

        /// <summary>
        ///4.2.2	参建单位进出场信息（表名：ProjectCorpInoutInfo） Content
        /// </summary>
        /// <returns></returns>
        public static HttpContent GetProjectCorpInoutInfoContent()
        {
            #region 数据结构
            var project = new
            {
                Code = _keySecret.ProjectKeyId,//项目编码 必填 关联项目基本信息
                CorpName = ProjectName,//企业名称	NVARCHAR(200)	M	
                CorpCode = UnitCode,//统一社会信用代码    VARCHAR(18) M
                InOut = 1,//进出场类型  Int	M	1=进 2 = 出
                OccurTime = $"{DateTime.Now:yyyy-MM-dd}",//注册日期 DATE    M 
            };
            #endregion

            var data = new List<object> { project };
            var input = new
            {
                type = "参建单位进出场信息",
                data
            };
            var content = AddContentHeaderParameters(input);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            //content.Headers.Add("ContentType", "application/json");
            Console.WriteLine($"[四川]参建单位进出场信息[{project.CorpName}]信息数据发送");
            return content;
        }


        /// <summary>
        /// 4.2.3	管理人员信息（表名：ProjectPMInfo）Content
        /// </summary>
        /// <returns></returns>
        public static HttpContent GetProjectPMRegisterContent()
        {
            var encodePhoto = Utilities.Base64Phto.RemoveBase64ImagePrefix().UrlEncode();

            #region 数据结构
            var person = new
            {
                Code = _keySecret.ProjectKeyId,//项目编码 必填 关联项目基本信息
                CorpCode = UnitCode,//企业统一社会信用代码 必填
                CorpName = ProjectName, //企业名称 必填
                //1 专业分包
                //2 设备分包
                //3 材料分包
                //4 后勤服务
                //5 特殊设备
                //6 劳务分包
                //7 监理单位
                //8 建设单位
                //9 总承包单位
                //10 勘察单位
                //11 设计单位
                //12 其它
                CorpType = 1, //参建类型 必填
                //1 项目经理
                //2 技术负责人
                //3 总监理工程师
                //4 专业监理工程师
                //5 监理员
                //6 施工员
                //7 质量员
                //8 安全员
                //9 标准员
                //10 材料员
                //11 机械员
                //12 劳务员
                //13 其他
                PType = 1, //人员类型 必填
                PMName = "班组名称",//班组名称 必填
                PMIDCardType = 1,//证件类型 必填 1居民身份证 2护照
                PMIDCardNumber = AESEncrypt.Encrypt("500108199003079558", Password, AppSecret?.Substring(0, 16)),//证件号码 必填   aes
                PMPhone = "18975213456",//手机号码 必填
            };
            #endregion

            var data = new List<object> { person };
            var input = new
            {
                type = "管理人员信息",
                data
            };
            var content = AddContentHeaderParameters(input);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            //content.Headers.Add("ContentType", "application/json");
            Console.WriteLine($"[四川]管理人员[{person.PMName}]注册信息数据发送");
            return content;
        }

        /// <summary>
        /// 4.3.1	项目班组信息（表名：TeamMasterInfo）Content
        /// </summary>
        /// <returns></returns>
        public static HttpContent GetTeamMasterInfoContent()
        {
            var encodePhoto = Utilities.Base64Phto.RemoveBase64ImagePrefix().UrlEncode();

            #region 数据结构
            var person = new
            {
                Code = _keySecret.ProjectKeyId,//项目编码 必填 关联项目基本信息
                TeamSysNo = "班组编号",//班组编号      INT M   本项目唯一编号
                CorpCode = UnitCode,//班组所在企业统一社会信用代码      VARCHAR(18) M
                CorpName = "班组编号",  //班组所在企业名称      NVARCHAR(200)   M
                TeamName = "班组名称", //班组名称       NVARCHAR(100)   M   同一个项目上的班组名称不能重复
                TeamLeaderName = "班组长姓名", //班组长姓名    NVARCHAR(50)    M
                TeamLeaderPhone = "13212036528",//班组长联系电话  NVARCHAR(50)    M
                Remark = "备注",//备注   NVARCHAR(200)   O
            };
            #endregion

            var data = new List<object> { person };
            var input = new
            {
                type = "项目班组信息",
                data
            };
            var content = AddContentHeaderParameters(input);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            //content.Headers.Add("ContentType", "application/json");
            Console.WriteLine($"[四川]项目班组信息[{person.TeamSysNo}]数据发送");
            return content;
        }

        /// <summary>
        /// 4.3.2	班组进出场信息（表名：TeamInoutInfo）Content
        /// </summary>
        /// <returns></returns>
        public static HttpContent GetTeamInoutInfoContent()
        {
            #region 数据结构
            var person = new
            {
                Code = _keySecret.ProjectKeyId,//项目编码 必填 关联项目基本信息
                TeamSysNo = "班组编号",//班组编号      INT M   本项目唯一编号
                InOut = 1,//进出场类型   Int M   1 = 进 2 = 出
                OccurTime = $"{DateTime.Now:yyyy-MM-dd}",   //发生时间   DATETIME    M
                AttInfo = "12345", //附件NVARCHAR(50)    O   关联附件数据表
            };
            #endregion

            var data = new List<object> { person };
            var input = new
            {
                type = "班组进出场信息",
                data
            };
            var content = AddContentHeaderParameters(input);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            //content.Headers.Add("ContentType", "application/json");
            Console.WriteLine($"[四川]班组进出场信息[{person.TeamSysNo}]信息数据发送");
            return content;
        }


        /// <summary>
        /// 4.3.3	建筑工人信息（表名：ProjectWorkerInfo）Content
        /// </summary>
        /// <returns></returns>
        public static HttpContent GetProjectWorkerRegisterContent()
        {
            //ProjectWorkerInfo
            var encodePhoto = Utilities.Base64Phto.RemoveBase64ImagePrefix().UrlEncode();

            #region 数据结构
            var person = new
            {
                Code = _keySecret.ProjectKeyId,//项目编码 必填 关联项目基本信息
                CorpCode = UnitCode,//企业统一社会信用代码 必填
                CorpName = ProjectName, //企业名称 必填
                TeamSysNo = 10000,//班组编号 必填 关联项目班组信息表
                TeamName = "班组名称",//班组名称 必填
                WorkerName = "姓名",//姓名 必填
                IsTeamLeader = false,//是否班组长 必填
                IDCardType = 1,//证件类型 必填 1居民身份证 2护照
                IDCardNumber = "500108199003079558",//证件号码 必填
                Age = 22,//年龄 必填
                Gender = "男",//性别 必填  男/女
                Nation = "汉",//民族 必填
                Address = "住址",//住址 必填
                headImage = encodePhoto,//"头像base64/url", //头像 必填 关联附件数据表
                //1   中共党员 2   中共预备党员 3   共青团员 4   民革党员 5   民盟盟员 6   民建会员
                //7   民进会员 8   农工党党员 9   致公党党员 10  九三学社社员 11  台盟盟员 12  无党派人士 13  群众
                PoliticsType = "13", //政治面貌 必填
                CultureLevelType = "3",//文化程度 必填 1	小学 2   初中 3   高中 4   中专 5   大专 6   本科 7   硕士 8   博士 99  其他
                GrantOrg = "发证机关",//发证机关 必填
                //砌筑工（建筑瓦工、窑炉修筑工、瓦工）	010
                //钢筋工 020
                //架子工（普通架子工、附着升降脚手架安装拆卸工、附着升降脚手架安装拆卸工、高处作业吊篮操作工）	030
                //混凝土工（混凝土搅拌工、混凝土浇筑工、混凝土模具工）	040
                //模板工（混凝土模板工）	050
                //机械设备安装工 060
                //通风工 070
                //起重工（安装起重工）	080
                //安装钳工    090
                //电气设备安装工（电气安装调试工）	100
                //管工（管道工）	110
                //变电安装工   120
                //电工（弱电工）	130
                //司泵工 140
                //挖掘、铲运和桩工机械司机（推土、铲运机司机、挖掘机司机、打桩工）	150
                //桩机操作工   160
                //起重信号工（起重信号司索工）	170
                //建筑起重机械安装拆卸工 180
                //装饰装修工（抹灰工、油漆工、镶贴工、涂裱工、装饰装修木工）	190
                //室内成套设施安装工   200
                //建筑门窗幕墙安装工（幕墙安装工、建筑门窗安装工）	210
                //幕墙制作工   220
                //防水工 230
                //木工（手工木工、精细木工、木雕工）	240
                //石工（石作业工、石雕工）	250
                //泥塑工 260
                //焊工（电焊工）	270
                //爆破工 280
                //除尘工 290
                //测量工（测量放线工）	300
                //线路架设工   310
                //砧细工 320
                //砧刻工 330
                //彩绘工 340
                //匾额工 350
                //推光漆工    360
                //砌花街工    370
                //金属工 380
                WorkType = "010",//工种名称 必填
                NativePlace = "中国 四川 成都",//籍贯 必填
                Mobile = "18975213456",//手机号码 必填
                IssueCardPicUrl = "https://",//照片  非必填 关联附件数据表
                HasContract = "是",//是否有劳动合同 必填
                //HasBuyInsurance = "是",//是否购买工伤或意外伤害保险 非必填
                //Remark = "500108199003079558"//备注 非必填
            };
            #endregion

            var data = new List<object> { person };
            var input = new
            {
                type = "建筑工人信息",
                data
            };
            var content = AddContentHeaderParameters(input);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            //content.Headers.Add("ContentType", "application/json");
            Console.WriteLine($"[四川]建筑工人[{person.WorkerName}]注册信息数据发送");
            return content;
        }

        /// <summary>
        /// 4.3.4	建筑工人进出场信息（表名：ProjectWorkerInoutInfo）
        /// </summary>
        /// <returns></returns>
        public static HttpContent GetProjectWorkerInoutContent()
        {
            //ProjectWorkerInoutInfo
            var item = new
            {
                Code = _keySecret.ProjectKeyId,//项目编码 必填 关联项目基本信息
                IDCardNumber = AESEncrypt.Encrypt("500108199003079558", Password, AppSecret?.Substring(0, 16)),//证件号码 必填 关联建筑工人信息，AES
                InOut = 1,//进出场类型 必填1=进 2 = 出
                OccurTime = DateTime.Now//发生时间 必填
            };
            var data = new List<object> { item };
            var input = new
            {
                type = "建筑工人进出场信息",
                data
            };
            var content = AddContentHeaderParameters(input);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            //content.Headers.Add("ContentType", "application/json");
            Console.WriteLine($"[四川]项目：{item.Code}人员[{item.IDCardNumber}]进出场信息数据发送");
            return content;
        }

        /// <summary>
        /// 4.3.5	劳动合同信息（表名：WorkerContractInfo）
        /// </summary>
        /// <returns></returns>
        public static HttpContent GetWorkerContractInfoContent()
        {
            var item = new
            {
                Code = _keySecret.ProjectKeyId,//项目编码 必填 关联项目基本信息
                CorpCode = UnitCode,//企业统一社会信用代码	NVARCHAR(18)	M	
                CorpName = ProjectName,//企业名称	NVARCHAR(200)	M	
                WorkerName = "姓名",//姓名 VARCHAR(50)    M
                //1   居民身份证
                //2   护照
                IDCardType = 1,//证件类型 INT M
                IDCardNumber = AESEncrypt.Encrypt("500108199003079558", Password, AppSecret?.Substring(0, 16)),//证件号码 VARCHAR(18) M   AES
                ContractCode = "合同编号",//合同编号     NVARCHAR(50)    M
                //1   固定期限合同
                //2   以完成一定工作为期限的合同
                ContractPeriodType = 1,//合同期限类型    INT M   详见数据字典表合同期限类型
                SignDate = $"{DateTime.Now:yyyy-MM-dd}",//签订日期     DATE    M
                StartDate = $"{DateTime.Now:yyyy-MM-dd}",//开始日期           DATE    M
                EndDate = $"{DateTime.Now:yyyy-MM-dd}",//结束时期   DATE    M
                Unit = "Kg",//计量单位  INT O   详见数据字典表计量单位
                UnitPrice = 12000,//计量单价   DECIMAL(18, 2)   O
                ContractAtt = "",//合同附件     NVARCHAR(50)    O   关联附件数据表
            };
            var data = new List<object> { item };
            var input = new
            {
                type = "劳动合同信息",
                data
            };
            var content = AddContentHeaderParameters(input);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            //content.Headers.Add("ContentType", "application/json");
            Console.WriteLine($"[四川]项目：{item.Code}劳动合同信息数据发送");
            return content;
        }

        /// <summary>
        /// 4.3.6	考勤信息（表名：WorkerAttendanceInfo）
        /// </summary>
        /// <returns></returns>
        public static HttpContent GetWorkerAttendanceContent()
        {
            //WorkerAttendanceInfo

            var attendance = new
            {
                Code = _keySecret.ProjectKeyId,//项目编码 必填 关联项目基本信息
                TeamSysNo = 10000,//int 班组编号 必填 关联项目班组信息表  管理人员传递： -100
                TeamName = "班组名称",//班组名称 必填  管理人员  填写 "管理班组"
                WorkerName = "姓名",//姓名 必填
                IDCardType = 1,//证件类型 必填 1居民身份证 2护照
                IDCardNumber = AESEncrypt.Encrypt("500108199003079558", Password, AppSecret?.Substring(0, 16)),//证件号码 必填 关联建筑工人信息，AES
                Direction = 1,//进出场类型 必填1=进 2 = 出
                Date = $"{DateTime.Now:yyyyMMddHHmmss}",//考勤时间 必填 yyyyMMddHHmmss
                ImageURL = "https://",//考勤近照  非必填 关联附件数据表
                AttendType = 1,//通行方式  非必填 1   刷身份证 2   人脸识别 3   指纹 4   虹膜 5   移动设备 99  其他
                //Lng = 1m,//WGS84经度  非必填 关联附件数据表
                //Lat = 1m,//WGS84纬度  非必填 关联附件数据表
            };
            var data = new List<object>() { attendance };
            var input = new { type = "考勤信息", data };
            var content = AddContentHeaderParameters(input);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            //content.Headers.Add("ContentType", "application/json");
            Console.WriteLine($"[四川]项目：{attendance.Code}建筑工人[{attendance.IDCardNumber}]考勤信息数据发送");
            return content;
        }

        /// <summary>
        /// 扬尘设备信息表(表名：DustDeviceInfo)
        /// </summary>
        /// <returns></returns>
        public static HttpContent GetDustDeviceContent()
        {
            //DustDeviceInfo
            var input = new
            {
                Code = "项目编码",//项目编码      必填 关联项目基本信息
                Name = ProjectName,//项目名称     必填
                DeviceID = Guid.NewGuid().ToString(),//设备ID            必填  唯一编号，guid字符串
                DeviceModel = "设备型号",//设备型号       必填
                DeviceName = "设备名称",//设备名称        必填
                Manufacturer = "生产厂商", //生产厂商     必填
                Batch = "批次",//批次                 必填
                SN = "SN",//SN                      非必填
                SIM = "SIM",//SIM                    非必填
                DeviceState = 0,//设备状态       非必填 1   正常 2   已拆机3   护暂停
                Unit = "对接单位",//对接单位              必填
                Person = "联系人",//联系人              非必填
                Phone = "18975213456",//联系电话             非必填
                Text = "备注", //备注                 非必填
                Longitude = 0m,//经度             非必填
                Latitude = 0m,//纬度              非必填
                Url = "https://",//视频地址               非必填
            };
            var content = AddContentHeaderParameters(input);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            //content.Headers.Add("ContentType", "application/json");
            Console.WriteLine($"[四川]项目：{input.Code}扬尘设备[{input.DeviceID}]信息数据发送");
            return content;
        }

        /// <summary>
        /// 扬尘监测数据表(表名：DustMonitorInfo)
        /// </summary>
        /// <returns></returns>
        public static HttpContent GetDustMonitorContent()
        {
            //DustMonitorInfo
            var input = new
            {
                MonitoringTime = DateTime.Now,// 监测时间         必填
                Code = "项目编码",//项目编码      必填 关联项目基本信息
                Name = ProjectName,//项目名称     必填
                DeviceID = Guid.NewGuid().ToString(),//设备ID            必填  唯一编号，guid字符串
                SaveTime = DateTime.Now,// 存储时间         必填
                PM10 = 0m,//PM10Value             必填
                PM25Value = 0m,//PM2.5            必填
                Voice = 0m,//噪声                 必填
                Temperature = 0m,//温度           非必填
                Humidity = 0m,//湿度              非必填
                WindSpeed = 0m,//风速             非必填
                WindDirection = 0m,//风向         非必填
                Atmospheric = 0m,//大气压         非必填
                Type = 0,//数据类型              必填   0   实时值 1   分钟平均值 2   小时平均值 3   昼平均值
                Pm25Monitor = 0m, //Pm2.5国控值   非必填
                Pm10Monitor = 0m,//Pm10国控值     非必填
                SprayStatus = 0 //喷淋状态      必填  0 = 关闭；1 = 开启
            };
            var content = AddContentHeaderParameters(input);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            //content.Headers.Add("ContentType", "application/json");
            Console.WriteLine($"[四川]项目：{input.Code}扬尘设备[{input.DeviceID}]监测数据发送");
            return content;
        }

        /// <summary>
        /// 扬尘超标数据表(表名：DustExcessiveInfo)
        /// </summary>
        /// <returns></returns>
        public static HttpContent GetDustExcessiveContent()
        {
            //DustExcessiveInfo
            var input = new
            {
                MonitoringTime = DateTime.Now,// 监测时间         必填
                Code = "项目编码",//项目编码      必填 关联项目基本信息
                Name = ProjectName,//项目名称     必填
                DeviceID = Guid.NewGuid().ToString(),//设备ID            必填  唯一编号，guid字符串
                PM10 = 0m,//PM10Value             必填
                PM25Value = 0m,//PM2.5            必填
                Voice = 0m,//噪声                 必填
                ECode = 0m//超标数据编号         必填
            };
            var content = AddContentHeaderParameters(input);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            //content.Headers.Add("ContentType", "application/json");
            Console.WriteLine($"[四川]项目：{input.Code}扬尘设备[{input.DeviceID}]超标数据发送");
            return content;
        }

        /// <summary>
        /// 扬尘超标视频叠加参数报警图片(表名：DustExcessiveImg)
        /// </summary>
        /// <returns></returns>
        public static HttpContent GetDustExcessiveImgContent()
        {
            //DustExcessiveInfo
            var input = new
            {
                ImageText = 0m,//图片                 必填
                ECode = 0m//超标数据编号          必填 关联扬尘超标数据表
            };
            var content = AddContentHeaderParameters(input);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            //content.Headers.Add("ContentType", "application/json");
            Console.WriteLine($"[四川]扬尘设备超标数据编号{input.ECode}视频叠加参数报警图片数据发送");
            return content;
        }

        private static StringContent AddContentHeaderParameters(object input)
        {
            //参数名 必选  类型范围    说明
            //access_token    是   String  访问令牌。成功调用login接口后返回
            //sign    否   String  请求参数签名值（预留）
            //signCert    否   String  签名证书序列号
            //sign_method 否   String  签名方法（预留）
            //timestamp   否   Date    应用程序发出请求的客户端时间。平台在接收到请求后，会与当前服务端时间比较，如果误差范围大于10分钟，将请求视为无效
            //request_id  否   String  应用程序发出请求的唯一标识号。平台如果在误差时间内接收到多个request_id相同的请求，除最早收到的一个请求外，其它请求将视为重放攻击而被忽略
            Console.WriteLine($"Json序列化input:");
            Console.WriteLine($"input：{JsonConvert.DeserializeObject<object>(TextJsonConvert.SerializeObject(input))}");

            var content = new StringContent(TextJsonConvert.SerializeObject(input), Encoding.UTF8);
            return content;
        }

    }

}
