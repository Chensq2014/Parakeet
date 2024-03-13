using Common.Dtos;
using Microsoft.AspNetCore.Mvc;
using Parakeet.Net.Localization;
using System.Net;
using Volo.Abp.AspNetCore.Mvc;

namespace Parakeet.Net.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class NetController : AbpController// AbpControllerBase
{
    protected NetController()
    {
        LocalizationResource = typeof(NetResource);
    }


    #region JsonResultExtension

    //protected JsonResult Ok()
    //{
    //    Response.StatusCode = 200;
    //    return SJson();
    //}

    protected JsonResult BadRequest(string msg)
    {
        Response.StatusCode = (int)HttpStatusCode.BadRequest;
        return FJson($"错误:{msg}");
    }

    protected JsonResult DJson()
    {
        return DJson("null");
    }

    protected JsonResult DJson(object data)
    {
        return new JsonResult(data);
        //return new JsonResult
        //{
        //    Data = data,
        //    MaxJsonLength = int.MaxValue,//扩展这个避免多个页面因数据量太多报错
        //    JsonRequestBehavior = JsonRequestBehavior.AllowGet
        //};
    }

    protected JsonResult SJson()
    {
        return RJson(true);
    }

    protected JsonResult SJson(object data)
    {
        return new JsonResult(new AjaxReturnMessage { Status = true, Data = data, Msg = "请求成功" });
        //return new JsonResult
        //{
        //    Data = new Message { Status = true, Msg = "请求成功" },
        //    MaxJsonLength = Int32.MaxValue,
        //    JsonRequestBehavior = JsonRequestBehavior.AllowGet
        //};
    }

    /// <summary>
    /// 返回错误+对象
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    protected JsonResult EJson(object data)
    {
        return FJson(data);
    }

    /// <summary>
    /// 返回错误消息+对象
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private JsonResult FJson(object data)
    {
        return new JsonResult(new AjaxReturnMessage { Status = false, Data = data, Msg = "错误！" });
        //return new JsonResult
        //{
        //    Data = new Message { Status = false, Data = data, Msg = "错误！" },
        //    MaxJsonLength = Int32.MaxValue,
        //    JsonRequestBehavior = JsonRequestBehavior.AllowGet
        //};
    }

    /// <summary>
    /// 提供给返回字符串专用
    /// </summary>
    /// <param name="status"></param>
    /// <param name="msg"></param>
    /// <returns></returns>
    private JsonResult RJson(bool status, string msg = "成功！")
    {
        return new JsonResult(new AjaxReturnMessage { Status = status, Msg = msg });
        //return new JsonResult
        //{
        //    Data = new Message { Status = status, Msg = msg },
        //    MaxJsonLength = Int32.MaxValue,
        //    JsonRequestBehavior = JsonRequestBehavior.AllowGet
        //};
    }

    #endregion
}
