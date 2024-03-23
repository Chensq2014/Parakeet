using Common.RabbitMQModule.Core;
using Common.RabbitMQModule.Tests;
using RabbitMQ.Client;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client.Events;

namespace Parakeet.Net.Producer.Standard.Topic
{
    /// <summary>
    /// ExchangeType.Topic 主题模式(routingKey匹配) 生产者 发送消息测试
    /// </summary>
    public class TopicSend
    {
        /// <summary>
        /// Topic 生产者 发送消息测试 topic(主题模式：支持路由规则，routing key中可以含 .*/.#  【*匹配一个单词、#匹配多个单词】)
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
                    //生产者通道代理声明topic交换机
                    var exchange = "topic_exchange";

                    // 注意，重复声明交换机和队列会报lock错误，
                    // 交换机和队列的声明交给消费者，消费者来绑定队列到指定交换机和路由键
                    // 生产者只需要往交换机指定路由键里面扔数据即可
                    //channel.ExchangeDeclare(exchange, ExchangeType.Topic,durable:true,autoDelete:false);

                    //生产者通道代理声明队列及队列的routingkey
                    var queueInfo1 = new QueueInfo
                    {
                        Queue = "topic_queue1",
                        RoutingKey = "user.data.insert.update.delete",
                        Exchange = exchange
                    };

                    var queueInfo2 = new QueueInfo
                    {
                        Queue = "topic_queue2",
                        RoutingKey = "user.data.register",
                        Exchange = exchange
                    };

                    var queueInfo3 = new QueueInfo
                    {
                        Queue = "topic_queue3",
                        RoutingKey = "user.data.insert",
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
                            var message = $"RabbitMQ Topic {i + 1} Message=>RoutingKey：{info.RoutingKey}";
                            var body = Encoding.UTF8.GetBytes(message);
                            //发送消息的时候，需要指定routingkey发送
                            channel.BasicPublish(info.Exchange, info.RoutingKey, false, null, body);
                            Console.WriteLine($"Send to Topic: {message}");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// ExchangeType.Topic模式 消费者 接收消息测试 api
        /// </summary>
        public static async Task ConsumerMessage()
        {
            //获取rabbitmq连接 生产者可以写using
            var connection = RabbitMQTestHelper.GetConnection();
            {
                //在连接基础上建立一个消费者通道代理
                var channel = connection.CreateModel();
                {
                    //消费者通道代理声明topic交换机
                    var exchange = "topic_exchange";

                    channel.ExchangeDeclare(exchange, ExchangeType.Topic, durable: true, autoDelete: false);

                    //消费者通道代理声明队列及队列的routingkey
                    var queueInfo1 = new QueueInfo
                    {
                        Queue = "topic_queue1",
                        RoutingKey = "user.data.#",
                        Exchange = exchange
                    };

                    var queueInfo2 = new QueueInfo
                    {
                        Queue = "topic_queue2",
                        RoutingKey = "user.data.*",
                        Exchange = exchange
                    };

                    var queueInfo3 = new QueueInfo
                    {
                        Queue = "topic_queue3",
                        RoutingKey = "user.data.insert",
                        Exchange = exchange
                    };

                    channel.QueueDeclare(queue: queueInfo1.Queue, durable: true);
                    channel.QueueDeclare(queue: queueInfo2.Queue, durable: true);
                    channel.QueueDeclare(queue: queueInfo3.Queue, durable: true);
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
