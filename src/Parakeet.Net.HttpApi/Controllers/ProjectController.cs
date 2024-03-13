using Common.Dtos;
using Common.Entities;
using Common.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Parakeet.Net.Attributes;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Common.Enums;

namespace Parakeet.Net.Controllers
{
    /// <summary>
    /// 项目
    /// </summary>
    //[ApiExplorerSettings(GroupName = "V3")]
    //[CustomRoute(VersionType.V2)]
    [Route("/api/parakeet/project/[action]")]
    public class ProjectController : BaseEntityController<Project>
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IProjectAppService _projectAppService;
        public ProjectController(IProjectAppService projectAppService, IWebHostEnvironment environment) : base(projectAppService)
        {
            _projectAppService = projectAppService;
            _environment = environment;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ProjectMap(InputIdDto input)
        {
            ViewBag.ProjectId = input.Id;
            return View();
        }

        [HttpGet]
        public IActionResult Map()
        {
            var filePath = Path.Combine(_environment.WebRootPath,$@"/upload/projectMap/default.jpg");
            //var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            //文件名必须编码，否则会有特殊字符(如中文)无法在此下载。
            string encodeFilename = HttpUtility.UrlEncode(Path.GetFileName(filePath), Encoding.GetEncoding("UTF-8"));
            Response.Headers.Add("Content-Disposition", "inline; filename=" + encodeFilename);
            return File(filePath, "image/png");
        }

        /// <summary>
        /// UploadFile Request.Form.Files接收文件 Request.Form["uploadGuid"]传递uploadGuid
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UploadFile(InputIdDto input)//IFormFile uploadFile参数文件过大使用参数获取会出现序列化错误
        {
            await _projectAppService.UploadFile(input);
            return SJson();
        }

        /// <summary>
        /// 下载题库文件
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public virtual async Task<IActionResult> Tiku()
        {
            var filePath = Path.Combine(_environment.WebRootPath, $@"/upload/files/题库专家_1.0.6.apk");
            //var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            //return File(fs, "application/vnd.android.package-archive");
            //文件名必须编码，否则会有特殊字符(如中文)无法在此下载。
            string encodeFilename = HttpUtility.UrlEncode(Path.GetFileName(filePath), Encoding.GetEncoding("UTF-8"));
            Response.Headers.Add("Content-Disposition", "attachment; filename=" + encodeFilename);
            return File(filePath, "application/vnd.android.package-archive");
        }


        [HttpGet]
        [CustomRoute(VersionType.V1)]
        public IActionResult Error()
        {
            var path = $@"{_environment.WebRootPath}/upload/projectMap/default.jpg";
            //Stream fs = new FileStream(filePath, FileMode.Open,FileAccess.Read);
            Response.Headers.Add("content-disposition", "inline; filename=default.jpg");
            return File(path, "image/jpg");
        }

        [HttpGet]
        [CustomRoute(VersionType.V2, "Error")]
        public IActionResult Error2()
        {
            var path = $@"{_environment.WebRootPath}/upload/projectMap/default.jpg";
            //Stream fs = new FileStream(filePath, FileMode.Open,FileAccess.Read);
            Response.Headers.Add("content-disposition", "inline; filename=default.jpg");
            return File(path, "image/jpg");
        }
    }
}
