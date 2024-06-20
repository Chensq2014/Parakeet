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
    /// 服务接口 跨语言的(接口)模板文件，提供了一套接口(模板)(c#代码--接口/基类)
    /// GreeterService严格遵循了gRPC的格式要求，还约束了序列化工具 google protobuffer
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
        /// 不是一次性计算完，全部返回结果，而是分批次返回
        /// 15*500 然后一起返回
        /// 
        /// 500  500  500  500==
        /// 
        /// yield状态机
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
                await responseStream.WriteAsync(new BathTheCatResp() { Message = $"number++ ={++number}！" });
                await Task.Delay(500);//此处主要是为了方便客户端能看出流调用的效果
            }
            //难道不是一次性返回全部数据，而是一点点的返回？
        }

        public override async Task<IntArrayModel> SelfIncreaseClient(IAsyncStreamReader<BathTheCatReq> requestStream, ServerCallContext context)
        {
            IntArrayModel intArrayModel = new IntArrayModel();
            while (await requestStream.MoveNext())
            {
                intArrayModel.Number.Add(requestStream.Current.Id + 1);
                Log.Logger.Information($"SelfIncreaseClient Number {requestStream.Current.Id} 获取到并处理.");
                Thread.Sleep(100);
            }
            return intArrayModel;
        }

        public override async Task SelfIncreaseDouble(IAsyncStreamReader<BathTheCatReq> requestStream, IServerStreamWriter<BathTheCatResp> responseStream, ServerCallContext context)
        {
            while (await requestStream.MoveNext())
            {
                Log.Logger.Information($"SelfIncreaseDouble Number {requestStream.Current.Id} 获取到并处理.");
                await responseStream.WriteAsync(new BathTheCatResp { Message = $"number++ ={requestStream.Current.Id + 1}！" });
                await Task.Delay(500);//此处主要是为了方便客户端能看出流调用的效果
            }

        }

    }
}
