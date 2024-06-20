using AlibabaCloud.SDK.Dm20151123.Models;
using Microsoft.Extensions.Options;
using Common.Dtos;
using Parakeet.Net.ServiceGroup.AlibabaSdk;
using System;
using System.Collections.Generic;
using Common.Emails;
using Tea;

namespace Parakeet.Net.Emails
{
    /// <summary>
    /// 阿里云邮件服务
    /// https://dm.console.aliyun.com/#/directmail/Home/cn-hangzhou
    /// </summary>
    public class AlibabaCloudSdkAppService : CustomerAppService, IAlibabaCloudSdkAppService
    {
        private AlibabaSdkOption _option;
        public AlibabaCloudSdkAppService(IOptionsMonitor<AlibabaSdkOption> optionsMonitor)
        {
            _option = optionsMonitor.CurrentValue;
        }

        #region 获取客户端

        /// <summary>
        /// 获取阿里云邮件服务客户端
        /// </summary>
        /// <returns></returns>
        AlibabaCloud.SDK.Dm20151123.Client GetAlibabaDmClient()
        {
            AlibabaCloud.OpenApiClient.Models.Config config = new AlibabaCloud.OpenApiClient.Models.Config
            {
                // 您的 AccessKey ID
                AccessKeyId = _option.AppKey,
                // 您的 AccessKey Secret
                AccessKeySecret = _option.AppSecret,
                // 访问的域名
                Endpoint = _option.ServerUrl//"dm.aliyuncs.com"//
            };
            return new AlibabaCloud.SDK.Dm20151123.Client(config);
        }

        #endregion

        #region 账号测试
        /// <summary>
        /// 阿里ak-sk账号测试 获取账户信息
        /// </summary>
        public ResponseWrapper<object> AccountTest()
        {
            var result = new ResponseWrapper<object>();
            var client = GetAlibabaDmClient();
            var descAccountSummaryRequest = new AlibabaCloud.SDK.Dm20151123.Models.DescAccountSummaryRequest();

            var runtime = new AlibabaCloud.TeaUtil.Models.RuntimeOptions
            {
                IgnoreSSL = true,//忽略https证书错误
                Autoretry = true,//自动重试
                MaxAttempts = 3 //最大重试次数
            };
            try
            {
                var response = client.DescAccountSummaryWithOptions(descAccountSummaryRequest, runtime);
                Console.WriteLine(response?.StatusCode == 200
                    ? $"response.StatusCode:{response.StatusCode}"
                    : $"response.StatusCode:{response?.StatusCode}_response.Body.RequestId:{response?.Body.RequestId}");
                result.Code = response?.StatusCode ?? 0;
                result.Data = response;
            }
            catch (TeaException error)
            {
                result.Message = error.Message;
                result.Data = error;
                AlibabaCloud.TeaUtil.Common.AssertAsString(error.Message);
            }

            return result;
        }
        #endregion

        #region 域名

        /// <summary>
        /// 创建域名
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResponseWrapper<object> CreateDomain(DomainConfigDto input)
        {
            var result = new ResponseWrapper<object>();
            var client = GetAlibabaDmClient();
            var createDomainRequest = new CreateDomainRequest
            {
                DomainName = input.DomainName //"aksoinfo.com"
            };
            var runtime = new AlibabaCloud.TeaUtil.Models.RuntimeOptions
            {
                ReadTimeout = 60000,
                ConnectTimeout = 10000,
                IgnoreSSL = true,//忽略https证书错误
                Autoretry = true,//自动重试
                MaxAttempts = 3 //最大重试次数
            };
            try
            {
                var response = client.CreateDomainWithOptions(createDomainRequest, runtime);
                Console.WriteLine(response?.StatusCode == 200
                    ? $"response.StatusCode:{response.StatusCode}"
                    : $"response.StatusCode:{response?.StatusCode}_response.Body.RequestId:{response?.Body.RequestId}");
                //返回数据格式
                //"DomainId": 357402,
                //"RequestId": "7D077FDD-71F2-5294-980C-678A9DFC6402"
                result.Code = response?.StatusCode ?? 0;
                result.Data = response;
                //input.DomainId = response.Body.DomainId;
            }
            catch (TeaException error)
            {
                result.Message = error.Message;
                result.Data = error;
                AlibabaCloud.TeaUtil.Common.AssertAsString(error.Message);
            }
            //catch (Exception ex)
            //{
            //    var error = new TeaException(new Dictionary<string, object>
            //    {
            //        { "message", ex.Message }
            //    });
            //    AlibabaCloud.TeaUtil.Common.AssertAsString(error.Message);
            //}

            return result;
        }

        /// <summary>
        /// 根据关键字查询域名
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResponseWrapper<object> QueryDomainByParam(QueryDomainByParamDto input)
        {
            var result = new ResponseWrapper<object>();
            var client = GetAlibabaDmClient();
            var queryDomainByParamRequest = new AlibabaCloud.SDK.Dm20151123.Models.QueryDomainByParamRequest
            {
                KeyWord = input.KeyWord,
                Status = input.Status
            };
            var runtime = new AlibabaCloud.TeaUtil.Models.RuntimeOptions
            {
                ReadTimeout = 60000,
                ConnectTimeout = 10000,
                IgnoreSSL = true,//忽略https证书错误
                Autoretry = true,//自动重试
                MaxAttempts = 3  //最大重试次数
            };
            try
            {
                var response = client.QueryDomainByParamWithOptions(queryDomainByParamRequest, runtime);
                Console.WriteLine(response?.StatusCode == 200
                    ? $"response.StatusCode:{response.StatusCode}"
                    : $"response.StatusCode:{response?.StatusCode}_response.Body.RequestId:{response?.Body.RequestId}");
                result.Code = response?.StatusCode ?? 0;
                result.Data = response;
            }
            catch (TeaException error)
            {
                result.Message = error.Message;
                result.Data = error;
                AlibabaCloud.TeaUtil.Common.AssertAsString(error.Message);
            }

            return result;
        }


