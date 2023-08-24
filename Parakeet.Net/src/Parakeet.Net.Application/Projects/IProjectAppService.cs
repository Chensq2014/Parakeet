using Parakeet.Net.Dtos;
using Parakeet.Net.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.DependencyInjection;

namespace Parakeet.Net.Projects
{
    /// <summary>
    /// 项目服务 IProjectAppService
    /// </summary>
    [Description("项目")]
    public interface IProjectAppService: IBaseNetAppService<Project>, ITransientDependency
    {
        /// <summary>
        ///  获取列表数据(分页)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<IPagedResult<ProjectDto>> GetPagedResult(GetProjectsInputDto input);

        /// <summary>
        ///  获取所有项目列表数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<List<ProjectDto>> GetProjectFilterList(InputIdNullDto input);

        /// <summary>
        /// 获取项目下拉列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<List<KeyValueDto<Guid, string>>> GetProjectSelectList(InputIdNullDto input);

        /// <summary>
        /// UploadFile Request.Form.Files接收文件 Request.Form["uploadGuid"]传递uploadGuid
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task UploadFile(InputIdDto input);

        /// <summary>
        /// 获取项目全部附件列表 不分页
        /// </summary>
        /// <param name="input">项目Id</param>
        /// <returns></returns>
        Task<List<ProjectAttachmentDto>> GetProjectAttachments(InputIdNullDto input);
    }
}