using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Parakeet.Net.Entities
{
    //[Table("T_Licenses", Schema = "public")]
    public class Ticket : BaseEntity
    {
        public Ticket() { }
        public Ticket(Guid id)
        {
            SetEntityPrimaryKey(id);
        }

        public string Name { get; set; }
        /// <summary>
        /// AppId
        /// </summary>
        [Description("AppId"), MaxLength(CustomerConsts.MaxLength64)]
        public string AppId { get; set; }

        /// <summary>
        /// AppKey
        /// </summary>
        [Description("AppKey"), MaxLength(CustomerConsts.MaxLength64)]
        public string AppKey { get; set; }

        /// <summary>
        /// AppSecret
        /// </summary>
        [Description("AppSecret"), MaxLength(CustomerConsts.MaxLength255)]
        public string AppSecret { get; set; }

        /// <summary>
        /// Token
        /// </summary>
        [Description("Token")]
        public string Token { get; set; }

        /// <summary>
        /// Version
        /// </summary>
        [Description("Version")]
        public int Version { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        [Description("过期时间")]
        public DateTime ExpiredAt { get; set; }
    }
}
