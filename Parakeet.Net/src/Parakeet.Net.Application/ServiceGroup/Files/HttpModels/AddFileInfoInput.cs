using System;
using System.Collections.Generic;

namespace Parakeet.Net.ServiceGroup.Files.HttpModels
{
    public class AddFileInfoInput
    {
        public string Id { get; set; }

        public string ContentType { get; set; }

        public string Filename { get; set; }

        public long Length { get; set; }

        /// <summary>
        ///     此文件是否是公开的，当为true时未认证用户也能访问该文件
        /// </summary>
        public bool IsPublic { get; set; }

        /// <summary>
        ///     此文件是否私有，当为true只能上传者自己才能访问该文件,如果IsPublic为false,IsPrivate为false,那么只要是认证用户都可以访问此文件
        /// </summary>
        public bool IsPrivate { get; set; }

        /// <summary>
        ///     当IsPublic为true时，可以设置公开的截至时间，时间到期后只能上传的用户访问该文件
        /// </summary>
        public DateTime? ExpiryPublicTime { get; set; }

        public Dictionary<string, object> Metadata { get; set; }
    }
}
