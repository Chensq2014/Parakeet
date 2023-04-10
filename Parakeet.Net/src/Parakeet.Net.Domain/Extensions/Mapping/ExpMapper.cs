using Parakeet.Net.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Parakeet.Net.Extensions.Mapping
{
    /// <summary>
    /// 生成表达式目录树  泛型缓存  确保实体类Dto与实体类有相同的属性和字段 排除懒加载属性/字段
    /// </summary>
    /// <typeparam name="TIn">实体类</typeparam>
    /// <typeparam name="TOut">实体类Dto</typeparam>
    public class ExpMapper<TIn, TOut>//Mapper`2
    {
        private static Func<TIn, TOut> _func;//静态变量缓存泛型表达式目录树构成的Mapping委托
        static ExpMapper()
        {
            var parameterExp = Expression.Parameter(typeof(TIn), "m");
            var memberBindingList = new List<MemberBinding>();
            //属性
            foreach (var prop in typeof(TOut).GetProperties())
            {
                if (prop.IsDefined(typeof(NotSetAttribute), true)) continue;//排除导航属性/字段
                var name = typeof(TIn).GetProperty(prop.Name);//dto实体上的属性 实体上不一定有
                if (name is null) continue;
                var propertyExp = Expression.Property(parameterExp, name);
                var memberBinding = Expression.Bind(prop, propertyExp);
                memberBindingList.Add(memberBinding);
            }
            //字段
            foreach (var prop in typeof(TOut).GetFields())
            {
                if (prop.IsDefined(typeof(NotSetAttribute), true)) continue;//排除导航属性/字段
                var name = typeof(TIn).GetField(prop.Name);//dto实体上的属性 实体上不一定有
                if (name is null) continue;
                var property = Expression.Field(parameterExp, name);
                var memberBinding = Expression.Bind(prop, property);
                memberBindingList.Add(memberBinding);
            }
            var memberInitExpression = Expression.MemberInit(Expression.New(typeof(TOut)), memberBindingList.ToArray());
            var lambda = Expression.Lambda<Func<TIn, TOut>>(memberInitExpression, new ParameterExpression[] { parameterExp });
            _func = lambda.Compile();//拼装是一次性的,实体类型到dto类型只需要Compile一次就会被缓存下来
        }

        /// <summary>
        /// 调用方法
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static TOut Trans(TIn t)
        {
            return _func(t);
        }

        /// <summary>
        /// 调用泛型方法
        /// </summary>
        /// <param name="tIns"></param>
        /// <returns></returns>
        public static List<TOut> Trans(List<TIn> tIns)
        {
            return tIns.Select(m => _func(m)).ToList();
        }

        /// <summary>
        /// 调用泛型方法
        /// </summary>
        /// <param name="tIns"></param>
        /// <returns></returns>
        public static IList<TOut> Trans(IList<TIn> tIns)
        {
            return tIns.Select(m => _func(m)).ToList();
        }

        /// <summary>
        /// 调用泛型方法
        /// </summary>
        /// <param name="tIns"></param>
        /// <returns></returns>
        public static IEnumerable<TOut> Trans(IEnumerable<TIn> tIns)
        {
            return tIns.Select(m => _func(m));
        }
    }
}
