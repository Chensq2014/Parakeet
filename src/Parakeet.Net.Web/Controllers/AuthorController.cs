using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace Parakeet.Net.Controllers
{
    /// <summary>
    /// 作者简介
    /// </summary>
    [Route("/api/parakeet/author/[action]")]
    public class AuthorController : NetController
    {
        
        private readonly IWebHostEnvironment _environment;

        public AuthorController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet,ResponseCache(Duration = 600)]//缓存600s
        public IActionResult Profile()
        {
            var path = Path.Combine(_environment.WebRootPath, $@"/upload/files/profile.pdf");
            Response.Headers.Append("content-disposition", "inline; filename=profile.pdf");
            return File(path, "application/pdf");
        }

        [HttpGet, ResponseCache(Duration = 600)]//缓存600s
        public IActionResult Document()
        {
            var path = Path.Combine(_environment.WebRootPath, $@"/upload/files/profile.docx");
            Response.Headers.Append("content-disposition", "inline; filename=profile.docx");
            return File(path, "application/docx");
        }
    }
}
