using AutoMapper;
using Common.Entities;
using Common.Dtos;
using Volo.Abp.AutoMapper;

namespace Parakeet.Net.Projects
{
    public class ProjectDtoMapper : Profile
    {
        public ProjectDtoMapper()
        {
            //CreateMap<DemoProject, ProjectItemDto>()
            //    .ForMember(x => x.UnitProjectCount, opt => opt.MapFrom(x => x.DemoUnitProjects.Count()));
            //CreateMap<DemoProject, ProjectDto>()
            //    .ForMember(x => x.UnitProjectCount, opt => opt.MapFrom(x => x.DemoUnitProjects.Count()));
            //CreateMap<DemoUnitProject, DemoUnitProjectItemDto>();
            CreateMap<Project, ProjectDto>()
                .ForMember(m => m.ProjectUsers, opt => opt.Ignore())
                //.ForMember(m => m.Devices, opt => opt.Ignore())
                .ForMember(m => m.UploadGuid, opt => opt.Ignore())
                //.ForMember(m => m.OrganizationName, opt => opt.MapFrom(n => n.Organization.Name));
                .Ignore(x => x.Sections)
                .Ignore(x => x.HouseTotal)
                .Ignore(x => x.WorkerTotal)
                .Ignore(x => x.CostTotal)
                .Ignore(x => x.ProfitTotal);
            CreateMap<ProjectUser, ProjectUserDto>()
                .ForMember(m => m.User, opt => opt.Ignore());
            CreateMap<ProjectAttachment, ProjectAttachmentDto>();
        }
    }
}