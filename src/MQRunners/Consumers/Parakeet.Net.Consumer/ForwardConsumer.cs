using Common.Dtos;
using Common.Extensions;
using Common.RabbitMQModule.Consumers;
using Common.RabbitMQModule.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Parakeet.Net.Consumer
{
    /// <summary>
    /// 转发模块消费者抽象类
    /// </summary>
    /// <typeparam name="TRecord"></typeparam>
    public abstract class ForwardConsumer<TRecord> : RabbitMQConsumer where TRecord : DeviceRecordDto
    {
        protected readonly ILogger Logger;

        protected ForwardConsumer(IServiceProvider serviceProvider)
        {
            Logger = serviceProvider.GetService<ILogger<RabbitMQConsumer>>();
            EventHandlers.Add(EventHandler);
            BatchEventHandlers.Add(BatchEventHandler);
            InitRabbitMq();
        }

        private void InitRabbitMq()
        {
            QueueList.Add(QueueInfo);
            EventBusExchange = Exchange;
            Config.AutoAck = true;//转发数据只需要通知接收者一次
        }

        protected abstract QueueInfo QueueInfo { get; }

        protected abstract string Exchange { get; }

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
                Logger?.LogWarning("数据为空");
                return;
            }
            await EventProcess(wrapperData);
        }

        /// <summary>
        /// EventHandler-->EventProcess 处理强类型数据
        /// </summary>
        /// <param name="wrapperData">序列化后的待消费数据</param>
        /// <returns></returns>
        protected abstract Task EventProcess(WrapperData<TRecord> wrapperData);

        /// <summary>
        /// 批量EventProcess 处理强类型数据
        /// </summary>
        /// <param name="wrapperDataList">序列化后的批量待消费数据</param>
        /// <returns></returns>
        protected abstract Task BatchEventProcess(List<WrapperData<TRecord>> wrapperDataList);

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
