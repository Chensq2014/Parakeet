using System;

namespace Parakeet.Net.Dtos
{
    public class CaptchaResultDto
    {
        /// <summary>验证码</summary>
        public string CaptchaCode { get; set; }

        /// <summary>Base64</summary>
        public byte[] CaptchaByteData { get; set; }

        /// <summary>Base64字符串</summary>
        public string CaptchBase64Data =>Convert.ToBase64String(CaptchaByteData);

        /// <summary>生成时间</summary>
        public DateTime Timestamp { get; set; }
    }
}
