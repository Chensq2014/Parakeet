using AutoMapper;
using Parakeet.Net.Entities;
using Parakeet.Net.Dtos;

namespace Parakeet.Net.Needs
{
    /// <summary>
    /// 客户需求实体-dto映射
    /// </summary>
    public class NeedDtoMapper:Profile
    {
        public NeedDtoMapper()
        {
            CreateMap<Need, NeedDto>();
            CreateMap<NeedAttachment, NeedAttachmentDto>();
        }
    }
}
