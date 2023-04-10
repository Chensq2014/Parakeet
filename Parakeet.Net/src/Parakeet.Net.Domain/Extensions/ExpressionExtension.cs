using Parakeet.Net.Dtos;
using Parakeet.Net.Entities;
using Parakeet.Net.Extensions.Visitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Data.SqlClient;

namespace Parakeet.Net.Extensions
{
    /// <summary>
    /// 表达式目录树扩展
    /// </summary>
    public static class ExpressionExtension
    {
        #region 动态拼装表达式目录树
        /// <summary>
        /// 根据字段属性名称拼接动态表达式目录树,用于(界面插入/修改数据时)重复性验证
        /// 常用几种基础类型(可空枚举,枚举,可空整型,整型,可空日期和日期类型)的字段表达式目录树扩展
        /// </summary>
        /// <typeparam name="TEntity">实体</typeparam>
        /// <typeparam name="TPrimaryKey">实体主键类型</typeparam>
        /// <param name="option">实体某属性字段选项类</param>
        /// <returns></returns>
        public static Expression<Func<TEntity, bool>> DynamicField<TEntity, TPrimaryKey>(FieldCheckOptionDto<TPrimaryKey> option) where TEntity : BaseEntity<TPrimaryKey>
        {
            //ParameterExpression
            var param = Expression.Parameter(typeof(TEntity), "m");
            // n.field
            var property = Expression.PropertyOrField(param, option.Field);
            // value
            var valueField = typeof(TEntity).GetProperty(option.Field);
            //if (valueField == null) return null;
            if (valueField is null) throw new Exception("类型" + typeof(TEntity).Name + "不存在字段" + option.Field);
            var fullName = valueField.PropertyType.FullName;

            ConstantExpression right;//组装表达式目录树
            if (valueField.PropertyType.BaseType != null
                && valueField.PropertyType.IsValueType
                && valueField.PropertyType.IsGenericType
                && valueField.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)
                && valueField.PropertyType.GetGenericArguments()[0].IsEnum)
            {
                if (string.IsNullOrWhiteSpace(option.Value)) throw new Exception("数据类型出错");
                var obj = Enum.ToObject(valueField.PropertyType.GetGenericArguments()[0],
                    Enum.Parse(valueField.PropertyType.GetGenericArguments()[0], option.Value));
                right = Expression.Constant(obj, valueField.PropertyType);
            }
            else if (valueField.PropertyType.BaseType != null && valueField.PropertyType.IsEnum)
            {
                if (!int.TryParse(option.Value, out var enumValue)) throw new Exception("数据类型出错");
                right = Expression.Constant(Enum.ToObject(valueField.PropertyType, enumValue), valueField.PropertyType);
            }
            else if (fullName == typeof(Guid).FullName)
            {
                right = Expression.Constant(option.Value.HasValue() && Guid.Parse(option.Value) != Guid.Empty ? Guid.Parse(option.Value) : default(Guid?), typeof(Guid));
            }
            else if (fullName == typeof(Guid?).FullName)
            {
                right = Expression.Constant(option.Value.HasValue() ? Guid.Parse(option.Value) : default(Guid?), typeof(Guid?));
            }
            else if (fullName == typeof(int).FullName)
            {
                right = Expression.Constant(option.Value.HasValue() ? Convert.ToInt32(option.Value) : default(int), typeof(int));
            }
            else if (fullName == typeof(int?).FullName)
            {
                right = Expression.Constant(option.Value.HasValue() ? Convert.ToInt32(option.Value) : default(int?), typeof(int?));
            }
            else if (fullName == typeof(long).FullName)
            {
                right = Expression.Constant(option.Value.HasValue() ? Convert.ToInt64(option.Value) : default(long), typeof(long));
            }
            else if (fullName == typeof(long?).FullName)
            {
                right = Expression.Constant(option.Value.HasValue() ? Convert.ToInt64(option.Value) : default(long?), typeof(long?));
            }
            else if (fullName == typeof(DateTime).FullName)
            {
                if (option.Value.IndexOf("GMT", StringComparison.Ordinal) > 0)
                {
                    option.Value = option.Value.Substring(0, option.Value.IndexOf("GMT", StringComparison.Ordinal)).Trim();
                }
                right = Expression.Constant(Convert.ToDateTime(option.Value), typeof(DateTime));
            }
            else if (fullName == typeof(DateTime?).FullName)
            {
                if (option.Value.IndexOf("GMT", StringComparison.Ordinal) > 0)
                {
                    option.Value = option.Value.Substring(0, option.Value.IndexOf("GMT", StringComparison.Ordinal)).Trim();
                }
                right = Expression.Constant(Convert.ToDateTime(option.Value), typeof(DateTime?));
            }
            else
            {
                right = Expression.Constant(option.Value);
            }

