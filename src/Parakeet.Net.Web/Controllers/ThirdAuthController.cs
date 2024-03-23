using Common.Dtos;
using Common.Extensions;
using Common.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Caching;

namespace Parakeet.Net.Web.Controllers
{
    /// <summary>
    /// 企业微信，钉钉等第第三方授权管理
    /// </summary>
    [Route("/api/parakeet/thirdAuth/[action]")]
    public class ThirdAuthController : AbpController
    {
        private readonly IWebHostEnvironment _environment;
        private IOptionsMonitor<WeixinOptionDto> _weixinOptionsMonitor;
        private readonly IDistributedCache<WeixinSdkTicketReturnDto> _cacheSdkTicketManager;
        private readonly IDistributedCache<WeixinAccessTokenReturnDto> _cacheAccessTokenManager;
        protected readonly IHttpClientFactory _httpClientFactory;

        public ThirdAuthController(IWebHostEnvironment environment,
            IOptionsMonitor<WeixinOptionDto> weixinOptionsMonitor,
            IDistributedCache<WeixinSdkTicketReturnDto> cacheSdkTicketManager,
            IDistributedCache<WeixinAccessTokenReturnDto> cacheAccessTokenManager,
            IHttpClientFactory httpClientFactory)
        {
            _environment = environment;
            _weixinOptionsMonitor = weixinOptionsMonitor;
            _cacheSdkTicketManager = cacheSdkTicketManager;
            _cacheAccessTokenManager = cacheAccessTokenManager;
            _httpClientFactory = httpClientFactory;
        }

        #region Weixin授权登录

        /// <summary>
        /// 构造网页授权链接返回给前端
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<string> GetAuthorizationUrl([FromQuery] InputUrlDto input)
        {
            var option = _weixinOptionsMonitor.CurrentValue;
            //input.Url = "http://shuangquan.aksoinfo.com/api/parakeet/thirdAuth/WeixinReturn";//GetWeixinLoginUserInfo
            input.Url = input.Url.UrlEncode();
            var state = (DateTime.Now.Ticks / 1000).ToString().UrlEncode();
            var url = $@"https://open.weixin.qq.com/connect/oauth2/authorize?appid={option.AppId}&redirect_uri={input.Url}&response_type=code&scope=snsapi_base&state={state}#wechat_redirect";
            return await Task.FromResult(url);
        }

        /// <summary>
        /// 获取授权页面 构造扫码登录链接
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<string> GetQrCodeAuthorizationUrl(InputUrlDto input)
        {
            var option = _weixinOptionsMonitor.CurrentValue;
            //input.Url = "http://shuangquan.aksoinfo.com/api/parakeet/thirdAuth/WeixinReturn";//GetWeixinLoginUserInfo
            input.Url = input.Url.UrlEncode();
            var state = (DateTime.Now.Ticks / 1000).ToString().UrlEncode();
            //Logger.LogError($"{input.Url}");
            var url = $@"https://open.work.weixin.qq.com/wwopen/sso/qrConnect?appid={option.AppId}&agentid={option.AgentId}&redirect_uri={input.Url}&state={state}";
            return await Task.FromResult(url);
        }

        /// <summary>
        /// 企业微信回调接口 获取访问用户身份
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        public async Task<WeixinAuthUserIdReturnDto> WeixinReturn(WeixinAuthReturnDto input)
        {
            var tokenResult = await GetAccessToken();//获取 自建应用的token?
            //var url = $@"https://qyapi.weixin.qq.com/cgi-bin/user/getuserinfo?access_token={tokenResult.Access_token}&code={input.Code}";
            var url = $@"https://qyapi.weixin.qq.com/cgi-bin/auth/getuserinfo?access_token={tokenResult.Access_token}&code={input.Code}";
            var result = await ClientGet<WeixinAuthUserIdReturnDto>(new InputUrlDto { Url = url });
            return result;
        }


