using Microsoft.AspNetCore.Http;
using Parakeet.Net.Localization;
using System;
using System.Linq;
using Volo.Abp.Application.Services;
using Volo.Abp.AutoMapper;

namespace Parakeet.Net;

/* Inherit your application services from this class.
 */
public abstract class CustomerAppService : ApplicationService
{
    public IMapperAccessor MapperAccessor => LazyServiceProvider.LazyGetRequiredService<IMapperAccessor>();
    public AutoMapper.IConfigurationProvider Configuration => MapperAccessor.Mapper.ConfigurationProvider;
    protected IHttpContextAccessor ContextAccessor => LazyServiceProvider.LazyGetRequiredService<IHttpContextAccessor>();
    protected CustomerAppService()
    {
        LocalizationResource = typeof(NetResource);
        ObjectMapperContext = typeof(NetApplicationModule);
    }

    /// <summary>
    /// 获取请求url/formbody上的Id或者key
    /// </summary>
    /// <returns></returns>
    protected virtual Guid? GetRequestPrimarykey()
    {
        string key = GetRequestPrimarykeyString();
        return string.IsNullOrWhiteSpace(key)
            ? default(Guid?)
            : Guid.Parse(key);
    }

    /// <summary>
    /// 获取请求url/formbody上的Id或者key 字符串
    /// </summary>
    /// <returns></returns>
    protected virtual string GetRequestPrimarykeyString()
    {
        string key = null;
        var context = ContextAccessor.HttpContext;
        if (context != null)
        {
            if (context.Request.Method.Equals("GET"))
            {
                key = context.Request.Query["key"].Any()
                    ? context.Request.Query["key"].ToString()
                    : context.Request.Query["id"].ToString();
            }
            else
            {
                key = context.Request.Form["key"].Any()
                    ? context.Request.Form["key"].ToString()
                    : context.Request.Form["id"].ToString();
            }
        }

        return key;
    }

    /// <summary>
    /// 从httpcontext FormBody获取values对象字符串
    /// </summary>
    /// <returns></returns>
    protected virtual string GetFormValuesString()
    {
        var formData = ContextAccessor.HttpContext?.Request.Form["values"].ToString();
        //formData = formData.Replace(" ", "");//去掉所有空格
        return formData?.Trim();
    }
}
