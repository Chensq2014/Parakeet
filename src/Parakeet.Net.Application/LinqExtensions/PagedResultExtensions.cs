using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Parakeet.Net.LinqExtensions
{
    public static class PagedResultExtensions
    {
        public static IPagedResult<T> ToPageResult<T>(this IQueryable<T> query, int pageIndex, int pageSize,
            bool findTotalCount = true)
        {
            var pageResult = new PagedResultDto<T>();
            if (findTotalCount)
            {
                var totalCount = query.Count();
                pageResult.TotalCount = totalCount;
            }

            pageResult.Items = query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            return pageResult;
        }

        public static IPagedResult<T> ToPageResult<T>(this IQueryable<T> query, IPagedResultRequest pagedResultRequest,
            bool findTotalCount = true)
        {
            var pageResult = new PagedResultDto<T>();
            if (findTotalCount)
            {
                var totalCount = query.Count();
                pageResult.TotalCount = totalCount;
            }

            pageResult.Items = query.Skip(pagedResultRequest.SkipCount).Take(pagedResultRequest.MaxResultCount)
                .ToList();
            return pageResult;
        }

        public static async Task<IPagedResult<T>> ToPageResultAsync<T>(this IQueryable<T> query, int pageIndex,
            int pageSize, bool findTotalCount = true)
        {
            var pageResult = new PagedResultDto<T>();
            if (findTotalCount) pageResult.TotalCount = await query.CountAsync();
            pageResult.Items = await query.Skip(pageIndex * pageSize).Take(pageSize).ToListAsync();
            return pageResult;
        }

        public static async Task<IPagedResult<T>> ToPageResultAsync<T>(this IQueryable<T> query,
            IPagedResultRequest pagedResultRequest, bool findTotalCount = true)
        {
            var pageResult = new PagedResultDto<T>();
            if (findTotalCount) pageResult.TotalCount = await query.CountAsync();
            pageResult.Items = await query.Skip(pagedResultRequest.SkipCount).Take(pagedResultRequest.MaxResultCount)
                .ToListAsync();
            return pageResult;
        }
        public static IPagedResult<T> ToPageResultAsync<T>(this IEnumerable<T> query,
            IPagedResultRequest pagedResultRequest, bool findTotalCount = true)
        {
            var pageResult = new PagedResultDto<T>();
            if (query != null)
            {
                if (findTotalCount)
                {
                    pageResult.TotalCount = query.LongCount();
                }
                pageResult.Items = query.Skip(pagedResultRequest.SkipCount).Take(pagedResultRequest.MaxResultCount)
                    .ToList();
            }
            return pageResult;
        }
    }
}