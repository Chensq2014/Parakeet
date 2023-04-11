using Parakeet.Net.Enums;
using Parakeet.Net.Extensions;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 设备转发器
    /// </summary>
    public class MediatorDto : BaseDto
    {
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

        ///// <summary>
        ///// 多服务地址【含端口号】 转发模块可转发至多处 非必填字段
        ///// 适用于同content转发到多个服务器
        ///// </summary>
        //[Description("多服务地址")]
        //public string Hosts { get; set; }

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
        public virtual ICollection<DeviceMediatorDto> DeviceMediators { get; set; } = new List<DeviceMediatorDto>();

        #endregion

        #region 扩展

        /// <summary>
        /// 重写ToString()方便消息队列取值
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Area}.{DeviceType.ToInt()}.{HandlerTypeName}";
        }

        /// <summary>
        /// 处理类 MediatorHandlerType 的description
        /// </summary>
        public virtual string HandlerTypeName => HandlerType.DisplayName();

        /// <summary>
        /// 获取请求url
        /// </summary>
        /// <returns></returns>
        public virtual string Url => Port == 80 || Port == 443 ? $"{Host}{Uri}" : $"{Host}:{Port}{Uri}";

        /// <summary>
        /// 获取请求Host+Port  获取token只需要host:port
        /// </summary>
        /// <returns></returns>
        public virtual string HostPortString => Port == 80 || Port == 443 ? $"{Host}" : $"{Host}:{Port}";

        #endregion
    }
}
