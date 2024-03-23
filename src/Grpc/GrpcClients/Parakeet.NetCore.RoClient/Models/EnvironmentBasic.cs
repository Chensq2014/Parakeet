using System;

namespace Parakeet.Net.ROClient.Models
{
    /// <summary>
    /// 环境设备基础信息
    /// </summary>
    public class EnvironmentBasic : DeviceBasic
    {
        public override string CommandName => "add_environment_basic";
    }
}