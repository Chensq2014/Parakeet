using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Parakeet.Net.Helper
{
    /// <summary>
    /// sql帮助类
    /// </summary>
    public class SqlHelper
    {
         #region 检测客户的输入中是否有危险字符串

        /// <summary>
        /// 检测是否有Sql危险字符
        /// </summary>
        /// <param name="str">要判断字符串</param>
        /// <returns>判断结果</returns>
        public static bool IsSafeSqlString(string str)
        {
            return !Regex.IsMatch(str, @"[;|\/|\(|\)|\[|\]|\}|\{|@|\*]");
        }

        /// <summary>
        /// 检测客户输入的字符串是否有效。
        /// 当检测到客户的输入中有攻击性危险字符串,则返回false,有效返回true。
        /// </summary>
        /// <param name="input">要检测的字符串</param>
        public static bool IsValidInput(string input)
        {
            try
            {
                var pass = true;
                ////替换单引号
                //input = input.Replace("'", "''").Trim();

                //检测攻击性危险字符串
                //string testString = "and |or |exec |insert |select |delete |update |count |chr |mid |master |truncate |char |declare ";
                var array = new List<string>
                {
                    "and "
                    ,"exec "
                    ,"insert "
                    ,"select "
                    ,"delete "
                    ,"update "
                    ,"count "
                    ,"from "
                    ,"drop "
                    ,"asc "
                    ,"char "
                    ,"or "
                    //,"%"
                    //,";"
                    //,":"
                    //,"\'"
                    //,"\""
                    //,"-"
                    ,"chr "
                    ,"mid "
                    ,"master "
                    ,"truncate "
                    ,"char "
                    ,"declare "
                    ,"SiteName"
                    ,"net user"
                    ,"xp_cmdshell"
                    ,"/add"
                    ,"exec master.dbo.xp_cmdshell"
                    ,"net localgroup administrators"
                };
                foreach (string str in array)
                {
                    if (input.ToLower().IndexOf(str, StringComparison.Ordinal) > -1)
                    {
                        pass = false;
                        return pass;
                    }
                }

                //未检测到攻击字符串
                pass = IsSafeSqlString(input);
                return pass;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion #region 检测客户的输入中是否有危险字符串
    }
}
