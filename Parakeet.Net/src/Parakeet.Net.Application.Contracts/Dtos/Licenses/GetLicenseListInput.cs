using System.ComponentModel;

namespace Parakeet.Net.Dtos.Licenses
{
    public class GetLicenseListInput : PagedInputDto
    {
        #region 基础过滤字段
        /// <summary>
        /// AppId
        /// </summary>
        [Description("AppId")]
        public string AppId { get; set; }

        /// <summary>
        /// AppKey
        /// </summary>
        [Description("AppKey")]
        public string AppKey { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Description("名称")]
        public string Name { get; set; }

        #endregion

    }
}