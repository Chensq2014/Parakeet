using Parakeet.Net.CustomAttributes;
using Parakeet.Net.Extensions;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Volo.Abp.Domain.Values;

namespace Parakeet.Net.ValueObjects
{
    /// <summary>
    ///     附件信息
    /// </summary>
    public class Attachment : ValueObject
    {
        /// <summary>
        /// 默认一个构造函数
        /// </summary>
        public Attachment(){}

        /// <summary>
        /// 附件
        /// </summary>
        /// <param name="name">附件名称</param>
        /// <param name="extention">附件扩展名</param>
        /// <param name="size">附件尺寸</param>
        /// <param name="virtualPath">附件扩展虚拟路劲默认为空</param>
        /// <param name="path">附件扩展路劲默认为空</param>
        /// <param name="key">附件key</param>
        public Attachment(string name, string extention, decimal? size = 0, string virtualPath = "", string path = "", string key = "")
        {
            Name = name;
            Extention = extention;
            Size = size;
            VirtualPath = virtualPath;
            Path = path;
            Key = key;
            //Content = content;
            //Bytes = bytes;
        }

        /// <summary>
        ///     文件key(使用外部文件服务产生的文件key)
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength128)]
        public string Key { get; set; }

        /// <summary>
        ///     文件Content字符串,Base64或json字符串
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        ///     文件二进制流
        /// </summary>
        public byte[] Bytes { get; set; }

        /// <summary>
        ///     名称(包含扩展名) //Path.HasValue() ? System.IO.Path.GetFileName(Path) : string.Empty;
        ///    (不含扩展名)      //Path.HasValue() ? Path.GetFileNameWithoutExtension(Path) : string.Empty;
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength255),Description("文件名"), NotSet]
        public string Name { get; set; }

        /// <summary>
        ///     文件扩展名 Path.HasValue() ? Path.GetExtension(Path)?.ToLower() : string.Empty;
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength128),Description("扩展名"), NotSet]
        public string Extention { get; set; }

        /// <summary>
        ///     文件大小
        /// </summary>
        public decimal? Size { get; set; }

        /// <summary>
        ///     扩展字段:文件绝对路径/url地址等备用
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength255),Description("文件绝对路径")]
        public string Path { get; set; }

        /// <summary>
        ///     扩展字段:文件虚拟路径（相对路径备用）
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength255),Description("虚拟路径")]
        public string VirtualPath { get; set; }

        /// <summary>
        /// 获取不含扩展名的文件名
        /// </summary>
        /// <param name="name">含扩展名的文件名</param>
        /// <returns></returns>
        public string GetFileNameWithoutExtension(string name)
        {
            return Name?.Split('.')?.FirstOrDefault() ?? string.Empty;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Name;
            yield return Extention;
            yield return Size;
            yield return Path;
            yield return VirtualPath;
            yield return Key;
            yield return Content;
            yield return Bytes;
        }
    }
}