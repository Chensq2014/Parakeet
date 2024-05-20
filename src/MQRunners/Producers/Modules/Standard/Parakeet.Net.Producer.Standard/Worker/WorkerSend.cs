using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.RabbitMQModule.Core;
using Common.RabbitMQModule.Tests;
using RabbitMQ.Client.Events;
using Serilog;

namespace Parakeet.Net.Producer.Standard.Worker
{
    /// <summary>
    /// 默认工作模式(属于ExchangeType.Direct直连类型的默认交换机routkey模式) 只声明队列(使用默认交换机和routkey) 发送消息测试
    /// (默认交换隐式绑定到每个队列，路由密钥等于队列名称。无法显式绑定到默认exchange或从默认exchange取消绑定。也不能删除)
    /// </summary>
    public class WorkerSend
    {
        //(默认交换隐式绑定到每个队列，路由密钥等于队列名称。无法显式绑定到默认exchange或从默认exchange取消绑定。也不能删除)
        public static QueueInfo WorkerQueue = new QueueInfo
        {
            Queue = "worker_queue",
            RoutingKey = "worker_queue",//worker队列  RoutingKey与Queue相同
            //Exchange = exchange
        };

        /// <summary>
        /// Worker 生产者
        /// </summary>
        /// <returns></returns>
        public static async Task SendMessage()
        {
            //获取rabbitmq连接 生产者可以写using
            using var connection = RabbitMQTestHelper.GetConnection();
            {
                //在连接基础上建立一个生产者通道代理
                using var channel = connection.CreateModel();
                {
                    //生产者通道代理声明队列 队列只需要队列名称，RoutingKey和Exchange由系统默认
                    //var queueInfo = new QueueInfo
                    //{
                    //    Queue = "worker_queue",
                    //    RoutingKey = "worker_queue",//worker队列  routingKey是队列名称
                    //    //Exchange = exchange
                    //};

                    //此语句(声明队列)放消费者模块，由消费者先声明队列，产生队列后再运行此生产者，往此队列里面放消息
                    //channel.QueueDeclare(queueInfo.Queue,false,false,false,null);

                    for (int i = 0; i < 30; i++)
                    {
                        var message = $"RabbitMQ Worker {i + 1} Message";
                        var body = Encoding.UTF8.GetBytes(message);
                        channel.BasicPublish("", WorkerQueue.RoutingKey, false, null, body);
                        Console.WriteLine("send Task {0} message", i + 1);
                    }
                }
            }
        }

        /// <summary>
        /// worker工作模式 消费者 接收消息测试
        /// Exchange: (AMQP default) 是direct类型的默认交换机
        /// The default exchange is implicitly bound to every queue, with a routing key equal to the queue name.
        /// It is not possible to explicitly bind to, or unbind from the default exchange. It also cannot be deleted.
        /// (默认交换隐式绑定到每个队列，路由密钥等于队列名称。无法显式绑定到默认exchange或从默认exchange取消绑定。也不能删除)
        /// </summary>
        public static async Task ConsumerMessage()
        {
            //获取rabbitmq连接 生产者可以写using
            var connection = RabbitMQTestHelper.GetConnection();
            {
                //在连接基础上建立一个消费者通道代理
                var channel = connection.CreateModel();
                {
                    ////Worker方式使用默认交换机，消费者通道代理不需要声明 交换机
                    //var exchange = "direct_exchange";//Exchange: (AMQP default) 是direct类型的默认交换机

                    //channel.ExchangeDeclare(exchange, ExchangeType.Direct);

                    //消费者通道代理声明队列及队列的routingkey

                    channel.QueueDeclare(
                        queue: WorkerQueue.Queue,
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null
                        );

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (obj, eventArgs) =>
                    {
                        var body = eventArgs.Body;
                        var message = Encoding.UTF8.GetString(body.ToArray());
                        Console.WriteLine($"Worker Queue:{eventArgs.Exchange}_{eventArgs.RoutingKey}_Received:{message}");
                        // 消费完成后需要手动签收消息，如果不写该代码就容易导致重复消费问题
                        channel.BasicAck(eventArgs.DeliveryTag, true); // 批量签收可以降低每次签收性能损耗
                    };

                    channel.BasicConsume(WorkerQueue.Queue, false, "", false, false, null, consumer);

                    await Task.CompletedTask;
                }
            }
        }
    }
}