        /// <summary>
        /// 获取访问用户身份
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        public async Task<WeixinAuthUserIdReturnDto> GetWeixinLoginUserInfo(WeixinAuthReturnDto input)
        {
            var tokenResult = await GetAccessToken();//获取 自建应用的token?
            var url = $@"https://qyapi.weixin.qq.com/cgi-bin/auth/getuserinfo?access_token={tokenResult.Access_token}&code={input.Code}";
            var result = await ClientGet<WeixinAuthUserIdReturnDto>(new InputUrlDto { Url = url });
            return result;
        }


        /// <summary>
        /// 获取微信tokenResult
        /// </summary>
        /// <returns></returns>
        /// <exception cref="KnowException"></exception>
        [HttpGet]
        public async Task<WeixinSdkSignatureReturnDto> GetWeixinApiSignature([FromQuery] WeixinSdkSignatureInputDto input)
        {
            var option = _weixinOptionsMonitor.CurrentValue;
            var result = new WeixinSdkSignatureReturnDto
            {
                AppId = option.AppId,
                AgentId = option.AgentId,
                Noncestr = input.Noncestr,
                Timestamp = input.Timestamp,
                Url = input.Url
            };
            var accessTokenResult = input.IsEnterPrise
                ? await GetWeixinEnterpriseSdkTicketToken()
                : await GetWeixinApplicationSdkTicketToken();
            result.Signature = $"jsapi_ticket={accessTokenResult.Ticket}&noncestr={result.Noncestr}&timestamp={result.Timestamp}&url={result.Url}";
            result.Signature = SHAHelper.SHA1(result.Signature);
            return result;
        }

        /// <summary>
        /// 获取企业微信 的jsapi_ticket
        /// </summary>
        /// <returns></returns>
        private async Task<WeixinSdkTicketReturnDto> GetWeixinSdkTicketToken()
        {
            var accessTokenResult = await GetAccessToken();
            //redis 缓存 缓存Sdk Token 50分钟 与accessTokenResult 缓存频率一起
            var key = $"ThirdAuth:Weixin:Application:{accessTokenResult.Access_token.Substring(0, 10)}";
            var tokenUrl = $@"https://qyapi.weixin.qq.com/cgi-bin/get_jsapi_ticket?access_token={accessTokenResult.Access_token}";
            var tokenResult = await _cacheSdkTicketManager.GetOrAddAsync(key, async () => await ClientGet<WeixinSdkTicketReturnDto>(new InputUrlDto { Url = tokenUrl }), () => new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(50)
            });
            if (tokenResult.IsExpired())
            {
                tokenResult = await ClientGet<WeixinSdkTicketReturnDto>(new InputUrlDto { Url = tokenUrl });
            }
            return tokenResult;
        }
        

