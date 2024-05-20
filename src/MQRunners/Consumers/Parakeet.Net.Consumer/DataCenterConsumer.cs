using Common.Dtos;
using Common.Entities;
using Common.Extensions;
using Common.RabbitMQModule.Consumers;
using Common.RabbitMQModule.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Volo.Abp.ObjectMapping;

namespace Parakeet.Net.Consumer
{
    /// <summary>
    /// 存储数据持久化消费者抽象基类
    /// </summary>
    public abstract class DataCenterConsumer<TModule, TEntity, TRecord> : RabbitMQConsumer where TModule : AbpModule where TEntity : DeviceRecord where TRecord : DeviceRecordDto
    {
        protected readonly IServiceProvider ServiceProvider;
        protected readonly IRepository<TEntity, Guid> Repository;
        protected readonly IFreeSql<TModule> FreeSql;
        protected readonly IObjectMapper _objectMapper;
        protected readonly ILogger<TEntity> Logger;
        protected DataCenterConsumer(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            _objectMapper = serviceProvider.GetRequiredService<IObjectMapper>();
            Repository = serviceProvider.GetService<IRepository<TEntity, Guid>>();
            FreeSql = serviceProvider.GetService<IFreeSql<TModule>>();
            Logger = serviceProvider.GetService<ILogger<TEntity>>();
            EventHandlers.Add(EventHandler);
            BatchEventHandlers.Add(BatchEventHandler);
            InitRabbitMq();
        }
        private void InitRabbitMq()
        {
            QueueList.Add(QueueInfo);
            EventBusExchange = Exchange;
        }

        /// <summary>
        /// 队列信息 属于子类信息 子类设计赋值
        /// </summary>
        protected abstract QueueInfo QueueInfo { get; }

        /// <summary>
        /// 交换机 属于子类信息 子类设计赋值
        /// </summary>
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
            await EventProcess((wrapperData, eventData.tag));
        }

        /// <summary>
        /// EventHandler-->EventProcess 处理强类型数据
        /// </summary>
        /// <param name="wrapperData">序列化后的待消费数据</param>
        /// <returns></returns>
        protected virtual async Task EventProcess((WrapperData<TRecord>, ulong) wrapperData)
        {
            try
            {
                var record = _objectMapper.Map<TRecord, TEntity>(wrapperData.Item1.Data);
                await Repository.InsertAsync(record);
                AckedTags.Add(wrapperData.Item2);
            }
            catch (Exception e)
            {
                Logger.LogError($"未知错误:{e.Message}", e);
                AckedTags.Remove(wrapperData.Item2);
            }
        }

        /// <summary>
        /// 批量EventProcess 处理强类型数据
        /// </summary>
        /// <param name="wrapperDataList">序列化后的批量待消费数据</param>
        /// <param name="fetch">每次取数</param>
        /// <param name="pgCopy">是否批量拷贝 默认false</param>
        /// <returns></returns>
        protected virtual async Task BatchEventProcess(List<(WrapperData<TRecord> Records, ulong Tag)> wrapperDataList, int fetch = 100, bool pgCopy = false)
        {
            #region 批量插入

            try
            {
                if (pgCopy)
                {
                    //直接使用freesql批量拷贝插入 
                    var records = wrapperDataList.Select(x => x.Records).Select(m => m.Data).ToList();
                    var entities = _objectMapper.Map<List<TRecord>, List<TEntity>>(records);
                    await FreeSql.Insert(entities).ExecutePgCopyAsync();
                    //await Repository.BulkInsert(entities);
                }
                else
                {
                    var bufferCount = wrapperDataList.Count % fetch == 0 ? wrapperDataList.Count / fetch : (wrapperDataList.Count / fetch + 1);
                    for (var i = 0; i < bufferCount; i++)
                    {
                        var items = wrapperDataList.Skip(i * fetch).Take(fetch).ToList();
                        //var records = items.Select(m => m.Data).ToList();
                        //var bufferData = _objectMapper.Map<List<TRecord>, List<TEntity>>(records);
                        //await Repository.BulkInsert(bufferData);
                        //AckedTags.AddRange(items.Select(x => x.Tag));

                        foreach (var wrapperData in items)
                        {
                            await EventProcess(wrapperData);
                        }
                        await Task.Delay(100);
                    }
                }
                AckedTags.AddRange(wrapperDataList.Select(x => x.Tag));
            }
            catch (Exception e)
            {
                Logger.LogError($"批量处理失败{e.Message}", e);
                //foreach (var wrapperData in wrapperDataList)
                //{
                //    await EventProcess(wrapperData);
                //}
                AckedTags.Clear();
            }

            #endregion

        }

        /// <summary>
        /// 子类消费者消费委托逻辑 处理批量消息数据
        /// </summary>
        /// <param name="eventDataList">从消息队列二进制转成的utf8 批量字符串 消息数据</param>
        /// <returns></returns>
        protected virtual async Task BatchEventHandler(List<(string utf8Str, ulong tag)> eventDataList)
        {
            var wrapperDataList = new List<(WrapperData<TRecord>, ulong)>();
            foreach (var eventData in eventDataList)
            {
                var record = eventData.utf8Str.Deserialize<WrapperData<TRecord>>();
                wrapperDataList.Add((record, eventData.tag));
            }
            await BatchEventProcess(wrapperDataList);
        }
    }
}
