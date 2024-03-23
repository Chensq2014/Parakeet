using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Parakeet.Net.GrpcService.Services
{
    /// <summary>
    /// ����ӿ� �����Ե�(�ӿ�)ģ���ļ����ṩ��һ�׽ӿ�(ģ��)(c#����--�ӿ�/����)
    /// GreeterService�ϸ���ѭ��gRPC�ĸ�ʽҪ�󣬻�Լ�������л����� google protobuffer
    /// </summary>
    public class CustomMathService : CustomMath.CustomMathBase
    {
        private readonly ILogger<CustomMathService> _logger;
        public CustomMathService(ILogger<CustomMathService> logger)
        {
            _logger = logger;
        }

        public override Task<HelloReplyMath> SayHello(HelloRequestMath request, ServerCallContext context)
        {
            Log.Logger.Information($"this is Server SayHello {request.Name}");
            return Task.FromResult(new HelloReplyMath
            {
                Message = "Hello " + request.Name
            });
        }
        public override Task<CountResult> Count(Empty request, ServerCallContext context)
        {

            return Task.FromResult(new CountResult()
            {
                Count = DateTime.Now.Year
            });
        }

        public override Task<ResponseResult> Plus(RequestPara request, ServerCallContext context)
        {
            int iResult = request.ILeft + request.IRight;
            ResponseResult responseResult = new ResponseResult()
            {
                Result = iResult,
                Message = "Success"
            };

            return Task.FromResult(responseResult);
        }

        /// <summary>
        /// ����һ���Լ����꣬ȫ�����ؽ�������Ƿ����η���
        /// 15*500 Ȼ��һ�𷵻�
        /// 
        /// 500  500  500  500==
        /// 
        /// yield״̬��
        /// </summary>
        /// <param name="request"></param>
        /// <param name="responseStream"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task SelfIncreaseServer(IntArrayModel request, IServerStreamWriter<BathTheCatResp> responseStream, ServerCallContext context)
        {
            foreach (var item in request.Number)
            {
                int number = item;
                Log.Logger.Information($"This is {number} invoke");
                await responseStream.WriteAsync(new BathTheCatResp() { Message = $"number++ ={++number}��" });
                await Task.Delay(500);//�˴���Ҫ��Ϊ�˷���ͻ����ܿ��������õ�Ч��
            }
            //�ѵ�����һ���Է���ȫ�����ݣ�����һ���ķ��أ�
        }

        public override async Task<IntArrayModel> SelfIncreaseClient(IAsyncStreamReader<BathTheCatReq> requestStream, ServerCallContext context)
        {
            IntArrayModel intArrayModel = new IntArrayModel();
            while (await requestStream.MoveNext())
            {
                intArrayModel.Number.Add(requestStream.Current.Id + 1);
                Log.Logger.Information($"SelfIncreaseClient Number {requestStream.Current.Id} ��ȡ��������.");
                Thread.Sleep(100);
            }
            return intArrayModel;
        }

        public override async Task SelfIncreaseDouble(IAsyncStreamReader<BathTheCatReq> requestStream, IServerStreamWriter<BathTheCatResp> responseStream, ServerCallContext context)
        {
            while (await requestStream.MoveNext())
            {
                Log.Logger.Information($"SelfIncreaseDouble Number {requestStream.Current.Id} ��ȡ��������.");
                await responseStream.WriteAsync(new BathTheCatResp { Message = $"number++ ={requestStream.Current.Id + 1}��" });
                await Task.Delay(500);//�˴���Ҫ��Ϊ�˷���ͻ����ܿ��������õ�Ч��
            }

        }

    }
}
