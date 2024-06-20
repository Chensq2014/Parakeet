using Common.Dtos;
using Common.Entities;
using Common.Extensions;
using Common.RabbitMQModule.Consumers;
using Common.RabbitMQModule.Core;
using Common.RabbitMQModule.Producers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Parakeet.Net.Caches;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.ObjectMapping;

namespace Parakeet.Net.Shunt
{
    /// <summary>
    /// 分流模块消费者抽象父类
    /// </summary>
    /// <typeparam name="TConsumer">【RabbitMQConsumer】子类</typeparam>
    /// <typeparam name="TRecord">设备数据记录【DeviceRecordDto】</typeparam>
    public abstract class ShuntConsumer<TConsumer, TRecord> : RabbitMQConsumer where TRecord : DeviceRecordDto
    {
        /// <summary>
        /// 在MQ模块已注册
        /// </summary>
        protected readonly IFreeSql FreeSql;

        /// <summary>
        /// 框架里面已注册 serilog替换注册
        /// </summary>
        protected readonly ILogger<TConsumer> Logger;

        /// <summary>
        /// 在MQ模块已注册
        /// </summary>
        protected readonly IProducerContainer ProducerContainer;

        /// <summary>
        /// 在当前模块已注册为shuntProducer单例
        /// </summary>
        protected readonly IProducer ShuntProducer;
        /// <summary>
        /// abp automapper模块已注册
        /// </summary>
        protected readonly IObjectMapper ObjectMapper;

        ///// <summary>
        ///// 在Domain模块已注册
        ///// </summary>
        //protected readonly IEasyCachingProvider _cachingProvider;

        ///// <summary>
        ///// 在Domain模块已注册
        ///// </summary>
        //protected readonly ICacheContainer<MultilevelCache<DeviceDto>, DeviceDto> _cacheContainer;

        //protected readonly IRepository<Device, Guid> _deviceRepository;

        /// <summary>
        /// 设备缓存
        /// </summary>
        protected readonly DevicePool DevicePool;


        ///// <summary>
        ///// 框架已注册
        ///// </summary>
        //protected readonly IServiceProvider _serviceProvider;


        /// <summary>
        /// 子类队列信息
        /// </summary>
        protected abstract QueueInfo QueueInfo { get; }

        ///// <summary>
        ///// 当前交换机信息【QueueInfo里面的Exchange相同】
        ///// </summary>
        //protected abstract string Exchange { get; }

        /// <summary>
        /// 处理类型字符串【扩展字段】
        /// </summary>
        protected abstract string HandlerType { get; }


        /// <summary>
        /// 抽象消费者构造函数
        /// </summary>
        /// <param name="freeSql">freeSql</param>
        protected ShuntConsumer(IServiceProvider serviceProvider)
        {
            ProducerContainer = serviceProvider.GetService<IProducerContainer>();//Microsoft.Extensions.DependencyInjection
            FreeSql = serviceProvider.GetService<IFreeSql>();
            Logger = serviceProvider.GetService<ILogger<TConsumer>>();
            ShuntProducer = serviceProvider.GetService<IProducer>();
            ObjectMapper = serviceProvider.GetService<IObjectMapper>();
            DevicePool = serviceProvider.GetService<DevicePool>();
            InitRabbitMq();
        }


        /// <summary>
        /// 抽象消费者构造函数
        /// </summary>
        /// <param name="freeSql">freeSql</param>
        /// <param name="logger">logger</param>
        /// <param name="producerContainer">生产者-消费者管理类容器</param>
        /// <param name="producer">生产者</param>
        /// <param name="objectMapper">对象映射</param>
        protected ShuntConsumer(
            //IServiceProvider serviceProvider,
            DevicePool devicePool,
            IFreeSql freeSql,
            ILogger<TConsumer> logger,
            IProducerContainer producerContainer,
            IProducer producer,
            IObjectMapper objectMapper
            //IEasyCachingProvider cachingProvider,
            //ICacheContainer<MultilevelCache<DeviceDto>, DeviceDto> cacheContainer
            )
        {
            //_serviceProvider = serviceProvider;
            ProducerContainer = producerContainer;//serviceProvider.GetService<IProducerContainer>();
            FreeSql = freeSql;
            Logger = logger;
            ShuntProducer = producer;
            ObjectMapper = objectMapper;
            //_cachingProvider = cachingProvider;
            //_cacheContainer = cacheContainer;
            DevicePool = devicePool;
            InitRabbitMq();
        }


