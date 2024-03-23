using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace Parakeet.Net.Web.Controllers
{
    public class HomeController : AbpController
    {
        /// <summary>
        /// 特性是编译时确定的，特性的构造函数只能是常量
        /// ServiceFilter:
        /// TypeFilter：
        /// </summary>
        /// <returns></returns>
        //[ServiceFilter(typeof(CustomExceptionFilterAttribute))]
        //[TypeFilter(typeof(CustomExceptionFilterAttribute))]//不需要再Module里面配置
        //[CustomIocFilterFactory(typeof(CustomExceptionFilterAttribute))]//IFilterFactory 就是filter工厂，任何环节都可以用工厂代替实例，filter里面就有serviceProvider,既可依赖注入
        //[DisableAuditing]
        [ResponseCache(Duration = 600)]//缓存600s
        public IActionResult Index()
        
        {
            #region Asp.net 6/5大对象
            //HttpContext:http请求的上下文，任何一个环节其实都是需要httpcontext，需要的参数信息，处理的中间结果，最终的结果，都是放在httpcontext，是一个贯穿始终的对象
            //所谓的6大对象，其实就是httpcontext的六大属性
            //base.HttpContext.Request:url参数form参数 url地址 urlreferer content-encoding，就是http请求提供的各种信息，后台里面可以拿到的
            //base.HttpContext.Request.Headers["user-Agent"];包括自定义--BasicAuth; Mvc5请求信息解读是 asp.net_iisapi按照http协议解析出来的。netcore是由kestrel??单独接口解析...

            //base.HttpContext.Response：响应，
            //Application:  mvc5 里面 应用程序内的多个会话和请求之间共享信息  用来做全局，多个用户共享，统计网站请求数等  netcore里面没有这个了
            //Server :mvc5 也就是个帮助类库
            //Session:base.HttpContext.Session 用户登录验证，登录时写入，验证时获取，验证码，跳转当前页，一个用户一个session，字典式
            //Cookie:Mvc5 用户登录验证，登录时写入，验证时获取 ValidateAntiForgeryToen 保存用户数据(记住账号,访问历史);一个用户一个cokie，字典氏

            //Http协议
            #endregion

            #region 渲染页面过程分析

            //MVC5:
            //View--ViewResult--ActionResult 
            //1、找cshtml---交给viewEngineCollection来findview
            //2、生成html输入response,找的是一个cshtml,这里变成了一个IView，就可以生成html输出了
            //如果你也需要写个框架，前后端混合在一起，才能生成动态数据，你会怎么设计呢？
            //设计--模板--模板特殊标签--根据标签替换 {{Name}}--根据标签替换--不同的数据会产生不同的页面(发布系统) 
            //但是，这种不够灵活，或者写不出来（多层对象属性，viewbg.tile或者for循环，需要反射，因为模板不可能太智能)
            //还有个思路：就是把整个cshtml生成一个类，把html当成字符串拼装起来，那就很灵活了，
            //既然是后台代码为什么修改后不用编译就能直接看到效果呢？因为再次被编译（md5文件监控，有变化就重新编译）
            //通过反射找到类型--找dll--找到C:\Users\parakeet\AppData\Local\Temp\VS\... 临时文件夹--4个dll 动态编译的dll
            //每个cshtml会生成一个类（WebViewPage）--execute方法，一个文件夹会生成一个dll
            //通过扩展 可以达到一套后台多套前端...

            //netcore 里面 前端页面的父类是 RazorPageBase
            #endregion

            return View();
            //return Redirect("/swagger");
        }

        /// <summary>
        /// 默认错误页
        /// </summary>
        /// <returns></returns>
        public IActionResult Error()
        {
            return View();
        }
    }
}
