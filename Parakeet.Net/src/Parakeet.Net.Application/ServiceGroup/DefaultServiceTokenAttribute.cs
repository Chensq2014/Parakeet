//using Microsoft.Extensions.Options;
//using Nito.AsyncEx;
//using System.Net.Http.Headers;
//using System.Threading.Tasks;

//namespace Parakeet.Net.ServiceGroup
//{
//    /// <summary>
//    ///     默认token注入过滤器：微服务与微服务系统级接口调用将请求头中的Authorization(Token)注入过滤器
//    /// </summary>
//    public class DefaultServiceTokenAttribute : ApiActionFilterAttribute
//    {
//        //private IConfigurationRoot _appConfiguration;
//        //public ServiceTokenAttribute(IConfigurationRoot appConfiguration)
//        //{
//        //    _appConfiguration = appConfiguration;
//        //}

//        /// <summary>异步锁</summary>
//        private readonly AsyncLock _asyncRoot = new AsyncLock();

//        /// <summary>最近请求到的token</summary>
//        private TokenResult _token;

//        /// <summary>获取或设置过滤器的执行排序索引</summary>
//        public override int OrderIndex { get; set; }

//        protected AbpIdentityClientOptions ClientOptions { get; set; }
        
//        /// <summary>
//        /// 重写父类ApiActionFilterAttribute 准备请求前逻辑
//        /// </summary>
//        /// <param name="context"></param>
//        /// <returns></returns>
//        public override async Task OnBeginRequestAsync(ApiActionContext context)
//        {
//            ClientOptions = context.GetService<IOptions<AbpIdentityClientOptions>>().Value;
//            using (await _asyncRoot.LockAsync())
//            {
//                await InitOrRefreshTokenAsync().ConfigureAwait(false);
//            }
//            AccessTokenResult(context, _token);
//        }

//        /// <summary>
//        /// 重写父类ApiActionFilterAttribute 请求完成之后逻辑
//        /// </summary>
//        /// <param name="context"></param>
//        /// <returns></returns>
//        public override Task OnEndRequestAsync(ApiActionContext context)
//        {
//            // 写扩展逻辑...
//            return Task.CompletedTask;//base.OnEndRequestAsync(context);
//        }

//        /// <summary>
//        /// 初始化或刷新token
//        /// </summary>
//        /// <returns></returns>
//        private async Task InitOrRefreshTokenAsync()
//        {
//            if (_token == null)
//            {
//                _token = await RequestTokenResultAsync().ConfigureAwait(false);
//            }
//            else if (_token.IsExpired())
//            {
//                if (_token.CanRefresh())
//                {
//                    _token = await RequestRefreshTokenAsync(_token.RefreshToken).ConfigureAwait(false);
//                }
//                else
//                {
//                    _token = await RequestTokenResultAsync().ConfigureAwait(false);
//                }
//            }

//            _token.EnsureSuccess();
//        }

//        /// <summary>
//        ///     应用AccessToken
//        ///     默认为添加到请求头的Authorization
//        /// </summary>
//        /// <param name="context">请求上下文</param>
//        /// <param name="tokenResult">token结果</param>
//        protected virtual void AccessTokenResult(ApiActionContext context, TokenResult tokenResult)
//        {
//            context.RequestMessage.Headers.Authorization =
//                new AuthenticationHeaderValue("Bearer", tokenResult.AccessToken);
//        }

//        /// <summary>
//        ///     请求获取token
//        ///     可以使用TokenClient来请求
//        /// </summary>
//        /// <returns></returns>
//        protected async Task<TokenResult> RequestTokenResultAsync()
//        {
//            var clientConfiguration = ClientOptions.IdentityClients["Parakeet_Server"];
//            var tokenClient = new TokenClient(clientConfiguration.Authority.TrimEnd('/') + "/connect/token");
//            var result = await tokenClient.RequestClientCredentialsAsync(clientConfiguration.ClientId,
//                clientConfiguration.ClientSecret, clientConfiguration.Scope);
//            return result;
//        }

//        /// <summary>
//        ///     请求刷新token
//        ///     可以使用TokenClient来刷新
//        /// </summary>
//        /// <param name="refreshToken">获取token时返回的refresh_token</param>
//        /// <returns></returns>
//        protected async Task<TokenResult> RequestRefreshTokenAsync(string refreshToken)
//        {
//            var clientConfiguration = ClientOptions.IdentityClients["Parakeet_Server"];
//            var tokenClient = new TokenClient(clientConfiguration.Authority.TrimEnd('/') + "/connect/token");
//            return await tokenClient.RequestRefreshTokenAsync(clientConfiguration.ClientId,
//                clientConfiguration.ClientSecret, refreshToken);
//        }
//    }
//}