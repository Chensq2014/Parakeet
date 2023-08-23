//using Microsoft.Extensions.Options;
//using System;
//using System.Security.Cryptography;
//using System.Text;
//using System.Threading.Tasks;
//using Volo.Abp;

//namespace Parakeet.Net.ServiceGroup.Sign
//{
//    /// <summary>
//    /// AppKey过滤器
//    /// </summary>
//    public class SignAppKeyAttribute: ApiActionFilterAttribute
//    {
//        public override async Task OnBeginRequestAsync(ApiActionContext context)
//        {
//            var signOption =context.GetService<IOptions<SignOption>>().Value;
//            Check.NotNull(signOption, nameof(signOption));
//            var appId = signOption.AppId;
//            var securityKey= signOption.SecurityKey;
//            var time = DateTimeToStamp(DateTime.Now).ToString();
//            var sign = MD5Hash($"{appId}.{time}.{securityKey}");
//            context.RequestMessage.Headers.Add("app_id", appId);
//            context.RequestMessage.Headers.Add("time", time);
//            context.RequestMessage.Headers.Add("sign", sign);
//            await base.OnBeginRequestAsync(context);
//        }

//        private string MD5Hash(string input)
//        {
//            using (var md5 = MD5.Create())
//            {
//                var result = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
//                var strResult = BitConverter.ToString(result);
//                return strResult.Replace("-", "");
//            }

//        }

//        private long DateTimeToStamp(DateTime time)
//        {
//            var startTime = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
//            return (long)(time - startTime).TotalMilliseconds;
//        }
//    }
//}