//using AutoMapper.QueryableExtensions;
//using Common.Dtos;
//using Common.Entities;
//using Common.Interfaces;
//using DevExtreme.AspNet.Data;
//using DevExtreme.AspNet.Data.ResponseModel;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Threading.Tasks;
//using Common.Extensions;
//using Common.ValueObjects;
//using Volo.Abp;
//using Volo.Abp.Application.Dtos;
//using Volo.Abp.Guids;

//namespace Parakeet.Net.Projects
//{
//    /// <summary>
//    /// 项目服务
//    /// </summary>
//    public class ProjectAppService : BaseNetAppService<Project>, IProjectAppService
//    {
//        private readonly IWebHostEnvironment _environment;
//        private readonly INetRepository<ProjectAttachment> _projectAttachmentRepository;
//        public ProjectAppService(
//            INetRepository<Project> baseRepository,
//            IWebHostEnvironment environment,
//            INetRepository<ProjectAttachment> projectAttachmentRepository) : base(baseRepository)
//        {
//            _projectAttachmentRepository = projectAttachmentRepository;
//            _environment = environment;
//        }

//        #region 重写GetGridData 获取分页数据GetPagedResult
//        /// <summary>
//        /// 获取Get(扩展) 提供给devExtreme
//        /// </summary>
//        /// <param name="loadOptions"></param>
//        /// <returns></returns>
//        public override async Task<LoadResult> GetGet(DataSourceLoadOptionsBase loadOptions)
//        {
//            var result = await DataSourceLoader.LoadAsync(await GridDto<ProjectDto>(), loadOptions);
//            return result;
//        }

//        /// <summary>
//        /// 获取GridData(Post扩展)  提供给devExtreme
//        /// </summary>
//        /// <param name="input"></param>
//        /// <returns></returns>
//        public override async Task<LoadResult> GridData(LoadOptionInputDto input)
//        {
//            //根据input过滤数据源
//            var result = await DataSourceLoader.LoadAsync((await GridDto<ProjectDto>()).Where(m => m.Id == input.Id), input.LoadOptions);
//            return result;
//        }

//        ///// <summary>
//        ///// 自定义直接获取全部数据 参考默认父类GridDto<ProjectDto>()
//        ///// </summary>
//        ///// <returns></returns>
//        //[RemoteService(IsEnabled = true, IsMetadataEnabled = false)]
//        //public IQueryable<ProjectDto> Grid()
//        //{
//        //    //var virtualPath = $"{CustomConfigurationManager.WebRootPath}\\upload\\{nameof(Need)}";
//        //    var result = Repository.AsNoTracking()
//        //        .OrderByDescending(o => o.CreationTime)
//        //        .ProjectTo<ProjectDto>(Configuration);
//        //    return result;
//        //}

//        /// <summary>
//        ///  获取列表数据(分页)
//        /// </summary>
//        /// <param name="input"></param>
//        /// <returns></returns>
//        public async Task<IPagedResult<ProjectDto>> GetPagedResult(GetProjectsInputDto input)
//        {
//            return await base.GetPagedResult<ProjectDto>(input);
//            //return await GetBaseRepository()
//            //    .AsNoTracking()
//            //    .WhereIf(input.Filter.HasValue(), x =>
//            //        x.Name.Contains(input.Filter))
//            //    .OrderBy(input.Sorting)
//            //    .ProjectTo<NeedDto>(Configuration)
//            //    .ToPageResultAsync(input);
//        }

//        /// <summary>
//        ///  获取所有项目列表数据
//        /// </summary>
//        /// <param name="input"></param>
//        /// <returns></returns>
//        public async Task<List<ProjectDto>> GetProjectFilterList(InputIdNullDto input)
//        {
//            return await (await Repository.GetQueryableAsync())
//                .AsNoTracking()
//                .WhereIf(input.Id.HasValue, x => x.Id == input.Id)
//                .ProjectTo<ProjectDto>(Configuration)
//                .ToListAsync();
//        }


//        /// <summary>
//        /// 获取项目下拉列表
//        /// </summary>
//        /// <param name="input"></param>
//        /// <returns></returns>
//        public async Task<List<KeyValueDto<Guid, string>>> GetProjectSelectList(InputIdNullDto input)
//        {
//            var projects = await (await Repository.GetQueryableAsync()).AsNoTracking()
//                .WhereIf(input.Id.HasValue, x => x.Id == input.Id)
//                .Select(m => new KeyValueDto<Guid, string>(m.Id, m.Name))
//                .ToListAsync();
//            return projects;
//        }

