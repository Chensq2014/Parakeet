using Common.RabbitMQModule.Core;
using Common.RabbitMQModule.Tests;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client.Events;

namespace Parakeet.Net.Producer.Standard.Fanout
{
    /// <summary>
    /// ExchangeType.Fanout 广播模式 生产者 发送消息测试
    /// </summary>
    public class FanoutSend
    {
        /// <summary>
        /// ExchangeType.Fanout 生产者 发送消息测试(带优先级 Priority)
        /// 注意，重复声明交换机和队列会报lock错误，
        /// 交换机和队列的声明交给消费者，消费者来绑定队列到指定交换机和路由键
        /// 生产者只需要往交换机指定路由键里面扔数据即可
        /// </summary>
        public static async Task SendMessage()
        {
            //获取rabbitmq连接 生产者可以写using
            using var connection = RabbitMQTestHelper.GetConnection();
            {
                //在连接基础上建立一个生产者通道代理
                using var channel = connection.CreateModel();
                {
                    //生产者通道代理声明fanout交换机
                    var exchange = "fanout_exchange";

                    // 注意，重复声明交换机和队列会报lock错误，
                    // 交换机的声明交给生产者 队列的声明交给消费者，
                    // 消费者来绑定队列到指定交换机和路由键
                    // 生产者只需要往交换机指定路由键里面扔数据即可
                    channel.ExchangeDeclare(exchange: exchange, type: ExchangeType.Fanout, durable: true, autoDelete: false);

                    //生产者通道代理声明队列及队列的routingkey
                    var queueInfo1 = new QueueInfo
                    {
                        Queue = "fanout_queue1",
                        RoutingKey = "fanout_queue1",//fanout 广播模式 routkey设置无效
                        Exchange = exchange
                    };

                    var queueInfo2 = new QueueInfo
                    {
                        Queue = "fanout_queue2",
                        RoutingKey = "fanout_queue2",
                        Exchange = exchange
                    };

                    var queueInfo3 = new QueueInfo
                    {
                        Queue = "fanout_queue3",
                        RoutingKey = "fanout_queue3",
                        Exchange = exchange
                    };

                    //channel.QueueDeclare(queueInfo1.Queue);
                    //channel.QueueDeclare(queueInfo2.Queue);
                    //channel.QueueDeclare(queueInfo3.Queue);
                    ////生产者通道代理将队列绑定到交换机，这时需要指定 路由键 routkey
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
                            var message = $"RabbitMQ Fanout {i + 1} Message=>RoutingKey：{info.RoutingKey}";
                            var body = Encoding.UTF8.GetBytes(message);
                            //发送消息的时候，需要指定routingkey basicProperties发送
                            var props = channel.CreateBasicProperties();
                            props.Persistent = true;
                            //var priority = Console.ReadLine();
                            //props.Priority = (byte) int.Parse(priority);//设置消息优先级

                            //channel.BasicPublish(exchange:info.Exchange, routingKey:info.RoutingKey, mandatory:false, basicProperties:null, body:body);
                            channel.BasicPublish(exchange:info.Exchange, "" , mandatory: false, basicProperties: null, body: body);
                            Console.WriteLine($"Send to Fanout: {message}");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// ExchangeType.Fanout模式 消费者 接收消息测试 api
        /// </summary>
        public static async Task ConsumerMessage()
        {
            //获取rabbitmq连接 生产者可以写using
            var connection = RabbitMQTestHelper.GetConnection();
            {
                //在连接基础上建立一个消费者通道代理
                var channel = connection.CreateModel();
                {
                    //消费者通道代理声明fanout交换机
                    var exchange = "fanout_exchange";
                    // 注意，重复声明交换机和队列会报lock错误，
                    // 交换机的声明交给生产者 队列的声明交给消费者，
                    // 消费者来绑定队列到指定交换机和路由键
                    // 生产者只需要往交换机指定路由键里面扔数据即可
                    //channel.ExchangeDeclare(exchange: exchange, type: ExchangeType.Fanout, durable: true, autoDelete: false);

                    //消费者通道代理声明队列及队列的routingkey
                    var queueInfo1 = new QueueInfo
                    {
                        Queue = "fanout_queue1",
                        RoutingKey = "fanout_queue1",//fanout 广播模式 routkey设置无效
                        Exchange = exchange
                    };

                    var queueInfo2 = new QueueInfo
                    {
                        Queue = "fanout_queue2",
                        RoutingKey = "fanout_queue2",
                        Exchange = exchange
                    };

                    var queueInfo3 = new QueueInfo
                    {
                        Queue = "fanout_queue3",
                        RoutingKey = "fanout_queue3",
                        Exchange = exchange
                    };

                    //x-max-priority属性必须设置，否则消息优先级不生效
                    var dic = new Dictionary<string, object>();//{{"x-max-priority", 50}};
                    dic.Add("x-max-priority", 50);
                    channel.QueueDeclare(
                        queue: queueInfo1.Queue,
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: dic);

                    channel.QueueDeclare(
                        queue: queueInfo2.Queue,
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: dic);

                    channel.QueueDeclare(
                        queue: queueInfo3.Queue,
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: dic);

                    //消费者通道代理将队列绑定到交换机，这时需要指定 路由键 routkey fancout模式 routkey应该是无效的，此处跟queue名称一样
                    channel.QueueBind(queueInfo1.Queue, queueInfo1.Exchange, queueInfo1.RoutingKey);
                    channel.QueueBind(queueInfo2.Queue, queueInfo2.Exchange, queueInfo2.RoutingKey);
                    channel.QueueBind(queueInfo3.Queue, queueInfo3.Exchange, queueInfo3.RoutingKey);

                    //channel.BasicQos(prefetchSize: 0, prefetchCount: 50, global: false);//一次取50条


                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (obj, eventArgs) =>
                    {
                        var body = eventArgs.Body;
                        var message = Encoding.UTF8.GetString(body.ToArray());
                        Console.WriteLine($"{eventArgs.Exchange}_{eventArgs.RoutingKey}_Received:{message}");
                        // 消费完成后需要手动签收消息，如果不写该代码就容易导致重复消费问题
                        channel.BasicAck(deliveryTag:eventArgs.DeliveryTag, multiple:true); // 批量签收可以降低每次签收性能损耗
                    };

                    //await Task.Delay(10000);
                    channel.BasicConsume(queue: queueInfo1.Queue, autoAck: false, consumer: consumer);

                    //await Task.Delay(10000);
                    channel.BasicConsume(queueInfo2.Queue, false, consumer);

                    //await Task.Delay(10000);
                    channel.BasicConsume(queueInfo3.Queue, false, consumer);

                    await Task.CompletedTask;
                }
            }
        }
    }
}
