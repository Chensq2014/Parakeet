//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;
//using Parakeet.Net.Extensions;
//using Common.Helpers;
//using System.Text;
//using System.Threading.Tasks;
//using Volo.Abp;

//namespace Parakeet.Net.ServiceGroup.JianWei
//{
//    /// <summary>
//    /// ChongqingJianWei的AppKey
//    /// </summary>
//    public class ChongqingJianWeiAppKeyAttribute : ApiActionFilterAttribute
//    {
//        public override async Task OnBeginRequestAsync(ApiActionContext context)
//        {
//            var option = context.GetService<IOptions<ChongqingJianWeiOption>>().Value;
//            Check.NotNull(option, nameof(option));
//            context.RequestMessage.Headers.Add("X-Tsign-Open-App-Id", option.AppId);
//            context.RequestMessage.Headers.Add("X-Tsign-Open-App-Secret", option.SecurityKey);

//            var rCode = NumberExtensions.GenerateRandomString(15);
//            var ts = DateTimeExtensions.GetUnixTime();
//            var keyId = $"{option?.SupplierKeyId}_{option?.ProjectKeyId}";
//            var string2Encrypt = $"{rCode}_{ts}_{option?.SupplierKeySecret}_{option?.ProjectKeySecret}";
//            var signature = SHAHelper.SHA1(string2Encrypt,Encoding.UTF8);
//            context.RequestMessage.Headers.Add("keyId", keyId);
//            context.RequestMessage.Headers.Add("ts", ts.ToString());
//            context.RequestMessage.Headers.Add("rCode", rCode);
//            context.RequestMessage.Headers.Add("signature", signature);

//            var _logger = context.GetService<ILogger<ChongqingJianWeiAppKeyAttribute>>();
//            _logger.LogWarning($"[设备][{option.FakeNo}]请求同步重庆建委人员数据加密信息:");
//            _logger.LogWarning($"keyId:{keyId}");
//            _logger.LogWarning($"ts:{ts}");
//            _logger.LogWarning($"random:{rCode}");
//            _logger.LogWarning($"signature:{signature}");

//            await base.OnBeginRequestAsync(context);
//        }
//    }
//}
