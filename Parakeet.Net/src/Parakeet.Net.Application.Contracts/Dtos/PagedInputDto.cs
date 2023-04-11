using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 分页过滤项
    /// </summary>
    public abstract class PagedInputDto : ISortedResultRequest, IPagedResultRequest
    {
        protected PagedInputDto()
        {
            MaxResultCount = CustomerConsts.DefaultPageSize;
            SkipCount = PageIndex * MaxResultCount ?? SkipCount;
        }

        /// <summary>
        ///     传递页码后影响SkipCount的值
        /// </summary>
        public int? PageIndex { get; set; }

        /// <summary>
        ///     提供给前端传递字符串或者json字符串的数据过滤对象，后端可以直接获取或者反序列化为需要的对象
        /// </summary>
        public string Filter { get; set; }

        /// <summary>
        ///     创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }

        ///// <summary>
        ///// 每页数据量 用MaxResultCount 代替
        ///// </summary>
        //public int? PageSize { get; set; }

        [Range(1, CustomerConsts.MaxPageSize)] public int MaxResultCount { get; set; }

        [Range(0, int.MaxValue)] public int SkipCount { get; set; }

        /// <summary>
        /// 是否需要获取总页数，默认为true
        /// </summary>
        public bool FindTotalCount { get; set; } = true;

        /// <summary>
        ///     格式："CreationTime DESC, LastModificationTime DESC" Or "CreationTime, LastModificationTime"
        /// </summary>
        public virtual string Sorting { get; set; } = $"{nameof(CreationTime)} DESC";
    }
}