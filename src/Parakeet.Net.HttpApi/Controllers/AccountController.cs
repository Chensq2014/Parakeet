using Common.Helpers;
using Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using Volo.Abp.Identity;

namespace Parakeet.Net.Controllers
{
    [AllowAnonymousAttribute]
    public class AccountController : NetController
    {
        //private ILoggerFactory _factory = null;
        //private ILogger<AccountController> _logger = null;
        //private readonly IRepository<AppUser> _userRepository;
        //private readonly IdentityUserManager _identityUserManager;
        //private readonly IDistributedCache<string> _cacheManager;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IPersonalCacheAppService _personalCacheAppService;
        public AccountController(
            //ILoggerFactory factory,
            //ILogger<AccountController> logger,
            //IRepository<AppUser> userRepository,
            //IdentityUserManager identityUserManager,
            //IDistributedCache<string> cacheManager,
            IHttpClientFactory clientFactory,
            IPersonalCacheAppService personalCacheAppService)
        {
            //_factory = factory;
            //_logger = logger;
            //_userRepository = userRepository;
            //_identityUserManager = identityUserManager;
            //_cacheManager = cacheManager;
            _personalCacheAppService = personalCacheAppService;
            _clientFactory = clientFactory;
        }
        [ResponseCache(Duration = 600)]//缓存600s
        public IActionResult Index()
        {
            //1、viewdata
            ViewData.Add(new KeyValuePair<string, object>("key", "value"));
            ViewData["key"] = "value";
            //2、viewbag
            ViewBag.CurrentDate = DateTime.Now;
            ViewBag.Name = "value2";
            //ViewBag与ViewData 都是同一个字典，相同key会覆盖
            //3、TempData 基于session存储，单词请求数据不丢失，跨页面传值，只用一次。
            TempData["CurrentUser"] = new IdentityUserDto
            {
                Name = "User",
                UserName = "1231432"
            };

            //this._logger.LogError("_logger");

            //base.HttpContext.Response.WriteAsync("Index Executed");
            //this._factory.CreateLogger<AccountController>().LogDebug("_factory");
            //return this.RedirectToAction("Temp");
            return View("Temp");
        }

        [ResponseCache(Duration = 600)]//缓存600s
        public IActionResult Temp()
        {
            base.ViewBag.User = TempData["CurrentUser"];
            return View();
        }

        #region 登录与退出登录 验证码 ---Volo.Abp.Account.Web.Areas.Account.Controllers Login
        ////[Authorize(Roles = "admin")]
        ////[Authorize(Policy = "AdvancedOnly")]

        //[HttpGet, AllowAnonymous]
        //public ViewResult Login()
        //{
        //    return View();
        //}

        //[HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        //public async Task<IActionResult> Login(LoginDto input)//(string account, string password, string verify, bool rememberMe)
        //{
        //    Logger.LogDebug($"{input.UserNameOrEmailAddress} 登陆系统");
        //    //var currentUser = await _userRepository.FirstOrDefaultAsync(m => m.UserName == input.Account);
        //    //var currentUser = await _identityUserManager.FindByLoginAsync(input.Account,input.Password);
        //    if (!input.Verify.ToUpper().Equals(await _personalCacheAppService.GetCacheCode()))
        //    {
        //        throw new UserFriendlyException("验证码已失效");
        //    }
        //    #region 访问identityserver的login
        //    var client = _clientFactory.CreateClient(CustomerConsts.AppName);
        //    client.BaseAddress = new Uri(CustomConfigurationManager.Configuration["AuthServer:Authority"]);
        //    var url = $@"/Account/Login";//{client.BaseAddress.Host}
        //    var content = new StringContent(JsonConvert.SerializeObject(input), Encoding.UTF8);
        //    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        //    await client.PostAsync(url, content);
        //    #endregion

        //    #region 第一种方法使用session：session + filter

        //    ////测试阶段不用验证验证码
        //    //// && verify.Equals(base.HttpContext.Session.GetString("CheckCode"),StringComparison.CurrentCultureIgnoreCase)
        //    //if (input.Verify.Equals(base.HttpContext.Session.GetString("CheckCode"), StringComparison.CurrentCultureIgnoreCase))
        //    //{
        //    //    if (currentUser != null)
        //    //    {
        //    //        base.HttpContext.Session.SetString("CurrentUser", Newtonsoft.Json.JsonConvert.SerializeObject(currentUser));
        //    //        if (!string.IsNullOrWhiteSpace(base.HttpContext.Session.GetString("CurrentUrl")))
        //    //        {
        //    //            string url = base.HttpContext.Session.GetString("CurrentUrl");
        //    //            base.HttpContext.Session.Remove("CurrentUrl");
        //    //            return base.Redirect(url);
        //    //        }
        //    //        else
        //    //        {
        //    //            return base.Redirect("/Home/Index");
        //    //        }
        //    //    }
        //    //    else
        //    //    {
        //    //        ModelState.AddModelError("failed", "用户名或密码错误");
        //    //    }
        //    //}
        //    //else
        //    //{
        //    //    ModelState.AddModelError("failed", "验证码错误");
        //    //}
        //    #endregion

        //    #region cookie
        //    //{
        //    //    //就很像一个CurrentUser,转成一个claimIdentity
        //    //    var claimIdentity = new ClaimsIdentity("Cookie");
        //    //    claimIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, currentUser.Id.ToString()));
        //    //    claimIdentity.AddClaim(new Claim(ClaimTypes.Name, currentUser.Name));
        //    //    claimIdentity.AddClaim(new Claim(ClaimTypes.Email, currentUser.Email));
        //    //    claimIdentity.AddClaim(new Claim(ClaimTypes.Role, currentUser.Role));
        //    //    var claimsPrincipal = new ClaimsPrincipal(claimIdentity);
        //    //    // 在上面注册AddAuthentication时，指定了默认的Scheme，在这里便可以不再指定Scheme。
        //    //    base.HttpContext.SignInAsync(claimsPrincipal).Wait();//不就是写到cookie
        //    //}
        //    #endregion

        //    return View();
        //}

        ///// <summary>
        ///// 请用TokenAuth退出登录
        ///// </summary>
        ///// <returns></returns>
        //public async Task<IActionResult> Logout()
        //{
        //    await base.HttpContext.SignOutAsync();
        //    var client = _clientFactory.CreateClient(CustomerConsts.AppName);
        //    client.BaseAddress = new Uri(CustomConfigurationManager.Configuration["AuthServer:Authority"]);
        //    var url = $@"Account/Logout";//{client.BaseAddress.Host}
        //    await client.GetAsync(url);
        //    return this.Redirect("~/");//Home/Index
        //}

        [AllowAnonymous]
        public void Verify()
        {
            var bitmap = ImageHelper.CreateVerifyCode(out string code);
            //base.HttpContext.Session.SetString("CheckCode", code);
            _personalCacheAppService.SetCacheCode(code);
            bitmap.Save(base.Response.Body, System.DrawingCore.Imaging.ImageFormat.Gif);//png/gif
            base.Response.ContentType = "image/gif";
        }

        [AllowAnonymous]
        public IActionResult VerifyCode()
        {
            var bitmap = VerifyCodeHelper.CreateVerifyCode(out string code);
            //base.HttpContext.Session.SetString("CheckCode", code);
            _personalCacheAppService.SetCacheCode(code);
            var stream = new MemoryStream();
            bitmap.Save(stream, ImageFormat.Gif);//png/gif
            return File(stream.ToArray(), "image/gif");
            //return _personalCacheAppService.GetValidCodeImage();
        }
        #endregion

    }
}