//        #endregion

//        #region 重写删除扩展
//        /// <summary>
//        /// 根据主键PrimaryKey删除实体 DeleteDelete 默认提供给devExtreme
//        /// </summary>
//        /// <returns></returns>
//        public override async Task DeleteDelete()
//        {
//            var key = GetTPrimaryKey();
//            var attachments = (await _projectAttachmentRepository.GetQueryableAsync()).Where(m => m.ProjectId == key);
//            foreach (var attachment in attachments)
//            {
//                var path = $"{_environment.WebRootPath}{attachment.Attachment.VirtualPath}";
//                if (System.IO.File.Exists(path))
//                {
//                    System.IO.File.Delete(path);
//                }
//            }
//            await _projectAttachmentRepository.DeleteAsync(m => m.ProjectId == key);
//            await Repository.DeleteAsync(m => m.Id == key);
//        }

//        #endregion

//        /// <summary>
//        /// UploadFile Request.Form.Files接收文件 Request.Form["uploadGuid"]传递uploadGuid
//        /// </summary>
//        /// <param name="input"></param>
//        /// <returns></returns>
//        [RemoteService(IsEnabled = true, IsMetadataEnabled = false)]
//        public async Task UploadFile(InputIdDto input)//IFormFile uploadFile参数文件过大使用参数获取会出现序列化错误
//        {
//            try
//            {
//                //var uploadGuid = Context.HttpContext.Request.Form["uploadGuid"];
//                var project = await GetByPrimaryKey(input);
//                var order = project != null && project.Attachments.Any() ? project.Attachments.Max(m => m.Order ?? 0) : 0;
//                var virtualPath = $@"/images/projectmap/{project?.Id.ToString()}";
//                var pathForSaving = $"{_environment.WebRootPath}{virtualPath}";
//                if (FileExtension.CreateFolderIfNeeded(pathForSaving))
//                {
//                    //foreach (IFormFile uploadFile in Request.Form.Files)
//                    {
//                        var uploadFile = ContextAccessor.HttpContext?.Request.Form.Files[0];
//                        if (uploadFile != null && uploadFile.Length > 0)
//                        {
//                            var path = Path.Combine(pathForSaving, uploadFile.FileName);
//                            //文件名不重复，当已存在同名文件时，还需要先删除文件
//                            //FileExtension.ClearFile(path);
//                            await FileExtension.SaveFile(uploadFile, path);
//                            project?.Attachments.Add(new ProjectAttachment
//                            {
//                                //path.Remove(0,CustomConfigurationManager.RootPath.Length)
//                                Attachment = new Attachment(uploadFile.FileName, Path.GetExtension(path).ToLower(), uploadFile.Length, $@"{virtualPath}/{uploadFile.FileName}"),
//                                Order = ++order,
//                                ProjectId = project.Id,
//                                CreatorId = CurrentUser?.Id
//                            });
//                        }
//                    }
//                }
//            }
//            catch (Exception e)
//            {
//                throw new UserFriendlyException(e.Message);
//            }
//            //return SJson();
//        }

//        /// <summary>
//        /// 获取项目全部附件列表 不分页
//        /// </summary>
//        /// <param name="input">项目Id</param>
//        /// <returns></returns>
//        public async Task<List<ProjectAttachmentDto>> GetProjectAttachments(InputIdNullDto input)
//        {
//            var result = await (await Repository.GetQueryableAsync()).AsNoTracking()
//                .Where(m => m.Id == input.Id)
//                .SelectMany(n => n.Attachments)
//                .OrderByDescending(n => n.Order)
//                .ProjectTo<ProjectAttachmentDto>(Configuration)
//                .ToListAsync();
//            return result;
//        }

//        /// <summary>
//        /// Test
//        /// </summary>
//        /// <returns></returns>
//        public async Task<Guid> Test()
//        {
//            var guid = GuidGenerator.Create();
//            if (GuidGenerator is SequentialGuidGenerator)
//            {
//                Console.WriteLine($"{nameof(SequentialGuidGenerator)}");
//            }
//            if (GuidGenerator is SimpleGuidGenerator)
//            {
//                Console.WriteLine($"{nameof(SimpleGuidGenerator)}");
//            }

//            return guid;
//        }
//    }
//}