        /// <summary>
        /// 删除域名
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResponseWrapper<object> DeleteDomain(DomainConfigDto input)
        {
            var result = new ResponseWrapper<object>();
            var client = GetAlibabaDmClient();
            var deleteDomainRequest = new AlibabaCloud.SDK.Dm20151123.Models.DeleteDomainRequest
            {
                DomainId = input.DomainId
            };
            var runtime = new AlibabaCloud.TeaUtil.Models.RuntimeOptions
            {
                ReadTimeout = 60000,
                ConnectTimeout = 10000,
                IgnoreSSL = true,//忽略https证书错误
                Autoretry = true,//自动重试
                MaxAttempts = 3  //最大重试次数
            };
            try
            {
                var response = client.DeleteDomainWithOptions(deleteDomainRequest, runtime);
                Console.WriteLine(response?.StatusCode == 200
                    ? $"response.StatusCode:{response.StatusCode}"
                    : $"response.StatusCode:{response?.StatusCode}_response.Body.RequestId:{response?.Body.RequestId}");
                result.Code = response?.StatusCode ?? 0;
                result.Data = response;
            }
            catch (TeaException error)
            {
                result.Message = error.Message;
                result.Data = error;
                AlibabaCloud.TeaUtil.Common.AssertAsString(error.Message);
            }
            //catch (Exception ex)
            //{
            //    var error = new TeaException(new Dictionary<string, object>
            //    {
            //        { "message", ex.Message }
            //    });
            //    AlibabaCloud.TeaUtil.Common.AssertAsString(error.Message);
            //}

            return result;
        }


        /// <summary>
        /// 验证域名
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResponseWrapper<object> CheckDomain(DomainConfigDto input)
        {
            var result = new ResponseWrapper<object>();
            var client = GetAlibabaDmClient();
            var checkDomainRequest = new AlibabaCloud.SDK.Dm20151123.Models.CheckDomainRequest
            {
                DomainId = input.DomainId
            };
            var runtime = new AlibabaCloud.TeaUtil.Models.RuntimeOptions
            {
                ReadTimeout = 60000,
                ConnectTimeout = 10000,
                IgnoreSSL = true,//忽略https证书错误
                Autoretry = true,//自动重试
                MaxAttempts = 3  //最大重试次数
            };
            try
            {
                var response = client.CheckDomainWithOptions(checkDomainRequest, runtime);
                Console.WriteLine(response?.StatusCode == 200
                    ? $"response.StatusCode:{response.StatusCode}"
                    : $"response.StatusCode:{response?.StatusCode}_response.Body.RequestId:{response?.Body.RequestId}");
                result.Code = response?.StatusCode ?? 0;
                result.Data = response;
            }
            catch (TeaException error)
            {
                result.Message = error.Message;
                result.Data = error;
                AlibabaCloud.TeaUtil.Common.AssertAsString(error.Message);
            }

            return result;
        }

        /// <summary>
        /// 配置域名
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResponseWrapper<object> DescDomain(DomainConfigDto input)
        {
            var result = new ResponseWrapper<object>();
            var client = GetAlibabaDmClient();
            var descDomainRequest = new AlibabaCloud.SDK.Dm20151123.Models.DescDomainRequest
            {
                DomainId = input.DomainId
            };
            var runtime = new AlibabaCloud.TeaUtil.Models.RuntimeOptions
            {
                ReadTimeout = 60000,
                ConnectTimeout = 10000,
                IgnoreSSL = true,//忽略https证书错误
                Autoretry = true,//自动重试
                MaxAttempts = 3  //最大重试次数
            };
            try
            {
                var response = client.DescDomainWithOptions(descDomainRequest, runtime);
                Console.WriteLine(response?.StatusCode == 200
                    ? $"response.StatusCode:{response.StatusCode}"
                    : $"response.StatusCode:{response?.StatusCode}_response.Body.RequestId:{response?.Body.RequestId}");
                result.Code = response?.StatusCode ?? 0;
                result.Data = response;
            }
            catch (TeaException error)
            {
                result.Message = error.Message;
                result.Data = error;
                AlibabaCloud.TeaUtil.Common.AssertAsString(error.Message);
            }

            return result;
        }


        #endregion

        #region 地址

        /// <summary>
        /// 创建发信地址 CreateMailAddressRequest
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResponseWrapper<object> CreateMailAddress(CreateMailAddressRequest input)
        {
            var result = new ResponseWrapper<object>();
            var client = GetAlibabaDmClient();
            var runtime = new AlibabaCloud.TeaUtil.Models.RuntimeOptions
            {
                ReadTimeout = 60000,
                ConnectTimeout = 10000,
                //HttpProxy = "192.168.1.1",//http代理
                //HttpsProxy = "192.168.1.1",//https代理
                //NoProxy = "192.168.1.1",//白名单
                IgnoreSSL = true,//忽略https证书错误
                Autoretry = true,//自动重试
                MaxAttempts = 3 //最大重试次数
            };
            try
            {
                var response = client.CreateMailAddressWithOptions(input, runtime);
                Console.WriteLine(response?.StatusCode == 200
                    ? $"response.StatusCode:{response.StatusCode}"
                    : $"response.StatusCode:{response?.StatusCode}_response.Body.RequestId:{response?.Body.RequestId}");
                result.Code = response?.StatusCode ?? 0;
                result.Data = response;
            }
            catch (TeaException error)
            {
                result.Message = error.Message;
                result.Data = error;
                AlibabaCloud.TeaUtil.Common.AssertAsString(error.Message);
            }

            return result;
        }


