using Common.RabbitMQModule.Core;
using Common.RabbitMQModule.Tests;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client.Events;

namespace Parakeet.Net.Producer.Standard.Delay
{
    /// <summary>
    /// 延迟队列存储的对象是对应的延迟消息；所谓“延迟消息” 是指当消息被发送以后，并不想让消费者立刻拿到消息，
    /// 而是等待特定时间后，消费者才能拿到这个消息进行消费。
    /// </summary>
    public class DelaySend
    {
        private static string _exchangeNormal = "Exchange.Normal";  //定义一个用于接收 正常 消息的交换机
        private static string _exchangeRetry = "Exchange.Retry";    //定义一个用于接收 重试 消息的交换机
        private static string _exchangeFail = "Exchange.Fail";      //定义一个用于接收 失败 消息的交换机
        private static string _queueNormal = "Queue.Noraml";        //定义一个用于接收 正常 消息的队列
        private static string _queueRetry = "Queue.Retry";          //定义一个用于接收 重试 消息的队列
        private static string _queueFail = "Queue.Fail";            //定义一个用于接收 失败 消息的队列

        /// <summary>
        /// 延迟(死信)交换机 生产者 发送消息测试
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
                    // 注意，重复声明交换机和队列会报lock错误，重复调用此接口会引发此问题，建议测试将交换机声明也交给消费者
                    // 交换机的声明交给生产者 队列的声明交给消费者【实际工作中应该这么做】，
                    // 消费者来绑定队列到指定交换机和路由键
                    // 生产者只需要往交换机指定路由键里面扔数据即可
                    //channel.ExchangeDeclare(exchange: _exchangeNormal, type: ExchangeType.Direct, durable: true);
                    //channel.ExchangeDeclare(exchange: _exchangeRetry, type: ExchangeType.Direct, durable: true);
                    //channel.ExchangeDeclare(exchange: _exchangeFail, type: ExchangeType.Direct, durable: true);


                    //生产者通道代理声明队列及队列的routingkey
                    //定义队列参数
                    var queueNormalArgs = new Dictionary<string, object>();
                    {
                        //指定死信交换机，用于将 Noraml 队列中失败的消息投递给 Fail 交换机
                        queueNormalArgs.Add("x-dead-letter-exchange", _exchangeFail);
                        //设置DLX的路由key，DLX会根据该值去找到死信消息存放的队列
                        queueNormalArgs.Add("x-dead-letter-routing-key", "queueFail");
                        //设置消息的存活时间，即过期时间
                    }

                    var queueRetryArgs = new Dictionary<string, object>();
                    {
                        //指定死信交换机，用于将 Retry 队列中超时的消息投递给 Noraml 交换机
                        queueRetryArgs.Add("x-dead-letter-exchange", _exchangeFail);

                        //设置DLX的路由key，DLX会根据该值去找到死信消息存放的队列
                        queueRetryArgs.Add("x-dead-letter-routing-key", "queueFail");

                        //定义 queueRetry 的消息最大停留时间 (原理是：等消息超时后由 broker 自动投递给当前绑定的死信交换机)
                        //定义最大停留时间为防止一些 待重新投递 的消息、没有定义重试时间而导致内存溢出
                        queueRetryArgs.Add("x-message-ttl", 6000);
                    }

                    var queueFailArgs = new Dictionary<string, object>();
                    {
                        //暂无
                    }

                    var queueNormal = new QueueInfo
                    {
                        Arguments = queueNormalArgs,
                        Queue = _queueNormal,
                        RoutingKey = "queueNormal",
                        Exchange = _exchangeNormal
                    };

                    var queueRetry = new QueueInfo
                    {
                        Arguments = queueRetryArgs,
                        Queue = _queueRetry,
                        RoutingKey = "queueRetry",
                        Exchange = _exchangeRetry
                    };

                    var queueFail = new QueueInfo
                    {
                        Arguments = queueFailArgs,
                        Queue = _queueFail,
                        RoutingKey = "queueFail",
                        Exchange = _exchangeFail
                    };

