using Common;
using Common.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Validation.AspNetCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Modularity;

namespace Parakeet.Net.Web.Extentions
{
    public static class AuthenticationExtention
    {
        public static void AddCommonAuthentication(this ServiceConfigurationContext context)
        {
            //注意 如果多次AddAuthentication 就会创建多个builder造成冲突或命名空间不一致
            var configuration = context.Services.GetConfiguration();

            context.Services
                .ForwardIdentityAuthenticationForBearer(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)
                .AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";//OpenIdConnectDefaults.AuthenticationScheme;//替换为你的默认认证方案名称
                options.DefaultChallengeScheme = "oidc";
                options.DefaultSignInScheme = "oidc";
            })
                .AddCookie("Cookies", options =>
                {
                    options.ExpireTimeSpan = TimeSpan.FromDays(365);
                    options.CheckTokenExpiration();
                })
                .AddAbpOpenIdConnect("oidc", options =>
                {
                    options.Authority = configuration["AuthServer:Authority"];
                    options.RequireHttpsMetadata = configuration.GetValue<bool>("AuthServer:RequireHttpsMetadata");
                    options.ResponseType = OpenIdConnectResponseType.CodeIdToken;

                    options.ClientId = configuration["AuthServer:ClientId"];
                    options.ClientSecret = EncodingEncryptHelper.DEncrypt(configuration["AuthServer:ClientSecret"]);

                    options.UsePkce = true;
                    options.SaveTokens = true;
                    options.GetClaimsFromUserInfoEndpoint = true;
                    //options.ReturnUrlParameter=//options.RedirectUri = "https://your-app-url/signin-oidc";
                    //options.SignedOutRedirectUri//options.PostLogoutRedirectUri = "https://your-app-url/logout";

                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                    options.Scope.Add("roles");
                    options.Scope.Add("email");
                    options.Scope.Add("phone");
                    options.Scope.Add("parakeet");
                })
                .AddJwtBearer("Bearer", options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        // The signing key must match!
                        //ValidateIssuerSigningKey = true,
                        //IssuerSigningKey = ,
                        // Validate the JWT Issuer (iss)claim
                        ValidateIssuer = true,
                        ValidIssuer = "Issuer",
                        // Validate the JWT Audience (aud) claim
                        ValidateAudience = true,
                        ValidAudience = "Audience",
                        // Validate the token expiry
                        ValidateLifetime = true,
                        // If you want to allow a certain amount of clock drift, set that here
                        ClockSkew = TimeSpan.Zero,
                        RequireExpirationTime = true
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = OnJwtBearerMessageReceived,
                        OnAuthenticationFailed = OnJwtBearerAuthenticationFailed
                    };
                })
                .AddMicrosoftIdentityWebApp(context.Services.GetConfiguration(), CommonConsts.AzureAdSectionName, OpenIdConnectDefaults.AuthenticationScheme);


            #region IdentityServer

            //AddIdentityServer
            context.Services.AddIdentityServer() //定义处理规则
                .AddDeveloperSigningCredential() //默认的开发者证书--临时证书--生产环境为了保证token不失效，证书是不变的
                .AddInMemoryClients(ClientInitConfig.GetClients())
                .AddInMemoryApiResources(ClientInitConfig.GetApiResources());

            //// 添加IdentityServer4的配置
            //context.Services.AddIdentityServer()
            //.AddInMemoryConfiguration(new IdentityServer4.Configuration.InMemoryConfiguration
            //{
            //    Clients = new List<IdentityServer4.Configuration.Client>
            //    {
            //       new IdentityServer4.Configuration.Client
            //       {
            //          ClientId = "your-client-id",
            //          ClientSecret = "your-client-secret",
            //          AllowAccessTokensViaBrowser = true,
            //          RedirectUris = new List<string> { "https://your-app-url/signin-oidc" },
            //          PostLogoutRedirectUris = new List<string> { "https://your-app-url/logout" },
            //          Scopes = new List<string> { "openid", "profile", "email" }
            //       }
            //    }
            //});


            /*
            * This configuration is used when the AuthServer is running on the internal network such as docker or k8s.
            * Configuring the redirecting URLs for internal network and the web
            * The login and the logout URLs are configured to redirect to the AuthServer real DNS for browser.
            * The token acquired and validated from the the internal network AuthServer URL.
            */
            if (configuration.GetValue<bool>("AuthServer:IsContainerized"))
            {
                context.Services.Configure<OpenIdConnectOptions>("oidc", options =>
                {
                    options.TokenValidationParameters.ValidIssuers = new[]
                    {
                        configuration["AuthServer:MetaAddress"]!.EnsureEndsWith('/'),
                        configuration["AuthServer:Authority"]!.EnsureEndsWith('/')
                 };

                    options.MetadataAddress = configuration["AuthServer:MetaAddress"]!.EnsureEndsWith('/') +
                                            ".well-known/openid-configuration";

                    var previousOnRedirectToIdentityProvider = options.Events.OnRedirectToIdentityProvider;
                    options.Events.OnRedirectToIdentityProvider = async ctx =>
                    {
                        // Intercept the redirection so the browser navigates to the right URL in your host
                        ctx.ProtocolMessage.IssuerAddress = configuration["AuthServer:Authority"]!.EnsureEndsWith('/') + "connect/authorize";

                        if (previousOnRedirectToIdentityProvider != null)
                        {
                            await previousOnRedirectToIdentityProvider(ctx);
                        }
                    };
                    var previousOnRedirectToIdentityProviderForSignOut = options.Events.OnRedirectToIdentityProviderForSignOut;
                    options.Events.OnRedirectToIdentityProviderForSignOut = async ctx =>
                    {
                        // Intercept the redirection for signout so the browser navigates to the right URL in your host
                        ctx.ProtocolMessage.IssuerAddress = configuration["AuthServer:Authority"]!.EnsureEndsWith('/') + "connect/logout";

                        if (previousOnRedirectToIdentityProviderForSignOut != null)
                        {
                            await previousOnRedirectToIdentityProviderForSignOut(ctx);
                        }
                    };
                });
            }

            #endregion
        }



        private static Task OnJwtBearerMessageReceived(Microsoft.AspNetCore.Authentication.JwtBearer.MessageReceivedContext context)
        {
            if (string.IsNullOrEmpty(context.Token))
            {
                var token = context.HttpContext.Request.Query[CommonConsts.AccessTokenName].FirstOrDefault();
                if (string.IsNullOrEmpty(token))
                {
                    context.Token = token;
                }
                else
                {

                    context.Token = context.HttpContext.Request.Cookies[CommonConsts.AccessTokenName];
                }
            }
            return Task.CompletedTask;
        }

        private static Task OnJwtBearerAuthenticationFailed(Microsoft.AspNetCore.Authentication.JwtBearer.AuthenticationFailedContext context)
        {
            return Task.CompletedTask;
        }
 
    }

}
