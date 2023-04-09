using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 上传文件dto
    /// </summary>
    public class ImportFileDto //: IValidatableObject
    {
        /// <summary>
        /// 上传验证码
        /// </summary>
        //[Required]
        public string Code { get; set; }
        /// <summary>
        /// 创建页面没有保存数据前，使用保存文件功能保存的缓存文件，生成一个全球唯一Guid字符串作为目录，
        /// ~/upload/temp/{UploadGuid}/目录下，以区别网站的并发情况
        /// </summary>
        [Description("上传Guid")]
        public string UploadGuid { get; set; }

        ///// <summary>
        ///// 手机
        ///// </summary>
        //[Description("手机"), Required, Regex(Regxes.MobilePhone)]
        //public string Phone { get; set; }

        /// <summary>
        /// 单文件上传
        /// </summary>
        public IFormFile UploadFile { get; set; }

        /// <summary>
        /// 上传文件 UploadFiles对应与上传控件dxFileUploader的名称
        /// </summary>
        [Description("上传文件")]
        public List<IFormFile> Files { get; set; } = new List<IFormFile>();

        #region 最多一次性上传20个附件

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
        //public IFormFile Files20 { get; set; }

        #endregion

        /// <summary>
        /// 自定义的上传文件最多20个...
        /// </summary>
        private int AddFiles()
        {
            Files.AddRange(new List<IFormFile>
            {
                Files0,Files1,Files2,Files3,Files4,Files5,Files6,Files7,Files8,Files9,
                Files10,Files11,Files12,Files13,Files14,Files15,Files16,Files17,Files18,Files19//,Files20
            });
            return Files.Count;
        }


        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    //if (!Phone.Validate())
        //    //{
        //    //    yield return new ValidationResult("请填写至少一个查询条件!");
        //    //}
        //    AddFiles();//验证完毕之后，初始化Files集合 此处仅为程序使用提供初始化服务，由前端控制上传个数验证
        //    if (Files.Count == 0)
        //    {
        //        yield return new ValidationResult("请上传一个文件!");
        //    }
        //}
    }
}
