namespace Parakeet.Net.Dtos.Requests
{
    /// <summary>
    /// 用户敏感信息返回类
    /// </summary>
    public class WeixinSpecialPropReturnDto: WebClientResultBase
    {
        /// <summary>
        /// 成员UserID
        /// </summary>
        public string Userid { get; set; }
        /// <summary>
        /// 性别。0表示未定义，1表示男性，2表示女性。仅在用户同意snsapi_privateinfo授权时返回真实值，否则返回0.
        /// </summary>
        public string Gender { get; set; }
        /// <summary>
        /// 头像url。仅在用户同意snsapi_privateinfo授权时返回
        /// </summary>
        public string Avatar { get; set; }
        /// <summary>
        /// 员工个人二维码（扫描可添加为外部联系人），仅在用户同意snsapi_privateinfo授权时返回
        /// </summary>
        public string Qr_code { get; set; }
        /// <summary>
        /// 手机，仅在用户同意snsapi_privateinfo授权时返回，第三方应用不可获取
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 邮箱，仅在用户同意snsapi_privateinfo授权时返回，第三方应用不可获取
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 企业邮箱，仅在用户同意snsapi_privateinfo授权时返回，第三方应用不可获取
        /// </summary>
        public string Biz_mail { get; set; }

        /// <summary>
        /// 仅在用户同意snsapi_privateinfo授权时返回，第三方应用不可获取
        /// </summary>
        public string Address { get; set; }

    }
}
