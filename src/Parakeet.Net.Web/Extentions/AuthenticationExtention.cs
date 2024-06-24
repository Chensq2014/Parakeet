using Common;
using Common.Dtos;
using Common.Helpers;
using Common.JWTExtend;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Validation.AspNetCore;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Volo.Abp.Security.Claims;

namespace Parakeet.Net.Web.Extentions
{
    public static class AuthenticationExtention
    {
        public static void AddCommonAuthentication(this ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();

            #region HS256 对称可逆加密 加密key 可以自定义一个字符串，加解密都是对称的使用同样的加密key 
            //builder.Services.AddScoped<IJWTService, JWTHSService>();
            //builder.Services.Configure<JWTTokenOptions>(builder.Configuration.GetSection(CommonConsts.JWTTokenSectionName));
            #endregion

            #region RS256 非对称可逆加密，产生key与publickey(组对) 需要获取一次公钥给三方服务，加密key为生成token密钥
            context.Services.AddScoped<IJWTService, JWTRSService>();
            context.Services.Configure<JWTTokenOptions>(configuration.GetSection(CommonConsts.JWTTokenSectionName));
            var tokenOptions = new JWTTokenOptions();
            configuration.Bind(CommonConsts.JWTTokenSectionName, tokenOptions);
            //这里的tokenOptions.SecurityKey其实没有意义了,换成下面的密钥 keyParams

            //程序启动时，即初始化一组秘钥
            string keyDir = Directory.GetCurrentDirectory();
            if (!RSAHelper.TryGetKeyParameters(keyDir, true, out RSAParameters keyParams))
            {
                keyParams = RSAHelper.GenerateAndSaveKey(keyDir);
            }

            #endregion


            //注意:如果多次AddAuthentication 就会创建多个builder造成冲突或命名空间不一致
            //你应该确保只在应用程序的启动过程中调用AddAuthentication一次，并在该调用中配置所有必要的认证方案和选项。
            //如果你需要添加多个认证处理器或方案，你可以使用AddAuthentication的链式调用方法来配置它们，而不是多次调用AddAuthentication。

            context.Services
                .ForwardIdentityAuthenticationForBearer(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)
                .Configure<AbpClaimsPrincipalFactoryOptions>(options =>
                {
                    options.IsDynamicClaimsEnabled = true;
                })
            .AddAuthentication(options =>
            {
                options.DefaultScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;//JwtBearerDefaults.AuthenticationScheme;
                //options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                //options.DefaultScheme = "Cookies";//OpenIdConnectDefaults.AuthenticationScheme;//替换为你的默认认证方案名称
                //options.DefaultChallengeScheme = "oidc";
                //options.DefaultSignInScheme = "oidc";
            })
            ////.AddCookie("Cookies", options =>
            ////{
            ////    options.ExpireTimeSpan = TimeSpan.FromDays(365);
            ////    options.CheckTokenExpiration();
            ////})
            //.AddAbpOpenIdConnect("oidc", options =>
            //{
            //    options.Authority = configuration["AuthServer:Authority"];
            //    options.RequireHttpsMetadata = configuration.GetValue<bool>("AuthServer:RequireHttpsMetadata");
            //    options.ResponseType = OpenIdConnectResponseType.CodeIdToken;

            //    options.ClientId = configuration["AuthServer:ClientId"];
            //    options.ClientSecret = configuration["AuthServer:ClientSecret"];
            //    //options.ClientSecret = EncodingEncryptHelper.DEncrypt(configuration["AuthServer:ClientSecret"]);

            //    options.UsePkce = true;
            //    options.SaveTokens = true;
            //    options.GetClaimsFromUserInfoEndpoint = true;
            //    //options.ReturnUrlParameter=//options.RedirectUri = "https://your-app-url/signin-oidc";
            //    //options.SignedOutRedirectUri//options.PostLogoutRedirectUri = "https://your-app-url/logout";

            //    options.Scope.Add("openid");
            //    options.Scope.Add("profile");
            //    options.Scope.Add("roles");
            //    options.Scope.Add("email");
            //    options.Scope.Add("phone");
            //    options.Scope.Add("parakeet");
            //})
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {

                    ////自定义校验规则，可以新登录后将之前的无效
                    ////当JWT使用非对称加密算法时，IssuerSigningKey 是私钥，用于生成JWT的签名。
                    ////公钥用于验证JWT的签名，但不是IssuerSigningKey。
                    ////当JWT使用对称加密算法时，IssuerSigningKey 是共享的对称密钥，用于签名和验证JWT。
                    ////在实际应用中，你应根据你的安全需求和使用的JWT库来配置适当的签名算法和密钥。
                    ////The signing key must match!
                    //ValidateIssuerSigningKey = true,//是否验证SecurityKey
                    //IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOptions.SecurityKey)),
                    ValidateIssuerSigningKey = true,//是否验证SecurityKey
                    IssuerSigningKey = new RsaSecurityKey(keyParams),

                    IssuerSigningKeyValidator = (m, n, z) =>
                    {
                        Console.WriteLine("This is IssuerSigningKeyValidator");
                        return true;
                    },//自定义校验过程

                    IssuerValidator = (m, n, z) =>
                    {
                        Console.WriteLine("This is IssuerValidator");
                        return "http://localhost:5726";
                    },//自定义校验过程
                    AudienceValidator = (m, n, z) =>
                    {
                        Console.WriteLine("This is AudienceValidator");
                        return true;
                        //return m != null && m.FirstOrDefault().Equals(this.Configuration["Audience"]);
                    },
                    // Validate the JWT Issuer (iss)claim
                    ValidateIssuer = true,//是否验证Issuer
                    ValidIssuer = tokenOptions.Issuer,//"Issuer"，这两项和前面签发jwt的设置一致,
                    // Validate the JWT Audience (aud) claim
                    ValidateAudience = true,//是否验证Audience
                    ValidAudience = tokenOptions.Audience,//"Audience",
                    // Validate the token expiry
                    ValidateLifetime = true,//是否验证失效时间
                    // If you want to allow a certain amount of clock drift, set that here
                    ClockSkew = TimeSpan.Zero,
                    RequireExpirationTime = true
                };
                #region Events
                //即提供了委托扩展，也可以直接new新对象，override方法

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = OnJwtBearerMessageReceived,
                    OnAuthenticationFailed = OnJwtBearerAuthenticationFailed,
                    OnChallenge = context =>
                    {
                        Console.WriteLine($"This JWT Authentication OnChallenge");
                        context.Response.Headers["JWTChallenge"] = "expired";//告诉客户端是过期了
                        return Task.CompletedTask;
                    },
                    OnForbidden = context =>
                    {
                        Console.WriteLine($"This JWT Authentication OnForbidden");
                        context.Response.Headers["JWTForbidden"] = "1";//
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        Console.WriteLine($"This JWT Authentication OnTokenValidated");
                        context.Response.Headers["JWTTokenValidated"] = "1";//
                        return Task.CompletedTask;
                    }
                };
                #endregion
            })
            .AddMicrosoftIdentityWebApp(context.Services.GetConfiguration(), CommonConsts.AzureAdSectionName, OpenIdConnectDefaults.AuthenticationScheme);


            #region IdentityServer

            ////AddIdentityServer
            //context.Services.AddIdentityServer() //定义处理规则
            //    .AddDeveloperSigningCredential() //默认的开发者证书--临时证书--生产环境为了保证token不失效，证书是不变的
            //    .AddInMemoryClients(ClientInitConfig.GetClients())
            //    .AddInMemoryApiResources(ClientInitConfig.GetApiResources());

            ////// 添加IdentityServer4的配置
            ////context.Services.AddIdentityServer()
            ////.AddInMemoryConfiguration(new IdentityServer4.Configuration.InMemoryConfiguration
            ////{
            ////    Clients = new List<IdentityServer4.Configuration.Client>
            ////    {
            ////       new IdentityServer4.Configuration.Client
            ////       {
            ////          ClientId = "your-client-id",
            ////          ClientSecret = "your-client-secret",
            ////          AllowAccessTokensViaBrowser = true,
            ////          RedirectUris = new List<string> { "https://your-app-url/signin-oidc" },
            ////          PostLogoutRedirectUris = new List<string> { "https://your-app-url/logout" },
            ////          Scopes = new List<string> { "openid", "profile", "email" }
            ////       }
            ////    }
            ////});


            ///*
            //* This configuration is used when the AuthServer is running on the internal network such as docker or k8s.
            //* Configuring the redirecting URLs for internal network and the web
            //* The login and the logout URLs are configured to redirect to the AuthServer real DNS for browser.
            //* The token acquired and validated from the the internal network AuthServer URL.
            //*/
            //if (configuration.GetValue<bool>("AuthServer:IsContainerized"))
            //{
            //    context.Services.Configure<OpenIdConnectOptions>("oidc", options =>
            //    {
            //        options.TokenValidationParameters.ValidIssuers = new[]
            //        {
            //            configuration["AuthServer:MetaAddress"]!.EnsureEndsWith('/'),
            //            configuration["AuthServer:Authority"]!.EnsureEndsWith('/')
            //     };

            //        options.MetadataAddress = configuration["AuthServer:MetaAddress"]!.EnsureEndsWith('/') +
            //                                ".well-known/openid-configuration";

            //        var previousOnRedirectToIdentityProvider = options.Events.OnRedirectToIdentityProvider;
            //        options.Events.OnRedirectToIdentityProvider = async ctx =>
            //        {
            //            // Intercept the redirection so the browser navigates to the right URL in your host
            //            ctx.ProtocolMessage.IssuerAddress = configuration["AuthServer:Authority"]!.EnsureEndsWith('/') + "connect/authorize";

            //            if (previousOnRedirectToIdentityProvider != null)
            //            {
            //                await previousOnRedirectToIdentityProvider(ctx);
            //            }
            //        };
            //        var previousOnRedirectToIdentityProviderForSignOut = options.Events.OnRedirectToIdentityProviderForSignOut;
            //        options.Events.OnRedirectToIdentityProviderForSignOut = async ctx =>
            //        {
            //            // Intercept the redirection for signout so the browser navigates to the right URL in your host
            //            ctx.ProtocolMessage.IssuerAddress = configuration["AuthServer:Authority"]!.EnsureEndsWith('/') + "connect/logout";

            //            if (previousOnRedirectToIdentityProviderForSignOut != null)
            //            {
            //                await previousOnRedirectToIdentityProviderForSignOut(ctx);
            //            }
            //        };
            //    });
            //}

            #endregion

            //替换IClaimsTransformation 默认实现
            //context.Services.Replace(ServiceDescriptor.Singleton<IAuthenticationHandler, CustomAuthenticationHandler>());
            context.Services.Replace(ServiceDescriptor.Singleton<IClaimsTransformation, CustomNoopClaimsTransformation>());
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
