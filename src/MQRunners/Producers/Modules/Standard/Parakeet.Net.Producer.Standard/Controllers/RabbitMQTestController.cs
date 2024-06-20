using Common.Dtos;
using Common.Entities;
using Common.RabbitMQModule.Core;
using Common.RabbitMQModule.Tests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Parakeet.Net.Controllers;
using Parakeet.Net.Producer.Standard.Confirm;
using Parakeet.Net.Producer.Standard.DCL;
using Parakeet.Net.Producer.Standard.Delay;
using Parakeet.Net.Producer.Standard.Direct;
using Parakeet.Net.Producer.Standard.Fanout;
using Parakeet.Net.Producer.Standard.Topic;
using Parakeet.Net.Producer.Standard.Worker;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Parakeet.Net.Producer.Standard.Controllers
{
    /// <summary>
    /// RabbitMqTest api 
    /// </summary>
    //[Route("api/[Controller]/[Action]"),ApiController]
    public class RabbitMQTestController : IOTControllerBase
    {
        private readonly IRepository<Device, Guid> _deviceRepository;
        private readonly IMQTest _mqTest;
        public RabbitMQTestController(IServiceProvider serviceProvider, IRepository<Device, Guid> deviceRepository, IMQTest mqTest) : base(serviceProvider)
        {
            _deviceRepository = deviceRepository;
            _mqTest = mqTest;
        }

        /// <summary>
        /// RabbitMQ 依赖注入测试
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ResponseWrapper<string>> MQTest()
        {
            //var device = await _deviceRepository.FirstOrDefaultAsync(m => m.IsEnable);//测试成功，正常使用repository
            var response = new ResponseWrapper<string>
            {
                Success = true,
                Data = "RabbitMQ 生产者推送Direct数据成功"
            };
            try
            { 
                var connection = _mqTest.GetConnection();//这样依赖注入每次都会开启一个连接 在这个连接里面再开启通道代理
                {
                    //在连接基础上建立一个生产者通道代理
                    var channel = connection.CreateModel();
                    {
                        //生产者通道代理声明direct交换机
                        var exchange = "direct_exchange";

                        // 注意，重复声明交换机和队列会报lock错误，重复调用此接口会引发此问题，建议测试将交换机声明也交给消费者
                        // 交换机的声明交给生产者 队列的声明交给消费者【实际工作中应该这么做】，
                        // 消费者来绑定队列到指定交换机和路由键
                        // 生产者只需要往交换机指定路由键里面扔数据即可

                        ////为解决重复调用此接口，声明交换机前，应判断此交换机名是否存在，存在就不再声明了
                        channel.ExchangeDeclare(exchange: exchange, type: ExchangeType.Direct, durable: true, autoDelete: false);

                        //生产者通道代理声明队列及队列的routingkey
                        var queueInfo = new QueueInfo
                        {
                            Queue = "direct_queue0",
                            RoutingKey = "routingKey0",
                            Exchange = exchange
                        };

                        //队列 持久化将 durable参数设置为true
                        //exclusive:的队列只对首次声明它的连接可见，并且在连接断开时自动删除
                        //auto-delete:断开时自动删除
                        channel.QueueDeclare(queue: queueInfo.Queue, durable: true, exclusive: false, autoDelete: false);
                        //消费者通道代理将队列绑定到交换机，这时需要指定 路由键 routkey
                        channel.QueueBind(queueInfo.Queue, queueInfo.Exchange, queueInfo.RoutingKey);

                        var consumer = new EventingBasicConsumer(channel);
                        consumer.Received += (obj, eventArgs) =>
                        {
                            var body = eventArgs.Body;
                            var message = Encoding.UTF8.GetString(body.ToArray());
                            Logger.LogInformation($"{eventArgs.Exchange}_{eventArgs.RoutingKey}_Received:{message}");
                            // 消费完成后需要手动签收消息，如果不写该代码就容易导致重复消费问题
                            channel.BasicAck(eventArgs.DeliveryTag, true); // 批量签收可以降低每次签收性能损耗
                        };

                        channel.BasicConsume(queueInfo.Queue, false, consumer);

                        for (int i = 0; i < 2; i++)
                        {
                            Logger.LogWarning($"生产者准备发送第第{i}次数据...................................");
                            await Task.Delay(3000);
                            Publish(queueInfo, channel);
                            Logger.LogWarning($"生产者发送第第{i}次数据完毕...................................");
                        }
                        await Task.CompletedTask;

                        void Publish(QueueInfo info, IModel model)
                        {
                            for (int i = 0; i < 10; i++)
                            {
                                var message = $"RabbitMQ Direct {i + 1} Message=>RoutingKey：{info.RoutingKey}";
                                var body = Encoding.UTF8.GetBytes(message);
                                //发送消息的时候，需要指定routingkey发送
                                var properties = model.CreateBasicProperties();
                                properties.Persistent = true;
                                //Publish中的 mandatory 说明：
                                //当mandatory设置为false时，出现上述情况broker会直接将消息丢弃;
                                //当mandatory标志位设置为true时，如果exchange根据自身类型和消息routingKey无法找到一个合适的queue存储消息，那么broker会调用basic.return方法将消息返还给生产者;
                                //通俗的讲，mandatory标志告诉broker代理服务器至少将消息route到一个队列中，否则就将消息return给发送者;
                                model.BasicPublish(
                                    exchange: info.Exchange,
                                    routingKey: info.RoutingKey,
                                    mandatory: false,
                                    basicProperties: properties,
                                    body: body);
                                Console.WriteLine($"Send to Direct: {message}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Data = $"错误消息：{ex.Message}";
            }
            return response;
        }

        /// <summary>
        /// RabbitMQ 生产者推送Direct数据测试api
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ResponseWrapper<string>> DirectProducerTest()
        {
            //var device = await _deviceRepository.FirstOrDefaultAsync(m => m.IsEnable);//测试成功，正常使用repository
            var response = new ResponseWrapper<string>
            {
                Success = true,
                Data = "RabbitMQ 生产者推送Direct数据成功"
            };
            try
            {
                await DirectSend.SendMessage();
                //await Task.Delay(10000);
                //await DirectProducerTest();
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Data = $"错误消息：{ex.Message}";
            }
            return response;
        }

        /// <summary>
        /// RabbitMQ 消费者接收Direct数据测试api
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ResponseWrapper<string>> DirectConsumerTest()
        {
            var response = new ResponseWrapper<string>
            {
                Success = true,
                Data = "RabbitMQ 消费者接收Direct数据成功"
            };
            try
            {
                await DirectSend.ConsumerMessage();
                await Task.Delay(5000);
                await DirectProducerTest();
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Data = $"错误消息：{ex.Message}";
            }
            return response;
        }
        
        
        /// <summary>
        /// RabbitMQ 生产者推送Worker数据测试api
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ResponseWrapper<string>> WorkerProducerTest()
        {
            //var device = await _deviceRepository.FirstOrDefaultAsync(m => m.IsEnable);
            var response = new ResponseWrapper<string>
            {
                Success = true,
                Data = "RabbitMQ 生产者推送Direct(Worker)数据成功"
            };
            try
            {
                await WorkerSend.SendMessage();
                //await Task.Delay(10000);
                //await WorkerProducerTest();
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Data = $"错误消息：{ex.Message}";
            }
            return response;
        }

        /// <summary>
        /// RabbitMQ 消费者接收Worker数据测试api
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ResponseWrapper<string>> WorkerConsumerTest()
        {
            var response = new ResponseWrapper<string>
            {
                Success = true,
                Data = "RabbitMQ 消费者接收Direct(Worker)数据成功"
            };
            try
            {
                await WorkerSend.ConsumerMessage();
                await Task.Delay(5000);
                await WorkerProducerTest();
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Data = $"错误消息：{ex.Message}";
            }
            return response;
        }

        /// <summary>
        /// RabbitMQ 生产者推送Fanout数据测试api
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ResponseWrapper<string>> FanoutProducerTest()
        {
            //var device = await _deviceRepository.FirstOrDefaultAsync(m => m.IsEnable);
            var response = new ResponseWrapper<string>
            {
                Success = true,
                Data = "RabbitMQ 生产者推送Fanout数据成功"
            };
            try
            {
                await FanoutSend.SendMessage();
                //await Task.Delay(10000);
                //await FanoutProducerTest();
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Data = $"错误消息：{ex.Message}";
            }
            return response;
        }

        /// <summary>
        /// RabbitMQ 消费者接收Fanout数据测试api
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ResponseWrapper<string>> FanoutConsumerTest()
        {
            var response = new ResponseWrapper<string>
            {
                Success = true,
                Data = "RabbitMQ 消费者接收Fanout数据成功"
            };
            try
            {
                await FanoutSend.ConsumerMessage();
                await Task.Delay(5000);
                await FanoutProducerTest();
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Data = $"错误消息：{ex.Message}";
            }
            return response;
        }

        /// <summary>
        /// RabbitMQ 生产者推送Topic数据测试api
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ResponseWrapper<string>> TopicProducerTest()
        {
            //var device = await _deviceRepository.FirstOrDefaultAsync(m => m.IsEnable);
            var response = new ResponseWrapper<string>
            {
                Success = true,
                Data = "RabbitMQ 生产者推送Topic数据成功"
            };
            try
            {
                await TopicSend.SendMessage();
                //await Task.Delay(10000);
                //await TopicProducerTest();
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Data = $"错误消息：{ex.Message}";
            }
            return response;
        }

        /// <summary>
        /// RabbitMQ 消费者接收Topic数据测试api
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ResponseWrapper<string>> TopicConsumerTest()
        {
            var response = new ResponseWrapper<string>
            {
                Success = true,
                Data = "RabbitMQ 消费者接收Topic数据成功"
            };
            try
            {
                await TopicSend.ConsumerMessage();
                await Task.Delay(5000);
                await TopicProducerTest();
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Data = $"错误消息：{ex.Message}";
            }
            return response;
        }


        /// <summary>
        /// RabbitMQ 生产者推送死信队列数据测试api
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ResponseWrapper<string>> DCLProducerTest()
        {
            //var device = await _deviceRepository.FirstOrDefaultAsync(m => m.IsEnable);
            var response = new ResponseWrapper<string>
            {
                Success = true,
                Data = "RabbitMQ 生产者推送Topic数据成功"
            };
            try
            {
                await DCLSend.SendMessage();
                //await Task.Delay(10000);
                //await DCLProducerTest();
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Data = $"错误消息：{ex.Message}";
            }
            return response;
        }

        /// <summary>
        /// RabbitMQ 消费者接收死信队列数据测试api
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ResponseWrapper<string>> DCLConsumerTest()
        {
            var response = new ResponseWrapper<string>
            {
                Success = true,
                Data = "RabbitMQ 消费者接收数据成功"
            };
            try
            {
                await DCLSend.ConsumerMessage();
                await Task.Delay(5000);
                await DCLProducerTest();
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Data = $"错误消息：{ex.Message}";
            }
            return response;
        }

        /// <summary>
        /// RabbitMQ 生产者推送死信(Delay)队列数据测试api
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ResponseWrapper<string>> DelayProducerTest()
        {
            //var device = await _deviceRepository.FirstOrDefaultAsync(m => m.IsEnable);
            var response = new ResponseWrapper<string>
            {
                Success = true,
                Data = "RabbitMQ 生产者推送Topic数据成功"
            };
            try
            {
                await DelaySend.SendMessage();
                //await Task.Delay(10000);
                //await DelayProducerTest();
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Data = $"错误消息：{ex.Message}";
            }
            return response;
        }

        /// <summary>
        /// RabbitMQ 消费者接收死信(Delay)队列数据测试api
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ResponseWrapper<string>> DelayConsumerTest()
        {
            var response = new ResponseWrapper<string>
            {
                Success = true,
                Data = "RabbitMQ 消费者接收数据成功"
            };
            try
            {
                await DelaySend.ConsumerMessage();
                await Task.Delay(5000);
                await DelayProducerTest();
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Data = $"错误消息：{ex.Message}";
            }
            return response;
        }

        /// <summary>
        /// RabbitMQ 生产者推送Confirm队列数据测试api
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ResponseWrapper<string>> ConfirmProducerTest()
        {
            //var device = await _deviceRepository.FirstOrDefaultAsync(m => m.IsEnable);
            var response = new ResponseWrapper<string>
            {
                Success = true,
                Data = "RabbitMQ 生产者推送Topic数据成功"
            };
            try
            {
                await ConfirmSend.SendMessage();
                //await Task.Delay(10000);
                //await ConfirmProducerTest();
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Data = $"错误消息：{ex.Message}";
            }
            return response;
        }

        /// <summary>
        /// RabbitMQ 消费者接收Confirm队列数据测试api
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ResponseWrapper<string>> ConfirmConsumerTest()
        {
            var response = new ResponseWrapper<string>
            {
                Success = true,
                Data = "RabbitMQ 消费者接收数据成功"
            };
            try
            {
                await ConfirmSend.ConsumerMessage();
                await Task.Delay(5000);
                await ConfirmProducerTest();
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Data = $"错误消息：{ex.Message}";
            }
            return response;
        }
    }
}
