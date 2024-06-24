using Common.Dtos;
using Common.JWTExtend;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Parakeet.Net.Jwt
{
    /// <summary>
    ///     JwtHssAppService服务 对称可逆加密
    /// </summary>
    public class JwtHssAppService : CustomerAppService, IJwtRssAppService
    {

        private static Dictionary<string, CurrentUserModel> _tokenCache = new Dictionary<string, CurrentUserModel>();

        #region Option注入
        private readonly JWTTokenOptions _jwtTokenOptions;
        public JwtHssAppService(IOptionsMonitor<JWTTokenOptions> jwtTokenOptions)
        {
            this._jwtTokenOptions = jwtTokenOptions.CurrentValue;
        }
        #endregion

        public string GetToken(CurrentUserModel userModel)
        {
            return this.IssueToken(userModel);
        }

        #region 刷新Token
        /// <summary>
        /// 刷新token的有效期问题上端校验
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        public string GetTokenByRefresh(string refreshToken)
        {
            //refreshToken在有效期，但是缓存可能没有？ 还能去手动清除--比如改密码了，清除缓存，用户来刷新token就发现没有了，需要重新登陆
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

        /// <summary>
        /// 2个token  就是有效期不一样
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public Tuple<string, string> GetTokenWithRefresh(CurrentUserModel userInfo)
        {
            string token = this.IssueToken(userInfo, 60);//1分钟
            string refreshToken = this.IssueToken(userInfo, 60 * 60 * 24 * 7);//7*24小时
            _tokenCache.Add(refreshToken, userInfo);

            return Tuple.Create(token, refreshToken);
        }
        #endregion


        private string IssueToken(CurrentUserModel userModel, int second = 600)
        {
            var claims = new[]
            {
                   new Claim(ClaimTypes.Name, userModel.Name),
                   new Claim("EMail", userModel.EMail),
                   new Claim("Account", userModel.Account),
                   new Claim("Age", userModel.Age.ToString()),
                   new Claim("Id", userModel.Id.ToString()),
                   new Claim("Mobile", userModel.Mobile),
                   new Claim(ClaimTypes.Role,userModel.Role),
                   new Claim("Role", "Assistant"),//这个不能默认角色授权，动态角色授权
                   new Claim("Sex", userModel.Sex.ToString())//各种信息拼装
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this._jwtTokenOptions.SecurityKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            /**
             * Claims (Payload)
                Claims 部分包含了一些跟这个 token 有关的重要信息。 JWT 标准规定了一些字段，下面节选一些字段:
                iss: The issuer of the token，token 是给谁的
                sub: The subject of the token，token 主题
                exp: Expiration Time。 token 过期时间，Unix 时间戳格式
                iat: Issued At。 token 创建时间， Unix 时间戳格式
                jti: JWT ID。针对当前 token 的唯一标识
                除了规定的字段外，可以包含其他任何 JSON 兼容的字段。
             * */
            var token = new JwtSecurityToken(
                issuer: this._jwtTokenOptions.Issuer,
                audience: this._jwtTokenOptions.Audience,
                claims: claims,
                expires: DateTime.Now.AddSeconds(second),//10分钟有效期
                notBefore: DateTime.Now,//立即生效  DateTime.Now.AddMilliseconds(30),//30s后有效
                signingCredentials: creds);
            string returnToken = new JwtSecurityTokenHandler().WriteToken(token);
            return returnToken;
        }

    }
}