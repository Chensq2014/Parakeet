using Common.Dtos;
using System.Collections.Generic;

namespace Parakeet.Net.EnumServices
{
    /// <summary>
    ///     枚举类型专用接口
    /// </summary>
    public interface IEnumAppService
    {
        #region 获取 枚举项集合 EnumTypeItemKeyNameDescriptions 返回给上端可组织任意返回数据格式

        /// <summary>
        ///     返回系统所有枚举类型 类型名称->类型描述/全名
        /// </summary>
        /// <returns></returns>
        IEnumerable<KeyValueDto<string, string>> GetAllEnumTypeNames();

        /// <summary>
        ///     获取 枚举项集合 EnumTypeItemKeyNameDescriptions
        /// </summary>
        /// <param name="input">枚举类型名称</param>
        /// <returns></returns>
        List<EnumTypeItemDto> GetEnumTypeItemKeyNameDescriptions(InputNameDto input);

        #endregion
    }
}