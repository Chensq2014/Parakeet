using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Parakeet.Net.GrpcLessonServer
{
    public class LessonService : Lesson.LessonBase
    {
        private readonly ILogger<LessonService> _logger;
        public LessonService(ILogger<LessonService> logger)
        {
            _logger = logger;
        }

        //[Authorize("grpcEMail")]//Asp.NetCore
        public override Task<LessonReply> FindLesson(LessonRequest request, ServerCallContext context)
        {
            return Task.FromResult(new LessonReply()
            {
                Lesson = new LessonReply.Types.LessonModel
                {
                    Id = request.Id,
                    Name = "架构师蜕变营",
                    Remark = "行业精英"
                }
            });
        }
    }
}
