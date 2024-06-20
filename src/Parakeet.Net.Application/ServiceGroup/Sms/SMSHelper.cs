﻿namespace Parakeet.Net.ServiceGroup.Sms
{
    /// <summary>
    /// 发送短信帮助类
    /// 目前资料系统没有使用该帮助类
    /// 发送短信是统一调用SpiderBim的SSO短信相关接口
    /// </summary>
    public class SMSHelper
    {
        public static bool SendSMS(string phone, string message)
        {
            //string product = "Dysmsapi";//短信API产品名称（短信产品名固定，无需修改）
            //string domain = "dysmsapi.aliyuncs.com";//短信API产品域名（接口地址固定，无需修改）
            //string accessKeyId = "LTAIMq0TMsE0m4Qq";//你的accessKeyId，参考本文档步骤2
            //string accessKeySecret = "";//你的accessKeySecret，参考本文档步骤2
            //IClientProfile profile = DefaultProfile.GetProfile("cn-hangzhou", accessKeyId, accessKeySecret);
            //// IAcsClient client = new DefaultAcsClient(profile);
            //// SingleSendSmsRequest request = new SingleSendSmsRequest();
            ////初始化ascClient,暂时不支持多region（请勿修改）
            //DefaultProfile.AddEndpoint("cn-hangzhou", "cn-hangzhou", product, domain);
            //IAcsClient acsClient = new DefaultAcsClient(profile);
            //SendSmsRequest request = new SendSmsRequest();
            //try
            //{
            //    //必填:待发送手机号。支持以逗号分隔的形式进行批量调用，批量上限为1000个手机号码
            //    //批量调用相对于单条调用及时性稍有延迟,验证码类型的短信推荐使用单条调用的方式，
            //    //发送国际 /港澳台消息时，接收号码格式为00+国际区号+号码，如“0085200000000”
            //    request.PhoneNumbers = phone;
            //    //必填:短信签名-可在短信控制台中找到
            //    request.SignName = "SpiderBim";
            //    //必填:短信模板-可在短信控制台中找到，发送国际/港澳台消息时，请使用国际/港澳台短信模版
            //    request.TemplateCode = "SMS_152513207";
            //    //可选:模板中的变量替换JSON串,如模板内容为"亲爱的${name},您的验证码为${code}"时,此处的值为
            //    request.TemplateParam = "{\"code\":\"" + message + "\"}";
            //    //可选:outId为提供给业务方扩展字段,最终在短信回执消息中将此值带回给调用者
            //    //request.OutId = "yourOutId";
            //    //请求失败这里会抛ClientException异常
            //    SendSmsResponse sendSmsResponse = acsClient.GetAcsResponse(request);
            //    if (sendSmsResponse.Code == "OK")
            //    {
            //        return true;
            //    }
            //    //将短信发送频率限制在正常的业务流控范围内，
            //    //默认流控：短信验证码 ：使用同一个签名，对同一个手机号码发送短信验证码，支持1条 / 分钟，5条 / 小时 ，累计10条 / 天。

            //    Console.WriteLine(sendSmsResponse.Message);
            //}
            //catch (ServerException ex)
            //{
            //    Console.WriteLine(ex.Message);
            //    return false;
            //}
            //catch (ClientException ex)
            //{
            //    Console.WriteLine(ex.Message);
            //    return false;
            //}

            return false;
        }
    }
}
