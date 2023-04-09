using System.Collections.Generic;
using System.ComponentModel;
using Parakeet.Net.ValueObjects;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 邮件Dto
    /// </summary>
    [Description("邮件")]
    public class MailDto
    {
        public MailDto(string host, string userName, string license, string from, string to, int port = 587)
        {
            Host = host;
            UserName = userName;
            License = license;
            From = from;
            To = to;
            Port = port;
        }

        /// <summary>
        /// 发件人
        /// </summary>
        [Description("发件人")]
        public string From { get; set; }

        /// <summary>
        /// 收件人
        /// </summary>
        [Description("收件人")]
        public string To { get; set; }

        /// <summary>
        /// 抄送人
        /// </summary>
        [Description("抄送人")]
        public string Cc { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [Description("标题")]
        public string Subject { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [Description("内容")]
        public string Body { get; set; }

        /// <summary>
        /// 内容底部
        /// </summary>
        [Description("内容底部")]
        public string Footer { get; set; }

        /// <summary>
        /// 服务器
        /// </summary>
        [Description("服务器")]
        public string Host { get; set; }

        /// <summary>
        /// 邮件服务端口
        /// </summary>
        [Description("邮件服务端口")]
        public int Port { get; set; }

        /// <summary>
        /// 邮件用户名
        /// </summary>
        [Description("邮件用户名")]
        public string UserName { get; set; }

        /// <summary>
        /// 邮件密码
        /// </summary>
        [Description("邮件密码")]
        public string License { get; set; }
        
        /// <summary>
        /// 是否ssl加密
        /// </summary>
        [Description("是否ssl加密")]
        public bool EnableSsl { get; set; }

        /// <summary>
        /// 附件
        /// </summary>
        [Description("附件")]
        public virtual List<Attachment> Attachments { get; set; } = new List<Attachment>();

    }
}
