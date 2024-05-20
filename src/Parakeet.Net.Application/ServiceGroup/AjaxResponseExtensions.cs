using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp;

namespace Parakeet.Net.ServiceGroup
{
    public static class AjaxResponseExtensions
    {
        ///// <summary>
        ///// 验证并返回结果
        ///// </summary>
        ///// <typeparam name="TResult"></typeparam>
        ///// <param name="ajaxResponse"></param>
        ///// <returns></returns>
        //public static TResult ToResult<TResult>(this AjaxResponse<TResult> ajaxResponse)
        //{
        //    if (!ajaxResponse.Success)
        //    {
        //        throw new AbpException(ajaxResponse.Error.ToJsonString());
        //    }
        //    return ajaxResponse.Result;
        //}
    }
}
