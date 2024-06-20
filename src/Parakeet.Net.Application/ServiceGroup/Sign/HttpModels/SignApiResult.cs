//using System.Threading.Tasks;
//using Volo.Abp;
//using WebApiClientCore;

//namespace Parakeet.Net.ServiceGroup.Sign.HttpModels
//{
//    public class SignApiResult
//    {
//        [AliasAs("code")]
//        public int Code { get; set; }

//        [AliasAs("msg")]
//        public string Msg { get; set; }
//    }

//    public static class SignApiResultExtensions
//    {
//        public static async Task<TResult> ToResult<TResult>(this ITask<TResult> taskResult)
//            where TResult : SignApiResult
//        {
//            var result = await taskResult.InvokeAsync();
//            if (result.Code != 1)
//            {
//                //抛出友好异常并记录错误日志
//                throw new UserFriendlyException( result.Msg, result.Code.ToString(),"");
//            }
//            return result;
//        }
//    }

//    public class RealNameAuthApiResult: SignApiResult
//    {
//        /// <summary>
//        /// 认证结果代码：【0：认证成功，其他代码认证表示认证失败，详细信息参考message】
//        /// </summary>
//        [AliasAs("status")]
//        public string Status { get; set; }

//        /// <summary>
//        /// 认证结果信息
//        /// </summary>
//        [AliasAs("message")]
//        public string Message { get; set; }

//        /// <summary>
//        /// 调用时间
//        /// </summary>
//        [AliasAs("result_time")]
//        public string ResultTime { get; set; }

//        /// <summary>
//        /// 调用结果，true表示调用接口成功，false表示调用接口失败
//        /// </summary>
//        [AliasAs("success")]
//        public bool Success { get; set; }
//    }

//    public class EnterpriseCheckApplyApiResult : SignApiResult
//    {
//        /// <summary>
//        /// 申请结果（20：申请成功 30 or 10：申请失败）
//        /// </summary>
//        [AliasAs("status")]
//        public string Status { get; set; }

//        /// <summary>
//        /// 申请结果信息
//        /// </summary>
//        [AliasAs("message")]
//        public string Message { get; set; }

//        /// <summary>
//        /// 交易流水号
//        /// </summary>
//        [AliasAs("txSN")]
//        // ReSharper disable once InconsistentNaming
//        public string TxSN { get; set; }
//    }

//    public class EnterpriseCheckApiResult : SignApiResult
//    {
//        /// <summary>
//        /// 申请结果（20：申请成功 30 or 10：申请失败）
//        /// </summary>
//        [AliasAs("status")]
//        public string Status { get; set; }

//        /// <summary>
//        /// 申请结果信息
//        /// </summary>
//        [AliasAs("message")]
//        public string Message { get; set; }
//    }
//}
