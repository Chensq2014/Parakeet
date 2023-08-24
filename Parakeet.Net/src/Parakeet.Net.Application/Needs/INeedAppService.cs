using Microsoft.AspNetCore.Mvc;
using Parakeet.Net.Dtos;
using Parakeet.Net.Entities;
using System.ComponentModel;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.DependencyInjection;

namespace Parakeet.Net.Needs
{
    /// <summary>
    /// 需求 这个接口父类都直接依赖了实体层 故此类接口不能写在Contracts层共享了
    /// </summary>
    [Description("需求")]
    public interface INeedAppService : IBaseNetAppService<Need>, ITransientDependency
    {
        /// <summary>
        ///  获取列表数据(分页)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<IPagedResult<NeedDto>> GetPagedResult(NeedPagedInputDto input);

        /// <summary>
        /// UploadFile Request.Form.Files接收文件 Request.Form["uploadGuid"]传递uploadGuid
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task UploadFiles(InputIdDto input);

        /// <summary>
        /// 获取附件名
        /// </summary>
        /// <param name="attachment">附件实体</param>
        /// <returns></returns>
        string GetAttachmentName(NeedAttachment attachment);

        /// <summary>
        /// 根据主键PrimaryKey删除实体 DeleteDelete 默认提供给devExtreme
        /// </summary>
        /// <returns></returns>
        Task<IActionResult> DeleteAttachments(InputIdDto input);

        /// <summary>
        /// POST: Need/Create
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Message> Create(NeedCreateDto input);

        ///// <summary>
        ///// body里面可以构造html
        ///// </summary>
        ///// <param name="need"></param>
        ///// <param name="sendToCustomer"></param>
        ///// <returns></returns>
        //Task<string> GetMailBody(Need need, bool sendToCustomer = false);

        ///// <summary>
        ///// 获取通用邮件传输对象
        ///// </summary>
        ///// <returns></returns>
        //Task<MailDto> GetMailDto(Guid id);
    }
}
