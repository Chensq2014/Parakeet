using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Parakeet.Net.Caches;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Common.Dtos;
using Common.Entities;
using Common.Extensions;
using Common.Helpers;

namespace Parakeet.Net.ROServer.Handlers
{
    /// <summary>
    /// 自定义权限验证 鉴权 给user claims赋值
    /// </summary>
    public class GrpcAuthticateHandler : IAuthenticationHandler
    {
        public AuthenticationScheme Scheme { get; private set; }
        protected HttpContext Context { get; private set; }

        private readonly LicensePool _licensePool;

        public GrpcAuthticateHandler(LicensePool licensePool)
        {
            _licensePool = licensePool;
        }

        public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context)
        {
            Scheme = scheme;
            Context = context;
            return Task.CompletedTask;
        }

        public async Task<AuthenticateResult> AuthenticateAsync()
        {
            var appId = Context.Request.Headers["AppId"].ToString();
            var appKey = Context.Request.Headers["AppKey"].ToString();
            var appToken = Context.Request.Headers["AppToken"].ToString();
            var timeStamp = Context.Request.Headers["TimeStamp"].ToString();

            if (string.IsNullOrEmpty(appId))
            {
                await Task.CompletedTask;
                Log.Logger.Error($"认证失败,AppId为空");
                return AuthenticateResult.Fail(new Exception("认证失败,AppId为空"));
            }

            if (string.IsNullOrEmpty(appKey))
            {
                Log.Logger.Error($"认证失败,AppKey为空");
                return AuthenticateResult.Fail(new Exception("认证失败,AppKey为空"));
            }

            if (string.IsNullOrEmpty(appToken))
            {
                Log.Logger.Error($"认证失败,Token为空");
                return AuthenticateResult.Fail(new Exception("认证失败,Token为空"));
            }

            if (string.IsNullOrEmpty(timeStamp))
            {
                Log.Logger.Error($"认证失败,时间戳为空");
                return AuthenticateResult.Fail(new Exception("认证失败,时间戳为空"));
            }

            if (!long.TryParse(timeStamp, out var unixTimeStamp))
            {
                return AuthenticateResult.Fail(new Exception("认证失败,时间戳无效"));
            }

            Log.Logger.Information($"GRPC认证：AppId:[{appId}] & AppKey:[{appKey}] & AppToken:[{appToken}] & TimeStamp:[{timeStamp}]");

            var seconds = Math.Abs(DateTime.Now.ToUnixTimeTicks(10) - unixTimeStamp);
            if (seconds > TimeSpan.FromHours(10).TotalSeconds) //时间戳操作10小时判断为无效请求 兼容之前数据
            {
                Log.Logger.Error($"认证失败,时间戳过期从header获取到unixTimeStamp：{unixTimeStamp}_与服务器相差：{seconds}");
                return AuthenticateResult.Fail(new Exception($"认证失败,时间戳过期,从header获取到unixTimeStamp：{unixTimeStamp}_服务器时间戳{DateTime.Now.ToUnixTimeTicks(10)}_与服务器相差：{seconds}"));
            }

            if (string.IsNullOrEmpty(appToken))
            {
                return AuthenticateResult.NoResult();
            }

            var license = _licensePool[appId, appKey];

            var ticketWrapper = new TicketWrapper
            {
                Ticket = new Ticket(license.Id)
                {
                    CreationTime = license.CreationTime,
                    IsDeleted = license.IsDeleted,
                    Name = license.Name,
                    AppId = license.AppId,
                    AppKey = license.AppKey,
                    ExpiredAt = license.ExpiredAt,
                    AppSecret = license.AppSecret,
                    Token = license.Token
                },
                LicenseResources = license.LicenseResources.ToList(),
                //CacheExpiredAt = DateTime.Now.AddHours(1),
                RequestCount = 0
            };

            if (ticketWrapper.Ticket == null)
            {
                Log.Logger.Error($"License不存在");
                return AuthenticateResult.Fail(new Exception("License不存在"));
            }

            if (ticketWrapper.Ticket.ExpiredAt < DateTime.Now)
            {
                Log.Logger.Error($"AppId:[{ticketWrapper.Ticket.AppId}] - AppKey:[{ticketWrapper.Ticket.AppKey}]所属License过期 - [{ticketWrapper.Ticket.ExpiredAt}]");
                return AuthenticateResult.Fail(new Exception("License过期"));
            }

            var token = SHAHelper.HMACSHA256(ticketWrapper.Ticket.AppId, ticketWrapper.Ticket.AppKey, ticketWrapper.Ticket.AppSecret, unixTimeStamp);

            if (token != appToken)
            {
                Log.Logger.Error($"Token无效");
                return AuthenticateResult.Fail(new Exception("Token无效"));
            }

            var claimsIdentities = new List<ClaimsIdentity>
            {
                new ClaimsIdentity(new List<Claim>
                    {
                        new Claim("appId", ticketWrapper.Ticket.AppId),
                        new Claim("appKey", ticketWrapper.Ticket.AppKey),
                        new Claim("appSecret", ticketWrapper.Ticket.AppSecret),
                    }
                )
            };

            var claims = ticketWrapper.LicenseResources.Select(r => new Claim(ClaimTypes.Role, r.Code));
            claimsIdentities.Add(new ClaimsIdentity(claims));

            var pricipal = new ClaimsPrincipal(claimsIdentities);

            return AuthenticateResult.Success(new AuthenticationTicket(pricipal, "grpc"));
        }

        public Task ChallengeAsync(AuthenticationProperties properties)
        {
            //properties.Items.Values
            return Task.CompletedTask;
        }

        public Task ForbidAsync(AuthenticationProperties properties)
        {
            Context.Response.StatusCode = 403;
            return Task.CompletedTask;
        }
    }
}