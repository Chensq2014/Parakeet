using Common.Dtos;
using System.Collections.Generic;

namespace Parakeet.Net.EnumServices
{
    /// <summary>
    ///     枚举类型专用Service
    /// </summary>
    public class EnumAppService : CustomerAppService, IEnumAppService
    {
        /// <summary>
        ///     返回系统所有枚举类型 类型名称->类型描述/全名
        /// </summary>
        /// <returns></returns>
        public IEnumerable<KeyValueDto<string, string>> GetAllEnumTypeNames()
        {
            return EnumContext.Instance.GetAllEnumTypeNames();
        }

        #region 获取 枚举项集合 EnumTypeItemKeyNameDescriptions 返回给上端可组织任意返回数据格式

        /// <summary>
        ///     获取 枚举项集合 EnumTypeItemKeyNameDescriptions
        /// </summary>
        /// <param name="input">枚举类型名称</param>
        /// <returns></returns>
        public List<EnumTypeItemDto> GetEnumTypeItemKeyNameDescriptions(InputNameDto input)
        {
            return EnumContext.Instance.GetEnumTypeItemKeyNameDescriptions(input);
        }

        #endregion
    }
}