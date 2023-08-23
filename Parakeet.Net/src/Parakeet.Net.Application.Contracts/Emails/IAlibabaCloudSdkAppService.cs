using AlibabaCloud.SDK.Dm20151123.Models;
using Parakeet.Net.Dtos;

namespace Parakeet.Net.Emails
{
    /// <summary>
    /// 自定义阿里云邮件服务
    /// </summary>
    public interface IAlibabaCloudSdkAppService
    {
        #region 账号测试

        /// <summary>
        /// 阿里ak-sk账号测试
        /// </summary>
        public ResponseWrapper<object> AccountTest();

        #endregion

        #region 域名

        /// <summary>
        /// 创建域名
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResponseWrapper<object> CreateDomain(DomainConfigDto input);

        /// <summary>
        /// 根据关键字查询域名
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResponseWrapper<object> QueryDomainByParam(QueryDomainByParamDto input);


        /// <summary>
        /// 删除域名
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResponseWrapper<object> DeleteDomain(DomainConfigDto input);


        /// <summary>
        /// 验证域名
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResponseWrapper<object> CheckDomain(DomainConfigDto input);

        /// <summary>
        /// 配置域名
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResponseWrapper<object> DescDomain(DomainConfigDto input);


        #endregion

        #region 地址

        /// <summary>
        /// 创建发信地址 CreateMailAddressRequest
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResponseWrapper<object> CreateMailAddress(CreateMailAddressRequest input);


        /// <summary>
        /// 删除发信地址 DeleteMailAddressWithOptions
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResponseWrapper<object> DeleteMailAddress(DeleteMailAddressRequest input);


        /// <summary>
        /// 设置发信地址SMTP密码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResponseWrapper<object> ModifyPWByDomain(ModifyPWByDomainRequest input);

        /// <summary>
        /// 设置发信地址的通知地址 UpdateMailAddressMsgCallBackUrlWithOptions
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResponseWrapper<object> UpdateMailAddressMsgCallBackUrl(UpdateMailAddressMsgCallBackUrlRequest input);

        /// <summary>
        /// 查询无效地址 QueryInvalidAddressWithOptions
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResponseWrapper<object> QueryInvalidAddress(QueryInvalidAddressRequest input);

        /// <summary>
        /// 根据参数查询邮件地址 QueryMailAddressByParamWithOptions
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResponseWrapper<object> QueryMailAddressByParam(QueryMailAddressByParamRequest input);

        /// <summary>
        /// 验证回信地址 ApproveReplyMailAddressWithOptions
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResponseWrapper<object> ApproveReplyMailAddress(ApproveReplyMailAddressRequest input);

        /// <summary>
        /// 验证回信地址发送邮件 CheckReplyToMailAddressWithOptions
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResponseWrapper<object> CheckReplyToMailAddress(CheckReplyToMailAddressRequest input);

        #endregion

        #region 收件人

        /// <summary>
        /// 创建收件人列表 CreateReceiverWithOptions
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResponseWrapper<object> CreateReceiver(CreateReceiverRequest input);

        /// <summary>
        /// 删除收件人列表 DeleteReceiverWithOptions
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResponseWrapper<object> DeleteReceiver(DeleteReceiverRequest input);

        /// <summary>
        /// 删除单个收件人 DeleteReceiverDetailWithOptions
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResponseWrapper<object> DeleteReceiverDetail(DeleteReceiverDetailRequest input);

        /// <summary>
        /// 查询收件人列表 QueryReceiverByParamWithOptions
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResponseWrapper<object> QueryReceiverByParam(QueryReceiverByParamRequest input);

        /// <summary>
        /// 查询收件人 QueryReceiverDetailWithOptions
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResponseWrapper<object> QueryReceiverDetail(QueryReceiverDetailRequest input);

        /// <summary>
        /// 创建单收件人 SaveReceiverDetailWithOptions
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResponseWrapper<object> SaveReceiverDetail(SaveReceiverDetailRequest input);

        #endregion

        #region 标签

        /// <summary>
        /// 创建标签 CreateTagWithOptions
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResponseWrapper<object> CreateTag(CreateTagRequest input);

        /// <summary>
        /// 删除标签 DeleteTagWithOptions
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResponseWrapper<object> DeleteTag(DeleteTagRequest input);

        /// <summary>
        /// 修改标签 ModifyTagWithOptions
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResponseWrapper<object> ModifyTag(ModifyTagRequest input);

        /// <summary>
        /// 查询标签 QueryTagByParamWithOptions
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResponseWrapper<object> QueryTagByParam(QueryTagByParamRequest input);

        #endregion

        #region 推送邮件

        /// <summary>
        /// 推送邮件 OnlyTest
        /// </summary>
        public ResponseWrapper<object> SendEmail();

        /// <summary>
        /// 推送邮件 Single
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResponseWrapper<object> SendSingleEmail(SingleSendMailRequest input);

        /// <summary>
        /// 推送邮件 batch
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResponseWrapper<object> SendBatchEmail(BatchSendMailRequest input);

        /// <summary>
        /// 根据模板推送邮件 SendTestByTemplate
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResponseWrapper<object> SendTestByTemplate(SendTestByTemplateRequest input);

        #endregion

        #region 查询邮件

        /// <summary>
        /// 查询邮件队列任务 QueryTaskByParamWithOptions
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResponseWrapper<object> QueryTaskByParam(QueryTaskByParamRequest input);


        /// <summary>
        /// 统计 SenderStatisticsByTagNameAndBatchIDWithOptions
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResponseWrapper<object> StatisticsByTagNameAndBatchRequest(
            SenderStatisticsByTagNameAndBatchIDRequest input);
        /// <summary>
        /// 统计 SenderStatisticsDetailByParamWithOptions
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResponseWrapper<object> SenderStatisticsDetailByParamWithOptions(
            SenderStatisticsDetailByParamRequest input);

        /// <summary>
        /// 追踪 GetTrackListRequest
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResponseWrapper<object> GetTrackListRequest(GetTrackListRequest input);

        /// <summary>
        /// 追踪 GetTrackListRequest
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResponseWrapper<object> GetTrackListByMailFromAndTagNameWithOptions(
            GetTrackListByMailFromAndTagNameRequest input);


        #endregion
    }
}