        /// <summary>
        /// 初始化RabbitMQ 配置交换机，队列，消费者委托等
        /// </summary>
        private void InitRabbitMq()
        {
            //消费者子类初始化消费队列信息
            QueueList.Add(QueueInfo);
            Config.Requeue = false;

            //子类增加 消费者委托逻辑
            EventHandlers.Add(EventHandler);
            BatchEventHandlers.Add(BatchEventHandler);

            //分流生产者【另一个连接】声明交换机【发送数据到交换机供其它模块声明的消费者消费】
            //设计上【另一个连接】保持交换机名称不变 EventBusExchange=QueueInfo.Exchange
            ShuntProducer.ExchangeDeclare(exchange: QueueInfo.Exchange);
        }


        /// <summary>
        /// 子类消费者消费委托逻辑
        /// </summary>
        /// <param name="eventData">从消息队列二进制转成的utf8字符串</param>
        /// <returns></returns>
        protected virtual async Task EventHandler((string utf8Str, ulong tag) eventData)
        {
            var wrapperData = eventData.utf8Str.Deserialize<WrapperData<TRecord>>();
            if (wrapperData == null)
            {
                Logger.LogWarning("分流数据为空");
                return;
            }

            Logger.LogInformation($"正在分流设备[{wrapperData.Data.Device.FakeNo}]于[{wrapperData.Data.RecordTime}]产生的数据");

            await EventProcess(wrapperData);
        }

        /// <summary>
        /// EventHandler-->EventProcess 处理强类型数据
        /// </summary>
        /// <param name="wrapperData">序列化后的待消费数据</param>
        /// <returns></returns>
        protected virtual async Task EventProcess(WrapperData<TRecord> wrapperData)
        {
            var device = DevicePool[wrapperData.Data.Device.SerialNo];
            if (device == null)
            {
                Logger.LogWarning($"未在数据中找到设备SerialNo[{wrapperData.Data.Device.SerialNo}]:[{HandlerType}]");
                return;
            }

            //shunt消费者公共分流逻辑
            wrapperData.Data.DeviceId = device.Id;
            wrapperData.Data.Device = ObjectMapper.Map<Device, DeviceDto>(device);
            //设计Forward,Persist两个字段就是为了不让Device.Mediators重复，
            //一个Mediator即可 同时转发或者存储这里不需要排重，确保每个Mediator的routingKey不同即可
            //【这里只是浪费了一点生产者发送Forward=false和Persist=false的消息队列到Forward和Persist模块】
            foreach (var dm in wrapperData.Data.Device.Mediators)
            {
                await ShuntProducer.PublishAsync(wrapperData, dm.Mediator.ToString(), EventBusExchange);
            }
        }

        /// <summary>
        /// 批量EventProcess 处理强类型数据
        /// </summary>
        /// <param name="wrapperDataList">序列化后的批量待消费数据</param>
        /// <returns></returns>
        protected virtual async Task BatchEventProcess(List<WrapperData<TRecord>> wrapperDataList)
        {
            foreach (var wrapperData in wrapperDataList)
            {
                await EventProcess(wrapperData);
            }
        }


        /// <summary>
        /// 子类消费者消费委托逻辑 处理批量消息数据
        /// </summary>
        /// <param name="eventDataList">从消息队列二进制转成的utf8 批量字符串 消息数据</param>
        /// <returns></returns>
        protected virtual async Task BatchEventHandler(List<(string utf8Str, ulong tag)> eventDataList)
        {
            var deviceRecords = new List<WrapperData<TRecord>>();
            foreach (var eventData in eventDataList)
            {
                var record = eventData.utf8Str.Deserialize<WrapperData<TRecord>>();
                deviceRecords.Add(record);
            }

            await BatchEventProcess(deviceRecords);
        }

        /// <summary>
        /// 如果直接用二进制传递数据，就要使用这个方法进行二进制直接转强类型
        /// </summary>
        /// <param name="body">二进制流</param>
        /// <returns>WrapperData<TRecord> 强类型</returns>
        protected virtual WrapperData<TRecord> BytesToObject(byte[] body)
        {
            try
            {
                var wrapperData = TextJsonConvert.Deserialize<WrapperData<TRecord>>(body);
                return wrapperData;
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"序列化失败,转换为{TextJsonConvert.SerializeObject(new WrapperData<TRecord>())}");
                return null;
            }
        }

    }
}
