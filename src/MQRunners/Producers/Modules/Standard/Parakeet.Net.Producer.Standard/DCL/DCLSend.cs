using Common.RabbitMQModule.Core;
using Common.RabbitMQModule.Tests;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client.Events;

namespace Parakeet.Net.Producer.Standard.DCL
{
    /// <summary>
    /// 什么是死信队列?
    /// 死信队列是用于接收普通队列发生失败的消息，其原理与普通队列相同；
    /// > 失败消息如：被消费者拒绝的消息、TTL超时的消息、队列达到最大数量无法写入的消息；
    /// 死信队列创建方法：
    /// > 在创建普通队列时，在参数"x-dead-letter-exchange"中定义失败消息转发的目标交换机；
    /// > 再创建一个临时队列，订阅"x-dead-letter-exchange"中指定的交换机；
    /// > 此时的临时队列就能接收到普通队列失败的消息了；
    /// > 可在消息的 Properties.headers.x-death 属性中查询到消息投递源信息和消息被投递的次数；
    /// 
    /// 死信队列 生产者 发送消息测试
    /// 消息被拒(basic.reject or basic.nack)并且没有重新入队(requeue=false)；
    /// 当前队列中的消息数量已经超过最大长度。
    /// 消息在队列中过期，即当前消息在队列中的存活时间已经超过了预先设置的TTL(Time To Live)时间；
    /// 
    /// eg:为每个需要使用死信的业务队列配置一个死信交换机，这里同一个项目的死信交换机可以共用一个，然后为每个业务队列分配一个单独的路由key。
    /// 有了死信交换机和路由key后，接下来，就像配置业务队列一样，配置死信队列，然后绑定在死信交换机上。也就是说，死信队列并不是什么特殊的队列，
    /// 只不过是绑定在死信交换机上的队列。死信交换机也不是什么特殊的交换机，只不过是用来接受死信的交换机，所以可以为任何类型【Direct、Fanout、Topic】。
    /// 一般来说，会为每个业务队列分配一个独有的路由key，并对应的配置一个死信队列进行监听，也就是说，一般会为每个重要的业务队列配置一个死信队列。
    /// </summary>
    public class DCLSend
    {
        private static string _exchangeNormal = "Exchange.Normal";  //定义一个用于接收 正常 消息的交换机
        private static string _exchangeRetry = "Exchange.Retry";    //定义一个用于接收 重试 消息的交换机
        private static string _exchangeFail = "Exchange.Fail";      //定义一个用于接收 失败 消息的交换机
        private static string _queueNormal = "Queue.Noraml";        //定义一个用于接收 正常 消息的队列
        private static string _queueRetry = "Queue.Retry";          //定义一个用于接收 重试 消息的队列
        private static string _queueFail = "Queue.Fail";            //定义一个用于接收 失败 消息的队列

        /// <summary>
        /// 死信交换机 生产者 发送消息测试
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
                    //channel.ExchangeDeclare(exchange: _exchangeNormal, type: ExchangeType.Topic, durable: true);
                    //channel.ExchangeDeclare(exchange: _exchangeRetry, type: ExchangeType.Topic, durable: true);
                    //channel.ExchangeDeclare(exchange: _exchangeFail, type: ExchangeType.Topic, durable: true);


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
                    channel.ExchangeDeclare(exchange: _exchangeNormal, type: ExchangeType.Topic, durable: true);
                    channel.ExchangeDeclare(exchange: _exchangeRetry, type: ExchangeType.Topic, durable: true);
                    channel.ExchangeDeclare(exchange: _exchangeFail, type: ExchangeType.Topic, durable: true);

                    //消费者通道代理声明队列及队列的routingkey

                    //定义队列参数
                    var queueNormalArgs = new Dictionary<string, object>();
                    {
                        //指定死信交换机，用于将 Noraml 队列中失败的消息投递给 Fail 交换机
                        queueNormalArgs.Add("x-dead-letter-exchange", _exchangeFail);
                        //设置DLX的路由key，DLX会根据该值去找到死信消息存放的队列
                        queueNormalArgs.Add("x-dead-letter-routing-key", "queueFail");
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
                        RoutingKey = "queueRetry",// "queueNormal",//
                        Exchange = _exchangeRetry
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
                        queue: queueRetry.Queue,
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: queueRetry.Arguments);
                    channel.QueueDeclare(
                        queue: queueFail.Queue,
                        durable: true,
                        exclusive: true,
                        autoDelete: true,
                        arguments: queueFail.Arguments);
                    //消费者通道代理将队列绑定到交换机，这时需要指定 路由键 routkey
                    channel.QueueBind(queueNormal.Queue, queueNormal.Exchange, queueNormal.RoutingKey);
                    channel.QueueBind(queueRetry.Queue, queueRetry.Exchange, queueRetry.RoutingKey);
                    channel.QueueBind(queueFail.Queue, queueFail.Exchange, queueFail.RoutingKey);

