using Parakeet.Net.Dtos;
using System;
using System.Linq.Expressions;
using Volo.Abp.Domain.Entities;

namespace Parakeet.Net.Extensions
{
    /// <summary>
    /// FieldCheckOptionDto扩展类
    /// </summary>
    public static class FieldCheckOptionDtoExtension
    {
        /// <summary>
        /// 用于重复性检查的Lambda表达式
        /// </summary>
        /// <typeparam name="TEntity">实体</typeparam>
        /// <typeparam name="TPrimaryKey">实体主键类型</typeparam>
        /// <param name="option">实体某属性字段选项类</param>
        /// <returns></returns>
        public static Expression<Func<TEntity, bool>> CheckLambda<TEntity, TPrimaryKey>(this FieldCheckOptionDto<TPrimaryKey> option)
            where TEntity : Entity<TPrimaryKey>
        {
            return ExpressionExtension.DynamicField<TEntity, TPrimaryKey>(option);
        }
    }
}