        /// <summary>
        /// 删除发信地址 DeleteMailAddressWithOptions
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResponseWrapper<object> DeleteMailAddress(DeleteMailAddressRequest input)
        {
            var result = new ResponseWrapper<object>();
            var client = GetAlibabaDmClient();
            var runtime = new AlibabaCloud.TeaUtil.Models.RuntimeOptions
            {
                ReadTimeout = 60000,
                ConnectTimeout = 10000,
                //HttpProxy = "192.168.1.1",//http代理
                //HttpsProxy = "192.168.1.1",//https代理
                //NoProxy = "192.168.1.1",//白名单
                IgnoreSSL = true,//忽略https证书错误
                Autoretry = true,//自动重试
                MaxAttempts = 3 //最大重试次数
            };
            try
            {
                var response = client.DeleteMailAddressWithOptions(input, runtime);
                Console.WriteLine(response?.StatusCode == 200
                    ? $"response.StatusCode:{response.StatusCode}"
                    : $"response.StatusCode:{response?.StatusCode}_response.Body.RequestId:{response?.Body.RequestId}");
                result.Code = response?.StatusCode ?? 0;
                result.Data = response;
            }
            catch (TeaException error)
            {
                result.Message = error.Message;
                result.Data = error;
                AlibabaCloud.TeaUtil.Common.AssertAsString(error.Message);
            }

            return result;
        }


        /// <summary>
        /// 设置发信地址SMTP密码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResponseWrapper<object> ModifyPWByDomain(ModifyPWByDomainRequest input)
        {
            var result = new ResponseWrapper<object>();
            var client = GetAlibabaDmClient();
            var runtime = new AlibabaCloud.TeaUtil.Models.RuntimeOptions
            {
                ReadTimeout = 60000,
                ConnectTimeout = 10000,
                IgnoreSSL = true,//忽略https证书错误
                Autoretry = true,//自动重试
                MaxAttempts = 3  //最大重试次数
            };
            try
            {
                var response = client.ModifyPWByDomainWithOptions(input, runtime);
                Console.WriteLine(response?.StatusCode == 200
                    ? $"response.StatusCode:{response.StatusCode}"
                    : $"response.StatusCode:{response?.StatusCode}_response.Body.RequestId:{response?.Body.RequestId}");
                result.Code = response?.StatusCode ?? 0;
                result.Data = response;
            }
            catch (TeaException error)
            {
                result.Message = error.Message;
                result.Data = error;
                AlibabaCloud.TeaUtil.Common.AssertAsString(error.Message);
            }

            return result;
        }

        ///// <summary>
        ///// 设置发信地址的通知地址 UpdateMailAddressMsgCallBackUrlWithOptions
        ///// </summary>
        ///// <param name="input"></param>
        ///// <returns></returns>
        //public ResponseWrapper<object> UpdateMailAddressMsgCallBackUrl(UpdateMailAddressMsgCallBackUrlRequest input)
        //{
        //    var result = new ResponseWrapper<object>();
        //    var client = GetAlibabaDmClient();
        //    var runtime = new AlibabaCloud.TeaUtil.Models.RuntimeOptions
        //    {
        //        ReadTimeout = 60000,
        //        ConnectTimeout = 10000,
        //        IgnoreSSL = true,//忽略https证书错误
        //        Autoretry = true,//自动重试
        //        MaxAttempts = 3  //最大重试次数
        //    };
        //    try
        //    {
        //        var response = client.UpdateMailAddressMsgCallBackUrlWithOptions(input, runtime);
        //        Console.WriteLine(response?.StatusCode == 200
        //            ? $"response.StatusCode:{response.StatusCode}"
        //            : $"response.StatusCode:{response?.StatusCode}_response.Body.RequestId:{response?.Body.RequestId}");
        //        result.Code = response?.StatusCode ?? 0;
        //        result.Data = response;
        //    }
        //    catch (TeaException error)
        //    {
        //        result.Message = error.Message;
        //        result.Data = error;
        //        AlibabaCloud.TeaUtil.Common.AssertAsString(error.Message);
        //    }

        //    return result;
        //}

        /// <summary>
        /// 查询无效地址 QueryInvalidAddressWithOptions
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResponseWrapper<object> QueryInvalidAddress(QueryInvalidAddressRequest input)
        {
            var result = new ResponseWrapper<object>();
            var client = GetAlibabaDmClient();
            var runtime = new AlibabaCloud.TeaUtil.Models.RuntimeOptions
            {
                ReadTimeout = 60000,
                ConnectTimeout = 10000,
                IgnoreSSL = true,//忽略https证书错误
                Autoretry = true,//自动重试
                MaxAttempts = 3  //最大重试次数
            };
            try
            {
                var response = client.QueryInvalidAddressWithOptions(input, runtime);
                Console.WriteLine(response?.StatusCode == 200
                    ? $"response.StatusCode:{response.StatusCode}"
                    : $"response.StatusCode:{response?.StatusCode}_response.Body.RequestId:{response?.Body.RequestId}");
                result.Code = response?.StatusCode ?? 0;
                result.Data = response;
            }
            catch (TeaException error)
            {
                result.Message = error.Message;
                result.Data = error;
                AlibabaCloud.TeaUtil.Common.AssertAsString(error.Message);
            }

            return result;
        }

        /// <summary>
        /// 根据参数查询邮件地址 QueryMailAddressByParamWithOptions
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResponseWrapper<object> QueryMailAddressByParam(QueryMailAddressByParamRequest input)
        {
            var result = new ResponseWrapper<object>();
            var client = GetAlibabaDmClient();
            var runtime = new AlibabaCloud.TeaUtil.Models.RuntimeOptions
            {
                ReadTimeout = 60000,
                ConnectTimeout = 10000,
                IgnoreSSL = true,//忽略https证书错误
                Autoretry = true,//自动重试
                MaxAttempts = 3  //最大重试次数
            };
            try
            {
                var response = client.QueryMailAddressByParamWithOptions(input, runtime);
                Console.WriteLine(response?.StatusCode == 200
                    ? $"response.StatusCode:{response.StatusCode}"
                    : $"response.StatusCode:{response?.StatusCode}_response.Body.RequestId:{response?.Body.RequestId}");
                result.Code = response?.StatusCode ?? 0;
                result.Data = response;
            }
            catch (TeaException error)
            {
                result.Message = error.Message;
                result.Data = error;
                AlibabaCloud.TeaUtil.Common.AssertAsString(error.Message);
            }

            return result;
        }

