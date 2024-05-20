using Common.Dtos;
using Common.Helpers;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Parakeet.Net.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.IdentityModel;

namespace Parakeet.Net.Controllers
{
    [Route("api/[controller]/[action]")]
    public class TokenAuthController : NetController
    {
        private readonly IdentityModelAuthenticationService _authenticationService;

        /// <summary>
        /// IServiceProvider负责提供实例 (IServiceCollection(context.Services)负责注册)
        /// </summary>
        private readonly IServiceProvider _serviceProvider;
        //public IHttpContextAccessor HttpContextAccessor;
        public TokenAuthController(
            //IHttpContextAccessor httpContextAccessor, 
            IdentityModelAuthenticationService authenticationService,
            IOptions<AbpIdentityClientOptions> options,
            IServiceProvider serviceProvider)
        {
            _authenticationService = authenticationService;
            _serviceProvider = serviceProvider;
            //HttpContextAccessor = httpContextAccessor;
            ClientOptions = options.Value;
        }

        /// <summary> 
        /// 从配置文件节点中自动获取
        /// </summary>
        protected AbpIdentityClientOptions ClientOptions { get; }

        /// <summary>
        /// 返回AccessToken
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        [HttpPost]//,Route("login")]
        public async Task<string> Authenticate([FromBody] AuthenticateModel login)
        {
            var configuration = new IdentityClientConfiguration(
                ClientOptions.IdentityClients.Default.Authority,
                ClientOptions.IdentityClients.Default.Scope,
                ClientOptions.IdentityClients.Default.ClientId,
                ClientOptions.IdentityClients.Default.ClientSecret,
                ClientOptions.IdentityClients.Default.GrantType,
                login.UserName,
                login.Password,
                false);
            try
            {
                return await _authenticationService.GetAccessTokenAsync(configuration);
            }
            catch (AbpException e)
            {
                //if (e.Message.Contains("invalid_username_or_password"))
                throw new UserFriendlyException("账号或密码错误");
            }
        }

        /// <summary>
        /// 刷新token
        /// </summary>
        /// <returns></returns>
        [HttpGet]//,Route("Refresh")
        public async Task<TokenResponse> RefreshToken(RefreshTokenInputDto input)
        {
            try
            {
                var refreshToken = input.RefreshToken ?? await HttpContext.GetTokenAsync("refresh_token");//HttpContextAccessor
                var client = new HttpClient();
                var response = await client.RequestRefreshTokenAsync(new RefreshTokenRequest
                {
                    //Address = _cloudConfigOptions.TokenUrl,
                    //RefreshToken = refreshToken,
                    //ClientId = _cloudConfigOptions.ClientId,
                    //ClientSecret = _cloudConfigOptions.ClientSecret,
                    //GrantType = input.GrantType ?? "refresh_token",//"authorization_code",//"client_credentials",//
                    //Scope = _cloudConfigOptions.Scope
                });
                return response;
            }
            catch
            {
                throw new UserFriendlyException("请求错误");
            }
        }

        //[HttpGet]
        //public Task<string> GetTest()
        //{
        //    var service =  _serviceProvider.GetRequiredService(typeof(IdentityModelAuthenticationService));//_serviceProvider.GetService(typeof(IdentityModelAuthenticationService));
        //    return Task.FromResult("1");
        //}



        /// <summary>
        /// 获取IOT接口token
        /// </summary>
        /// <param name="input">获取token输入类</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AppTokenInputDto> GetIotToken(AppTokenInputDto input)
        {
            //var token = SHAHelper.HMACSHA1(input.AppId,input.AppKey,input.AppSecret,input.TimeStamp);
            var token = SHAHelper.HMACSHA256(input.AppId,input.AppKey,input.AppSecret,input.TimeStamp);
            input.Token = token;
            return await Task.FromResult(input);
        }



    }
}