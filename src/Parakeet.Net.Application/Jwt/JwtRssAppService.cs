using Common.Dtos;
using Common.Helpers;
using Common.JWTExtend;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Parakeet.Net.Jwt
{
    /// <summary>
    ///     JwtRssAppService服务 非对称可逆加密
    /// </summary>
    public class JwtRssAppService : CustomerAppService, IJwtRssAppService
    {

        private static Dictionary<string, CurrentUserModel> _tokenCache = new Dictionary<string, CurrentUserModel>();

        #region Option注入
        private readonly JWTTokenOptions _jwtTokenOptions;
        public JwtRssAppService(IOptionsMonitor<JWTTokenOptions> jwtTokenOptions)
        {
            this._jwtTokenOptions = jwtTokenOptions.CurrentValue;
        }
        #endregion

        /// <summary>
        /// https://localhost:23517/api/parakeet/jwt-rss/token?Id=1&Name=chensq&Account=0740502106&Mobile=17772433927&EMail=290668617%40qq.com&Role=admin&Age=35&Sex=1
        /// </summary>
        /// <param name="userModel"></param>
        /// <returns></returns>
        public string GetToken(CurrentUserModel userModel)
        {
            return this.IssueToken(userModel);
        }

        /// <summary>
        /// 刷新token的有效期问题上端校验
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        public string GetTokenByRefresh(string refreshToken)
        {
            if (_tokenCache.ContainsKey(refreshToken))
            {
                string token = this.IssueToken(_tokenCache[refreshToken], 60);
                return token;
            }
            else
            {
                return "";
            }
        }

        public Tuple<string, string> GetTokenWithRefresh(CurrentUserModel userInfo)
        {
            string token = this.IssueToken(userInfo, 60);//1分钟
            string refreshToken = this.IssueToken(userInfo, 60 * 60 * 24);//24小时
            _tokenCache.Add(refreshToken, userInfo);

            return Tuple.Create(token, refreshToken);
        }



        private string IssueToken(CurrentUserModel userModel, int second = 600)
        {
            //string jtiCustom = Guid.NewGuid().ToString();//用来标识 Token
            var claims = new[]
            {
                   new Claim(ClaimTypes.Name, userModel.Name),
                   new Claim("EMail", userModel.EMail),
                   new Claim("Account", userModel.Account),
                   new Claim("Age", userModel.Age.ToString()),
                   new Claim("Id", userModel.Id.ToString()),
                   new Claim("Mobile", userModel.Mobile),
                   new Claim(ClaimTypes.Role,userModel.Role),
                   //new Claim("Role", userModel.Role),//这个不能角色授权
                   new Claim("Sex", userModel.Sex.ToString())//各种信息拼装
            };

            string keyDir = Directory.GetCurrentDirectory();
            if (RSAHelper.TryGetKeyParameters(keyDir, true, out RSAParameters keyParams) == false)
            {
                keyParams = RSAHelper.GenerateAndSaveKey(keyDir);
            }
            var credentials = new SigningCredentials(new RsaSecurityKey(keyParams), SecurityAlgorithms.RsaSha256Signature);

            var token = new JwtSecurityToken(
               issuer: this._jwtTokenOptions.Issuer,
               audience: this._jwtTokenOptions.Audience,
               claims: claims,
               expires: DateTime.Now.AddSeconds(second),//默认10分钟有效期
               notBefore: DateTime.Now,//立即生效  DateTime.Now.AddMilliseconds(30),//30s后有效
               signingCredentials: credentials);
            var handler = new JwtSecurityTokenHandler();
            string tokenString = handler.WriteToken(token);
            return tokenString;
        }

    }
}