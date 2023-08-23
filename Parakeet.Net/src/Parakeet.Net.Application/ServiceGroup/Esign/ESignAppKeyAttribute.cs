//using Microsoft.Extensions.Options;
//using System.Threading.Tasks;
//using Volo.Abp;

//namespace Parakeet.Net.ServiceGroup.Esign
//{
//    /// <summary>
//    /// e签宝的AppKey
//    /// </summary>
//    public class ESignAppKeyAttribute : ApiActionFilterAttribute
//    {
//        public override async Task OnBeginRequestAsync(ApiActionContext context)
//        {
//            var signOption = context.GetService<IOptions<ESignOption>>().Value;
//            Check.NotNull(signOption, nameof(signOption));
//            context.RequestMessage.Headers.Add("X-Tsign-Open-App-Id", signOption.AppId);
//            context.RequestMessage.Headers.Add("X-Tsign-Open-App-Secret", signOption.SecurityKey);
//            await base.OnBeginRequestAsync(context);
//        }
//    }
//}
