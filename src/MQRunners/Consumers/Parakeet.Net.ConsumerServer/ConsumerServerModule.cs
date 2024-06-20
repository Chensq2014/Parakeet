using Common.RabbitMQModule.Core;
using Common.RabbitMQModule.Tests;
using Common.Storage;
using Parakeet.Net.Consumer;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace Parakeet.Net.ConsumerServer
{
    /// <summary>
    /// 启动module必须引用 AbpAspNetCoreModule 因为这个module才重新注册了IApplicationBuilder
    /// context.Services.AddObjectAccessor<IApplicationBuilder>()
    /// 必须显示引用 AbpAutofacModule 才可以使用依赖注入
    /// </summary>
    [DependsOn(typeof(ConsumerModule))]
    public class ConsumerServerModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(ConsumerServerModule)} Start  ConfigureServices ....");
            base.ConfigureServices(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(ConsumerServerModule)} End  ConfigureServices ....");
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(ConsumerServerModule)} Start  OnApplicationInitialization ....");

            var app = context.GetApplicationBuilder();
            var env = context.GetEnvironment();
            var configuration = context.GetConfiguration();

            AsyncHelper.RunSync(async () =>
            {
                try
                {
                    await ConsumerMessage();
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"启动消费者错误消息:{ex.Message}");
                }
            });
            base.OnApplicationInitialization(context);
            Log.Warning($"{{0}}", $"{CacheKeys.LogCount++}、Module启动顺序_{nameof(ConsumerServerModule)} End  OnApplicationInitialization ....");
        }


        /// <summary>
        /// Direct 消费者 接收消息测试 api
        /// </summary>
        public static async Task ConsumerMessage()
        {
            //获取rabbitmq连接 生产者可以写using
            var connection = RabbitMQTestHelper.GetConnection();
            {
                //在连接基础上建立一个消费者通道代理
                var channel = connection.CreateModel();
                {
                    //消费者通道代理声明direct交换机
                    var exchange = "direct_exchange";

                    channel.ExchangeDeclare(exchange, ExchangeType.Direct);

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

                    channel.QueueDeclare(queueInfo1.Queue);
                    channel.QueueDeclare(queueInfo2.Queue);
                    channel.QueueDeclare(queueInfo3.Queue);
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
