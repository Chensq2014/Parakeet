using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Parakeet.Net.CustomAttributes
{
    /// <summary>
    /// object对象验证扩展
    /// </summary>
    [Description("属性公共扩展")]
    public static class AttributeExtention
    {
        /// <summary>
        /// 给所有类型 提供静态扩展验证方法 
        /// </summary>
        /// <typeparam name="T">当前实例所属类型，T也可以是object 因为object对象是所有对象的父级</typeparam>
        /// <param name="value">调用validate方法的当前实例</param>
        /// <returns></returns>
        public static bool Validate<T>(this T value)
        {
            var valid = true;
            var type = value.GetType();
            foreach (var prop in type.GetProperties())
            {
                if (prop.IsDefined(typeof(AbstractValidateAttribute), true))
                {
                    //同一个类的字段或属性上可能有多个属性
                    var array = prop.GetCustomAttributes(typeof(AbstractValidateAttribute), true);
                    foreach (AbstractValidateAttribute attribute in array)
                    {
                        valid = attribute.Validate(prop.GetValue(value));
                    }
                }
            }
            return valid;
        }
        
        /// <summary>
        /// 获取列名 PropertyInfo静态扩展
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static string GetColumnName(this PropertyInfo prop)
        {
            return prop.IsDefined(typeof(ColumnAttribute))//true columnName只找一个
                ? prop.GetCustomAttribute<ColumnAttribute>().Name
                : prop.Name;
        }

        /// <summary>
        /// PropertyInfo的父类是MemberInfo
        /// </summary>
        /// <param name="type">可以是type  也可以是property</param>
        /// <returns></returns>
        public static string GetMappingName(this MemberInfo type)
        {
            return type.IsDefined(typeof(ColumnAttribute))//true columnName只找一个
                ? type.GetCustomAttribute<ColumnAttribute>().Name
                : type.Name;
        }
    }




}
