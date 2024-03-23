using Common.Storage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Linq;
using System.Text;
using Common.Dtos;
using Parakeet.Net.Aop;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace Parakeet.Net.GrpcLessonServer
{
    [DependsOn(typeof(NetApplicationModule))]
    public class GrpcLessonModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(GrpcLessonModule)} Start PreConfigureServices ....");
            base.PreConfigureServices(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(GrpcLessonModule)} End PreConfigureServices ....");
        }
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(GrpcLessonModule)} Start ConfigureServices ....");
            //添加grpc配置
            context.Services.AddGrpc(options =>
            {
                options.Interceptors.Add<CustomServerLoggerInterceptor>();
            });

            #region jwt 校验 Hs

            var tokenOptions = new JWTTokenOptions();
            context.Services.GetConfiguration().Bind("JWTTokenOptions", tokenOptions);

            //鉴权
            context.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        //JWT有一些默认的属性，就是给鉴权时就可以筛选了
                        ValidateIssuer = true,//是否验证Issuer
                        ValidateAudience = true,//是否验证Audience
                        //ValidateLifetime = true,//是否验证失效时间
                        ValidateIssuerSigningKey = true,//是否验证SecurityKey
                        ValidAudience = tokenOptions.Audience,//
                        ValidIssuer = tokenOptions.Issuer,//Issuer，这两项和前面签发jwt的设置一致
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOptions.SecurityKey)),//拿到SecurityKey
                        //AudienceValidator = (m, n, z) =>
                        //{
                        //    //等同于去扩展了下Audience的校验规则---鉴权
                        //    return m != null && m.FirstOrDefault().Equals(this.Configuration["audience"]);
                        //},
                        //LifetimeValidator = (notBefore, expires, securityToken, validationParameters) =>
                        //{
                        //    return notBefore <= DateTime.Now
                        //    && expires >= DateTime.Now;
                        //    //&& validationParameters
                        //}//自定义校验规则
                    };
                });

            //授权策略
            context.Services.AddAuthorization(options =>
            {
                //添加grpcEMail 策略
                options.AddPolicy("grpcEMail", policyBuilder => policyBuilder.RequireAssertion(handlerContext =>
                    handlerContext.User.HasClaim(c => c.Type == "EMail") && handlerContext.User.Claims.First(c => c.Type.Equals("EMail")).Value.EndsWith("@qq.com")));
            });

            #endregion

            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(GrpcLessonModule)} End ConfigureServices ....");
        }


        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(GrpcLessonModule)} Start OnApplicationInitialization ....");
            var app = context.GetApplicationBuilder();
            //var configuration = context.GetConfiguration(); //env.GetAppConfiguration();
            if (context.GetEnvironment().IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRouting();

            #region 鉴权授权
            app.UseAuthentication();//鉴权 User赋值
            app.UseAuthorization();//判断User是否有claims等
            #endregion

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<LessonService>();

                endpoints.MapGet("/", async ctx =>
                {
                    await ctx.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });

            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(GrpcLessonModule)} End OnApplicationInitialization ....");
        }

    }
}
