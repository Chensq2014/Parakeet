using Common.Dtos;
using Common.Entities;
using Common.Enums;
using Common.Extensions;
using Common.Interfaces;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Emailing;
using Volo.Abp.Settings;
using Attachment = Common.ValueObjects.Attachment;

namespace Parakeet.Net.Needs
{
    /// <summary>
    /// 需求服务
    /// </summary>
    public class NeedAppService : BaseNetAppService<Need>, INeedAppService
    {
        private readonly IPortalRepository<Need> _needRepository;
        private readonly IPortalRepository<NeedAttachment> _needAttachmentRepository;
        private readonly IPersonalCacheAppService _personalCacheAppService;
        private readonly ISettingProvider _settingProvider;
        private readonly IEmailSender _emailSender;
        private readonly IWebHostEnvironment _environment;

        public NeedAppService(
            //IWebHostEnvironment env, 
            IPortalRepository<NeedAttachment> needAttachmentRepository,
            IPersonalCacheAppService personalCacheAppService,
            IWebHostEnvironment environment,
            IPortalRepository<Need> baseRepository,
            IEmailSender emailSender,
            ISettingProvider settingProvider) : base(baseRepository)
        {
            _emailSender = emailSender;
            _needRepository = baseRepository;
            _settingProvider = settingProvider;
            _environment = environment;
            _needAttachmentRepository = needAttachmentRepository;
            _personalCacheAppService = personalCacheAppService;
            //_env = env;
            //_configuration = AppConfigurations.Get(env.ContentRootPath, env.EnvironmentName, env.IsDevelopment());
        }

        #region 重写GetGridData 获取分页数据GetPagedResult
        /// <summary>
        /// 获取Get(扩展) 提供给devExtreme
        /// </summary>
        /// <param name="loadOptions"></param>
        /// <returns></returns>
        public override async Task<LoadResult> GetGet(DataSourceLoadOptionsBase loadOptions)
        {
            var result = await DataSourceLoader.LoadAsync(await GridDto<NeedDto>(), loadOptions);
            return result;
        }

        /// <summary>
        /// 获取GridData(Post扩展)  提供给devExtreme
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override async Task<LoadResult> GridData(LoadOptionInputDto input)
        {
            //根据input过滤数据源
            var result = await DataSourceLoader.LoadAsync((await GridDto<NeedDto>()).Where(m => m.Id == input.Id), input.LoadOptions);
            return result;
        }

        ///// <summary>
        ///// 自定义直接获取全部数据 参考默认父类GridDto<NeedDto>()
        ///// </summary>
        ///// <returns></returns>
        //[RemoteService(IsEnabled = true, IsMetadataEnabled = false)]
        //public IQueryable<NeedDto> Grid()
        //{
        //    //var virtualPath = $"{CustomConfigurationManager.WebRootPath}\\upload\\{nameof(Need)}";
        //    var result = Repository.AsNoTracking()
        //        .OrderByDescending(o => o.CreationTime)
        //        .ProjectTo<NeedDto>(Configuration);
        //    return result;
        //}

        /// <summary>
        ///  获取列表数据(分页)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<IPagedResult<NeedDto>> GetPagedResult(NeedPagedInputDto input)
        {
            return await base.GetPagedResult<NeedDto>(input);
            //return await GetBaseRepository()
            //    .AsNoTracking()
            //    .WhereIf(input.Filter.HasValue(), x =>
            //        x.Name.Contains(input.Filter))
            //    .OrderBy(input.Sorting)
            //    .ProjectTo<NeedDto>(Configuration)
            //    .ToPageResultAsync(input);
        }

        #endregion

        #region 重写删除扩展
        /// <summary>
        /// 根据主键PrimaryKey删除实体 DeleteDelete 默认提供给devExtreme
        /// </summary>
        /// <returns></returns>
        public override async Task DeleteDelete()
        {
            var key = GetTPrimaryKey();
            await DeleteAttachments(new InputIdDto { Id = key });
            await Repository.DeleteAsync(m => m.Id == key);
        }

        /// <summary>
        /// 根据主键PrimaryKey删除实体 DeleteDelete 默认提供给devExtreme
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> DeleteAttachments(InputIdDto input)
        {
            var need = await (await _needRepository.GetQueryableAsync())
                .Include(x => x.Attachments)
                .FirstOrDefaultAsync(m => m.Id == input.Id);
            need.RemoveAllAttachments();
            return SJson();
        }

        #endregion

