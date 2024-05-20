using Common;
using Common.Cache;
using Common.Dtos;
using Common.Extensions;
using EasyCaching.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Parakeet.Net.Caches;
using Parakeet.Net.Consumer.Chongqing.Dtos;
using Parakeet.Net.Consumer.Chongqing.Interfaces;
using Parakeet.Net.Consumer.Chongqing.Services;
using Parakeet.Net.Consumer.Forwards;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TokenResultDto = Parakeet.Net.Consumer.Chongqing.Dtos.TokenResultDto;

namespace Parakeet.Net.Consumer.Chongqing.RegisterModule.Forwards
{
    /// <summary>
    /// standard人员注册转发器
    /// </summary>
    public class PersonRegisterHttpForward : HttpForward<DeviceWorkerDto>, IPersonRegisterHttpForward
    {
        protected readonly IConfiguration _configuration;
        private readonly TokenConfigDto _tokenConfigDto;
        protected readonly IEasyCachingProvider CachingProvider;
        private readonly ICacheContainer<MultilevelCache<TokenResultDto>, TokenResultDto> _cacheContainer;
        public PersonRegisterHttpForward(
            IHttpClientFactory httpClientFactory,
            ILogger<DeviceWorkerDto> logger, 
            KeySecretPool keySecretPool,
            IOptionsMonitor<TokenConfigDto> tokenOptionsMonitor,
            IConfiguration configuration) : base(httpClientFactory, logger,keySecretPool)
        {
            _configuration = configuration;
            _tokenConfigDto = tokenOptionsMonitor.CurrentValue;
        }

        protected override async Task<HttpContent> GetHttpContentAsync(WrapperData<DeviceWorkerDto> wrapperData)
        {
            //var worker = wrapperData.Data.Worker;
            //var imageHost = _configuration.GetValue<string>("ImageHost");
            var keySecret = KeySecretPool[Magics.STANDARD, wrapperData.Data.DeviceId];
            var projectCode = wrapperData.Data.Device.Project?.Organization?.Code;
            if (string.IsNullOrWhiteSpace(keySecret?.ProjectKeySecret))
            {
                Logger.LogError($"[闸机]设备[{wrapperData.Data.Device.FakeNo}]项目编码[{projectCode}]人员采集未设置加密钥,禁止转发...");
                return null;
            }

            #region person

            var person = new
            {
                Code = projectCode, //项目编码 必填 关联项目基本信息
                CorpCode = wrapperData.Data.CorpCode, //班组所在企业统一社会信用代码 必填
                CorpName = wrapperData.Data.CorpName, //企业名称 必填
                TeamSysNo = wrapperData.Data.WorkerGroupCode, //10000,//班组编号 必填 关联项目班组信息表
                TeamName = wrapperData.Data.WorkerGroupName, //"班组名称",//班组名称 必填
                WorkerName = wrapperData.Data.Worker.Name, //"姓名",//姓名 必填
                IsTeamLeader = wrapperData.Data.GroupLeader, //是否班组长 必填
                IDCardType = 1, //证件类型 必填 1居民身份证 2护照
                //IDCardNumber = AESHelper.Encrypt(wrapperData.Data.IdCard, _tokenConfigDto.Password, keySecret.ProjectKeySecret), //证件号码 必填
                //Age = Math.Round((DateTime.Now - birthday).TotalDays / 365), //年龄 必填
                Gender = wrapperData.Data.Worker.Gender.DisplayName(), //性别 必填  男/女
                Nation = wrapperData.Data.Worker.Nation ?? "汉", //民族 必填
                Address = wrapperData.Data.Worker.Address ?? "住址", //住址 必填
                //headImage = ImageUrlToBase64Helper.TryConvertFullUrl(Logger, imageHost,
                //        worker.IdPhotoUrl), //"头像base64/url", //头像 必填 关联附件数据表
                //                            //1   中共党员 2   中共预备党员 3   共青团员 4   民革党员 5   民盟盟员 6   民建会员
                //                            //7   民进会员 8   农工党党员 9   致公党党员 10  九三学社社员 11  台盟盟员 12  无党派人士 13  群众
                PoliticsType = wrapperData.Data.PoliticsType, //13, //政治面貌 必填
                //CultureLevelType = wrapperData.Data.Education ?? 99, //3,//文化程度 必填 1	小学 2   初中 3   高中 4   中专 5   大专 6   本科 7   硕士 8   博士 99  其他
                GrantOrg = wrapperData.Data.Worker.IssuedBy ?? "发证机关", //发证机关 必填
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
                WorkType = wrapperData.Data.WorkerTypeCode, //工种名称 必填
                NativePlace = wrapperData.Data.AreaTenant?.Name, //"中国 四川 成都",//籍贯 必填
                Mobile = wrapperData.Data.PhoneNumber, //手机号码 必填
                //IssueCardPicUrl = ImageUrlToBase64Helper.TryConvertFullUrl(Logger, imageHost, worker.PhotoUrl),//照片  非必填 关联附件数据表
                HasContract = "是", //是否有劳动合同 必填
                                   //HasBuyInsurance = "是",//是否购买工伤或意外伤害保险 非必填
                                   //Remark = "备注"//备注 非必填
            };
            #endregion

            var input = new
            {
                type =  "建筑工人信息",
                data = new List<object> { person }
            };
            var content = new StringContent(JsonConvert.SerializeObject(input), Encoding.UTF8);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            //content.Headers.Add("ContentType", "application/json");

            Logger.LogInformation($"[闸机实名制]设备[{wrapperData.Data.Device.FakeNo}]数据发送{wrapperData.Data.Worker.Name}_{wrapperData.Data.PersonId}");
            return await Task.FromResult(content);
        }

        /// <summary>
        /// client request 扩展设置
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="fakeNo"></param>
        /// <param name="receiveUrl"></param>
        /// <returns></returns>
        protected override async Task<string> RequestExtends(HttpClient httpClient, string fakeNo = null,
            string receiveUrl = null)
        {
            //把token缓存到redis里面 缓存30分钟
            var cacheKey = string.Format("", fakeNo);
            var tokenResult = await _cacheContainer.GetCacheValue(cacheKey, () =>
                new MultilevelCache<TokenResultDto>(CachingProvider, cacheKey, async () =>
                {
                    var keySecret = KeySecretPool[Magics.CHONGQING,fakeNo];
                    _tokenConfigDto.AppKey = keySecret?.ProjectKeyId;
                    _tokenConfigDto.AppSecret = keySecret?.ProjectKeySecret;
                    return await TokenService.GetAccessToken(_tokenConfigDto);
                })
                {
                    RedisExpireTimeSpan = TimeSpan.FromMinutes(30)
                });
            if (tokenResult.Custom.IsExpired())
            {
                tokenResult = await TokenService.GetAccessToken(_tokenConfigDto);
            }
            return $"{receiveUrl}?access_token={tokenResult.Custom.Access_token}";
        }
    }
}