        /// <summary>
        /// 验证回信地址 ApproveReplyMailAddressWithOptions
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResponseWrapper<object> ApproveReplyMailAddress(ApproveReplyMailAddressRequest input)
        {
            var result = new ResponseWrapper<object>();
            var client = GetAlibabaDmClient();
            var runtime = new AlibabaCloud.TeaUtil.Models.RuntimeOptions
            {
                ReadTimeout = 60000,
                ConnectTimeout = 10000,
                IgnoreSSL = true,//忽略https证书错误
                Autoretry = true,//自动重试
                MaxAttempts = 3  //最大重试次数
            };
            try
            {
                var response = client.ApproveReplyMailAddressWithOptions(input, runtime);
                Console.WriteLine(response?.StatusCode == 200
                    ? $"response.StatusCode:{response.StatusCode}"
                    : $"response.StatusCode:{response?.StatusCode}_response.Body.RequestId:{response?.Body.RequestId}");
                result.Code = response?.StatusCode ?? 0;
                result.Data = response;
            }
            catch (TeaException error)
            {
                result.Message = error.Message;
                result.Data = error;
                AlibabaCloud.TeaUtil.Common.AssertAsString(error.Message);
            }

            return result;
        }

        /// <summary>
        /// 验证回信地址发送邮件 CheckReplyToMailAddressWithOptions
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResponseWrapper<object> CheckReplyToMailAddress(CheckReplyToMailAddressRequest input)
        {
            var result = new ResponseWrapper<object>();
            var client = GetAlibabaDmClient();
            var runtime = new AlibabaCloud.TeaUtil.Models.RuntimeOptions
            {
                ReadTimeout = 60000,
                ConnectTimeout = 10000,
                IgnoreSSL = true,//忽略https证书错误
                Autoretry = true,//自动重试
                MaxAttempts = 3  //最大重试次数
            };
            try
            {
                var response = client.CheckReplyToMailAddressWithOptions(input, runtime);
                Console.WriteLine(response?.StatusCode == 200
                    ? $"response.StatusCode:{response.StatusCode}"
                    : $"response.StatusCode:{response?.StatusCode}_response.Body.RequestId:{response?.Body.RequestId}");
                result.Code = response?.StatusCode ?? 0;
                result.Data = response;
            }
            catch (TeaException error)
            {
                result.Message = error.Message;
                result.Data = error;
                AlibabaCloud.TeaUtil.Common.AssertAsString(error.Message);
            }

            return result;
        }

        #endregion

        #region 收件人

        /// <summary>
        /// 创建收件人列表 CreateReceiverWithOptions
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResponseWrapper<object> CreateReceiver(CreateReceiverRequest input)
        {
            var result = new ResponseWrapper<object>();
            var client = GetAlibabaDmClient();
            var runtime = new AlibabaCloud.TeaUtil.Models.RuntimeOptions
            {
                ReadTimeout = 60000,
                ConnectTimeout = 10000,
                IgnoreSSL = true,//忽略https证书错误
                Autoretry = true,//自动重试
                MaxAttempts = 3  //最大重试次数
            };
            try
            {
                var response = client.CreateReceiverWithOptions(input, runtime);
                Console.WriteLine(response?.StatusCode == 200
                    ? $"response.StatusCode:{response.StatusCode}"
                    : $"response.StatusCode:{response?.StatusCode}_response.Body.RequestId:{response?.Body.RequestId}");
                result.Code = response?.StatusCode ?? 0;
                result.Data = response;
            }
            catch (TeaException error)
            {
                result.Message = error.Message;
                result.Data = error;
                AlibabaCloud.TeaUtil.Common.AssertAsString(error.Message);
            }

            return result;
        }

        /// <summary>
        /// 删除收件人列表 DeleteReceiverWithOptions
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResponseWrapper<object> DeleteReceiver(DeleteReceiverRequest input)
        {
            var result = new ResponseWrapper<object>();
            var client = GetAlibabaDmClient();
            var runtime = new AlibabaCloud.TeaUtil.Models.RuntimeOptions
            {
                ReadTimeout = 60000,
                ConnectTimeout = 10000,
                IgnoreSSL = true,//忽略https证书错误
                Autoretry = true,//自动重试
                MaxAttempts = 3  //最大重试次数
            };
            try
            {
                var response = client.DeleteReceiverWithOptions(input, runtime);
                Console.WriteLine(response?.StatusCode == 200
                    ? $"response.StatusCode:{response.StatusCode}"
                    : $"response.StatusCode:{response?.StatusCode}_response.Body.RequestId:{response?.Body.RequestId}");
                result.Code = response?.StatusCode ?? 0;
                result.Data = response;
            }
            catch (TeaException error)
            {
                result.Message = error.Message;
                result.Data = error;
                AlibabaCloud.TeaUtil.Common.AssertAsString(error.Message);
            }

            return result;
        }

        /// <summary>
        /// 删除单个收件人 DeleteReceiverDetailWithOptions
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResponseWrapper<object> DeleteReceiverDetail(DeleteReceiverDetailRequest input)
        {
            var result = new ResponseWrapper<object>();
            var client = GetAlibabaDmClient();
            var runtime = new AlibabaCloud.TeaUtil.Models.RuntimeOptions
            {
                ReadTimeout = 60000,
                ConnectTimeout = 10000,
                IgnoreSSL = true,//忽略https证书错误
                Autoretry = true,//自动重试
                MaxAttempts = 3  //最大重试次数
            };
            try
            {
                var response = client.DeleteReceiverDetailWithOptions(input, runtime);
                Console.WriteLine(response?.StatusCode == 200
                    ? $"response.StatusCode:{response.StatusCode}"
                    : $"response.StatusCode:{response?.StatusCode}_response.Body.RequestId:{response?.Body.RequestId}");
                result.Code = response?.StatusCode ?? 0;
                result.Data = response;
            }
            catch (TeaException error)
            {
                result.Message = error.Message;
                result.Data = error;
                AlibabaCloud.TeaUtil.Common.AssertAsString(error.Message);
            }

            return result;
        }

