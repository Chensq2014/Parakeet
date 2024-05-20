using Common.RabbitMQModule.Client;
using Common.RabbitMQModule.Core;
using Common.RabbitMQModule.Producers;

namespace Parakeet.Net.Shunt
{
    /// <summary>
    /// 分流模块生产者，要发消息到存储模块和转发模块或其它任何自定义模块
    /// </summary>
    public class ShuntProducer : RabbitMQProducer
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        /// <param name="rabbitMQClient">rabbitMQClient 默认依赖注入连接客户端</param>
        public ShuntProducer(IRabbitMQClient rabbitMQClient) : base(rabbitMQClient)
        {
        }

        /// <summary>
        /// 自定义构造函数
        /// </summary>
        /// <param name="rabbitMQClient">客户端连接</param>
        /// <param name="mqEventBus">mqEventBus 需要指定或者new一个传入</param>
        public ShuntProducer(IRabbitMQClient rabbitMQClient, RabbitMQEventBus mqEventBus) : base(rabbitMQClient, mqEventBus)
        {
        }
    }
}
