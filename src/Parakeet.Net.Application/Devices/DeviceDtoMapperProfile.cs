using AutoMapper;
using Common.Dtos;
using Common.Entities;
using Volo.Abp.AutoMapper;

namespace Parakeet.Net.Devices
{
    class DeviceDtoMapperProfile : Profile
    {
        public DeviceDtoMapperProfile()
        {
            CreateMap<Device, DeviceDto>()
                .Ignore(m=>m.AreaTenant)
                .ForMember(x => x.SupplierName, y => y.MapFrom(z => z.Supplier.Name))
                .ForMember(x => x.ProjectName, y => y.MapFrom(z => z.Project.Name));
            CreateMap<Device, DeviceListDto>()
                .ForMember(x => x.SupplierName, y => y.MapFrom(z => z.Supplier.Name))
                .ForMember(x => x.ProjectName, y => y.MapFrom(z => z.Project.Name));
            CreateMap<DeviceDto, Device>()
                .Ignore(x => x.IsDeleted)
                .Ignore(x => x.DeleterId)
                .Ignore(x => x.DeletionTime)
                .Ignore(x => x.LastModifierId)
                .Ignore(x => x.LastModificationTime)
                .Ignore(x => x.CreatorId)
                .Ignore(x => x.CreationTime)
                .Ignore(x => x.ExtraProperties)
                .Ignore(x => x.ConcurrencyStamp)
                .Ignore(x => x.Project)
                .Ignore(x => x.Supplier)
                .Ignore(x => x.Sequence)
                .Ignore(x => x.Threshold)
                .Ignore(x => x.AreaTenant)
                .Ignore(x => x.LocationArea)
                .Ignore(x => x.Extends)
                .Ignore(x => x.KeySecrets)
                .Ignore(x => x.DeviceWorkers)
                .Ignore(x => x.Mediators);
            CreateMap<DeviceListDto, Device>()
                .Ignore(x => x.IsDeleted)
                .Ignore(x => x.DeleterId)
                .Ignore(x => x.DeletionTime)
                .Ignore(x => x.LastModifierId)
                .Ignore(x => x.LastModificationTime)
                .Ignore(x => x.CreatorId)
                .Ignore(x => x.CreationTime)
                .Ignore(x => x.ExtraProperties)
                .Ignore(x => x.ConcurrencyStamp)
                .Ignore(x => x.Project)
                .Ignore(x => x.Supplier)
                .Ignore(x => x.Sequence)
                .Ignore(x => x.Threshold)
                .Ignore(x => x.AreaTenant)
                .Ignore(x => x.LocationArea)
                .Ignore(x => x.Extends)
                .Ignore(x => x.KeySecrets)
                .Ignore(x => x.DeviceWorkers)
                .Ignore(x => x.Mediators);
            CreateMap<CreateDeviceInputDto, Device>()
                .Ignore(x => x.IsDeleted)
                .Ignore(x => x.DeleterId)
                .Ignore(x => x.DeletionTime)
                .Ignore(x => x.LastModifierId)
                .Ignore(x => x.LastModificationTime)
                .Ignore(x => x.CreatorId)
                .Ignore(x => x.CreationTime)
                .Ignore(x => x.ExtraProperties)
                .Ignore(x => x.ConcurrencyStamp)
                .Ignore(x => x.Project)
                .Ignore(x => x.Supplier)
                .Ignore(x => x.Sequence)
                .Ignore(x => x.Threshold)
                .Ignore(x => x.AreaTenant)
                .Ignore(x => x.LocationArea)
                .Ignore(x => x.Extends)
                .Ignore(x => x.KeySecrets)
                .Ignore(x => x.DeviceWorkers)
                .Ignore(x => x.Mediators);
            CreateMap<UpdateDeviceInputDto, Device>()
                .Ignore(x => x.IsDeleted)
                .Ignore(x => x.DeleterId)
                .Ignore(x => x.DeletionTime)
                .Ignore(x => x.LastModifierId)
                .Ignore(x => x.LastModificationTime)
                .Ignore(x => x.CreatorId)
                .Ignore(x => x.CreationTime)
                .Ignore(x => x.ExtraProperties)
                .Ignore(x => x.ConcurrencyStamp)
                .Ignore(x => x.Project)
                .Ignore(x => x.Supplier)
                .Ignore(x => x.Sequence)
                .Ignore(x => x.Threshold)
                .Ignore(x => x.AreaTenant)
                .Ignore(x => x.LocationArea)
                .Ignore(x => x.Extends)
                .Ignore(x => x.KeySecrets)
                .Ignore(x => x.DeviceWorkers)
                .Ignore(x => x.Mediators);

            CreateMap<DeviceSequence, DeviceSequenceDto>();
            CreateMap<DeviceExtend, DeviceExtendDto>();
            CreateMap<DeviceKeySecret, DeviceKeySecretDto>();
            CreateMap<DeviceMediator, DeviceMediatorDto>();
            CreateMap<DeviceWorker, DeviceWorkerDto>()
                .Ignore(m => m.AreaTenant)
                .Ignore(m => m.LocationArea);
        }
    }
}