        /// <summary>
        /// 查询收件人列表 QueryReceiverByParamWithOptions
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResponseWrapper<object> QueryReceiverByParam(QueryReceiverByParamRequest input)
        {
            var result = new ResponseWrapper<object>();
            var client = GetAlibabaDmClient();
            var runtime = new AlibabaCloud.TeaUtil.Models.RuntimeOptions
            {
                ReadTimeout = 60000,
                ConnectTimeout = 10000,
                IgnoreSSL = true,//忽略https证书错误
                Autoretry = true,//自动重试
                MaxAttempts = 3  //最大重试次数
            };
            try
            {
                var response = client.QueryReceiverByParamWithOptions(input, runtime);
                Console.WriteLine(response?.StatusCode == 200
                    ? $"response.StatusCode:{response.StatusCode}"
                    : $"response.StatusCode:{response?.StatusCode}_response.Body.RequestId:{response?.Body.RequestId}");
                result.Code = response?.StatusCode ?? 0;
                result.Data = response;
            }
            catch (TeaException error)
            {
                result.Message = error.Message;
                result.Data = error;
                AlibabaCloud.TeaUtil.Common.AssertAsString(error.Message);
            }

            return result;
        }

        /// <summary>
        /// 查询收件人 QueryReceiverDetailWithOptions
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResponseWrapper<object> QueryReceiverDetail(QueryReceiverDetailRequest input)
        {
            var result = new ResponseWrapper<object>();
            var client = GetAlibabaDmClient();
            var runtime = new AlibabaCloud.TeaUtil.Models.RuntimeOptions
            {
                ReadTimeout = 60000,
                ConnectTimeout = 10000,
                IgnoreSSL = true,//忽略https证书错误
                Autoretry = true,//自动重试
                MaxAttempts = 3  //最大重试次数
            };
            try
            {
                var response = client.QueryReceiverDetailWithOptions(input, runtime);
                Console.WriteLine(response?.StatusCode == 200
                    ? $"response.StatusCode:{response.StatusCode}"
                    : $"response.StatusCode:{response?.StatusCode}_response.Body.RequestId:{response?.Body.RequestId}");
                result.Code = response?.StatusCode ?? 0;
                result.Data = response;
            }
            catch (TeaException error)
            {
                result.Message = error.Message;
                result.Data = error;
                AlibabaCloud.TeaUtil.Common.AssertAsString(error.Message);
            }

            return result;
        }

        /// <summary>
        /// 创建单收件人 SaveReceiverDetailWithOptions
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResponseWrapper<object> SaveReceiverDetail(SaveReceiverDetailRequest input)
        {
            var result = new ResponseWrapper<object>();
            var client = GetAlibabaDmClient();
            var runtime = new AlibabaCloud.TeaUtil.Models.RuntimeOptions
            {
                ReadTimeout = 60000,
                ConnectTimeout = 10000,
                IgnoreSSL = true,//忽略https证书错误
                Autoretry = true,//自动重试
                MaxAttempts = 3  //最大重试次数
            };
            try
            {
                var response = client.SaveReceiverDetailWithOptions(input, runtime);
                Console.WriteLine(response?.StatusCode == 200
                    ? $"response.StatusCode:{response.StatusCode}"
                    : $"response.StatusCode:{response?.StatusCode}_response.Body.RequestId:{response?.Body.RequestId}");
                result.Code = response?.StatusCode ?? 0;
                result.Data = response;
            }
            catch (TeaException error)
            {
                result.Message = error.Message;
                result.Data = error;
                AlibabaCloud.TeaUtil.Common.AssertAsString(error.Message);
            }

            return result;
        }

        #endregion

        #region 标签

        /// <summary>
        /// 创建标签 CreateTagWithOptions
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResponseWrapper<object> CreateTag(CreateTagRequest input)
        {
            var result = new ResponseWrapper<object>();
            var client = GetAlibabaDmClient();
            var runtime = new AlibabaCloud.TeaUtil.Models.RuntimeOptions
            {
                ReadTimeout = 60000,
                ConnectTimeout = 10000,
                IgnoreSSL = true,//忽略https证书错误
                Autoretry = true,//自动重试
                MaxAttempts = 3  //最大重试次数
            };
            try
            {
                var response = client.CreateTagWithOptions(input, runtime);
                Console.WriteLine(response?.StatusCode == 200
                    ? $"response.StatusCode:{response.StatusCode}"
                    : $"response.StatusCode:{response?.StatusCode}_response.Body.RequestId:{response?.Body.RequestId}");
                result.Code = response?.StatusCode ?? 0;
                result.Data = response;
            }
            catch (TeaException error)
            {
                result.Message = error.Message;
                result.Data = error;
                AlibabaCloud.TeaUtil.Common.AssertAsString(error.Message);
            }

            return result;
        }

        /// <summary>
        /// 删除标签 DeleteTagWithOptions
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResponseWrapper<object> DeleteTag(DeleteTagRequest input)
        {
            var result = new ResponseWrapper<object>();
            var client = GetAlibabaDmClient();
            var runtime = new AlibabaCloud.TeaUtil.Models.RuntimeOptions
            {
                ReadTimeout = 60000,
                ConnectTimeout = 10000,
                IgnoreSSL = true,//忽略https证书错误
                Autoretry = true,//自动重试
                MaxAttempts = 3  //最大重试次数
            };
            try
            {
                var response = client.DeleteTagWithOptions(input, runtime);
                Console.WriteLine(response?.StatusCode == 200
                    ? $"response.StatusCode:{response.StatusCode}"
                    : $"response.StatusCode:{response?.StatusCode}_response.Body.RequestId:{response?.Body.RequestId}");
                result.Code = response?.StatusCode ?? 0;
                result.Data = response;
            }
            catch (TeaException error)
            {
                result.Message = error.Message;
                result.Data = error;
                AlibabaCloud.TeaUtil.Common.AssertAsString(error.Message);
            }

            return result;
        }

