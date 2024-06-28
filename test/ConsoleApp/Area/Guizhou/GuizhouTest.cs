using ConsoleApp.Dtos;
using System;
using Serilog;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp.HttpClients;
using Newtonsoft.Json;
using Common.Dtos;
using Common.Helpers;
using Common.Extensions;

namespace ConsoleApp.Area.Guizhou
{
    /// <summary>
    /// 贵阳测试
    /// </summary>
    public static class GuizhouTest
    {
        /// <summary>
        /// 加密信息
        /// </summary>
        private static readonly KeySecret _keySecret = new KeySecret
        {
            SupplierKeyId = "SupplierKeyId",
            SupplierKeySecret = "SupplierKeySecret",
            ProjectKeyId = "1270995441467150337",
            ProjectKeySecret = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDPSBIzE4YExjUIuNl0Wn7sWIFTAv9/NEIC8OfB4AO2rFZnR+i+2g6LN8NLzamNd4KUCLvr/4kSpDHSPfCNTBia5kMNLqjd51SYuZH4m6pzfbnFDZvrcpGhuoUIYm6YXC5Fxh4zb074Ko32YjSk/eexbA03FFk4rv9qxR8kvG/bHQIDAQAB"
        };

        #region 获取token

        /// <summary>
        /// 软件服务商统一社会信用代码
        /// </summary>
        public static string UnitCode = "915001060891260711";
        /// <summary>
        /// 重庆科技(重庆)科技有限公司
        /// </summary>
        public static string ProjectName = "重庆科技(重庆)科技有限公司";

        /// <summary>
        /// Host
        /// </summary>
        public static string Host = "http://221.13.13.133";//"http://222.85.156.48"

        /// <summary>
        /// Port
        /// </summary>
        public static string Port = "21974";//17000


        #endregion

        #region 分包单位企业信息接口

        /// <summary>
        /// 获取分包单位企业信息接口Content
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetSubEnterpriseContent()
        {
            var input = new
            {
                unitCode = UnitCode,//"915001060891260711"
                roleType = "LABOR_SUB",//DEVICE_SUB MATERIAL_SUB LABOR_SUB PROFESSIONAL_SUB
                subHeadIdcard = "500108199003079558",
                subHeadTel = "18923654258",
                subHeadName = "测试",
                projectName = "测试项目",
                subsituation = "01",//01 02
                enterpriseName = ProjectName,
                inTimeStr = DateTime.Now.ToString("yyyy-MM-dd"),
                //outTimeStr = DateTime.Now.ToString("yyyy-MM-dd")  //不必填
            };
            var content = await GetContent(input);
            Console.WriteLine($"获取分包单位企业信息[{input.unitCode}]信息数据发送");
            return content;
        }



        /// <summary>
        /// 获取项目用工接口Content
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetProjectWorkerContent()
        {
            var input = new
            {
                UnifiedProjectCode = UnitCode,
                Page = 0
            };
            //var content = await GetContent(input);
            Console.WriteLine($"params:");
            Console.WriteLine($"{TextJsonConvert.SerializeObject(input)}");
            var list = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("params",TextJsonConvert.SerializeObject(input))
            };
            var content = new FormUrlEncodedContent(list);
            //content.Headers.Add("Authorization", $"Bearer {(await GetAccessToken())?.Custom.Access_token}");
            content.Headers.Add("ContentType", "application/x-www-form-urlencoded");
            //content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            Console.WriteLine($"获取项目[{input.UnifiedProjectCode}]用工信息数据发送");
            return content;
        }


        #endregion

        #region 设备心跳

