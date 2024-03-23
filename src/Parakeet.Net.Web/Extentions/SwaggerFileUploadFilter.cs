using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parakeet.Net.Extentions
{
    /// <summary>
    /// 上传文件参数封装 让swagger支持文件上传(新版本本来就支持了)
    /// </summary>
    public class SwaggerFileUploadFilter : IOperationFilter
    {
        #region 新版本
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (!context.ApiDescription.HttpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase) &&
                !context.ApiDescription.HttpMethod.Equals("PUT", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var fileParameters = context.ApiDescription.ActionDescriptor.Parameters
                .Where(n => n.ParameterType == typeof(IFormFile) ||
                            n.BindingInfo != null && n.BindingInfo.BindingSource == BindingSource.FormFile).ToList();

            if (fileParameters.Count == 0)
            {
                return;
            }

            //设置form-data 待测试
            operation.RequestBody.Content.Values
                .SelectMany(m => m.Encoding).ToList()
                .Add(new KeyValuePair<string, OpenApiEncoding>
                    (OpenApiConstants.Consumes, new OpenApiEncoding { ContentType = "multipart/form-data" }));

            //老版本写法:
            //operation.Consumes.Add("multipart/form-data");

            foreach (var fileParameter in fileParameters)
            {
                var parameter = operation.Parameters.Single(n => n.Name == fileParameter.Name);
                operation.Parameters.Remove(parameter);
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = parameter.Name,
                    Description = parameter.Description,
                    Required = parameter.Required
                });
            }
        }
        #endregion

        #region 老版本

        //public void Apply(Operation operation, OperationFilterContext context)
        //{
        //    if (!context.ApiDescription.HttpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase) &&
        //        !context.ApiDescription.HttpMethod.Equals("PUT", StringComparison.OrdinalIgnoreCase))
        //    {
        //        return;
        //    }

        //    try
        //    {
        //        var fileParameters = context.ApiDescription.ActionDescriptor.Parameters
        //            .Where(n => n.ParameterType == typeof(IFormFile) ||
        //                        (n.BindingInfo != null && n.BindingInfo.BindingSource == BindingSource.FormFile))
        //            .ToList();

        //        if (fileParameters.Count < 0)
        //        {
        //            return;
        //        }

        //        operation.Consumes.Add("multipart/form-data");

        //        foreach (var fileParameter in fileParameters)
        //        {
        //            var parameter = operation.Parameters.Single(n => n.Name == fileParameter.Name);
        //            operation.Parameters.Remove(parameter);
        //            operation.Parameters.Add(new NonBodyParameter
        //            {
        //                Name = parameter.Name,
        //                In = "formData",
        //                Description = parameter.Description,
        //                Required = parameter.Required,
        //                Type = "file"
        //            });
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //        throw;
        //    }
        //}

        #endregion
    }
}