                    #region 创建一个普通消息消费者
                    {
                        var consumer = new EventingBasicConsumer(channel);
                        consumer.Received += (sender, eventArgs) =>
                        {
                            var _sender = (EventingBasicConsumer)sender;            //消息传送者
                            var _channel = _sender.Model;                           //消息传送通道
                            var _message = eventArgs;  //(BasicDeliverEventArgs)    //消息传送参数
                            var _headers = _message.BasicProperties.Headers;        //消息头
                            var _content = Encoding.UTF8.GetString(_message.Body.ToArray());  //消息内容
                            var _death = default(Dictionary<string, object>);       //死信参数
                            if (_headers != null && _headers.ContainsKey("x-death"))
                            {
                                _death = (Dictionary<string, object>)(_headers["x-death"] as List<object>)?[0];
                            }

                            try
                            {
                                #region 消息处理
                                Console.WriteLine();
                                Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fff")}\t(1.0)消息接收：\r\n\t[deliveryTag={_message.DeliveryTag}]\r\n\t[consumerID={_message.ConsumerTag}]\r\n\t[exchange={_message.Exchange}]\r\n\t[routingKey={_message.RoutingKey}]\r\n\t[content={_content}]");

                                throw new Exception("模拟消息处理失败效果。");

                                //处理成功时
                                Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fff")}\t(1.1)处理成功：\r\n\t[deliveryTag={_message.DeliveryTag}]");

                                var body = eventArgs.Body;
                                var message = Encoding.UTF8.GetString(body.ToArray());
                                Console.WriteLine($"{eventArgs.Exchange}_{eventArgs.RoutingKey}_Received:{message}");
                                // 消费完成后(autoAct为false的消费者)需要手动签收消息，如果不写该代码就容易导致重复消费问题
                                //消息确认 (销毁当前消息) 批量签收可以降低每次签收性能损耗
                                _channel.BasicAck(deliveryTag: eventArgs.DeliveryTag, multiple: true);
                                #endregion
                            }
                            catch (Exception ex)
                            {
                                #region 消息处理失败时
                                var retryCount = (long)(_death?["count"] ?? default(long)); //查询当前消息被重新投递的次数 (首次则为0)

                                Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fff")}\t(1.2)处理失败：\r\n\t[deliveryTag={_message.DeliveryTag}]\r\n\t[retryCount={retryCount}]");

                                if (DateTime.Now.Ticks % 2 == 0)//retryCount >= 2 || 
                                {
                                    #region 投递第3次还没消费成功时，就转发给 exchangeFail 交换机
                                    //消息拒绝（投递给死信交换机，也就是上边定义的 ("x-dead-letter-exchange", _exchangeFail)）
                                    _channel.BasicNack(deliveryTag: _message.DeliveryTag, multiple: true, requeue: false);

                                    #endregion 投递第3次还没消费成功时，就转发给 exchangeFail 交换机
                                }
                                else
                                {
                                    #region 否则转发给 exchangeRetry 交换机

                                    //定义下一次投递的间隔时间 (单位：秒)
                                    //如：首次重试间隔10秒、第二次间隔20秒、第三次间隔30秒
                                    var interval = 5; //(retryCount + 1) * 10; 

                                    //定义下一次投递的间隔时间 (单位：毫秒)
                                    _message.BasicProperties.Expiration = (interval * 1000).ToString();

                                    //将消息投递给 _exchangeRetry (会自动增加 death 次数)
                                    //_message.RoutingKey 为queueNormal 这里要使用 retry的routingkey
                                    _channel.BasicPublish(exchange: _exchangeRetry, routingKey: "queueRetry", basicProperties: _message.BasicProperties, body: _message.Body);

                                    //消息确认 (销毁当前消息)
                                    _channel.BasicAck(deliveryTag: _message.DeliveryTag, multiple: true);
                                    #endregion
                                }

                                #endregion 消息处理失败时
                            }
                        };

                        channel.BasicConsume(queueNormal.Queue, false, consumer);
                    }
                    #endregion 创建一个普通消息消费者



                    #region 创建一个失败消息消费者

                    //{
                    //    var consumer = new EventingBasicConsumer(channel);

                    //    consumer.Received += (sender, eventArgs) =>
                    //    {
                    //        var _message = (BasicDeliverEventArgs)eventArgs;                //消息传送参数
                    //        var _content = Encoding.UTF8.GetString(_message.Body.ToArray());  //消息内容

                    //        Console.WriteLine();
                    //        Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fff")}\t(2.0)发现失败消息：\r\n\t[deliveryTag={_message.DeliveryTag}]\r\n\t[consumerID={_message.ConsumerTag}]\r\n\t[exchange={_message.Exchange}]\r\n\t[routingKey={_message.RoutingKey}]\r\n\t[content={_content}]");
                    //    };
                    //    //await Task.Delay(10000);
                    //    channel.BasicConsume(queueFail.Queue, true, consumer);
                    //}
                    #endregion 创建一个失败消息消费者

                    await Task.CompletedTask;
                }
            }
        }
    }
}
