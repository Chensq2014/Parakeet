using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Parakeet.Net.Dtos;
using Parakeet.Net.Entities;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace Parakeet.Net.LinqExtensions
{
    public static class QueryableExtensions
    {
        ///// <summary>
        ///// 从数据源中根据Id单独获取某个实体
        ///// </summary>
        ///// <typeparam name="TEntity"></typeparam>
        ///// <typeparam name="TKey"></typeparam>
        ///// <param name="query"></param>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //public static Task<TEntity> GetAsync<TEntity, TKey>(this IQueryable<TEntity> query,
        //    TKey id)
        //    where TEntity : class, IEntity<TKey>
        //{
        //    return query.FirstAsync(e => e.Id.Equals(id));
        //}

        /// <summary>
        /// AutoMap ProjectTo的源数据类型->目标数据类型dto
        /// </summary>
        /// <typeparam name="TEntity">源类型</typeparam>
        /// <typeparam name="TPrimaryKey">源类型主键</typeparam>
        /// <typeparam name="TDestination">目标类型</typeparam>
        /// <param name="repository">源</param>
        /// <param name="id">主键</param>
        /// <param name="configurationProvider">AutoMapper配置</param>
        /// <returns></returns>
        public static async Task<TDestination> GetProjectToDtoAsync<TEntity, TPrimaryKey, TDestination>(
            this IRepository<TEntity, TPrimaryKey> repository,
            TPrimaryKey id, IConfigurationProvider configurationProvider)
            where TEntity : class, IEntity<TPrimaryKey>
        {
            return await (await repository.GetQueryableAsync())
                .AsNoTracking()
                .Where(x => x.Id.Equals(id))
                .ProjectTo<TDestination>(configurationProvider)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// AutoMap ProjectTo的源数据类型->目标数据类型dto集合
        /// </summary>
        /// <typeparam name="TEntity">源类型</typeparam>
        /// <typeparam name="TPrimaryKey">源类型主键</typeparam>
        /// <typeparam name="TDestination">目标类型</typeparam>
        /// <param name="repository">源</param>
        /// <param name="id">主键</param>
        /// <param name="configurationProvider">AutoMapper配置</param>
        /// <returns></returns>
        public static async Task<List<TDestination>> GetProjectToListDtoAsync<TEntity, TPrimaryKey, TDestination>(
            this IRepository<TEntity, TPrimaryKey> repository,
            TPrimaryKey id, IConfigurationProvider configurationProvider)
            where TEntity : class, IEntity<TPrimaryKey>
        {
            return await (await repository.GetQueryableAsync())
                .AsNoTracking()
                .Where(x => x.Id.Equals(id))
                .ProjectTo<TDestination>(configurationProvider)
                .ToListAsync();
        }

        /// <summary>
        ///  时间范围过滤
        /// </summary>
        /// <typeparam name="TEntity">实体源类型</typeparam>
        /// <typeparam name="TDateRangeInput">时间范围过滤项接口</typeparam>
        /// <param name="query">实体IQueryable</param>
        /// <param name="input">时间范围过滤项</param>
        /// <returns></returns>
        public static IQueryable<TEntity> WhereDateRange<TEntity, TDateRangeInput>(this IQueryable<TEntity> query, TDateRangeInput input) where TEntity : DeviceRecord where TDateRangeInput : DateRangePagedInputDto
        {
            return query.WhereIf(input.StartDate.HasValue, x => x.RecordTime >= input.StartDate)
                .WhereIf(input.EndDate.HasValue, x => x.RecordTime <= input.EndDate);
        }
    }
}