                    ////队列 持久化将 durable参数设置为true
                    //channel.QueueDeclare(queue: queueInfo1.Queue, durable: true);
                    //channel.QueueDeclare(queue: queueInfo2.Queue, durable: true);
                    //channel.QueueDeclare(queue: queueInfo3.Queue, durable: true);
                    //消费者通道代理将队列绑定到交换机，这时需要指定 路由键 routkey


                    Publish(queueNormal, channel);
                    //Publish(queueRetry, channel);
                    //Publish(queueFail, channel);

                    await Task.CompletedTask;

                    void Publish(QueueInfo info, IModel channel)
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            var message = $"RabbitMQ DCLSendTest {i + 1} Message=>RoutingKey：{info.RoutingKey}";
                            var body = Encoding.UTF8.GetBytes(message);
                            //发送消息的时候，需要指定routingkey发送
                            var properties = channel.CreateBasicProperties();
                            properties.Persistent = true;
                            channel.BasicPublish(
                                exchange: info.Exchange,
                                routingKey: info.RoutingKey,
                                mandatory: false,
                                basicProperties: properties,
                                body: body);
                            Console.WriteLine($"Send to DCLSendTest: {message}");
                            //Task.Delay(1000);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 死信交换机 消费者 接收消息测试
        /// </summary>
        public static async Task ConsumerMessage()
        {
            //获取rabbitmq连接 生产者可以写using
            var connection = RabbitMQTestHelper.GetConnection();
            {
                //在连接基础上建立一个消费者通道代理
                var channel = connection.CreateModel();
                {
                    // 注意，重复声明交换机和队列会报lock错误，
                    // 交换机的声明交给生产者 队列的声明交给消费者，
                    // 消费者来绑定队列到指定交换机和路由键
                    // 生产者只需要往交换机指定路由键里面扔数据即可
                    channel.ExchangeDeclare(exchange: _exchangeNormal, type: ExchangeType.Direct, durable: true);
                    channel.ExchangeDeclare(exchange: _exchangeFail, type: ExchangeType.Direct, durable: true);

                    //消费者通道代理声明队列及队列的routingkey

                    //定义队列参数
                    var queueNormalArgs = new Dictionary<string, object>();
                    {
                        //队列过期时间
                        queueNormalArgs.Add("x-expires", 30000);
                        //队列上消息过期时间, 应小于队列过期时间
                        queueNormalArgs.Add("x-message-ttl", 12000);
                        //指定死信交换机，用于将 Noraml 队列中失败的消息投递给 Fail 交换机
                        queueNormalArgs.Add("x-dead-letter-exchange", _exchangeFail);
                        //设置DLX(死信交换机)的路由key，DLX会根据该值去找到死信消息存放的队列
                        queueNormalArgs.Add("x-dead-letter-routing-key", "queueFail");
                    }

                    var queueFailArgs = new Dictionary<string, object>();
                    {
                        //暂无
                    }
                    var queueNormal = new QueueInfo
                    {
                        Arguments = queueNormalArgs,
                        Queue = _queueNormal,
                        RoutingKey = "queueNormal",
                        Exchange = _exchangeNormal
                    };
                    var queueFail = new QueueInfo
                    {
                        Arguments = queueFailArgs,
                        Queue = _queueFail,
                        RoutingKey = "queueFail",
                        Exchange = _exchangeFail
                    };

                    //队列 持久化将 durable参数设置为true
                    channel.QueueDeclare(
                        queue: queueNormal.Queue,
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: queueNormal.Arguments);
                    channel.QueueDeclare(
                        queue: queueFail.Queue,
                        durable: true,
                        exclusive: true,
                        autoDelete: true,
                        arguments: queueFail.Arguments);
                    //消费者通道代理将队列绑定到交换机，这时需要指定 路由键 routkey
                    channel.QueueBind(queueNormal.Queue, queueNormal.Exchange, queueNormal.RoutingKey);
                    channel.QueueBind(queueFail.Queue, queueFail.Exchange, queueFail.RoutingKey);

                    //不设置交换机，消息过期后，直接进入死信队列
                    await Task.CompletedTask;
                }
            }
        }
    }
}
