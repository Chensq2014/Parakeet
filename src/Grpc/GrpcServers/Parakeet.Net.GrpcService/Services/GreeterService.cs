using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Parakeet.Net.GrpcService.Services
{
    /// <summary>
    /// 服务接口 跨语言的(接口)模板文件，提供了一套接口(模板)(c#代码--接口/基类)
    /// GreeterService严格遵循了gRPC的格式要求，还约束了序列化工具 google protobuffer
    /// </summary>
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name
            });
        }
    }
}
