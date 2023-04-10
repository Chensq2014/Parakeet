using System;
using System.Security.Cryptography;
using System.Text;

namespace Parakeet.Net.Helper
{
    /// <summary>
    /// SHSA加密Helper(SHA1或SHA256)
    /// </summary>
    public static class SHAHelper
    {
        /// <summary>
        /// SHA1 加密，返回大写字符串
        /// </summary>
        /// <param name="content">需要加密字符串</param>
        /// <returns>返回40位UTF8 大写</returns>
        public static string SHA1(string content)
        {
            return SHA1(content, Encoding.UTF8);
        }

        /// <summary>
        /// SHA1 加密，返回大写字符串
        /// </summary>
        /// <param name="content">需要加密字符串</param>
        /// <param name="encode">指定加密编码</param>
        /// <returns>返回40位大写字符串</returns>
        public static string SHA1(string content, Encoding encode)
        {
            try
            {
                var sha1 = new SHA1CryptoServiceProvider();
                byte[] bytes_in = encode.GetBytes(content);
                byte[] bytes_out = sha1.ComputeHash(bytes_in);
                sha1.Dispose();

                string result = BitConverter.ToString(bytes_out);
                result = result.Replace("-", "");
                return result.ToLower();
            }
            catch (Exception ex)
            {
                throw new Exception("SHA1加密出错：" + ex.Message);
            }
        }

        /// <summary>
        /// HMACSHA1加密
        /// </summary>
        /// <param name="appId">appId</param>
        /// <param name="appKey">appKey</param>
        /// <param name="appSecret">appSecret</param>
        /// <param name="timeStamp">timeStamp</param>
        /// <returns>加密base64字符串</returns>
        public static string HMACSHA1(string appId, string appKey, string appSecret, string timeStamp)
        {
            var plainText = $"a={appId}&s={appSecret}&t={timeStamp}&v={Magics.VERSION}";

            using var mac = new HMACSHA1(Encoding.UTF8.GetBytes(appKey));
            var hash = mac.ComputeHash(Encoding.UTF8.GetBytes(plainText));
            var pText = Encoding.UTF8.GetBytes(plainText);
            var all = new byte[hash.Length + pText.Length];
            Array.Copy(hash, 0, all, 0, hash.Length);
            Array.Copy(pText, 0, all, hash.Length, pText.Length);
            //using var md5 = MD5.Create();
            //return Convert.ToBase64String(md5.ComputeHash(all));
            return Convert.ToBase64String(all);//旧版
        }

        /// <summary>
        /// HMACSHA1加密
        /// </summary>
        /// <param name="appId">appId</param>
        /// <param name="appKey">appKey</param>
        /// <param name="appSecret">appSecret</param>
        /// <param name="timeStamp">timeStamp</param>
        /// <returns>加密base64字符串</returns>
        public static string HMACSHA1(string appId, string appKey, string appSecret, long timeStamp)
        {
            var plainText = $"a={appId}&s={appSecret}&t={timeStamp}&v={Magics.VERSION}";

            using var mac = new HMACSHA1(Encoding.UTF8.GetBytes(appKey));
            var hash = mac.ComputeHash(Encoding.UTF8.GetBytes(plainText));
            var pText = Encoding.UTF8.GetBytes(plainText);
            var all = new byte[hash.Length + pText.Length];
            Array.Copy(hash, 0, all, 0, hash.Length);
            Array.Copy(pText, 0, all, hash.Length, pText.Length);
            //using var md5 = MD5.Create();
            //return Convert.ToBase64String(md5.ComputeHash(all));
            return Convert.ToBase64String(all);//旧版
        }

        /// <summary>
        /// HMACSHA256
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="appKey"></param>
        /// <param name="appSecret"></param>
        /// <param name="unixTimeStamp"></param>
        /// <returns></returns>
        public static string HMACSHA256(string appId, string appKey, string appSecret, long unixTimeStamp)
        {
            var plainText = $"a={appId}&s={appSecret}&t={unixTimeStamp}&v={Magics.VERSION}";

            using var mac = new HMACSHA256(Encoding.UTF8.GetBytes(appKey));
            var hash = mac.ComputeHash(Encoding.UTF8.GetBytes(plainText));
            var pText = Encoding.UTF8.GetBytes(plainText);
            var all = new byte[hash.Length + pText.Length];
            Array.Copy(hash, 0, all, 0, hash.Length);
            Array.Copy(pText, 0, all, hash.Length, pText.Length);
            using var md5 = MD5.Create();
            return Convert.ToBase64String(md5.ComputeHash(all));
        }

        /// <summary>
        /// HMACSHA256
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="appKey"></param>
        /// <param name="appSecret"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static string HMACSHA256(string appId, string appKey, string appSecret, string token)
        {
            var plainText = $"a={appId}&s={appSecret}&t={token}&v={Magics.VERSION}";

            using var mac = new HMACSHA256(Encoding.UTF8.GetBytes(appKey));
            var hash = mac.ComputeHash(Encoding.UTF8.GetBytes(plainText));
            var pText = Encoding.UTF8.GetBytes(plainText);
            var all = new byte[hash.Length + pText.Length];
            Array.Copy(hash, 0, all, 0, hash.Length);
            Array.Copy(pText, 0, all, hash.Length, pText.Length);
            using var md5 = MD5.Create();
            return Convert.ToBase64String(md5.ComputeHash(all));
        }

        ///// <summary>
        ///// Creates a SHA256 hash of the specified input.
        ///// </summary>
        ///// <param name="input">The input.</param>
        ///// <returns>A hash</returns>
        //public static string Sha256(this string input)
        //{
        //    if (input.HasValue())
        //    {
        //        using var sha = SHA256.Create();
        //        var bytes = Encoding.UTF8.GetBytes(input);
        //        var hash = sha.ComputeHash(bytes);
        //        return Convert.ToBase64String(hash);
        //    }
        //    return input;
        //}
    }
}