        /// <summary>
        /// 修改标签 ModifyTagWithOptions
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResponseWrapper<object> ModifyTag(ModifyTagRequest input)
        {
            var result = new ResponseWrapper<object>();
            var client = GetAlibabaDmClient();
            var runtime = new AlibabaCloud.TeaUtil.Models.RuntimeOptions
            {
                ReadTimeout = 60000,
                ConnectTimeout = 10000,
                IgnoreSSL = true,//忽略https证书错误
                Autoretry = true,//自动重试
                MaxAttempts = 3  //最大重试次数
            };
            try
            {
                var response = client.ModifyTagWithOptions(input, runtime);
                Console.WriteLine(response?.StatusCode == 200
                    ? $"response.StatusCode:{response.StatusCode}"
                    : $"response.StatusCode:{response?.StatusCode}_response.Body.RequestId:{response?.Body.RequestId}");
                result.Code = response?.StatusCode ?? 0;
                result.Data = response;
            }
            catch (TeaException error)
            {
                result.Message = error.Message;
                result.Data = error;
                AlibabaCloud.TeaUtil.Common.AssertAsString(error.Message);
            }

            return result;
        }

        /// <summary>
        /// 查询标签 QueryTagByParamWithOptions
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResponseWrapper<object> QueryTagByParam(QueryTagByParamRequest input)
        {
            var result = new ResponseWrapper<object>();
            var client = GetAlibabaDmClient();
            var runtime = new AlibabaCloud.TeaUtil.Models.RuntimeOptions
            {
                ReadTimeout = 60000,
                ConnectTimeout = 10000,
                IgnoreSSL = true,//忽略https证书错误
                Autoretry = true,//自动重试
                MaxAttempts = 3  //最大重试次数
            };
            try
            {
                var response = client.QueryTagByParamWithOptions(input, runtime);
                Console.WriteLine(response?.StatusCode == 200
                    ? $"response.StatusCode:{response.StatusCode}"
                    : $"response.StatusCode:{response?.StatusCode}_response.Body.RequestId:{response?.Body.RequestId}");
                result.Code = response?.StatusCode ?? 0;
                result.Data = response;
            }
            catch (TeaException error)
            {
                result.Message = error.Message;
                result.Data = error;
                AlibabaCloud.TeaUtil.Common.AssertAsString(error.Message);
            }

            return result;
        }

        #endregion

        #region 推送邮件

        /// <summary>
        /// 推送邮件
        /// </summary>
        public ResponseWrapper<object> SendEmail()
        {
            var result = new ResponseWrapper<object>();
            try
            {
                var client = GetAlibabaDmClient();
                var runtime = new AlibabaCloud.TeaUtil.Models.RuntimeOptions
                {
                    ReadTimeout = 60000,
                    ConnectTimeout = 10000,
                    //HttpProxy = "192.168.1.1",//http代理
                    //HttpsProxy = "192.168.1.1",//https代理
                    //NoProxy = "192.168.1.1",//白名单
                    IgnoreSSL = true,//忽略https证书错误
                    Autoretry = true,//自动重试
                    MaxAttempts = 3 //最大重试次数
                };
                //single 
                var toAddresses = new List<string>
                {
                    "chenshuangquan@aksoinfo.com",
                    "290668617@qq.com"
                };
                foreach (var toAddress in toAddresses)
                {
                    var singleSendMailRequest = new SingleSendMailRequest
                    {
                        AccountName = "egmp@email-test.aksoegmp.com",
                        AddressType = 1,//0：为随机账号 1：为发信地址
                        ReplyToAddress = true,
                        ToAddress = toAddress,
                        Subject = "subject",
                        HtmlBody = "htmlbody",
                        TextBody = "textbody",
                        FromAlias = "阿克索",
                        ClickTrace = "1",
                        ReplyAddress = "290668617@qq.com",//"chenshuangquan@aksoinfo.com",//"egmp@aksoinfo.com",//
                        ReplyAddressAlias = "Akso"
                        //TagName = "ParakeetEmailTarget",//提前创建好的标签名
                    };
                    var response = client.SingleSendMailWithOptions(singleSendMailRequest, runtime);
                    //var response = client.BatchSendMailWithOptions(batchSendMailRequest, runtime);
                    Console.WriteLine(response?.StatusCode == 200
                        ? $"response.StatusCode:{response.StatusCode}"
                        : $"response.StatusCode:{response?.StatusCode}_response.Body.RequestId:{response?.Body.RequestId}");
                    result.Code = response?.StatusCode ?? 0;
                    result.Data = response;
                }
            }
            catch (TeaException error)
            {
                result.Message = error.Message;
                result.Data = error;
                AlibabaCloud.TeaUtil.Common.AssertAsString(error.Message);
            }

            return result;
        }

