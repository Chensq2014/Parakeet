//using Common.Dtos;
//using Common.Entities;
//using Common.Extensions;
//using Common.Interfaces;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Mvc;
//using Serilog;
//using System;
//using System.IO;
//using System.Text;
//using System.Threading.Tasks;
//using System.Web;
//using Volo.Abp.Settings;

//namespace Parakeet.Net.Controllers
//{
//    /// <summary>
//    /// 需求
//    /// </summary>
//    [ApiController]
//    [ApiExplorerSettings(GroupName = "V1")]
//    [Route("/api/parakeet/need/[action]")]
//    //[Authorize]//默认策略 [Authorize(Scheme="CustomPolicy")]//自定义策略   [Authorize(Roles="admin,Custom")] 角色鉴权
//    public class NeedController : BaseNetEntityController<Need>
//    {
//        private readonly IWebHostEnvironment _environment;
//        private readonly INeedAppService _needAppService;
//        private readonly INeedAttachmentAppService _needAttachmentAppService;
//        //private readonly IPersonalCacheAppService _personalCacheAppService;
//        //private readonly ISettingProvider _settingProvider;
//        //private readonly IEmailSender _emailSender;
//        //private readonly IHttpClientFactory _clientFactory;
//        public NeedController(INeedAppService baseService,
//            INeedAttachmentAppService needAttachmentService,
//            IWebHostEnvironment environment,
//            //IPersonalCacheAppService personalCacheAppService,
//            //IHttpClientFactory clientFactory,
//            //IEmailSender emailSender,
//            ISettingProvider settingProvider) : base(baseService)
//        {
//            _needAppService = baseService;
//            _needAttachmentAppService = needAttachmentService;
//            _environment = environment;
//            //_personalCacheAppService = personalCacheAppService;
//            //_settingProvider = settingProvider;
//            //_clientFactory = clientFactory;
//            //_emailSender = emailSender;
//        }


//        [HttpGet]
//        public IActionResult Index()
//        {
//            return View();
//        }

//        [HttpGet]
//        public IActionResult Add()
//        {
//            return View();
//        }

//        [HttpGet, ResponseCache(Duration = 6000)]//缓存6000s
//        public IActionResult Create()
//        {
//            return View();
//        }

//        #region  POST: Need/Create

//        [HttpPost]
//        public async Task<IActionResult> Create([FromForm]NeedCreateDto input)
//        {
//            return DJson(await _needAppService.Create(input));
//        }
//        #endregion


//        /// <summary>
//        /// UploadFile Request.Form.Files接收文件 Request.Form["uploadGuid"]传递uploadGuid
//        /// </summary>
//        /// <param name="input"></param>
//        /// <returns></returns>
//        [HttpPost]
//        public async Task<IActionResult> UploadFile(InputIdDto input)
//        {
//            //直接使用IFormFile uploadFile参数文件过大使用参数获取会出现序列化错误
//            await _needAppService.UploadFiles(input);
//            return SJson();
//        }

//        /// <summary>
//        /// 提供给用户下载
//        /// </summary>
//        /// <param name="input"></param>
//        /// <returns></returns>
//        [HttpGet]
//        public async Task<IActionResult> Download(InputIdDto input)
//        {
//            var attachment = await _needAttachmentAppService.GetByPrimaryKey(input);
//            var path = attachment?.Attachment?.Path ?? Path.Combine(_environment.WebRootPath, attachment?.Attachment?.VirtualPath);
//            var memoryStream = new MemoryStream();
//            await using (var stream = new FileStream(path, FileMode.Open))
//            {
//                await stream.CopyToAsync(memoryStream);
//            }
//            memoryStream.Seek(0, SeekOrigin.Begin);
//            //文件名必须编码，否则会有特殊字符(如中文)无法在此下载。
//            string encodeFilename = HttpUtility.UrlEncode(Path.GetFileName(path), Encoding.GetEncoding("UTF-8"));
//            base.Response.Headers.Add("Content-Disposition", "attachment; filename=" + encodeFilename);
//            return new FileStreamResult(memoryStream, "application/octet-stream");
//        }

//        /// <summary>
//        /// 下载物理文件(给img标签 src属性)
//        /// </summary>
//        /// <param name="input"></param>
//        /// <returns></returns>
//        [HttpGet]
//        public async Task<IActionResult> DownloadFile(InputIdDto input)
//        {
//            var attachment = await _needAttachmentAppService.GetByPrimaryKey(input);
//            if (attachment is null)
//            {
//                return null;
//            }
//            var path = attachment?.Attachment?.Path ?? Path.Combine(_environment.WebRootPath, attachment?.Attachment?.VirtualPath);
//            return PhysicalFile(path, "image/jpg", Path.GetFileName(path));//PhysicalFile 直接返回物理文件,给href
//        }

//        /// <summary>
//        /// 提供给用户删除文件
//        /// </summary>
//        /// <param name="input"></param>
//        /// <returns></returns>
//        [HttpPost]
//        public async Task<IActionResult> RemoveAttachment(InputIdDto input)
//        {
//            var attachment = await _needAttachmentAppService.GetByPrimaryKey(input);
//            if (attachment?.Attachment is null)
//            {
//                return null;
//            }
//            var path = attachment.Attachment.Path ?? $@"{_environment.WebRootPath}{attachment.Attachment.VirtualPath}";
//            FileExtension.ClearFile(path);
//            FileExtension.CheckClearParentDir(path);
//            await _needAttachmentAppService.GetBaseRepository().DeleteAsync(input.Id);
//            return SJson();
//        }



//        /// <summary>
//        /// 打开邮件时自动进入 <img style='display:none;' src='" + domail + "User/IsReader?username=zhangsan' />
//        /// </summary>
//        /// <param name="input"></param>
//        /// <returns></returns>
//        [HttpGet]
//        public async Task ReadEmail(InputIdDto input)
//        {
//            //如果能进入，代表别人已经看了
//            //统计input.Id 已经看了多少次
//            var need = await _needAppService.GetByPrimaryKey(input);
//            need.ReadTime = DateTime.Now;
//            Log.Logger.Information($"{need.Name}{need.ReadTime}读取邮件");
//        }
//    }
//}