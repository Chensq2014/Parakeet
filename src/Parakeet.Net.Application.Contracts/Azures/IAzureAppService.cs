using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Common.Dtos;
using System.Threading.Tasks;


namespace Parakeet.Net.Azures
{
    /// <summary>
    /// Azuer服务
    /// </summary>
    public interface IAzureAppService
    {
        /// <summary>
        /// 获取AzureOption
        /// </summary>
        /// <returns></returns>
        AzureOptionDto GetAzureOption();

        /// <summary>
        /// 获取AzureBlobClient
        /// </summary>
        /// <returns></returns>
        Task<BlobContainerClient> GetBlobContainer();

        /// <summary>
        /// 获取AzureQueueClient
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<QueueClient> GetAzureQueueClient(RecieveAzureMessageDto input);

        /// <summary>
        /// Recieve 接收消息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Response<QueueMessage>> RecieveMessageAsync(RecieveAzureMessageDto input);

        /// <summary>
        /// Recieve 批量接收消息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Response<QueueMessage[]>> RecieveMessagesAsync(RecieveAzureMessageDto input);

        /// <summary>
        /// Peek 接收消息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Response<PeekedMessage>> PickMessageAsync(RecieveAzureMessageDto input);

        /// <summary>
        /// delete消息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Response> DeleteMessageAsync(DeleteAzureMessageDto input);

        /// <summary>
        /// 向Azure队列发送消息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Response<SendReceipt>> SendAzureMessageAsync(SendAzureMessageDto input);



    }
}
