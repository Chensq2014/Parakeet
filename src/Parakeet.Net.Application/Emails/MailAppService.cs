//using System;
//using Common.Dtos;
//using System.Collections.Generic;
//using System.IO;
//using System.Net;
//using System.Net.Mail;
//using System.Net.Mime;
//using System.Text;
//using Volo.Abp.Emailing;

//namespace Parakeet.Net.Emails
//{
//    /// <summary>
//    /// 最原始邮件发送  要用企业邮箱还要在垃圾箱中查看
//    /// </summary>
//    public class MailAppService //: CustomerAppService,IMailAppService
//    {
//        private MailMessage _mail;//修改private为public 调试时可以更改其属性
//        private static SmtpClient _smtp =  new SmtpClient(CustomConfigurationManager.Configuration[$"Settings:{EmailSettingNames.Smtp.Host}"], Convert.ToInt32(CustomConfigurationManager.Configuration[$"Settings:{EmailSettingNames.Smtp.Port}"]))
//        {
//            EnableSsl = true,
//            UseDefaultCredentials = false,
//            DeliveryMethod = SmtpDeliveryMethod.Network,
//            Credentials = new NetworkCredential(CustomConfigurationManager.Configuration[$"Settings:{EmailSettingNames.Smtp.UserName}"], CustomConfigurationManager.Configuration[$"Settings:{EmailSettingNames.Smtp.Password}"])
//        };

//        public MailAppService(MailDto model)
//        {
//            _mail = new MailMessage(new MailAddress(model.From), new MailAddress(model.To));
//            //_mail.To.Add(model.To);
//            _mail.CC.Add(model.Cc);
//            _mail.Subject = model.Subject;
//            _mail.Body = model.Body;
//            _mail.IsBodyHtml = true;
//            _mail.Priority = MailPriority.High;
//            AddAttachments(model.Attachments);
//            //_smtp.Credentials = new NetworkCredential(model.UserName, model.License);
//        }

//        private void AddAttachments(List<ValueObjects.Attachment> attachments)
//        {
//            foreach (var attachment in attachments)
//            {
//                AddAttachment(attachment);
//            }
//        }

//        private void AddAttachment(ValueObjects.Attachment attachment)
//        {
//            //建立邮件附件类的一个对象，语法格式为System.Net.Mail.Attachment(文件名，文件格式)
//            var myAttachment = new Attachment(attachment.Path, MediaTypeNames.Application.Octet);
//            //MIME协议下的一个对象，用以设置附件的创建时间，修改时间以及读取时间
//            var disposition = myAttachment.ContentDisposition;
//            disposition.CreationDate = File.GetCreationTime(attachment.Path);
//            disposition.ModificationDate = File.GetLastWriteTime(attachment.Path);
//            disposition.ReadDate = File.GetLastAccessTime(attachment.Path);
//            _mail.Attachments.Add(myAttachment);
//        }

//        /// <summary>
//        /// Send Mail
//        /// </summary>
//        /// <param name="model">包含发件人、收件人、抄送人、主题、附件地址的对象</param>
//        public void SendAsync(MailDto model)
//        {
//            _mail.From = new MailAddress(model.From);
//            _mail.To.Clear();
//            _mail.To.Add(model.To);
//            _mail.CC.Clear();
//            if (!string.IsNullOrWhiteSpace(model.Cc))
//            {
//                _mail.CC.Add(model.Cc);
//            }
//            _mail.Subject = model.Subject;
//            _mail.Body = model.Body;
//            _mail.IsBodyHtml = true;
//            _mail.BodyEncoding = Encoding.UTF8;
//            _mail.Priority = MailPriority.High;
//            _mail.Attachments.Clear();

//            //邮件附件
//            AddAttachments(model.Attachments);

//            //smtp 发送
//            _smtp.SendAsync(_mail,null);
//            //_smtp.SendAsync(_mailObj,null);
//        }

//        /// <summary>
//        /// Default Sent
//        /// </summary>
//        public void Send()
//        {
//            _smtp.Send(_mail);
//            //_smtp.SendAsync(_mail,null);
//            //_smtp.SendAsync(_mailObj, new { Msg = "发送邮件..." });
//        }

//    }
//}
