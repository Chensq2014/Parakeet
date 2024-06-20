using Common.RabbitMQModule.Core;
using Common.RabbitMQModule.Tests;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parakeet.Net.Producer.Standard.Confirm
{
    /// <summary>
    ///  消息确认
    /// </summary>
    public class ConfirmSend
    {
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
            var connection = RabbitMQTestHelper.GetConnection();
            {
                //在连接基础上建立一个生产者通道代理
                var channel = connection.CreateModel();
                {
                    //生产者通道代理声明direct交换机
                    var exchange = "confirm_exchange";

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
                        Queue = "confirm_queue1",
                        RoutingKey = "confirm_routingKey",
                        Exchange = exchange
                    };

                    Publish(queueInfo1, channel);
                    await Task.CompletedTask;
                }
            }
        }


        public static void Publish(QueueInfo info, IModel channel)
        {
            try
            {
                //用于将当前channel设置成transaction事务模式
                //channel.TxSelect();

                //方式1：普通/批量confirm
                //方式2：异步确认Ack
                channel.ConfirmSelect();//开启消息确认模式
                /*-------------Return机制：不可达的消息消息监听--------------*/
                //这个事件就是用来监听我们一些不可达的消息的内容的：比如某些情况下，如果我们在发送消息时，当前的exchange不存在或者指定的routingkey路由不到，这个时候如果要监听这种不可达的消息，就要使用 return
                var evreturn = new EventHandler<BasicReturnEventArgs>((o, basic) =>
                {
                    var rc = basic.ReplyCode; //消息失败的code
                    var rt = basic.ReplyText; //描述返回原因的文本。
                    var msg = Encoding.UTF8.GetString(basic.Body); //失败消息的内容 basic.Body.Span
                                                                   //在这里我们可能要对这条不可达消息做处理，比如是否重发这条不可达的消息呀，或者这条消息发送到其他的路由中等等
                                                                   //System.IO.File.AppendAllText("d:/return.txt", "调用了Return;ReplyCode:" + rc + ";ReplyText:" + rt + ";Body:" + msg);
                    Console.WriteLine("send message failed,不可达的消息消息监听.");
                });
                //消息发送成功的时候进入到这个事件：即RabbitMq服务器告诉生产者，我已经成功收到了消息
                var basicAcks = new EventHandler<BasicAckEventArgs>((o, basic) =>
                {
                    Console.WriteLine("send message success,Acks.");
                });
                //消息发送失败的时候进入到这个事件：即RabbitMq服务器告诉生产者，
                //你发送的这条消息我没有成功的投递到Queue中，或者说我没有收到这条消息。
                var basicNacks = new EventHandler<BasicNackEventArgs>((o, basic) =>
                {
                    //MQ服务器出现了异常，可能会出现Nack的情况
                    Console.WriteLine("send message fail,Nacks.");
                });
                channel.BasicReturn += evreturn;
                channel.BasicAcks += basicAcks;
                channel.BasicNacks += basicNacks;

                //批量发送消息
                for (int i = 0; i < 10; i++)
                {
                    var message = $"RabbitMQ Direct {i + 1} Message=>RoutingKey：{info.RoutingKey}";
                    var body = Encoding.UTF8.GetBytes(message);
                    //发送消息的时候，需要指定routingkey发送
                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;
                    properties.DeliveryMode = 2;//1（nopersistent）非持久化，2（persistent）持久化

                    //Publish中的 mandatory 说明：
                    //当mandatory设置为false时，出现上述情况broker会直接将消息丢弃;
                    //当mandatory标志位设置为true时，如果exchange根据自身类型和消息routingKey无法找到一个合适的queue存储消息，那么broker会调用basic.return方法将消息返还给生产者;
                    //通俗的讲，mandatory标志告诉broker代理服务器至少将消息route到一个队列中，否则就将消息return给发送者;

                    //contentType: 消息的内容类型，如：text / plain
                    //contentEncoding: 消息内容编码
                    //headers:设置消息的header,类型为Map<String, Object>
                    //deliveryMode:1（nopersistent）非持久化，2（persistent）持久化
                    //priority:消息的优先级
                    //correlationId:关联ID
                    //replyTo:用于指定回复的队列的名称
                    //expiration:消息的失效时间
                    //messageId:消息ID
                    //timestamp:消息的时间戳
                    //type:类型
                    //userId:用户ID
                    //appId：应用程序ID
                    //custerId:集群ID

                    channel.BasicPublish(
                        exchange: info.Exchange,
                        routingKey: info.RoutingKey,
                        mandatory: true,
                        basicProperties: properties,
                        body: body);
                    Console.WriteLine($"Send to Confirm Direct: {message}");
                    //if (!channel.WaitForConfirms())
                    //{
                    //    Console.WriteLine("send message failed.");
                    //}
                }

                if (!channel.WaitForConfirms())
                {
                    Console.WriteLine("send message failed.");
                }

                //channel.TxCommit();//txCommit用于提交事务

            }
            catch (Exception ex)
            {
                channel.TxRollback();
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
                    var exchange = "confirm_exchange";

                    // 注意，重复声明交换机和队列会报lock错误，重复调用此接口会引发此问题，建议测试将交换机声明也交给消费者
                    // 交换机的声明交给生产者 队列的声明交给消费者【实际工作中应该这么做】，
                    // 消费者来绑定队列到指定交换机和路由键
                    // 生产者只需要往交换机指定路由键里面扔数据即可
                    channel.ExchangeDeclare(exchange: exchange, type: ExchangeType.Direct, durable: true, autoDelete: false);

                    //消费者通道代理声明队列及队列的routingkey
                    var queueInfo1 = new QueueInfo
                    {
                        Queue = "confirm_queue1",
                        RoutingKey = "confirm_routingKey",
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
                    //消费者通道代理将队列绑定到交换机，这时需要指定 路由键 routkey
                    channel.QueueBind(queueInfo1.Queue, queueInfo1.Exchange, queueInfo1.RoutingKey);

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

                    await Task.CompletedTask;
                }
            }
        }
    }
}
