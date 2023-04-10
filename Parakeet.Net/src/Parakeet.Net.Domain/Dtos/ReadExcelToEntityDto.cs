using Microsoft.AspNetCore.Http;
using NPOI.SS.UserModel;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 读取excel文件数据到实体参数
    /// </summary>
    public class ReadExcelToEntityDto
    {
        /// <summary>
        /// 上传的excel文件
        /// </summary>
        public IFormFile File { get; set; }

        /// <summary>
        /// excel文件绝对路径 (与File二选一)
        /// </summary>
        public string Path { get; set; } 
        
        /// <summary>
        /// 上传的excel文件
        /// </summary>
        public ISheet Sheet { get; set; }

        /// <summary>
        /// 读取数据开始行数默认0
        /// </summary>
        public int StartRowIndex{ get; set; } 

        /// <summary>
        /// 读取数据开始列数默认0
        /// </summary>
        public int StartColumnIndex{ get; set; }  

        /// <summary>
        /// 时间轴开始行数默认0
        /// </summary>
        public int DateRowIndex { get; set; }

        /// <summary>
        /// 忽略最后汇总行数默认0无汇总
        /// </summary>
        public int Skip { get; set; }
    }
}