        /// <summary>
        /// 获取企业微信企业的jsapi_ticket
        /// </summary>
        /// <returns></returns>
        private async Task<WeixinSdkTicketReturnDto> GetWeixinEnterpriseSdkTicketToken()
        {
            var accessTokenResult = await GetAccessToken();
            //redis 缓存 缓存Sdk Token 50分钟 与accessTokenResult 缓存频率一起
            var key = $"ThirdAuth:Weixin:Enterprise:{accessTokenResult.Access_token.Substring(0, 10)}";
            //获取企业的jsapi_ticket
            var tokenUrl = $@"https://qyapi.weixin.qq.com/cgi-bin/get_jsapi_ticket?access_token={accessTokenResult.Access_token}";
            var tokenResult = await _cacheSdkTicketManager.GetOrAddAsync(key, async () => await ClientGet<WeixinSdkTicketReturnDto>(new InputUrlDto { Url = tokenUrl }),
                () => new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(50)
                });
            if (tokenResult.IsExpired())
            {
                tokenResult = await ClientGet<WeixinSdkTicketReturnDto>(new InputUrlDto { Url = tokenUrl });
            }
            return tokenResult;
        }

        /// <summary>
        /// 获取企业微信 应用的jsapi_ticket
        /// </summary>
        /// <returns></returns>
        private async Task<WeixinSdkTicketReturnDto> GetWeixinApplicationSdkTicketToken()
        {
            var accessTokenResult = await GetAccessToken();
            //redis 缓存 缓存Sdk Token 50分钟 与accessTokenResult 缓存频率一起
            var key = $"ThirdAuth:Weixin:Application:{accessTokenResult.Access_token.Substring(0, 10)}";
            //获取应用的jsapi_ticket
            var tokenUrl = $@"https://qyapi.weixin.qq.com/cgi-bin/ticket/get?access_token={accessTokenResult.Access_token}&type=agent_config";
            ////获取企业的jsapi_ticket
            //var tokenUrl = $@"https://qyapi.weixin.qq.com/cgi-bin/get_jsapi_ticket?access_token={accessTokenResult.Access_token}";
            var tokenResult = await _cacheSdkTicketManager.GetOrAddAsync(key, async () => await ClientGet<WeixinSdkTicketReturnDto>(new InputUrlDto { Url = tokenUrl }),
                () => new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(50)
                });
            if (tokenResult.IsExpired())
            {
                tokenResult = await ClientGet<WeixinSdkTicketReturnDto>(new InputUrlDto { Url = tokenUrl });
            }
            return tokenResult;
        }

        /// <summary>
        /// 获取企业微信自建应用token
        /// </summary>
        /// <returns></returns>
        private async Task<WeixinAccessTokenReturnDto> GetAccessToken()
        {
            var option = _weixinOptionsMonitor.CurrentValue;
            var tokenUrl = $@"https://qyapi.weixin.qq.com/cgi-bin/gettoken?corpid={option.AppId}&corpsecret={option.SecurityKey}";
            var accessTokenResult = await _cacheAccessTokenManager.GetOrAddAsync(tokenUrl,
                async () => await ClientGet<WeixinAccessTokenReturnDto>(new InputUrlDto { Url = tokenUrl }),
                () => new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(90)
                });

            if (accessTokenResult.IsExpired())
            {
                accessTokenResult = await ClientGet<WeixinAccessTokenReturnDto>(new InputUrlDto { Url = tokenUrl });
            }

            return accessTokenResult;
        }


        [HttpGet]
        [Route("/WW_verify_qhm3cI3ukUc3ATyI.txt")]
        public IActionResult Verify()
        {
            var filePath = Path.Combine(_environment.WebRootPath, $@"/verify/WW_verify_qhm3cI3ukUc3ATyI.txt");
            //var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            //文件名必须编码，否则会有特殊字符(如中文)无法在此下载。
            var encodeFilename = HttpUtility.UrlEncode(Path.GetFileName(filePath), Encoding.GetEncoding("UTF-8"));
            Response.Headers.Add("Content-Disposition", "inline; filename=" + encodeFilename);
            return File(filePath, "txt/html");
        }

        #endregion

        #region 读取成员信息接口


        /// <summary>
        /// 获取成员ID列表
        /// </summary>
        /// <returns></returns>

        [HttpPost]
        public async Task<ResponseWrapper<WeixinUserListResponseDto>> GetWeinxinUserList(GetWeixinUserListInputDto input)
        {
            //获取企业成员的userid与对应的部门ID列表，预计于2022年8月8号发布
            //请求方式：POST（HTTPS）
            //请求地址：https://qyapi.weixin.qq.com/cgi-bin/user/list_id?access_token=ACCESS_TOKEN

            var accessTokenResult = await GetAccessToken();
            var result = await ClientPost<WeixinUserListResponseDto>(new RequestParameterDto
            {
                Host = $"https://qyapi.weixin.qq.com",
                Api = $"/cgi-bin/user/list_id?access_token={accessTokenResult.Access_token}",
                AccessToken = accessTokenResult.Access_token,
                ReturnObj = true,
                Content = new StringContent(JsonConvert.SerializeObject(new
                {
                    cursor = input.Cursor, //pageIndex
                    limit = input.Limit
                    //begin = input.StartDate ?? DateTime.Now.AddMinutes(-10),//请求10分钟以前考勤历史数据记录，因为每1分钟请求一次
                    //end = input.EndDate ?? DateTime.Now
                }), Encoding.UTF8)
            });

            return result;
        }


        /// <summary>
        /// 读取成员信息接口
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        public async Task<WeixinUserResponseDto> GetWeinxinUserBasicInfo([FromQuery] GetWeixinUserInputDto input)
        {
            //应用只能获取可见范围内的成员信息，且每种应用获取的字段有所不同，在返回结果说明中会逐个说明。
            //企业通讯录安全特别重要，企业微信将持续升级加固通讯录接口的安全机制，以下是关键的变更点：

            //从2022年6月20号20点开始，除通讯录同步以外的基础应用（如客户联系、微信客服、会话存档、日程等），
            //以及新创建的自建应用与代开发应用，调用该接口时，不再返回以下字段：头像、性别、手机、邮箱、企业邮箱、员工个人二维码、地址，
            //应用需要通过oauth2手工授权的方式获取管理员与员工本人授权的字段。

            //【重要】从2022年8月15日10点开始，“企业管理后台 - 管理工具 - 通讯录同步”的新增IP将不能再调用此接口，
            //企业可通过「获取成员ID列表」和「获取部门ID列表」接口获取userid和部门ID列表。
            //请求方式：GET（HTTPS）
            //请求地址：https://qyapi.weixin.qq.com/cgi-bin/user/get?access_token=ACCESS_TOKEN&userid=USERID
            //参数说明：
            //access_token  调用接口凭证
            //userid   成员UserID。对应管理端的帐号，企业内必须唯一。不区分大小写，长度为1 ~64个字节

            var accessTokenResult = await GetAccessToken();
            var url = $"https://qyapi.weixin.qq.com/cgi-bin/user/get?access_token={accessTokenResult.Access_token}&userid={input.UserId}";
            var result = await ClientGet<WeixinUserResponseDto>(new InputUrlDto { Url = url });

            return result;
        }


        /// <summary>
        /// 获取部门ID列表
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        public async Task<object> GetWeinxinDepartmentList([FromQuery] GetWeixinDepartmentListInputDto input)
        {
            //获取子部门ID列表
            //    最后更新：2022/01/13
            //请求方式：GET（HTTPS）
            //请求地址：https://qyapi.weixin.qq.com/cgi-bin/department/simplelist?access_token=ACCESS_TOKEN&id=ID

            var accessTokenResult = await GetAccessToken();
            var url = $"https://qyapi.weixin.qq.com/cgi-bin/department/simplelist?access_token={accessTokenResult?.Access_token}";
            if (input.DepartmentId.HasValue())
            {
                url = $"{url}&id={input.DepartmentId}";
            }
            var result = await ClientGet<WeixinDepartmentListResponseDto>(new InputUrlDto { Url = url });

            return result;
        }


        /// <summary>
        /// 通过手机号获取UserId
        /// </summary>
        /// <returns></returns>

        [HttpPost]
        public async Task<ResponseWrapper<WeixinUserResponseDto>> GetWeinxinUserIdByPhone(GetWeixinUserIdByPhoneInputDto input)
        {
            //手机号获取userid
            //    最后更新：2022 / 08 / 16
            //通过手机号获取其所对应的userid
            //请求方式：POST（HTTPS）
            //请求地址：https://qyapi.weixin.qq.com/cgi-bin/user/getuserid?access_token=ACCESS_TOKEN

            var accessTokenResult = await GetAccessToken();
            var result = await ClientPost<WeixinUserResponseDto>(new RequestParameterDto
            {
                Host = $"https://qyapi.weixin.qq.com",
                Api = $"/cgi-bin/user/getuserid?access_token={accessTokenResult.Access_token}",
                ReturnObj = true,
                Content = new StringContent(JsonConvert.SerializeObject(new
                {
                    mobile = input.Mobile
                }), Encoding.UTF8)
            });

            return result;
        }


        /// <summary>
        /// 通过手机号获取UserId
        /// </summary>
        /// <returns></returns>

        [HttpPost]
        public async Task<ResponseWrapper<WeixinSpecialPropReturnDto>> GetWeinxinUserSpecialInfoe(GetWeixinUserSpecialInfoInputDto input)
        {
            //获取访问用户敏感信息
            //    最后更新：2022 / 09 / 19
            //自建应用与代开发应用可通过该接口获取成员授权的敏感字段
            //    请求方式：POST（HTTPS）
            //请求地址：https://qyapi.weixin.qq.com/cgi-bin/auth/getuserdetail?access_token=ACCESS_TOKEN

            var accessTokenResult = await GetAccessToken();
            var result = await ClientPost<WeixinSpecialPropReturnDto>(new RequestParameterDto
            {
                Host = $"https://qyapi.weixin.qq.com",
                Api = $"/cgi-bin/auth/getuserdetail?access_token={accessTokenResult.Access_token}",
                //AccessToken = accessTokenResult.Access_token,
                ReturnObj = true,
                Content = new StringContent(JsonConvert.SerializeObject(new
                {
                    //AccessToken = accessTokenResult.Access_token,
                    user_ticket = input.UserTicket
                }), Encoding.UTF8)
            });

            return result;
        }

        /// <summary>
        /// 通过邮箱获取UserId
        /// </summary>
        /// <returns></returns>

        [HttpPost]
        public async Task<ResponseWrapper<WeixinUserResponseDto>> GetWeinxinUserIdByEmail(GetWeixinUserIdByEmailInputDto input)
        {
            //邮箱获取userid
            //    最后更新：2022 / 07 / 19
            //通过邮箱获取其所对应的userid。

            //请求方式：POST（HTTPS）
            //请求地址：https://qyapi.weixin.qq.com/cgi-bin/user/get_userid_by_email?access_token=ACCESS_TOKEN

            var accessTokenResult = await GetAccessToken();
            var result = await ClientPost<WeixinUserResponseDto>(new RequestParameterDto
            {
                Host = $"https://qyapi.weixin.qq.com",
                Api = $"/cgi-bin/user/get_userid_by_email?access_token={accessTokenResult.Access_token}",
                ReturnObj = true,
                Content = new StringContent(JsonConvert.SerializeObject(new
                {
                    email = input.Email,
                    email_type = input.EmailType
                }), Encoding.UTF8)
            });

            return result;
        }

        /// <summary>
        /// 发送应用消息
        /// </summary>
        /// <returns></returns>

        [HttpPost]
        public async Task<ResponseWrapper<WeixinUserResponseDto>> SendWeixinMessage()
        {
            //发送应用消息
            //    最后更新：2022 / 10 / 26
            //接口定义
            //    应用支持推送文本、图片、视频、文件、图文等类型。

            //请求方式：POST（HTTPS）
            //请求地址： https://qyapi.weixin.qq.com/cgi-bin/message/send?access_token=ACCESS_TOKEN

            var accessTokenResult = await GetAccessToken();
            var result = await ClientPost<WeixinUserResponseDto>(new RequestParameterDto
            {
                Host = $"https://qyapi.weixin.qq.com",
                Api = $"/cgi-bin/message/send?access_token={accessTokenResult.Access_token}",
                ReturnObj = true,
                AccessToken = accessTokenResult.Access_token,
                Content = new StringContent(JsonConvert.SerializeObject(new
                {
                    #region 公共字段

                    access_token = accessTokenResult.Access_token,
                    //指定接收消息的成员，成员ID列表（多个接收者用‘|’分隔，最多支持1000个）。
                    //特殊情况：指定为"@all"，则向该企业应用的全部成员发送
                    touser = "ChenShuangQuan|ZhouJinLong",
                    //指定接收消息的部门，部门ID列表，多个接收者用‘|’分隔，最多支持100个。
                    //当touser为"@all"时忽略本参数
                    //toparty = "1",
                    //指定接收消息的标签，标签ID列表，多个接收者用‘|’分隔，最多支持100个。
                    //当touser为"@all"时忽略本参数
                    //totag = "",
                    //企业应用的id，整型。企业内部开发，可在应用的设置页面查看；第三方服务商，可通过接口 获取企业授权信息 获取该参数值
                    agentid = _weixinOptionsMonitor.CurrentValue.AgentId,
                    //表示是否开启id转译，0表示否，1表示是，默认0。仅第三方应用需要用到，企业自建应用可以忽略。
                    enable_id_trans = 0,
                    //表示是否开启重复消息检查，0表示否，1表示是，默认0
                    enable_duplicate_check = 1,
                    //表示是否重复消息检查的时间间隔，默认1800s，最大不超过4小时
                    duplicate_check_interval = 1800,
                    //消息类型 text template_card textcard
                    msgtype = "text",//"template_card",//"textcard",//
                    #endregion

                    #region 文本类型 touser、toparty、totag不能同时为空，后面不再强调。

                    ////文本类型内容对象
                    //text = new
                    //{
                    //    //消息内容，最长不超过2048个字节，超过将截断（支持id转译）
                    //    content = $"测试通过接口发送一个带a标签的文本消息。\n点击一下可跳转到企业微信官网 <a href = \"http://work.weixin.qq.com\">企业微信</a>"
                    //},
                    //////表示是否是保密消息，0表示可对外分享，1表示不能分享且内容显示水印，默认为0
                    ////safe = 0

                    #endregion

                    #region 文本卡片消息 textcard

                    //textcard = new
                    //{
                    //    title = "领奖通知",
                    //    description = "<div class=\"gray\">2016年9月26日</div> <div class=\"normal\">恭喜你抽中iPhone 7一台，领奖码：xxxx</div><div class=\"highlight\">请于2016年10月10日前联系行政同事领取</div>",
                    //    url = "http://work.weixin.qq.com",
                    //    btntxt = "更多"
                    //}

                    #endregion

                    #region 文本通知类型 "msgtype="template_card", 暂不支持因为需要回调url？

                    //template_card = new
                    //{
                    //    card_type = "text_notice",
                    //    source = new
                    //    {
                    //        icon_url = "https://work.weixin.qq.com",
                    //        desc = "企业微信",
                    //        desc_color = 1
                    //    },
                    //    action_menu = new
                    //    {
                    //        desc = "卡片副交互辅助文本说明",
                    //        action_list = new List<object>
                    //        {
                    //            new { text = "接受推送", key= "A"},
                    //            new { text = "不再推送", key= "B"}
                    //        }
                    //    },
                    //    task_id = "task_id",
                    //    main_title = new
                    //    {
                    //        title = "欢迎使用企业微信",
                    //        desc = "您的好友正在邀请您加入企业微信"
                    //    },
                    //    quote_area = new
                    //    {
                    //        type = 1,
                    //        url = "https://work.weixin.qq.com",
                    //        title = "企业微信的引用样式",
                    //        quote_text = "企业微信真好用呀真好用"
                    //    },
                    //    emphasis_content = new
                    //    {
                    //        title = "100",
                    //        desc = "核心数据"
                    //    },
                    //    sub_title_text = "下载企业微信还能抢红包！",
                    //    horizontal_content_list = new List<object>{
                    //        new {
                    //            type = 1,
                    //            keyname = "企业微信官网",
                    //            value = "点击访问",
                    //            url = "https://work.weixin.qq.com"
                    //        },
                    //        new {
                    //            type = 2,
                    //            keyname = "企业微信下载",
                    //            value = "企业微信.apk",
                    //            media_id = "文件的media_id",
                    //            url = "https://work.weixin.qq.com"
                    //        },
                    //        new {
                    //            type = 3,
                    //            keyname = "员工信息",
                    //            value = "点击查看",
                    //            userid = "zhangsan",
                    //            url = "https://work.weixin.qq.com"
                    //        }
                    //    },
                    //    jump_list = new List<object>{
                    //        new {
                    //            type = 1,
                    //            title = "企业微信官网",
                    //            url = "https://work.weixin.qq.com"
                    //        },
                    //        new {
                    //            type = 2,
                    //            title = "跳转小程序",
                    //            appid = "小程序的appid",
                    //            pagepath = "/index.html",
                    //            url = "https://work.weixin.qq.com"
                    //        }
                    //    },
                    //    card_action = new
                    //    {
                    //        type = 2,
                    //        url = "https://work.weixin.qq.com",
                    //        appid = "小程序的appid",
                    //        pagepath = "/index.html"
                    //    }
                    //}

                    #endregion


                }), Encoding.UTF8)
            });

            return result;
        }


        #endregion

        #region httpClient请求

        /// <summary>
        /// 封装client Get请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task<T> ClientGet<T>(InputUrlDto input) where T : class
        {
            try
            {
                //using var client = new HttpClient();
                using var client = _httpClientFactory.CreateClient();
                //client.DefaultRequestHeaders.Add("signature", signature);
                //client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                //client.DefaultRequestVersion = HttpVersion.Version11;
                //client.BaseAddress = new Uri($@"https://");
                //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header
                //client.DefaultRequestHeaders.Add("content-type", "application/json");
                Console.WriteLine($"开始请求:{input.Url}");
                Console.WriteLine($"------------------------------------------------------------------------------------------------");
                var result = await client.GetAsync(input.Url);
                var msg = await result.Content.ReadAsStringAsync();

                if (result.IsSuccessStatusCode)
                {
                    var returnObj = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(msg);
                    Console.WriteLine($"请求{input.Url}结果:\n{Newtonsoft.Json.JsonConvert.SerializeObject(returnObj)}");
                    Console.WriteLine($"------------------------------------------------------------------------------------------------");
                    return returnObj;
                }
                else
                {
                    Console.WriteLine($"请求成功，返回失败！");
                    Console.WriteLine($"------------------------------------------------------------------------------------------------");
                }
            }
            catch
            {
                Console.WriteLine($"请求错误!");
                Console.WriteLine($"------------------------------------------------------------------------------------------------");
            }
            return null;
        }

        /// <summary>
        /// 封装请求
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task<ResponseWrapper<T>> ClientPost<T>(RequestParameterDto input) where T : class
        {
            //using var client = new HttpClient();
            using var client = _httpClientFactory.CreateClient();
            var fullUrl = RequestExtends(client, input.AccessToken, input.FullUrl);
            ContentExtends(input);
            var returnData = new ResponseWrapper<T>();
            try
            {
                var result = await client.PostAsync(fullUrl, input.Content);
                if (result.IsSuccessStatusCode)
                {
                    Console.WriteLine($"请求成功！");
                    var returnJson = await result.Content.ReadAsStringAsync();
                    if (input.ReturnObj)
                    {
                        returnData.Data = JsonConvert.DeserializeObject<T>(returnJson);//result.Content.As<T>();
                        Console.WriteLine($"请求结果:\n{JsonConvert.DeserializeObject(returnJson)}");
                    }
                    else
                    {
                        Console.WriteLine($"请求{fullUrl}结果:\n{JsonConvert.DeserializeObject<object>(returnJson)}");
                    }
                }
                else
                {
                    Console.WriteLine($"========================================================================================================");
                    Console.WriteLine($"{await result.Content.ReadAsStringAsync()}");
                    Console.WriteLine($"========================================================================================================");
                }
                returnData.Success = result.IsSuccessStatusCode;
                result.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"请求失败：\n{ex.Message}");
                returnData.Success = false;
            }
            return returnData;
        }

        /// <summary>
        /// client request 扩展设置
        /// </summary>
        /// <param name="client">HttpClient</param>
        /// <param name="token">token</param>
        /// <param name="receiveUrl">请求url</param>
        /// <returns></returns>
        private static string RequestExtends(HttpClient client, string token = null,
            string receiveUrl = null)
        {
            #region 重庆建委V1.0加密信息

            //var _keySecret = new
            //{
            //    SupplierKeyId = "ea5cbc75-1f18-4c92-a449-023f6de0f002",
            //    SupplierKeySecret = "igNd2gONoxWavfs4jfXR6SWcU8x6CdFnPep7",
            //    ProjectKeyId = "f0f50da3-3cc7-49aa-950f-5788e61dc36c",
            //    ProjectKeySecret = "ANQG5wj7S4kZXN0FhfNy9nxbGUQmckfw8ybZ"
            //};

            //var rCode = Utilities.GenerateRandomString(15);
            //var ts = DateTimeExtensions.GetUnixTime();
            //var keyId = $"{_keySecret?.SupplierKeyId}_{_keySecret?.ProjectKeyId}";
            //var signature = Utilities.GetTokenBySupplierAndProject(rCode, ts, _keySecret?.SupplierKeySecret,
            //    _keySecret?.ProjectKeySecret);

            ////Console.WriteLine($"[重庆建委采集设备][{serialNo}]请求同步数据加密信息:");
            //client.DefaultRequestHeaders.Add("keyId", keyId);
            //client.DefaultRequestHeaders.Add("ts", ts.ToString());
            //client.DefaultRequestHeaders.Add("rCode", rCode);
            //client.DefaultRequestHeaders.Add("signature", signature);
            //Console.WriteLine($"keyId:{keyId}");
            //Console.WriteLine($"ts:{ts}");
            //Console.WriteLine($"random:{rCode}");
            //Console.WriteLine($"signature:{signature}");
            #endregion

            #region 重庆V2版本需要设置 DefaultRequestHeaders

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            //Console.WriteLine($"开始请求:{receiveUrl}");
            //Console.WriteLine($"请求header:Authorization:Bearer {token}");
            ////return receiveUrl;
            #endregion

            #region 四川省厅需要设置 Url 带上?access_token

            //return $"{receiveUrl}?access_token={token}";

            #endregion

            #region 厦门会展一标段

            //client.DefaultRequestHeaders.Add("Cookie", $"satoken=f9f4183d-5b7b-4c9b-bda7-2401def00833");

            #endregion

            #region 其它设置

            //client.DefaultRequestVersion = HttpVersion.Version11;
            //client.BaseAddress = new Uri($@"https://");
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header
            //client.DefaultRequestHeaders.Add("content-type", "application/json");

            #endregion

            return receiveUrl;

        }


        private static RequestParameterDto ContentExtends(RequestParameterDto input)
        {
            #region content header

            //var json = TextJsonConvert.SerializeObject(input);
            //Console.WriteLine($"json字符串:{json}");
            //var setting = new JsonSerializerSettings
            //{
            //    ContractResolver = new CamelCasePropertyNamesContractResolver(), //序列化时key为驼峰样式
            //    DateTimeZoneHandling = DateTimeZoneHandling.Local,
            //    NullValueHandling = NullValueHandling.Ignore,
            //    DateFormatString = "yyyy/MM/dd HH:mm:ss",
            //    ReferenceLoopHandling = ReferenceLoopHandling.Ignore//忽略循环引用
            //};

            Console.WriteLine($"序列化input对象:");//TextJsonConvert在序列化input.Content=null存在各种bug
            //Console.WriteLine($"input：{Newtonsoft.Json.JsonConvert.DeserializeObject(TextJsonConvert.SerializeObject(input))}");
            Console.WriteLine($"input：{JsonConvert.DeserializeObject(JsonConvert.SerializeObject(input))}");

            //input.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            //input.Content.Headers.Add("ContentType", $"application/json");
            //input.Content.Headers.ContentType = new MediaTypeHeaderValue("multipart/form-data");
            //input.Content.Headers.Add("ContentType", $"multipart/form-data;charset=UTF-8");
            //input.Content.Headers.Add("ContentType", "application/x-www-form-urlencoded");

            #region cdceg添加header  已经在input.Content里添加好了
            //var tokenResult = await HttpClientPool.GetAccessToken(devKey, $@"{host}:{port}", tokenApi);
            //content.Headers.Add("AccessToken", tokenResult.AccessToken);
            #endregion

            #endregion

            return input;
        }

        #endregion
    }
}
