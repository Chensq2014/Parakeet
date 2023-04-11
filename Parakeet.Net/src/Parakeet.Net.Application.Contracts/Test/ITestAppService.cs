using System.Threading.Tasks;

namespace Parakeet.Net.Test
{
    /// <summary>
    /// 测试TestAppService接口
    /// </summary>
    public interface ITestAppService
    {
        /// <summary>
        /// 测试IOperationTransient IOperationSingleton IOperationScoped
        /// </summary>
        /// <returns></returns>
        Task OnGetOperation();

        /// <summary>
        /// 获取加密密码字符串
        /// </summary>
        /// <param name="password">未加密密码字符串</param>
        /// <returns>加密密码字符串</returns>
        Task<string> GetEncryptedPassword(string password);

        /// <summary>
        /// 添加生产者
        /// </summary>
        /// <returns></returns>

        Task AddProducer();

        /// <summary>
        /// 单生产者单消费者测试
        /// </summary>
        /// <returns></returns>
        Task OneProducerToOneTestAppService();

        /// <summary>
        /// 多生产者单消费者并发批量消费测试
        /// </summary>
        /// <returns></returns>
        Task MultipleProducerToOneConsumerConcurrencyTest();

    }
}
