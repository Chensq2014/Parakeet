using System;
using System.Collections.Generic;
using System.Text;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 配置文件
    /// </summary>
    public class JWTTokenOptions
    {
        public string Audience
        {
            get;
            set;
        }
        public string SecurityKey
        {
            get;
            set;
        }
        //public SigningCredentials Credentials
        //{
        //    get;
        //    set;
        //}
        public string Issuer
        {
            get;
            set;
        }
    }
}
