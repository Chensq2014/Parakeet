using Common;
using Common.Dtos;
using Common.RabbitMQModule.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Parakeet.Net.Shunt.Standard.Register.Consumers
{
    /// <summary>
    /// 注册实名认证后的人员信息
    /// </summary>
    public class PersonRegisterConsumer : ShuntConsumer<PersonRegisterConsumer, DeviceWorkerDto>
    {
        /// <summary>
        /// 只继承这个即可
        /// </summary>
        /// <param name="serviceProvider"></param>
        public PersonRegisterConsumer(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        ///// <summary>
        ///// 重载构造函数
        ///// </summary>
        ///// <param name="devicePool"></param>
        ///// <param name="freeSql"></param>
        ///// <param name="logger"></param>
        ///// <param name="producerContainer"></param>
        ///// <param name="producer"></param>
        ///// <param name="objectMapper"></param>
        //public PersonRegisterConsumer(DevicePool devicePool, IFreeSql freeSql, ILogger<PersonRegisterConsumer> logger, IProducerContainer producerContainer, IProducer producer, IObjectMapper objectMapper) : base(devicePool, freeSql, logger, producerContainer, producer, objectMapper)
        //{
        //}

        protected override QueueInfo QueueInfo => new QueueInfo
        {
            Queue = Magics.QUEUENAME,
            RoutingKey = Magics.STANDARD,
            Exchange = Magics.EXCHANGE
        };

        protected override string HandlerType => Magics.HANDLER_TYPE;


        #region 提供子类可重写消费者消费委托逻辑

        /// <summary>
        /// 消费者消费委托逻辑 提供子类可重写消费者消费委托逻辑
        /// </summary>
        /// <param name="wrapperData">封装的消息</param>
        /// <returns></returns>
        protected override Task EventProcess(WrapperData<DeviceWorkerDto> wrapperData)
        {
            return base.EventProcess(wrapperData);
        }
        protected override Task BatchEventProcess(List<WrapperData<DeviceWorkerDto>> wrapperDataList)
        {
            return base.BatchEventProcess(wrapperDataList);
        }

        #endregion

        #region 还可以更早一步重写委托
        protected override Task EventHandler((string utf8Str, ulong tag) eventData)
        {
            return base.EventHandler(eventData);
        }

        protected override Task BatchEventHandler(List<(string utf8Str, ulong tag)> eventDataList)
        {
            return base.BatchEventHandler(eventDataList);
        }
        #endregion


    }
}
