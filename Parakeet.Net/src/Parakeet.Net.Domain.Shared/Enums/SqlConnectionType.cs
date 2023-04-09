using System.ComponentModel;

namespace Parakeet.Net.Enums
{
    /// <summary>
    /// 读/写
    /// </summary>
    [Description("读写分离")]
    public enum SqlConnectionType
    {
        [Description("读")]
        Read = 0,
        [Description("写")]
        Write = 10
    }
}