        /// <summary>
        /// 获取环境数据Content
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetHeartbeatContent(string serialNo)
        {
            var input = new
            {
                deviceType = "1",//1扬尘 2塔吊 3施工升降机
                deviceNum = serialNo,//"1234569999",
                heartbeatTimeStr = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
            var content = await GetContent(input);
            Log.Logger.Debug($"设备[{input.deviceNum}]心跳数据发送");
            return content;
        }
        #endregion

        #region 环境

        /// <summary>
        /// 获取扬尘Content
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetEnvironmentContent()
        {
            var input = new
            {
                //timeStamp = DateTime.Now.ToUnixTimeTicks(13),
                deviceNum = "TSVS837819828",
                netState = "1",//网络状态
                noise = 63.5,
                pm25 = 35.2,
                pm10 = 27.9,
                dataTransactionTimeStr = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                windSpeed = 23.0,
                windDirection = "西南",
                temperature = 35.0,
                pressure = 23.0,
                humidity = 35.2,
                longitude = 1,
                latitude = 1
            };
            var content = await GetContent(input);
            Console.WriteLine($"[贵阳扬尘]设备[{input.deviceNum}]数据发送");
            return content;
        }

        #endregion

        #region 塔吊

        /// <summary>
        /// 获取塔吊在线监测Content
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetCraneRealTimeContent()
        {
            var input = new
            {
                deviceNum = "TSVS837819828",
                netState = "1",
                amplitude = 12.1,
                height = 0,
                hangingWeight = 0,
                operatorNum = "500108199003079558",
                attendanceNum = "操作人员考勤编号",
                dataTransactionTimeStr = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                longitude = 0,
                latitude = 0,
                rotationAngle = 0,
                momentPercentage = 0,
                brachium = 0,
                windSpeedPercentage = 0,
                windSpeed = 0,
                incidence = 0,
                incidencePercentage = 0,
                windScale = "5",
                brakingState = "111111"
            };
            var content = await GetContent(input);
            Console.WriteLine($"[贵阳塔吊]设备[{input.deviceNum}]数据发送");
            return content;
        }

        #endregion

        #region 施工升降机智能监测

        /// <summary>
        /// 获取施工升降机智能监测接口Content
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetLifterRealTimeContent()
        {
            var input = new
            {
                deviceNum = "TSVS837819828",
                netState = "1",
                weight = 12.1,
                weightLimit = 100,
                height = 0,
                doorLock = "110",
                operatorNum = "500108199003079558",
                attendanceNum = "操作人员考勤编号",
                dataTransactionTimeStr = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                longitude = 0,
                latitude = 0,
                heightPercentage = 0,
                inclinationPercentage = 0,
                peopleNum = 100,
                speed = 100,
                speedDirection = "1"//0停止 1上 2下
            };
            var content = await GetContent(input);
            Console.WriteLine($"[贵阳升降机]设备[{input.deviceNum}]智能监测数据发送");
            return content;
        }

        #endregion

        #region 批量上传考勤

        /// <summary>
        /// 获取智慧工地实名制接口Content
        /// 数据说明 json数组 且一次最多上传200条数据
        /// 上传考勤数据时，请按照“考勤打卡时间”排序上传（例：先上传2020-08-01 08:00:00的考勤，再上传2020-08-01 09:00:00），否则会导致上传失败
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetGateContent()
        {
            var input = new List<object>
            {
                new
                {
                    personIdCard = "500108199003079558",
                    cardNo = "23432423424",
                    trueName = "trueName",
                    projectName = "projectName",
                    workerType = "03",//03劳务人员 01管理人员
                    punchTimeStr = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:ss:mm"),
                    punchType = "1",//1	进场 2出场 3未识别
                    matchResult = "1",//比对结果1匹配0未匹配
                    similarityDegree = "0.7",//相似度,值范围0~1之间
                    photoUrl = "https://test",//"考勤现场抓拍的从业人员照片",
                    cropUrl = "https://test",//"考勤现场抓拍人脸比对结果的照片",
                    //001   人脸识别 002 虹膜识别 003 指纹识别 004 掌纹识别 005 身份证识别 
                    //006 实名卡 007 异常清退（适用于人员没有通过闸机系统出工地而导致人员状态不一致的情况） 
                    //008 一键开闸(需要与闸机交互) 009 应急通道（不需要与闸机交互） 010 二维码识别 011 其他方式
                    punchCardMode = "001",
                    channel = "52",
                    latitude = "12.2",
                    longitude = "71.2",
                },
                new
                {
                    personIdCard = "500108199003079558",
                    cardNo = "23432423425",
                    trueName = "trueName2",
                    projectName = "projectName",
                    workerType = "03",//03劳务人员 01管理人员
                    punchTimeStr = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:ss:mm"),
                    punchType = "1",//1	进场 2出场 3未识别
                    matchResult = "1",//比对结果1匹配0未匹配
                    similarityDegree = "0.7",//相似度,值范围0~1之间
                    photoUrl = "https://test",//"考勤现场抓拍的从业人员照片",
                    cropUrl = "https://test",//"考勤现场抓拍人脸比对结果的照片",
                    //001   人脸识别 002 虹膜识别 003 指纹识别 004 掌纹识别 005 身份证识别 
                    //006 实名卡 007 异常清退（适用于人员没有通过闸机系统出工地而导致人员状态不一致的情况） 
                    //008 一键开闸(需要与闸机交互) 009 应急通道（不需要与闸机交互） 010 二维码识别 011 其他方式
                    punchCardMode = "001",
                    channel = "52",
                    latitude = "12.2",
                    longitude = "71.2",
                }
            };
            var content = await GetContent(input);
            Console.WriteLine($"[贵阳考勤]设备考勤数据发送");
            return content;
        }

        #endregion

        #region 人员基本信息接口

        /// <summary>
        /// 获取人员基本信息接口Content
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetGateRegisterContent()
        {
            var input = new
            {
                trueName = "姓名",//      VARCHAR 是
                idCard = "500108199003079558",// 身份证号码   VARCHAR 是
                headImageUrl = "https://test",// 身份证头像url    VARCHAR 是
                address = "重庆沙坪坝",// 住址  VARCHAR 是
                nation = "汉",// 民族  VARCHAR 是
                phone = "18923654258",// 手机号 VARCHAR 是
                //1   小学以下 2   小学 3   初中 4   中专 5   高中
                //6   大学专科 7   大学本科 8   硕士研究生 9   博士研究生
                educationCode = "7",// 文化程度    VARCHAR 是   文化程度数据字典
                educationCodeDesc = "7",// 文化程度字典值 VARCHAR 否
                //01  中共党员 02  中共预备党员 03  共青团员 04  民革党员 05  民盟盟员
                //06  民建会员 07  民进会员 08  农工党党员 09  致公党党员 10  九三学社社员
                //11  台盟盟员 12  无党派人士 13  群众
                politicalType = "01",// 政治面貌    VARCHAR 否   政治面貌数据字典
                politicalTypeDesc = "1",// 文化程度字典值 VARCHAR 否
                frontPhoto = "https://test",// 身份证正面URL    VARCHAR 是
                backPhoto = "https://test",// 身份证反面URL    VARCHAR 是
                majorHistoryB = false,//  是否重大病史  BOOLEAN 是   是否数据字典
                majorHistoryDesc = "否",// 重大病史字典值 VARCHAR 否
                degreeCode = "1",// 1   博士后学位 2   博士学位 3   硕士学位 4   学士学位 5   无学位 99  其它
                degreeCodeDesc = "1",// 学位字典值   VARCHAR 否
                hasJoinedB = true,// 是否加入工会  BOOLEAN 否   是否数据字典
                hasJoinedDesc = "是",// 加入工会字典值 VARCHAR 否
                joinedTimeStr = $"{DateTime.Now:yyyy-MM-dd}",// 加入工会时间（格式为：yyyy - MM - dd）	VARCHAR 否
                urgentLinkMan = "紧急联系人姓名",// 紧急联系人姓名 VARCHAR 否
                urgentLinkManPhone = "18923654258",//   紧急联系人电话 VARCHAR 否
                maritalStatus = "1",// 婚姻状况 1   未婚 2   已婚 3   离异
                maritalStatusDesc = "未婚",// 婚姻状况字典值 VARCHAR 否
                personType = "03",// 人员类别    03	劳务人员 01  管理人员
                personTypeDesc = "管理人员",// 人员类别字典值 VARCHAR 否
                icCardNo = "从业人员IC卡号",// 从业人员IC卡号    VARCHAR 否
                authType = "1",// 实名类型   1   线上实名 2   现场实名
                signOrg = "发证机关",// 发证机关    VARCHAR 是
                validityPeriod = "2012010120300101",//   证件起止时间  VARCHAR 是   例如:2012010120200101身份证上读来的有效期的字符串且16位的字符串数字验证
            };
            var content = await GetContent(input);
            Console.WriteLine($"[贵阳考勤]人员基本信息发送");
            return content;
        }

        #endregion


        #region 班组信息接口

        /// <summary>
        /// 获取班组信息接口Content
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetTeamContent()
        {
            var input = new
            {
                teamName    = "班组名称",//    VARCHAR 是
                managerIdCard = "500108199003079558",// 班组负责人身份证号   VARCHAR 是
                //01  砌筑工02  抹灰工03  混凝土工04  模板工05  架子工06  油漆工07  电工08  木工09  防水工10  试验工13  保洁工14  钢筋工15  涂料工16  电焊工17  装饰装修工20  幕墙工27  支护工28  凿岩工29  爆破工30  打桩工43  管工45  电梯安装维修工49  塔式起重机驾驶员50  推土机驾驶员51  铲运机驾驶员52  挖掘机驾驶员53  塔式起重机指挥员54  水电安装工55  杂工56  消防工57  测量工58  矿长59  总工程师60  技术员61  瓦检员62  安全员63  驻矿员64  辅助工65  修工66  采煤工67  掘进工68  溜煤工69  抽采工70  赶车工71  爆破员72  铲煤工73  防突工74  机电工77  会计78  库管员79  井下运输80  充电工81  配电工82  水处理工83  环境卫生84  警卫85  保卫86  风枪工87  小车司机88  大车司机89  营销90  后勤91  信号工92  材料员93  出纳采购94  厨师95  炊事员96  搅拌机操作员97  罐车驾驶员98  装载机驾驶员99  二衬工100 喷浆工101 开挖工102 值班员103 炮工立架104 上料工105 衬砌工106 泵车司机107 叉车司机108 抽水工109 钢支撑工110 普工111 技工112 电梯工113 泥水工114 堵漏115 防护116 其它 117 升降机驾驶员
                teamType = "01",//   班组类型（与工种类型相同）	VARCHAR 是   工种字典
                unitCode = UnitCode,//    所属企业单位统一社会信用代码  VARCHAR 是
                entryTimeStr = $"{DateTime.Now.AddDays(-2):yyyy-MM-dd}",//    进场时间(格式为: yyyy - MM - dd)    VARCHAR 是
                quitTimeStr = $"{DateTime.Now.AddDays(-1):yyyy-MM-dd}",//离场时间(格式为: yyyy - MM - dd)    VARCHAR 否
                tel = "18923654258",// 劳务企业手机电话    VARCHAR 是
                remark = "备注"//  备注  VARCHAR 否
            };
            var content = await GetContent(input);
            Console.WriteLine($"[贵阳考勤]人员班组信息发送");
            return content;
        }

        #endregion



        #region 从业人员入职信息接口

        /// <summary>
        /// 获取从业人员入职信息接口Content
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetPersonPostContent()
        {
            var input = new
            {
                personIdCard ="500108199003079558",//    从业人员身份证号码   VARCHAR 是       从业人员
                cardNo = "考勤卡号",//  绑定卡号（考勤卡号）	VARCHAR 否       从业人员
                unitCode = UnitCode,//     所属企业统一社会信用代码(从业人员所属企业)  VARCHAR 是       从业人员
                workerType = "03",//   人员类别    03	劳务人员 01  管理人员
                positionTypeStr = "2",//  1	管理员2	负责人3	监督员4	专管员5	人事专员6	劳务员7	施工员8	质量员9	安全员10	标准员11	材料员12	机械员13	资料员14	设计员15	技术员16	监理总监17	技术总工18	项目经理19	项目总监20	其它
                teamId = "班组真实ID",//   班组ID    VARCHAR 是       劳务人员
                certifiedAppointmentB = true,//    是否持证上岗  BOOLEAN 是   是否数据字典  从业人员
                isTrainedB = true,//  是否培训    BOOLEAN 是   是否数据字典  从业人员
                entryTimeStr = $"{DateTime.Now.AddDays(-2):yyyy-MM-dd HH:mm:ss}",//    入职时间(时间格式: yyyy - MM - dd HH: mm:ss)	VARCHAR 是       从业人员
                workType = "01",// 01	砌筑工02	抹灰工03	混凝土工04	模板工05	架子工06	油漆工07	电工08	木工09	防水工10	试验工13	保洁工14	钢筋工15	涂料工16	电焊工17	装饰装修工20	幕墙工27	支护工28	凿岩工29	爆破工30	打桩工43	管工45	电梯安装维修工49	塔式起重机驾驶员50	推土机驾驶员51	铲运机驾驶员52	挖掘机驾驶员53	塔式起重机指挥员54	水电安装工55	杂工56	消防工57	测量工58	矿长59	总工程师60	技术员61	瓦检员62	安全员63	驻矿员64	辅助工65	修工66	采煤工67	掘进工68	溜煤工69	抽采工70	赶车工71	爆破员72	铲煤工73	防突工74	机电工77	会计78	库管员79	井下运输80	充电工81	配电工82	水处理工83	环境卫生84	警卫85	保卫86	风枪工87	小车司机88	大车司机89	营销90	后勤91	信号工92	材料员93	出纳采购94	厨师95	炊事员96	搅拌机操作员97	罐车驾驶员98	装载机驾驶员99	二衬工100	喷浆工101	开挖工102	值班员103	炮工立架104	上料工105	衬砌工106	泵车司机107	叉车司机108	抽水工109	钢支撑工110	普工111	技工112	电梯工113	泥水工114	堵漏115	防护116	其它117	升降机驾驶员
                isTeamB = false,// 是否为班组长 BOOLEAN 是 是否数据字典  劳务人员
                isContractB = false,// 是否签订用工合同 BOOLEAN 是 是否数据字典  劳务人员
                contractCode = "合同编号",//    合同编号 VARCHAR 是 劳务人员
                salaryType = "01",//  薪资计算方式  01月薪 02日薪 03计件
                simpleContractB = true,//  是否签订简易用工合同  BOOLEAN 是   是否数据字典 劳务人员
                bankAccount = "工资卡卡号",//  工资卡卡号   VARCHAR 是       劳务人员
                bankLink = "银行联号",//     银行联号 VARCHAR 是 劳务人员
                bankCode = "102",//  102	中国工商银行103	中国农业银行104	中国银行105	中国建设银行301	交通银行302	中信实业银行303	中国光大银行305	中国民生银行306	广东发展银行307	深圳发展银行308	招商银行309	兴业银行310	上海浦东发展银行401	贵州银行402	农村信用社404	贵阳银行405	贵州农信银行603	中国邮政储蓄银行999	其他银行
                industrialAccidentInsuranceB = true,//  是否购买工伤或意外伤害保险   BOOLEAN 是   是否数据字典 劳务人员
                medicalInsuranceB = true,//  是否参加城乡居民医疗保险    BOOLEAN 是   是否数据字典 劳务人员
                societyInsuranceB = true,//  是否参加城乡居民养老保险    BOOLEAN 是   是否数据字典 劳务人员
                headUrl = "https://test",//  最新人脸照片URL   VARCHAR 是       从业人员
                bankOfDeposit = "工商银行",//   开户行（工资卡银行名称）	VARCHAR 是       劳务人员
                unitPrice = "20000",//    薪资计算单价 VARCHAR 是 劳务人员
                address = "地址",// 现居住地址   VARCHAR 是       从业人员
                bankName = "工资卡账户名称",//     工资卡账户名称 VARCHAR 是 从业人员
                entryAttachment = "https://test",//  进场确认附件  VARCHAR 否       从业人员
            };
            var content = await GetContent(input);
            Console.WriteLine($"[贵阳考勤]从业人员入职信息接口信息发送");
            return content;
        }

        #endregion


        #region 从业人员离职接口

        /// <summary>
        /// 获取从业人员离职接口Content
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetPersonQuitContent()
        {
            var input = new
            {
                personIdCard = "500108199003079558",//   身份证号码   VARCHAR 是
                teamId = "班组真实ID",// 班组ID    VARCHAR 是
                quitTimeStr = $"{DateTime.Now.AddDays(-2):yyyy-MM-dd HH:mm:ss}",// 离职时间(时间格式: yyyy - MM - dd HH: mm:ss)	VARCHAR 是
                unitCode = UnitCode,// 所属企业单位统一社会信用代码  VARCHAR 是
            };
            var content = await GetContent(input);
            Console.WriteLine($"[贵阳考勤]从业人员离职信息发送");
            return content;
        }

        #endregion

        #region 从业人员资质信息接口

        /// <summary>
        /// 获取从业人员资质信息接口Content
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetQualicationContent()
        {
            var input = new
            {
                personIdCard = "500108199003079558",//   身份证号码   VARCHAR 是
                idType = "1",//  人员证书种类  1职业技能证2安装证3岗位证4职称证5职业注册证6其它
                pqName = "证书类型名称",// 证书类型名称  VARCHAR 是
                pqLevel = "1",//证书等级   1初级职称2中级职称3高级职称
                pqCode = "证书编号",//证书编号    VARCHAR 是
                pqEnableDateStr = $"{DateTime.Now.AddDays(-2):yyyy-MM-dd}",// 证书有效期（起)格式 yyyy-MM - dd   VARCHAR 是
                pqDisableDateStr = $"{DateTime.Now.AddDays(100):yyyy-MM-dd}",//证书有效期（止)格式 yyyy-MM - dd   VARCHAR 是
                licence = "发证机关",//发证机关    VARCHAR 是
                pqStatus = "1",//资质证书状态  VARCHAR 是   按照建市[2014]108号全国建筑市场监管与诚信信息系统基础数据库标准（试行）规定的编码及类型
                pqURL = "https://test",//证书附件URL VARCHAR 是
                pqLevelName = "证书等级名称",// 证书等级名称 VARCHAR 是
            };
            var content = await GetContent(input);
            Console.WriteLine($"[贵阳考勤]从业人员资质信息发送");
            return content;
        }

        #endregion

        #region 劳务人员工资接口

        /// <summary>
        /// 获取劳务人员工资接口Content
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetSalaryContent()
        {
            var input = new
            {
                personIdCard = "500108199003079558",//   身份证号码   VARCHAR 是
                payTimeStr = $"{DateTime.Now.AddDays(-2):yyyy-MM}",
                payAbleAmount = "15000",//    BigDecimal  应发工资    是
                actualAmount = "13000",//    BigDecimal  实发工资    是
                payRollCode = "元",//  VARCHAR 工资单编码   否
                unitCode = UnitCode,//    VARCHAR 所属企业统一社会信用代码    是
                teamId = "班组真实ID",//  VARCHAR 班组ID    是
                workerAccountNo = "工人工资卡号",//  VARCHAR 工人工资卡号  是
                workerBankCode = "102",//  102	中国工商银行103	中国农业银行104	中国银行105	中国建设银行301	交通银行302	中信实业银行303	中国光大银行305	中国民生银行306	广东发展银行307	深圳发展银行308	招商银行309	兴业银行310	上海浦东发展银行401	贵州银行402	农村信用社404	贵阳银行405	贵州农信银行603	中国邮政储蓄银行999	其他银行
                payRollBankName = "⼯⼈⼯资卡所属银⾏⽀⾏名称",//  VARCHAR	⼯⼈⼯资卡所属银⾏⽀⾏名称   是
                payRollTopBankName = "⼯⼈⼯资卡所属银⾏总⾏名称",//   VARCHAR	⼯⼈⼯资卡所属银⾏总⾏名称   是
                payrollBank = "⼯⼈⼯资卡所属银⾏",// VARCHAR	⼯⼈⼯资卡所属银⾏	是
                publicAccountNo = "工资代发银行卡号",//  VARCHAR 工资代发银行卡号    是
                publicBankCode = "102",//   VARCHAR 工资代发银行代码    是   银行编码
                publicBankName = "工资代发开户行支行名称",//  VARCHAR 工资代发开户行支行名称 是
                isReissueB = false,//  BOOLEAN 是否是补发   是
                issueTimeStr = $"{DateTime.Now.AddDays(-2):yyyy-MM-dd}",//    VARCHAR 发放日期(格式yyyy - MM - dd)  是
                issueDateStr = $"{DateTime.Now.AddDays(-2):yyyy-MM}",//    VARCHAR 发放年月（发放的具体工资年月 如6月1日发放5月的工资，则此值为2017 - 05）默认15日   是
                isFinancialConfirmationB = true,//     BOOLEAN 财务确认标志  是
                isPersonalCheckB = true,//    BOOLEAN 个人核对标志  是
                chargeLoss = "1000",//  BigDecimal  误工费 否
                alimony = "5000",//  BigDecimal  生活费 否
                settlementAmount = "10000",//    BigDecimal  结算量 否
                thirdSalayNo = "第三方工资单编号",//    VARCHAR 第三方工资单编号    是
                submitBatchNo = "提交批次号",//   VARCHAR 提交批次号   是
                submitApply = "提请平台⽴即代发",// VARCHAR 提请平台⽴即代发    是
                payStatus = "1",//   VARCHAR 发放状态   1	发放成功 2   发放失败 3   等待授权
                payMode = "2",//  VARCHAR 薪资发放方式  1	现金 2   银行代码
            };
            var content = await GetContent(input);
            Console.WriteLine($"[贵阳考勤]劳务人员工资信息发送");
            return content;
        }

        #endregion


        #region 项目工资接口

        /// <summary>
        /// 获取项目工资接口Content
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetProjectSalaryContent()
        {
            var input = new
            {
                salaryAttanchmentUrl = "https://test",
                salaryListUrl = "https://test",
                salaryNoticeAttanchmentUrl = "https://test",
                salaryDayStr = $"{DateTime.Now:yyyy-MM}"
            };
            var content = await GetContent(input);
            Console.WriteLine($"[贵阳考勤]项目工资发送");
            return content;
        }

        #endregion


        #region 劳务合同接口

        /// <summary>
        /// 获取劳务合同接口Content
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetContractContent()
        {
            var input = new
            {
                personIdCard = "500108199003079558",
                contracts = "https://test",
                contractCode = "合同编号",
                contractPeriodType = "1",//合同期限类型 0固定期限合同 1以完成一定工作为期限的合同
                startDateStr = $"{DateTime.Now:yyyy-MM-dd}",
                endDateStr = $"{DateTime.Now:yyyy-MM-dd}",
                unitCode = UnitCode,
                unit = "1",//1米 2平方米 3立方米
                unitPrice = "1000",
                appendixName = "合同附件名称",
                contractType = "1"//签订书面合同类型  1	纸质合同 2电子合同
            };
            var content = await GetContent(input);
            Console.WriteLine($"[贵阳]劳务合同数据资发送");
            return content;
        }

        #endregion

        #region 获取content

        private static async Task<HttpContent> GetContent(object input)
        {
            var json = TextJsonConvert.SerializeObject(input);
            Console.WriteLine($"Json序列化data:{JsonConvert.DeserializeObject<object>(json)}");
            var data = RSAHelper.EncryptByPublicKey(json, _keySecret.ProjectKeySecret);


            Console.WriteLine($"******************打印请求参数******************");
            Console.WriteLine($"unitCode:{UnitCode}");
            Console.WriteLine($"projectId:{_keySecret.ProjectKeyId}");
            Console.WriteLine($"加密后的data(base64):{data}");
            var list = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("unitCode", UnitCode),
                new KeyValuePair<string, string>("projectId", _keySecret.ProjectKeyId),
                new KeyValuePair<string, string>("data",data)
            };
            var content = new FormUrlEncodedContent(list);
            //content.Headers.Add("ContentType", "application/x-www-form-urlencoded");
            return content;
        }

        #endregion


    }

}
