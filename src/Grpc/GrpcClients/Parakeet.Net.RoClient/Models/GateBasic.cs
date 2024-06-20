using System;

namespace Parakeet.Net.ROClient.Models
{
    /// <summary>
    /// 闸机设备基础信息
    /// </summary>
    public class GateBasic : DeviceBasic
    {
        public override string CommandName => "add_gate_basic";

    }
}