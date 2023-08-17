using Parakeet.Net.Enums;
using Parakeet.Net.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Parakeet.Net.Entities
{
    /// <summary>
    /// 中介器:转发配置
    /// </summary>
    [Table("Parakeet_Mediators", Schema = "public")]
    [Description("转发器")]
    public class Mediator : BaseEntity
    {
        public Mediator()
        {
        }

        public Mediator(Guid id)
        {
            base.SetEntityPrimaryKey(id);
        }

        #region 基础字段

        /// <summary>
        /// 服务端名称
        /// </summary>
        [Description("服务端名称")]
        [MaxLength(CustomerConsts.MaxLength255)]
        public string Name { get; set; }

        /// <summary>
        /// 服务端区域码
        /// </summary>
        [Required]
        [Description("服务端区域码")]
        [MaxLength(CustomerConsts.MaxLength16)]
        public string Area { get; set; }

        /// <summary>
        /// 设备类型
        /// </summary>
        [Required]
        [Description("设备类型")]
        public DeviceType DeviceType { get; set; }

        /// <summary>
        /// 服务地址
        /// </summary>
        [Required]
        [Description("服务地址")]
        [MaxLength(CustomerConsts.MaxLength64)]
        public string Host { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        [Description("端口")]
        public int Port { get; set; } = 80;

        /// <summary>
        /// 接口地址
        /// </summary>
        [Description("接口地址")]
        [MaxLength(CustomerConsts.MaxLength255)]
        public string Uri { get; set; }

        /// <summary>
        /// 传输协议
        /// </summary>
        [Description("传输协议")]
        public TransportProtocolType Protocol { get; set; }

        /// <summary>
        /// 处理类
        /// </summary>
        [Required]
        [Description("处理类")]
        public MediatorHandlerType HandlerType { get; set; }

        #endregion

        #region 设备转发

        /// <summary>
        /// 设备转发
        /// </summary>
        [JsonIgnore]
        public virtual ICollection<DeviceMediator> DeviceMediators { get; set; } = new List<DeviceMediator>();

        #endregion

        #region 扩展

        /// <summary>
        /// 重写ToString()方便消息队列取值
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Area}.{DeviceType.ToInt()}.{HandlerType.DisplayName()}";
        }

        /// <summary>
        /// 获取请求url
        /// </summary>
        [Description("获取请求url")]
        public virtual string Url => Port == 80 || Port == 443 ? $"{Host}{Uri}" : $"{Host}:{Port}{Uri}";

        /// <summary>
        /// 获取请求Host+Port  获取token只需要host:port
        /// </summary>
        [Description("获取请求Host+Port  获取token只需要host:port")]
        public virtual string HostPortString => Port == 80 || Port == 443 ? $"{Host}" : $"{Host}:{Port}";

        #endregion
    }
}
