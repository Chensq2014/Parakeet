namespace Parakeet.Net.Dtos
{
    public class WrapperData<T> : WrapperData where T : DeviceRecordDto
    {
        public T Data { get; set; }
    }

    public class WrapperData
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; } = true;

        /// <summary>
        /// 响应代码
        /// 0:成功
        /// 1:秘钥错误
        /// 2:设备未注册
        /// </summary>
        public int Code { get; set; } = 0;

        /// <summary>
        /// 原始数据
        /// </summary>
        public byte[] Origin { get; set; }

        ///// <summary>
        ///// 设备转发器 在device里
        ///// </summary>
        //public DeviceMediatorDto DeviceMediator { get; set; }
    }
}