        /// <summary>
        /// UploadFile Request.Form.Files接收文件 Request.Form["uploadGuid"]传递uploadGuid
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [RemoteService(IsEnabled = true, IsMetadataEnabled = false)]
        public async Task UploadFiles(InputIdDto input)//IFormFile uploadFile参数文件过大使用参数获取会出现序列化错误
        {
            try
            {
                var context = ContextAccessor.HttpContext;
                var need = await (await Repository.GetQueryableAsync()).Include(x => x.Attachments).FirstOrDefaultAsync(m => m.Id == input.Id);//GetByPrimaryKey(input);
                var order = need != null && need.Attachments.Any() ? need.Attachments.Max(m => m.Order ?? 0) : 0;
                var virtualPath = $@"{(need != null
                    ? $@"/upload/need/{need.Id}/"
                    : $@"/upload/temp/{context.Request.Form["uploadGuid"]}/")}";
                var pathForSaving = $@"{_environment.WebRootPath}{virtualPath}";//Path.Combine(CustomConfigurationManager.WebRootPath, virtualPath);
                if (FileExtension.CreateFolderIfNeeded(pathForSaving))
                {
                    var attachments = new List<NeedAttachment>();
                    if (need?.Attachments.Count + context.Request.Form.Files.Count > 5)
                    {
                        throw new UserFriendlyException($"已存在{need?.Attachments.Count}个附件,您还能上传{5 - need?.Attachments.Count}个附件!");
                    }
                    foreach (var uploadFile in context.Request.Form.Files)//Request.Form.Files
                    {
                        //var uploadFile = context.Request.Form.Files[0];
                        if (uploadFile != null && uploadFile.Length > 0)
                        {
                            if (uploadFile.Length > 2097152)
                            {
                                throw new UserFriendlyException("单个文件不能大于2MB");
                            }
                            var path = Path.Combine(pathForSaving, uploadFile.FileName);
                            if (File.Exists(path))
                            {
                                //文件名不重复，当已存在同名文件时，自动跳过此附件
                                //File.Delete(path);//
                                throw new UserFriendlyException("文件名冲突");
                            }
                            await FileExtension.SaveFile(uploadFile, path);

                            attachments.Add(new NeedAttachment(GuidGenerator.Create())
                            {
                                //path.Remove(0,CustomConfigurationManager.RootPath.Length)
                                Attachment = new Attachment(uploadFile.FileName, Path.GetExtension(path).ToLower(), uploadFile.Length, $@"{virtualPath}/{uploadFile.FileName}", path)
                                {
                                    //要复制的 LOB 数据的长度(437896)超出了配置的最大值 65536。
                                    //请使 用存储过程 sp_configure 为 max text repl size 选项(默认值为 65536)增加配置的最大值。
                                    //配置值为 -1 表示无限制，其他值表示由数据类型施加限制
                                    ////Bytes = await FileExtension.GetBinaryData(file.OpenReadStream())
                                },
                                Order = ++order,
                                NeedId = need?.Id
                                //CreatorId = CurrentUser?.Id
                            });
                        }
                    }
                    need?.AddAttachments(attachments);
                }
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
                //return EJson(e.Message);
            }
        }

        /// <summary>
        /// 获取附件名称
        /// </summary>
        /// <param name="attachment"></param>
        /// <returns></returns>
        public string GetAttachmentName(NeedAttachment attachment)
        {
            try
            {
                string path = attachment.Attachment.Path;
                string attachmentName = path.Substring(path.LastIndexOf("\\", StringComparison.Ordinal) + 1);
                return attachmentName;
                //var url = string.Format("/Need/Download/{0}", attachment.Id);
                //return string.Format("<a href='{0}'>{1}</a>", url, attachmentName);
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// POST: Need/Create
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<AjaxReturnMessage> Create([FromForm] NeedCreateDto input)
        {
            try
            {
                // 验证码
                //Logger.LogDebug($"{nameof(NeedAppService)} 准备验证码.....");
                if (!input.Code.ToUpper().Equals(await _personalCacheAppService.GetCacheCode()))//base.HttpContext.Session.GetString("CheckCode")
                {
                    //Logger.LogDebug($"{nameof(NeedAppService)} 验证码错误.....");
                    throw new UserFriendlyException("验证码错误");
                }
                //自定义的上传文件最多20个...
                input.AddFiles();

                var need = new Need(GuidGenerator.Create())
                {
                    Name = input.Name,
                    Sex = input.Sex,
                    Email = input.Email,
                    Phone = input.Phone,
                    QNumber = input.QNumber,
                    Requirements = input.Requirements
                };
                //need.SetEntityPrimaryKey();

                #region 1、原始方式发送邮件

                var mailDto = await GetMailDto(need.Id);
                ////添加附件
                //mailDto.Subject = need.Name + await _settingProvider.GetOrNullAsync("Email.Subject");
                //mailDto.Body = $"{await GetMailBody(need)}{body}";
                #endregion

                #region _emailSender mailMessage 方式数据准备 

                var mailMessage = new MailMessage(mailDto.From, mailDto.To)//包含发件人、收件人、抄送人、主题、附件地址的对象
                {
                    Subject = mailDto.Subject,
                    IsBodyHtml = true,
                    Body = $"{await GetMailBody(need)}{mailDto.Body}"
                };
                mailMessage.CC.Add(mailDto.Cc);

                #endregion

                //record.Attachments = input.Attachments;
                //上传的附件，循环赋值 input.UploadFiles或者 Request.Form.Files
                var virtualPath = $@"/upload/need/{need.Id}/";
                var targetPath = $"{_environment.WebRootPath}{virtualPath}";//\\{input.UploadGuid}
                if (FileExtension.CreateFolderIfNeeded(targetPath))
                {
                    var attatchments = new List<NeedAttachment>();
                    //Request.Form.Files["files1"]
                    var files = input.Files.Where(m => m != null).ToList();
                    foreach (var file in files)
                    {
                        var path = Path.Combine(targetPath, file.FileName);
                        await FileExtension.SaveFile(file, path);
                        //添加attachments 并设置record 的attachments
                        var attachment = new NeedAttachment(GuidGenerator.Create())
                        {
                            //path.Remove(0,CustomConfigurationManager.RootPath.Length)
                            Attachment = new Attachment(file.FileName, Path.GetExtension(path).ToLower(), file.Length,
                                $@"{virtualPath}/{file.FileName}", path)
                            {
                                //要复制的 LOB 数据的长度(437896)超出了配置的最大值 65536。
                                //请使 用存储过程 sp_configure 为 max text repl size 选项(默认值为 65536)增加配置的最大值。
                                //配置值为 -1 表示无限制，其他值表示由数据类型施加限制
                                ////Bytes = await FileExtension.GetBinaryData(file.OpenReadStream())
                            },
                            //Order = ++order,
                            NeedId = need.Id,
                            CreatorId = CurrentUser?.Id
                        };
                        //attachment.SetEntityPrimaryKey(GuidGenerator.Create());
                        attatchments.Add(attachment);
                    }
                    if (attatchments.Any())
                    {
                        attatchments.ForEach(attatchment => need.Attachments.Add(attatchment));
                        mailDto.Attachments.AddRange(attatchments.Select(m => m.Attachment));
                    }
                    await GetBaseRepository().InsertAsync(need);
                }


                #region mailMessage添加附件

                mailDto.Attachments.ForEach(m =>
                {
                    //建立邮件附件类的一个对象，语法格式为System.Net.Mail.Attachment(文件名，文件格式)
                    var mailAttachment = new System.Net.Mail.Attachment(m.Path, MediaTypeNames.Application.Octet);
                    var disposition = mailAttachment.ContentDisposition;//MIME协议下的一个对象，用以设置附件的创建时间，修改时间以及读取时间
                    disposition.CreationDate = File.GetCreationTime(m.Path);
                    disposition.ModificationDate = File.GetLastWriteTime(m.Path);
                    disposition.ReadDate = File.GetLastAccessTime(m.Path);
                    mailMessage.Attachments.Add(mailAttachment);
                });

                #endregion

                #region 原始方式发送邮件 pass
                //var mail = new MailAppService(mailDto);
                ////send to parakeet
                //mail.Send();

                ////send to customer
                //mailDto.To = need.Email.Trim();
                //mailDto.Body = $"{ await GetMailBody(need, true)}{mailDto.Footer}";
                //mail.SendAsync(mailDto);
                #endregion

                #region _emailSender 方式发送邮件
                //send to parakeet

                mailMessage.Subject = $"来自{need.Name}{(need.Sex == Sex.Male ? "先生" : "小姐")}{mailMessage.Subject}";
                await SendEmailAsync(mailMessage);

                //send to customer
                mailMessage.CC.Clear();
                mailMessage.To.Clear();
                mailMessage.To.Add(new MailAddress(need.Email.Trim()));
                mailMessage.Subject = $"亲爱的{need.Name}{(need.Sex == Sex.Male ? "先生" : "小姐")}";
                mailMessage.Body = $"{await GetMailBody(need, true)}{mailDto.Footer}";
                await SendEmailAsync(mailMessage);
                #endregion

                //{CustomConfigurationManager.Configuration["SelfUrl"]}/api/parakeet/need/index"
                return new AjaxReturnMessage { Status = true, Url = $"url", Msg = "保存成功！" };
            }
            catch (Exception e)
            {
                Logger.LogError($"{nameof(NeedAppService)} {e.Message},保存失败.....");
                throw new UserFriendlyException(e.Message);
            }
        }

        private async Task SendEmailAsync(MailMessage message)
        {
            try
            {
                await _emailSender.SendAsync(message);
            }
            catch (Exception e)
            {
                Logger.LogError($"发送邮件失败：{e.Message}");
            }
        }

        /// <summary>
        /// body里面可以构造html
        /// </summary>
        /// <param name="need"></param>
        /// <param name="sendToCustomer"></param>
        /// <returns></returns>
        //[RemoteService(IsEnabled = true, IsMetadataEnabled = false)]
        private async Task<string> GetMailBody(Need need, bool sendToCustomer = false)
        {
            var title = $"{await _settingProvider.GetOrNullAsync("Email.FriendlyTitle")}小主人,收到来自{need.Name}{(need.Sex == Sex.Male ? "先生" : "小姐")}的需求如下：";
            var sign = await _settingProvider.GetOrNullAsync("Email.Sign");
            var note = "";
            var url = "";
            if (sendToCustomer)
            {
                title = $"{await _settingProvider.GetOrNullAsync("Email.FriendlyTitle")}{need.Name}{(need.Sex == Sex.Male ? "先生" : "小姐")},<br/>您提交的资料如下:";
                sign = await _settingProvider.GetOrNullAsync("Email.SignDetail");
                note = $"<br/>您的需求已收到，我们将尽快与您联系！<br/>";
                url = $"<br/>{await _settingProvider.GetOrNullAsync("Email.SignUrl")}";
            }
            //构造打开邮件自动回调网站的某个action 进行统计
            var body = $"{title}<br/>姓名：{need.Name}<br/>性别：{need.Sex?.DisplayName()}<br/>邮箱：{need.Email}<br/>电话：{need.Phone}<br/>QQ：{need.QNumber}<br/>需求：{need.Requirements}{note}<br/><br/>With Best Regards,<br/>{sign}{url}";
            return body;
        }

        /// <summary>
        /// 获取通用邮件传输对象
        /// </summary>
        /// <returns></returns>
        private async Task<MailDto> GetMailDto(Guid id)
        {
            var domain = "localhost";//CustomConfigurationManager.Configuration["SelfUrl"];
            var userName = await _settingProvider.GetOrNullAsync(EmailSettingNames.Smtp.UserName);
            var password = await _settingProvider.GetOrNullAsync(EmailSettingNames.Smtp.Password);
            var host = await _settingProvider.GetOrNullAsync(EmailSettingNames.Smtp.Host);
            var port = await _settingProvider.GetAsync<int>(EmailSettingNames.Smtp.Port);//int 不设置就默认为0
            var enableSsl = await _settingProvider.GetAsync<bool>(EmailSettingNames.Smtp.EnableSsl);//bool不设默认值就为false
            var from = await _settingProvider.GetOrNullAsync(EmailSettingNames.DefaultFromAddress);//CustomConfigurationManager.Configuration["App:Email:From"];
            var to = await _settingProvider.GetOrNullAsync("Email.To"); //CustomConfigurationManager.Configuration["App:Email:To"];
            //var title = await _settingProvider.GetOrNullAsync("Email.FriendlyTitle");
            //var sign = await _settingProvider.GetOrNullAsync("Email.Sign");
            var subject = await _settingProvider.GetOrNullAsync("Email.Subject");
            var cc = await _settingProvider.GetOrNullAsync("Email.Cc");

            #region 最原始方式发送邮件 pass
            //var guid = GuidGenerator.Create();
            var returnUrl = $@"{domain}/api/parakeet/need/readEmail";
            var virtualPath = "/upload/projectMap/default.jpg";
            var path = $@"{_environment.WebRootPath}{virtualPath}";//Path.Combine(CustomConfigurationManager.WebRootPath, virtualPath);

            //使用ssl 587端口加密
            var body = $@"<h1><img style='display:none;' src='{returnUrl}?id={id}'/></h1><img src='{domain}/api/parakeet/project/Map'/><img src='~{virtualPath}'/>";
            var mailModel = new MailDto(host, userName, password, from, to, port)
            {
                Cc = cc,
                Subject = subject,
                EnableSsl = enableSsl,
                Body = body,
                Footer = body
            };
            //添加body中默认需要显示的图片
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                var attachment = new Attachment("default.jpg", Path.GetExtension(path).ToLower(), fs.Length, virtualPath, path);
                //attachment.Bytes = await FileExtension.GetBinaryData(fs);//attachment.Bytes = await FileExtension.GetBinaryData(path);
                mailModel.Attachments.Add(attachment);
                fs.Close();
            }
            return mailModel;
            #endregion
        }

    }
}