        /// <summary>
        /// 推送邮件 Single
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResponseWrapper<object> SendSingleEmail(SingleSendMailRequest input)
        {
            var result = new ResponseWrapper<object>();
            var client = GetAlibabaDmClient();
            //var singleSendMailRequest = new AlibabaCloud.SDK.Dm20151123.Models.SingleSendMailRequest
            //{
            //    AccountName = "egmp@email-test.aksoegmp.com",
            //    AddressType = 1,//0：为随机账号 1：为发信地址
            //    ReplyToAddress = true,
            //    //ToAddress = "290668617@qq.com",//qq会发送到垃圾箱
            //    ToAddress = "chenshuangquan@aksoinfo.com",
            //    Subject = "subject",
            //    HtmlBody = "htmlbody",
            //    TextBody = "textbody",
            //    FromAlias = "Akso",
            //    ClickTrace = "1",
            //    ReplyAddress = "chenshuangquan@aksoinfo.com",
            //    ReplyAddressAlias = "回信地址别名",
            //    TagName = "ParakeetEmailTarget",//提前创建好的标签名
            //};
            var runtime = new AlibabaCloud.TeaUtil.Models.RuntimeOptions
            {
                ReadTimeout = 60000,
                ConnectTimeout = 10000,
                //HttpProxy = "192.168.1.1",//http代理
                //HttpsProxy = "192.168.1.1",//https代理
                //NoProxy = "192.168.1.1",//白名单
                IgnoreSSL = true,//忽略https证书错误
                Autoretry = true,//自动重试
                MaxAttempts = 3 //最大重试次数
            };
            try
            {
                var response = client.SingleSendMailWithOptions(input, runtime);
                Console.WriteLine(response?.StatusCode == 200
                    ? $"response.StatusCode:{response.StatusCode}"
                    : $"response.StatusCode:{response?.StatusCode}_response.Body.RequestId:{response?.Body.RequestId}");
                result.Code = response?.StatusCode ?? 0;
                result.Data = response;
            }
            catch (TeaException error)
            {
                result.Message = error.Message;
                result.Data = error;
                AlibabaCloud.TeaUtil.Common.AssertAsString(error.Message);
            }

            return result;
        }

        /// <summary>
        /// 推送邮件 batch
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResponseWrapper<object> SendBatchEmail(BatchSendMailRequest input)
        {
            var result = new ResponseWrapper<object>();
            var client = GetAlibabaDmClient();
            //var batchSendMailRequest = new AlibabaCloud.SDK.Dm20151123.Models.BatchSendMailRequest
            //{
            //    TemplateName = "简明",//"预先创建且通过审核的模板名称",
            //    AccountName = "egmp@email-test.aksoegmp.com",
            //    ReceiversName = "myself",//"预先创建且上传了收件人的收件人列表名称",
            //    AddressType = 1,//0：为随机账号 1：为发信地址
            //    ClickTrace = 1.ToString(), //1：为打开数据跟踪功能 0（默认）：为关闭数据跟踪功能
            //    TagName = "ParakeetEmailTarget",//"邮件标签名称",
            //    ReplyAddress = "290668617@qq.com",//"邮件标签名称",
            //    ReplyAddressAlias = "批量回复",
            //};
            var runtime = new AlibabaCloud.TeaUtil.Models.RuntimeOptions
            {
                ReadTimeout = 60000,
                ConnectTimeout = 10000,
                //HttpProxy = "192.168.1.1",//http代理
                //HttpsProxy = "192.168.1.1",//https代理
                //NoProxy = "192.168.1.1",//白名单
                IgnoreSSL = true,//忽略https证书错误
                Autoretry = true,//自动重试
                MaxAttempts = 3 //最大重试次数
            };
            try
            {
                var response = client.BatchSendMailWithOptions(input, runtime);
                Console.WriteLine(response?.StatusCode == 200
                    ? $"response.StatusCode:{response.StatusCode}"
                    : $"response.StatusCode:{response?.StatusCode}_response.Body.RequestId:{response?.Body.RequestId}");
                result.Code = response?.StatusCode ?? 0;
                result.Data = response;
            }
            catch (TeaException error)
            {
                result.Message = error.Message;
                result.Data = error;
                AlibabaCloud.TeaUtil.Common.AssertAsString(error.Message);
            }

            return result;
        }

        /// <summary>
        /// 根据模板推送邮件 SendTestByTemplate
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResponseWrapper<object> SendTestByTemplate(SendTestByTemplateRequest input)
        {
            var result = new ResponseWrapper<object>();
            var client = GetAlibabaDmClient();
            //var batchSendMailRequest = new AlibabaCloud.SDK.Dm20151123.Models.BatchSendMailRequest
            //{
            //    TemplateName = "简明",//"预先创建且通过审核的模板名称",
            //    AccountName = "egmp@email-test.aksoegmp.com",
            //    ReceiversName = "myself",//"预先创建且上传了收件人的收件人列表名称",
            //    AddressType = 1,//0：为随机账号 1：为发信地址
            //    ClickTrace = 1.ToString(), //1：为打开数据跟踪功能 0（默认）：为关闭数据跟踪功能
            //    TagName = "ParakeetEmailTarget",//"邮件标签名称",
            //    ReplyAddress = "290668617@qq.com",//"邮件标签名称",
            //    ReplyAddressAlias = "批量回复",
            //};
            var runtime = new AlibabaCloud.TeaUtil.Models.RuntimeOptions
            {
                ReadTimeout = 60000,
                ConnectTimeout = 10000,
                //HttpProxy = "192.168.1.1",//http代理
                //HttpsProxy = "192.168.1.1",//https代理
                //NoProxy = "192.168.1.1",//白名单
                IgnoreSSL = true,//忽略https证书错误
                Autoretry = true,//自动重试
                MaxAttempts = 3 //最大重试次数
            };
            try
            {
                var response = client.SendTestByTemplateWithOptions(input, runtime);
                Console.WriteLine(response?.StatusCode == 200
                    ? $"response.StatusCode:{response.StatusCode}"
                    : $"response.StatusCode:{response?.StatusCode}_response.Body.RequestId:{response?.Body.RequestId}");
                result.Code = response?.StatusCode ?? 0;
                result.Data = response;
            }
            catch (TeaException error)
            {
                result.Message = error.Message;
                result.Data = error;
                AlibabaCloud.TeaUtil.Common.AssertAsString(error.Message);
            }

            return result;
        }

        #endregion

        #region 查询邮件

