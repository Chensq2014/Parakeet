using Common.Dtos;
using Common.Entities;
using Common.Extensions;
using Common.Interfaces;
using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Parakeet.Net.Caches;
using Parakeet.Net.Equipment;
using Parakeet.Net.Protos;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;

namespace Parakeet.Net.ROServer.Services
{
    /// <summary>
    /// 设备反控接收服务Service
    /// </summary>
    public class ROService : ReverseCommand.ReverseCommandBase
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly DevicePool _devicePool;
        private readonly ILogger<ROService> _logger;
        private readonly IObjectMapper _objectMapper;
        private readonly IRepository<Device, Guid> _deviceRepository;//引用GrpcEFcoreModule

        #region Volo.Abp.ObjectMapping.IObjectMapper
        //private IMapperAccessor _mapperAccessor;

        //public IMapperAccessor MapperAccessor => LazyGetRequiredService(ref _mapperAccessor);
        //protected TService LazyGetRequiredService<TService>(ref TService reference)
        //    => LazyGetRequiredService(typeof(TService), ref reference);

        //protected readonly object ServiceProviderLock = new object();
        //protected TRef LazyGetRequiredService<TRef>(Type serviceType, ref TRef reference)
        //{
        //    if (reference == null)
        //    {
        //        lock (ServiceProviderLock)
        //        {
        //            if (reference == null)
        //            {
        //                reference = (TRef)_serviceProvider.GetRequiredService(serviceType);
        //            }
        //        }
        //    }
        //    return reference;
        //}

        //public AutoMapper.IConfigurationProvider Configuration => MapperAccessor.Mapper.ConfigurationProvider;

        //protected Type ObjectMapperContext { get; set; }
        //protected Volo.Abp.ObjectMapping.IObjectMapper ObjectMapper
        //{
        //    get
        //    {
        //        if (_objectMapper != null)
        //        {
        //            return _objectMapper;
        //        }

        //        if (ObjectMapperContext == null)
        //        {
        //            return LazyGetRequiredService(ref _objectMapper);
        //        }

        //        return LazyGetRequiredService(
        //            typeof(IObjectMapper<>).MakeGenericType(ObjectMapperContext),
        //            ref _objectMapper
        //        );
        //    }
        //}
        //private Volo.Abp.ObjectMapping.IObjectMapper _objectMapper;

        #endregion

        public ROService(IServiceProvider serviceProvider, IObjectMapper objectMapper, IRepository<Device, Guid> deviceRepository)
        {
            _serviceProvider = serviceProvider;
            _devicePool = serviceProvider.GetRequiredService<DevicePool>();
            _logger = serviceProvider.GetRequiredService<ILogger<ROService>>();
            _objectMapper = objectMapper;//serviceProvider.GetRequiredService<Volo.Abp.ObjectMapping.IObjectMapper>();
            _deviceRepository = deviceRepository;
        }

        //[Authorize("grpc")]
        public override async Task<ReverseReply> Execute(ReverseRequest request, ServerCallContext context)
        {
            var reply = new ReverseReply
            {
                Success = false,
                Message = $"执行失败"
            };
            try
            {
                var device = _devicePool.GetByFakeNo(request.SerialNo);
                //device = await _deviceRepository.FirstOrDefaultAsync(m=>m.FakeNo==request.SerialNo);
                if (device is null)
                {
                    reply.Message += $",未找到设备[{request.SerialNo}]";
                    return await Task.FromResult(reply);
                }

                if (device.Supplier is null)
                {
                    reply.Message += $",未找到设备[{request.SerialNo}]所属供应商";
                    return await Task.FromResult(reply);
                }

                //将采集设备作为一种独立的设备类型
                if (request.Command.Equals(EquipmentConstants.REGISTER_PERSON_COMMAND))
                {
                    device.Supplier.Code = EquipmentConstants.REGISTER;
                }
                var code = device.AreaTenant?.AreaCode ?? device.ParentArea;
                _logger.LogInformation($"{code}_{device.Supplier.Code}_{request.Command}");
                var command = _serviceProvider.Resolve<IReverseCommand>($"{code}_{device.Supplier.Code}_{request.Command}");
                if (command == null)
                {
                    reply.Message += $",未找到设备[{request.SerialNo}]对应的指令";
                }
                else
                {
                    #region MyRegion

                    ////第一种方式：直接写映射
                    ////var config = new MapperConfiguration(op => op.CreateMap<Device, DeviceDto>());

                    ////第二种方式：写Profile子类，在构造函数中写映射
                    //var config = new MapperConfiguration(op => op.AddProfile<ReverseDtoMapperProfile>());

                    //第三种方式：可以抽离profile子类，程序初始化时配置config

                    //使用时 注入IObjectMapper
                    var deviceDto = _objectMapper.Map<Device, DeviceDto>(device);
                    #endregion

                    var result = await command.ExecuteAsync(deviceDto, request.Body);
                    reply.Success = result.Success;
                    reply.Code = result.Code;
                    reply.Message = result.Message ?? result.Data;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"未知错误{ex.Message}");
                reply.Message += $",未知错误:{ex.Message}";
            }
            return await Task.FromResult(reply);
        }
    }
}
