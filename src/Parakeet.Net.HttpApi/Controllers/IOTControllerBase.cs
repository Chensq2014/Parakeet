using Common;
using Common.Dtos;
using Common.Entities;
using Common.RabbitMQModule.Producers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Parakeet.Net.Caches;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc;

namespace Parakeet.Net.Controllers
{
    /// <summary>
    /// 物联网Controller基类
    /// </summary>
    [Route("api/iot/[controller]/[action]"), ApiController]
    public class IOTControllerBase : AbpController
    {
        protected readonly DevicePool DevicePool;
        protected readonly IProducerContainer ProducerContainer;
        protected static ConcurrentDictionary<Type, ProducerAttribute> ProducerAttributes = new ConcurrentDictionary<Type, ProducerAttribute>();

        public IOTControllerBase(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            //为了不让子类controller构造函数依赖注入，这里从serviceProvider中获取
            ProducerContainer = serviceProvider.GetService<IProducerContainer>();
            DevicePool = serviceProvider.GetService<DevicePool>();
        }

        /// <summary>
        /// 获取请求原始数据
        /// </summary>
        /// <returns></returns>
        protected async Task<byte[]> GetOrigin()
        {
            try
            {
                var stream = Request.Body;
                byte[] bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                // 设置当前流的位置为流的开始
                stream.Seek(0, SeekOrigin.Begin);
                return bytes;
            }
            catch (Exception ex)
            {
                using var reader = new StreamReader(Request.Body);
                var origin = await reader.ReadToEndAsync().ConfigureAwait(false);
                return Encoding.UTF8.GetBytes(origin);
            }
        }

        /// <summary>
        /// 发布至 MQ
        /// </summary>
        /// <param name="buffer">二进制数据</param>
        /// <param name="name">生产者容器名称</param>
        /// <returns></returns>
        protected async Task PublishAsync(byte[] buffer, string name)
        {
            await ProducerContainer.GetProducer(CommonConsts.AppName, name).PublishAsync(buffer);
        }

        /// <summary>
        /// 发布至 MQ
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="wrapperData">数据对象</param>
        /// <param name="routingKey">生产者名称</param>
        /// <returns></returns>
        protected async Task PublishAsync<T>(WrapperData<T> wrapperData, string routingKey) where T : DeviceRecordDto
        {
            var type = GetType();
            if (!ProducerAttributes.TryGetValue(type, out var producerAttribute))
            {
                var attribute = type.GetCustomAttribute(typeof(ProducerAttribute));
                producerAttribute = (ProducerAttribute)attribute ?? throw new NotSupportedException("未在Controller类上定义ProducerAttribute");
                ProducerAttributes.TryAdd(type, producerAttribute);
            }

            if (producerAttribute != null)
            {
                await ProducerContainer.GetProducer(producerAttribute.GroupName, type.Name).PublishAsync(wrapperData, routingKey);
            }
        }

        /// <summary>
        /// 发布至MQ
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="wrapperData">数据包装层</param>
        /// <param name="name">生产者名称</param>
        /// <param name="routingKey">路由键</param>
        /// <returns></returns>
        protected async Task PublishAsync<T>(WrapperData<T> wrapperData, string name, string routingKey) where T : DeviceRecordDto
        {
            await ProducerContainer.GetProducer(CommonConsts.AppName, name).PublishAsync(wrapperData, routingKey);
        }


        /// <summary>
        /// 发布至 MQ
        /// </summary>
        /// <param name="buffer">二进制数组</param>
        /// <param name="name">生产者名称</param>
        /// <param name="routeKey">路由键</param>
        /// <returns>设备</returns>
        protected async Task PublishAsync(byte[] buffer, string name, string routeKey)
        {
            await ProducerContainer.GetProducer(CommonConsts.AppName, name).PublishAsync(buffer, routeKey);
        }

        /// <summary>
        /// 根据序列号获取设备信息
        /// </summary>
        /// <param name="serialNo">设备序列号</param>
        /// <returns>设备</returns>
        protected virtual Device GetDeviceBySerialNo(string serialNo)
        {
            return DevicePool[serialNo];
        }

        /// <summary>
        /// 根据转发编码获取设备
        /// </summary>
        /// <param name="fakeNo">设备转发编码</param>
        /// <returns>设备</returns>
        protected virtual Device GetDeviceByFakeNo(string fakeNo)
        {
            return DevicePool.GetByFakeNo(fakeNo);
        }

        /// <summary>
        /// 测试ModelState返回值
        /// </summary>
        /// <returns></returns>
        protected virtual ResponseWrapper<List<string>> GetModelStateError()
        {
            var errorArray = new List<string>();
            foreach (var key in ModelState.Keys)
            {
                var ms = ModelState[key];
                var errors = ms.Errors;
                if (errors != null)
                {
                    foreach (var error in errors)
                    {
                        errorArray.Add(error.ErrorMessage);
                    }
                }
            }
            return new ResponseWrapper<List<string>>
            {
                Code = 400,
                Count = errorArray.Count,
                Success = false,
                Data = errorArray
            };
        }
    }
}
