using System.ComponentModel;
namespace Parakeet.Net.Enums
{
    /// <summary>
    /// 登录状态
    /// </summary>
    [Description("登录状态")]
    public enum LoginResult
    {
        /// <summary>
        /// 登录成功
        /// </summary>
        [Description("登录成功")]
        Success = 0,
        /// <summary>
        /// 用户不存在
        /// </summary>
        [Description("用户不存在")]
        NoUser = 10,
        /// <summary>
        /// 密码错误
        /// </summary>
        [Description("密码错误")]
        WrongPwd = 20,
        /// <summary>
        /// 验证码错误
        /// </summary>
        [Description("验证码错误")]
        WrongVerify = 30,
        /// <summary>
        /// 账号被冻结
        /// </summary>
        [Description("账号被冻结")]
        Frozen = 40
    }
}
