namespace Parakeet.Net.Enums
{
    /// <summary>
    /// 转发类型 0:HTTP 10:TCP 20:UDP 30:SerialPort
    /// </summary>
    public enum TransportProtocolType
    {
        /// <summary>
        /// HTTP
        /// </summary>
        HTTP = 0,

        /// <summary>
        /// TCP
        /// </summary>
        TCP = 10,

        /// <summary>
        /// UDP
        /// </summary>
        UDP = 20,

        /// <summary>
        /// SerialPort
        /// </summary>
        SerialPort = 30
    }
}
