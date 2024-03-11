using Common.Dtos;
using Common.Entities;
using Common.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.DependencyInjection;

namespace Parakeet.Net.Suppliers
{
    /// <summary>
    /// 供应商服务 ISupplierAppService
    /// </summary>
    [Description("供应商")]
    public interface ISupplierAppService: IBaseNetAppService<Supplier>, ITransientDependency
    {
        /// <summary>
        ///  获取列表数据(分页)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<IPagedResult<SupplierDto>> GetPagedResult(GetSuppliersInputDto input);

        /// <summary>
        ///  获取所有供应商列表数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<List<SupplierDto>> GetSupplierFilterList(InputIdNullDto input);

        /// <summary>
        /// 获取供应商下拉列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<List<KeyValueDto<Guid, string>>> GetSupplierSelectList(InputIdNullDto input);

    }
}