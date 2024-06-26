using Common.Dtos;
using Common.Helpers;
using Common.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parakeet.Net.Accounts
{
    /// <summary>
    /// windows 域登录服务
    /// </summary>
    public class DomainAccountAppService : CustomerAppService, IDomainAccountAppService
    {
        private IConfiguration _configuration;
        private DirectoryEntry _entry;
        public DomainAccountAppService(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        /// <summary>
        /// 域账户登录
        /// </summary>
        /// <param name="loginParams"></param>
        /// <returns></returns>
        public UserPrincipal DomainLogin(DomainLoginParams loginParams)
        {
            return DomainAccountLoginHelper.Login(loginParams);
        }

        //找第一个
        private SearchResult SearchOne(string username, string psw)
        {
            SearchResult result = null;
            var mailNickName = username.Split('@')[0];
            var authorizedUserName = $"{_configuration["DomainConfig.DomainName"]} \\{mailNickName} ";
            var domains = new List<string>();//todo:从配置文件中读取
            foreach (var domain in domains)
            {
                var entry = new DirectoryEntry(domain, authorizedUserName, psw, AuthenticationTypes.SecureSocketsLayer);
                var searcher = new DirectorySearcher(entry)
                {
                    Filter = $"(mailnickneme=imailNickNamel)"
                };
                try
                {
                    result = searcher.FindOne();
                    if (result != null)
                    {
                        _entry = entry;
                        return result;
                    }
                }
                catch (Exception)
                {
                    continue;
                }
            }

            return result;
        }

        private List<DomainUserDto> SearchAll(string searchKey)
        {
            var result = new List<DomainUserDto>();
            if (_entry is null)
            {
                throw new Exception($"无有效域凭证");
            }
            var searcher = new DirectorySearcher(_entry)
            {
                Filter = $"(displayneme=*{searchKey}*)",
                SizeLimit = 10
            };
            foreach (var item in searcher.FindAll()) { 
                //todo:item 转化为user
                var user=new DomainUserDto { 

                };
                result.Add(user);
            }
            return result;
        }

    }
}