            var body = Expression.Equal(property, right);

            if (option.Id != null)
            {
                property = Expression.PropertyOrField(param, "Id");
                right = Expression.Constant(option.Id);
                var rightBody = Expression.NotEqual(property, right);
                body = Expression.And(body, rightBody); // n.field==value && n.Id != id
            }

            var lambda = Expression.Lambda<Func<TEntity, bool>>(body, param);
            return lambda;
        }

        #endregion

        #region 表达式目录树拼接扩展 实现多个lambda表达式的拼装 及返回结果

        /// <summary>
        /// 合并表达式 expr1 AND expr2
        /// expr1为null 直接返回expr2
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first is null ? second : first.AndAlso<T>(second, Expression.AndAlso);

            #region 实质

            //var mParameter = Expression.Parameter(typeof(T), "m");
            //var visitor = new NewExpressionVisitor(mParameter);
            //var left = visitor.Replace(first.Body);
            //var right = visitor.Replace(second.Body);
            //var body = Expression.And(left, right);
            //return Expression.Lambda<Func<T, bool>>(body, mParameter);

            #endregion
        }

        /// <summary>
        /// 合并表达式 expr1 OR expr2
        /// expr1为null 直接返回expr2
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first == null ? second : first.AndAlso<T>(second, Expression.OrElse);

            #region 实质
            //var newParameter = Expression.Parameter(typeof(T), "m");
            //var visitor = new NewExpressionVisitor(newParameter);
            //var left = visitor.Replace(first.Body);
            //var right = visitor.Replace(second.Body);
            //var body = Expression.Or(left, right);
            //return Expression.Lambda<Func<T, bool>>(body, newParameter);
            #endregion
        }

        /// <summary>
        /// IQueryable 的WhereBy泛型 表达式目录树过滤
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static IQueryable<T> WhereBy<T>(this IQueryable<T> entities, Expression<Func<T, bool>> func)
        {
            return entities.AsQueryable<T>().Where(func);
        }


        //public static Expression<Func<T, bool>> True<T>(this Expression<Func<T, bool>> expression)
        //{
        //    return s => true;
        //}



        /// <summary>
        /// 表达式加否
        /// 为null抛出异常
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> expr)
        {
            if (expr == null) return null;//throw new Exception("expr is null");
            var candidateExpr = expr.Parameters[0];
            var body = Expression.Not(expr.Body);

            return Expression.Lambda<Func<T, bool>>(body, candidateExpr);
        }

        /// <summary>
        /// List 的WhereBy泛型 表达式目录树过滤
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static List<T> WhereBy<T>(this List<T> entities, Expression<Func<T, bool>> func)
        {
            return entities.Where(func.Compile()).ToList();
        }


        private static Expression<Func<T, bool>> AndAlso<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second, Func<Expression, Expression, BinaryExpression> func)
        {
            var parameter = Expression.Parameter(typeof(T));

            var leftVisitor = new ReplaceExpressionVisitor(first.Parameters[0], parameter);
            var left = leftVisitor.Visit(first.Body);

            var rightVisitor = new ReplaceExpressionVisitor(second.Parameters[0], parameter);
            var right = rightVisitor.Visit(second.Body);

            return Expression.Lambda<Func<T, bool>>(func(left, right), parameter);
        }

        private class ReplaceExpressionVisitor : ExpressionVisitor
        {
            private readonly Expression _oldValue;
            private readonly Expression _newValue;

            public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
            {
                _oldValue = oldValue;
                _newValue = newValue;
            }

            public override Expression Visit(Expression node)
            {
                if (node == _oldValue)
                    return _newValue;
                return base.Visit(node);
            }
        }


        #endregion


        /// <summary>
        /// linq--to--sql 自定义拼装
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="expression"></param>
        /// <param name="sqlParameters"></param>
        /// <returns></returns>
        public static string ToWhere<TEntity>(this Expression<Func<TEntity, bool>> expression, out List<SqlParameter> sqlParameters)where TEntity : BaseEntity
        {
            CustomExpressionVisitor visitor = new CustomExpressionVisitor();
            visitor.Visit(expression);
            string where = visitor.GetWhere(out sqlParameters);
            return where;
        }
    }
}
