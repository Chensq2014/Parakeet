using Parakeet.Net.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Parakeet.Net.Extensions.Mapping
{
    /// <summary>
    /// 生成表达式目录树 缓存
    /// </summary>
    public class ExpressionMapper
    {
        private static Dictionary<string, object> _dic = new Dictionary<string, object>();

        /// <summary>
        /// 字典缓存表达式树
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="tIn"></param>
        /// <returns></returns>
        public static TOut Trans<TIn, TOut>(TIn tIn)
        {
            string key = $"funckey_{typeof(TIn).FullName}_{typeof(TOut).FullName}";
            if (!_dic.ContainsKey(key))
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
                Func<TIn, TOut> func = lambda.Compile();//拼装是一次性的
                _dic[key] = func;
            }
            return ((Func<TIn, TOut>)_dic[key]).Invoke(tIn);
        }
    }
}
