using Microsoft.AspNetCore.Http;
using Parakeet.Net.CustomAttributes;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Parakeet.Net.Dtos
{
    public class NeedCreateDto : NeedDto, IValidatableObject
    {
        /// <summary>
        /// 创建页面没有保存数据前，使用保存文件功能保存的缓存文件，生成一个全球唯一Guid字符串作为目录，
        /// ~/upload/temp/{UploadGuid}/目录下，以区别网站的并发情况
        /// </summary>
        [Description("上传Guid")]
        public string UploadGuid { get; set; }
        
        /// <summary>
        /// 验证码
        /// </summary>
        [Required]
        public string Code { get; set; }

        /// <summary>
        /// 上传文件 UploadFiles对应与上传控件dxFileUploader的名称
        /// </summary>
        [Description("上传文件")]
        public List<IFormFile> UploadFiles { get; set; } = new List<IFormFile>();
        public List<IFormFile> UploadFile { get; set; } = new List<IFormFile>();
        public List<IFormFile> Files { get; set; } = new List<IFormFile>();

        #region 上传20个附件

        public IFormFile Files0 { get; set; }
        public IFormFile Files1 { get; set; }
        public IFormFile Files2 { get; set; }
        public IFormFile Files3 { get; set; }
        public IFormFile Files4 { get; set; }
        public IFormFile Files5 { get; set; }
        public IFormFile Files6 { get; set; }
        public IFormFile Files7 { get; set; }
        public IFormFile Files8 { get; set; }
        public IFormFile Files9 { get; set; }
        public IFormFile Files10 { get; set; }
        public IFormFile Files11 { get; set; }
        public IFormFile Files12 { get; set; }
        public IFormFile Files13 { get; set; }
        public IFormFile Files14 { get; set; }
        public IFormFile Files15 { get; set; }
        public IFormFile Files16 { get; set; }
        public IFormFile Files17 { get; set; }
        public IFormFile Files18 { get; set; }
        public IFormFile Files19 { get; set; }

        #endregion

        /// <summary>
        /// 自定义的上传文件最多20个...
        /// </summary>
        public int AddFiles()
        {
            Files.AddRange(new List<IFormFile>
            {
                Files0,Files1,Files2,Files3,Files4,Files5,Files6,Files7,Files8,Files9,
                Files10,Files11,Files12,Files13,Files14,Files15,Files16,Files17,Files18,Files19
            });
            return Files.Count;
        }

        /// <summary>
        /// 验证方法 Validate在什么时候调用的
        /// </summary>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!Phone.Validate())
            {
                yield return new ValidationResult("请输入正确的电话号码!");
            }
            //AddFiles();//验证完毕之后，初始化Files集合 此处仅为程序使用提供初始化服务，由前端控制上传个数验证
            //if (Files.Count(m => m.Name.HasValue()) == 0)
            //{
            //    yield return new ValidationResult("至少上传1个文件");
            //}
            foreach (var formFile in Files)
            {
                if (formFile.Length > 2097152)
                {
                    yield return new ValidationResult("单个文件不能大于2MB");
                }
            }
        }
    }
}
