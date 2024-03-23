using Common.RabbitMQModule.Core;
using Common.RabbitMQModule.Tests;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client.Events;

namespace Parakeet.Net.Producer.Standard.Direct
{
    /// <summary>
    /// ExchangeType.Direct 直连模式 生产者 发送消息测试
    /// 什么是消息确认机制?
    /// MQ消息确认类似于数据库中用到的 commit 语句，用于告诉broker本条消息是被消费成功了还是失败了；
    /// 平时默认消息在被接收后就被自动确认了，需要在创建消费者时、设置 autoAck: false 即可使用手动确认模式；
    /// ====================================================================================================
    /// </summary>
    public class DirectSend
    {
        private readonly IMQTest _mqTest; 
        public DirectSend(IMQTest mqTest)
        {
            _mqTest = mqTest;
        }

        /// <summary>
        /// Direct 生产者 发送消息测试
        /// 注意，重复声明交换机和队列会报lock错误，
        /// 交换机和队列的声明交给消费者，消费者来绑定队列到指定交换机和路由键
        /// 生产者只需要往交换机指定路由键里面扔数据即可
        /// (业务上即使是交换机没有绑定任何队列，也不要影响生产者生产数据，所以声明交换机和绑定队列的代码写在消费者模块更合理)
        /// </summary>
        public static async Task SendMessage()
        {
            //获取rabbitmq连接 生产者可以写using 生产数据之后 可以丢掉连接 下次再重新打开连接即可
            //消费者的连接(TCP)是需要保持的，因为需要实时监听消费队列的数据
            using var connection = RabbitMQTestHelper.GetConnection();
            {
                //在连接基础上建立一个生产者通道代理
                using var channel = connection.CreateModel();
                {
                    //生产者通道代理声明direct交换机
                    var exchange = "direct_exchange";

                    // 注意，重复声明交换机和队列会报lock错误，重复调用此接口会引发此问题，建议测试将交换机声明也交给消费者
                    // 交换机的声明交给生产者 队列的声明交给消费者【实际工作中应该这么做】，
                    // 消费者来绑定队列到指定交换机和路由键
                    // 生产者只需要往交换机指定路由键里面扔数据即可

                    ////为解决重复调用此接口，声明交换机前，应判断此交换机名是否存在，存在就不再声明了
                    //channel.ExchangeDelete(exchange:exchange);//声明前先删除吧，如果没有会删除失败吗？
                    //channel.ExchangeDeclare(exchange: exchange, type: ExchangeType.Direct, durable: true, autoDelete: false);

                    //生产者通道代理声明队列及队列的routingkey
                    var queueInfo1 = new QueueInfo
                    {
                        Queue = "direct_queue1",
                        RoutingKey = "routingKey1",
                        Exchange = exchange
                    };

                    var queueInfo2 = new QueueInfo
                    {
                        Queue = "direct_queue2",
                        RoutingKey = "routingKey2",
                        Exchange = exchange
                    };

                    var queueInfo3 = new QueueInfo
                    {
                        Queue = "direct_queue3",
                        RoutingKey = "routingKey3",
                        Exchange = exchange
                    };


                    ////队列 持久化将 durable参数设置为true
                    ////exclusive:的队列只对首次声明它的连接可见，并且在连接断开时自动删除
                    ////auto-delete:断开时自动删除
                    //channel.QueueDeclare(queue: queueInfo1.Queue, durable: true, exclusive: false, autoDelete: false);
                    //channel.QueueDeclare(queue: queueInfo2.Queue, durable: true, exclusive: false, autoDelete: false);
                    //channel.QueueDeclare(queue: queueInfo3.Queue, durable: true, exclusive: false, autoDelete: false);
                    ////消费者通道代理将队列绑定到交换机，这时需要指定 路由键 routkey
                    //channel.QueueBind(queueInfo1.Queue, queueInfo1.Exchange, queueInfo1.RoutingKey);
                    //channel.QueueBind(queueInfo2.Queue, queueInfo2.Exchange, queueInfo2.RoutingKey);
                    //channel.QueueBind(queueInfo3.Queue, queueInfo3.Exchange, queueInfo3.RoutingKey);

                    Publish(queueInfo1, channel);
                    Publish(queueInfo2, channel);
                    Publish(queueInfo3, channel);

                    await Task.CompletedTask;

                    void Publish(QueueInfo info, IModel channel)
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            var message = $"RabbitMQ Direct {i + 1} Message=>RoutingKey：{info.RoutingKey}";
                            var body = Encoding.UTF8.GetBytes(message);
                            //发送消息的时候，需要指定routingkey发送
                            var properties = channel.CreateBasicProperties();
                            properties.Persistent = true;
                            //Publish中的 mandatory 说明：
                            //当mandatory设置为false时，出现上述情况broker会直接将消息丢弃;
                            //当mandatory标志位设置为true时，如果exchange根据自身类型和消息routingKey无法找到一个合适的queue存储消息，那么broker会调用basic.return方法将消息返还给生产者;
                            //通俗的讲，mandatory标志告诉broker代理服务器至少将消息route到一个队列中，否则就将消息return给发送者;
                            channel.BasicPublish(
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

        /// <summary>
        /// ExchangeType.Direct模式 消费者 接收消息测试 api
        /// </summary>
        public static async Task ConsumerMessage()
        {
            //获取rabbitmq连接 生产者可以写using
            var connection = RabbitMQTestHelper.GetConnection();
            {
                //在连接基础上建立一个消费者通道代理
                var channel = connection.CreateModel();
                {
                    //消费者通道代理声明direct交换机 持久化将 durable参数设置为true
                    var exchange = "direct_exchange";

                    // 注意，重复声明交换机和队列会报lock错误，重复调用此接口会引发此问题，建议测试将交换机声明也交给消费者
                    // 交换机的声明交给生产者 队列的声明交给消费者【实际工作中应该这么做】，
                    // 消费者来绑定队列到指定交换机和路由键
                    // 生产者只需要往交换机指定路由键里面扔数据即可
                    channel.ExchangeDeclare(exchange: exchange, type: ExchangeType.Direct, durable: true, autoDelete: false);

                    //消费者通道代理声明队列及队列的routingkey
                    var queueInfo1 = new QueueInfo
                    {
                        Queue = "direct_queue1",
                        RoutingKey = "routingKey1",
                        Exchange = exchange
                    };

                    var queueInfo2 = new QueueInfo
                    {
                        Queue = "direct_queue2",
                        RoutingKey = "routingKey2",
                        Exchange = exchange
                    };

                    var queueInfo3 = new QueueInfo
                    {
                        Queue = "direct_queue3",
                        RoutingKey = "routingKey3",
                        Exchange = exchange
                    };

                    //队列 持久化将 durable参数设置为true
                    //exclusive:的队列只对首次声明它的连接可见，并且在连接断开时自动删除
                    //auto-delete:断开时自动删除
                    channel.QueueDeclare(
                        queue: queueInfo1.Queue,
                        durable: true, 
                        exclusive: false,
                        autoDelete: false
                        //arguments: properties
                        );
                    channel.QueueDeclare(
                        queue: queueInfo2.Queue, 
                        durable: true, 
                        exclusive: false, 
                        autoDelete: false
                        );
                    channel.QueueDeclare(
                        queue: queueInfo3.Queue,
                        durable: true, 
                        exclusive: false, 
                        autoDelete: false);
                    //消费者通道代理将队列绑定到交换机，这时需要指定 路由键 routkey
                    channel.QueueBind(queueInfo1.Queue, queueInfo1.Exchange, queueInfo1.RoutingKey);
                    channel.QueueBind(queueInfo2.Queue, queueInfo2.Exchange, queueInfo2.RoutingKey);
                    channel.QueueBind(queueInfo3.Queue, queueInfo3.Exchange, queueInfo3.RoutingKey);

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (obj, eventArgs) =>
                    {
                        var body = eventArgs.Body;
                        var message = Encoding.UTF8.GetString(body.ToArray());
                        Console.WriteLine($"{eventArgs.Exchange}_{eventArgs.RoutingKey}_Received:{message}");
                        // 消费完成后需要手动签收消息，如果不写该代码就容易导致重复消费问题
                        channel.BasicAck(eventArgs.DeliveryTag, true); // 批量签收可以降低每次签收性能损耗
                    };

                    //await Task.Delay(10000);
                    channel.BasicConsume(queueInfo1.Queue, false, consumer);

                    await Task.Delay(5000);
                    channel.BasicConsume(queueInfo2.Queue, false, consumer);

                    await Task.Delay(5000);
                    channel.BasicConsume(queueInfo3.Queue, false, consumer);

                    await Task.CompletedTask;
                }
            }
        }
    }
}
