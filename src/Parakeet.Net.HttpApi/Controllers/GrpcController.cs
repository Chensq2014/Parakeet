//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using Grpc.Core;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Logging;
//using Parakeet.Net.Filters;
//using Parakeet.Net.GrpcLessonServer;
//using Parakeet.Net.GrpcService;
//using Volo.Abp.AspNetCore.Mvc;

//namespace Parakeet.Net.Controllers
//{
//    [Route("/api/[controller]/[action]")]
//    public class GrpcController : AbpController
//    {
//        private readonly CustomMath.CustomMathClient _customMathClient;
//        private readonly Lesson.LessonClient _lessonhClient;
//        public GrpcController(CustomMath.CustomMathClient customMathClient, Lesson.LessonClient lessonhClient)
//        {
//            _customMathClient = customMathClient;
//            _lessonhClient = lessonhClient;
//        }

//        /// <summary>
//        /// Grpc测试
//        /// </summary>
//        /// <returns></returns>
//        [HttpGet]
//        [TypeFilter(typeof(CustomActionFilterAttribute),Order = -1)]//TypeFilter方式，构造函数的参数默认反射注册，不再单独参数注册 order从小到大顺序
//        //[TypeFilter(typeof(CustomExceptionFilterAttribute))]
//        //[ServiceFilter(typeof(CustomExceptionFilterAttribute))]//ServiceFilter方式，参数需要单独先注册
//        [CustomIocFilterFactory(typeof(CustomExceptionFilterAttribute))]//同ServiceFilter方式一样，IFilterFactory 就是filter工厂，任何环节都可以用工厂代替实例，filter里面就有serviceProvider,既可依赖注入，但是参数必须先注册后，才能使用filter里面就有serviceProvider获取工厂创建的实例
//        public async Task<string> Index()
//        {
//            {
//                var reply = await _customMathClient.SayHelloAsync(new HelloRequestMath { Name = "Chensq" });
//                Logger.LogInformation($"CustomMath {Thread.CurrentThread.ManagedThreadId} 服务返回数据1:{reply.Message}");
//                base.ViewBag.Luck = reply.Message;
//            }
//            {
//                //var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiRWxldmVuIiwiRU1haWwiOiI1NzI2NTE3N0BxcS5jb20iLCJBY2NvdW50IjoieHV5YW5nQHpoYW94aUVkdS5OZXQiLCJBZ2UiOiIzMyIsIklkIjoiMTIzIiwiTW9iaWxlIjoiMTg2NjQ4NzY2NzEiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJBZG1pbiIsIlNleCI6IjEiLCJuYmYiOjE1OTA3NTgzNDcsImV4cCI6MTU5MDc2MTg4NywiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo1NzI2IiwiYXVkIjoiaHR0cDovL2xvY2FsaG9zdDo1NzI2In0.7vMHx62XENyhkksCjnT5AeT78K3zG-z7B3hzv8DGPDI";//请求IdertityServer获取token
//                //var headers = new Metadata { { "Authorization", $"Bearer {token}" } };
//                //var reply = await _lessonhClient.FindLessonAsync(new LessonRequest { Id = 523 }, headers);
//                var reply = await _lessonhClient.FindLessonAsync(new LessonRequest { Id = 523 });//会自动aop  获取token

//                Logger.LogInformation($"Lesson {Thread.CurrentThread.ManagedThreadId} 服务返回数据2:{reply.Lesson.Name}");
//                base.ViewBag.Luck = reply.Lesson.Name;
//            }
//            return ViewBag.Luck;
//        }
//    }
//}
