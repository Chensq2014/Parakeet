using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Common.Dtos;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Parakeet.Net.Azures
{
    /// <summary>
    /// Azure 服务接口
    /// </summary>
    public class AzureAppService : CustomerAppService, IAzureAppService
    {
        private readonly AzureOptionDto _azureOption;

        public AzureAppService(IOptionsMonitor<AzureOptionDto> azureOptionMonitor)
        {
            _azureOption = azureOptionMonitor.CurrentValue;
        }

        /// <summary>
        /// 获取AzureOption
        /// </summary>
        /// <returns></returns>
        public AzureOptionDto GetAzureOption()
        {
            return _azureOption;
        }

        /// <summary>
        /// 获取AzureBlobClient
        /// </summary>
        /// <returns></returns>
        public async Task<BlobContainerClient> GetBlobContainer()
        {
            var conn = GetAzureConnectionString();
            var container = new BlobContainerClient(conn, $"{_azureOption.AzureBlob.BlobReference}");
            await container.CreateIfNotExistsAsync();
            return container;
        }

        /// <summary>
        /// 获取AzureQueueClient
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<QueueClient> GetAzureQueueClient(RecieveAzureMessageDto input)
        {
            input.Conn ??= GetAzureConnectionString();
            var queueClient = new QueueClient(input.Conn, input.QueueName);
            await queueClient.CreateIfNotExistsAsync();
            return queueClient;
        }

        /// <summary>
        /// Recieve 接收消息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Response<QueueMessage>> RecieveMessageAsync(RecieveAzureMessageDto input)
        {
            var queueClient = await GetAzureQueueClient(input);
            var response = await queueClient.ReceiveMessageAsync();
            return response;
        }

        /// <summary>
        /// Recieve 批量接收消息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Response<QueueMessage[]>> RecieveMessagesAsync(RecieveAzureMessageDto input)
        {
            var queueClient = await GetAzureQueueClient(input);
            var responses = await queueClient.ReceiveMessagesAsync();
            return responses;
        }

        /// <summary>
        /// Peek 接收消息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Response<PeekedMessage>> PickMessageAsync(RecieveAzureMessageDto input)
        {
            var queueClient = await GetAzureQueueClient(input);
            var response = await queueClient.PeekMessageAsync();
            return response;
        }

        /// <summary>
        /// delete消息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Response> DeleteMessageAsync(DeleteAzureMessageDto input)
        {
            var queueClient = await GetAzureQueueClient(input);
            var response = await queueClient.DeleteMessageAsync(input.MessageId, input.PopReceipt);
            return response;
        }

        /// <summary>
        /// 向Azure队列发送消息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Response<SendReceipt>> SendAzureMessageAsync(SendAzureMessageDto input)
        {
            input.Conn ??= GetAzureConnectionString();
            return await SendMessageAsync(input);
        }


        /// <summary>
        /// 获取Azure 连接字符串
        /// </summary>
        /// <returns></returns>
        private string GetAzureConnectionString()
        {
            //_azureOption.AccountKeyDecrypted
            return $"DefaultEndpointsProtocol={_azureOption.Http};AccountName={_azureOption.AccountName};AccountKey={_azureOption.AccountKey};EndpointSuffix={_azureOption.EndpointSuffix}";
        }


        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task<Response<SendReceipt>> SendMessageAsync(SendAzureMessageDto input)
        {
            var queue = new QueueClient(input.Conn, input.QueueName);
            await queue.CreateIfNotExistsAsync();
            return await queue.SendMessageAsync(input.Json);
        }
    }
}