        /// <summary>
        /// 查询邮件队列任务 QueryTaskByParamWithOptions
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResponseWrapper<object> QueryTaskByParam(QueryTaskByParamRequest input)
        {
            var result = new ResponseWrapper<object>();
            var client = GetAlibabaDmClient();
            var runtime = new AlibabaCloud.TeaUtil.Models.RuntimeOptions
            {
                ReadTimeout = 60000,
                ConnectTimeout = 10000,
                IgnoreSSL = true,//忽略https证书错误
                Autoretry = true,//自动重试
                MaxAttempts = 3  //最大重试次数
            };
            try
            {
                var response = client.QueryTaskByParamWithOptions(input, runtime);
                Console.WriteLine(response?.StatusCode == 200
                    ? $"response.StatusCode:{response.StatusCode}"
                    : $"response.StatusCode:{response?.StatusCode}_response.Body.RequestId:{response?.Body.RequestId}");
                result.Code = response?.StatusCode ?? 0;
                result.Data = response;
            }
            catch (TeaException error)
            {
                result.Message = error.Message;
                result.Data = error;
                AlibabaCloud.TeaUtil.Common.AssertAsString(error.Message);
            }

            return result;
        }


        /// <summary>
        /// 统计 SenderStatisticsByTagNameAndBatchIDWithOptions
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResponseWrapper<object> StatisticsByTagNameAndBatchRequest(SenderStatisticsByTagNameAndBatchIDRequest input)
        {
            var result = new ResponseWrapper<object>();
            var client = GetAlibabaDmClient();
            var runtime = new AlibabaCloud.TeaUtil.Models.RuntimeOptions
            {
                ReadTimeout = 60000,
                ConnectTimeout = 10000,
                IgnoreSSL = true,//忽略https证书错误
                Autoretry = true,//自动重试
                MaxAttempts = 3  //最大重试次数
            };
            try
            {
                var response = client.SenderStatisticsByTagNameAndBatchIDWithOptions(input, runtime);
                Console.WriteLine(response?.StatusCode == 200
                    ? $"response.StatusCode:{response.StatusCode}"
                    : $"response.StatusCode:{response?.StatusCode}_response.Body.RequestId:{response?.Body.RequestId}");
                result.Code = response?.StatusCode ?? 0;
                result.Data = response;
            }
            catch (TeaException error)
            {
                result.Message = error.Message;
                result.Data = error;
                AlibabaCloud.TeaUtil.Common.AssertAsString(error.Message);
            }

            return result;
        }

        /// <summary>
        /// 统计 SenderStatisticsDetailByParamWithOptions
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResponseWrapper<object> SenderStatisticsDetailByParamWithOptions(SenderStatisticsDetailByParamRequest input)
        {
            var result = new ResponseWrapper<object>();
            var client = GetAlibabaDmClient();
            var runtime = new AlibabaCloud.TeaUtil.Models.RuntimeOptions
            {
                ReadTimeout = 60000,
                ConnectTimeout = 10000,
                IgnoreSSL = true,//忽略https证书错误
                Autoretry = true,//自动重试
                MaxAttempts = 3  //最大重试次数
            };
            try
            {
                var response = client.SenderStatisticsDetailByParamWithOptions(input, runtime);
                Console.WriteLine(response?.StatusCode == 200
                    ? $"response.StatusCode:{response.StatusCode}"
                    : $"response.StatusCode:{response?.StatusCode}_response.Body.RequestId:{response?.Body.RequestId}");
                result.Code = response?.StatusCode ?? 0;
                result.Data = response;
            }
            catch (TeaException error)
            {
                result.Message = error.Message;
                result.Data = error;
                AlibabaCloud.TeaUtil.Common.AssertAsString(error.Message);
            }

            return result;
        }

        /// <summary>
        /// 追踪 GetTrackListRequest
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResponseWrapper<object> GetTrackListRequest(GetTrackListRequest input)
        {
            var result = new ResponseWrapper<object>();
            var client = GetAlibabaDmClient();
            var runtime = new AlibabaCloud.TeaUtil.Models.RuntimeOptions
            {
                ReadTimeout = 60000,
                ConnectTimeout = 10000,
                IgnoreSSL = true,//忽略https证书错误
                Autoretry = true,//自动重试
                MaxAttempts = 3  //最大重试次数
            };
            try
            {
                var response = client.GetTrackListWithOptions(input, runtime);
                Console.WriteLine(response?.StatusCode == 200
                    ? $"response.StatusCode:{response.StatusCode}"
                    : $"response.StatusCode:{response?.StatusCode}_response.Body.RequestId:{response?.Body.RequestId}");
                result.Code = response?.StatusCode ?? 0;
                result.Data = response;
            }
            catch (TeaException error)
            {
                result.Message = error.Message;
                result.Data = error;
                AlibabaCloud.TeaUtil.Common.AssertAsString(error.Message);
            }

            return result;
        }

        /// <summary>
        /// 追踪 GetTrackListRequest
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResponseWrapper<object> GetTrackListByMailFromAndTagNameWithOptions(GetTrackListByMailFromAndTagNameRequest input)
        {
            var result = new ResponseWrapper<object>();
            var client = GetAlibabaDmClient();
            var runtime = new AlibabaCloud.TeaUtil.Models.RuntimeOptions
            {
                ReadTimeout = 60000,
                ConnectTimeout = 10000,
                IgnoreSSL = true,//忽略https证书错误
                Autoretry = true,//自动重试
                MaxAttempts = 3  //最大重试次数
            };
            try
            {
                var response = client.GetTrackListByMailFromAndTagNameWithOptions(input, runtime);
                Console.WriteLine(response?.StatusCode == 200
                    ? $"response.StatusCode:{response.StatusCode}"
                    : $"response.StatusCode:{response?.StatusCode}_response.Body.RequestId:{response?.Body.RequestId}");
                result.Code = response?.StatusCode ?? 0;
                result.Data = response;
            }
            catch (TeaException error)
            {
                result.Message = error.Message;
                result.Data = error;
                AlibabaCloud.TeaUtil.Common.AssertAsString(error.Message);
            }

            return result;
        }


        #endregion
    }
}
