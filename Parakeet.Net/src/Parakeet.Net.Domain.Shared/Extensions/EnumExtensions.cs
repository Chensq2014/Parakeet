using Parakeet.Net.Enums;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Reflection;

namespace Parakeet.Net.Extensions
{
    /// <summary>
    ///     枚举类型扩展
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        ///     枚举转int类型
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static int ToInt(this Enum source)
        {
            return ((IConvertible) source).ToInt32(null);
        }

        /// <summary>
        ///     int类型转枚举
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T ToEnum<T>(this int source)
        {
            return (T) Enum.ToObject(typeof(T), source);
        }

        /// <summary>
        ///     当前枚举值是否在枚举中被定义
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsDefined(this Enum source)
        {
            return Enum.IsDefined(source.GetType(), source);
        }

        /// <summary>
        ///     获得枚举使用<see cref="DescriptionAttribute" />或<see cref="DisplayAttribute" />或<see cref="DisplayNameAttribute" />
        ///     修饰的显示名称
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string DisplayName(this Enum source)
        {
            if (source is null)
            {
                return null;
            }
            var description = source.ToString();
            var fieldInfo = source.GetType().GetTypeInfo().GetField(description);
            if (fieldInfo is null)
            {
                return null;
            }
            var attributes2 = (DescriptionAttribute[]) fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes2?.Length > 0)
            {
                return attributes2[0].Description ?? description;
            }
            var attributes = (DisplayAttribute[]) fieldInfo.GetCustomAttributes(typeof(DisplayAttribute), false);
            if (attributes?.Length > 0)
            {
                return attributes[0].Name ?? attributes[0].Description;
            }
            var attributes3 = (DisplayNameAttribute[]) fieldInfo.GetCustomAttributes(typeof(DisplayNameAttribute), false);
            if (attributes3?.Length > 0)
            {
                return attributes3[0].DisplayName ?? description;
            }
            return description;
        }



        #region 创建枚举 ColorProvider 与枚举颜色

        /// <summary>
        /// 根据Type创建枚举 ColorProvider
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IColorProvider CreateColorProvider(Type type)
        {
            IColorProvider provider = null;
            if (type == typeof(PlanStatus))
            {
                provider = new PlanStatusColorProvider();
            }
            else if (type == typeof(UserStatus))
            {
                provider = new UserStatusColorProvider();
            }
            else if (type == typeof(TreeNodeLevel))
            {
                provider = new TreeNodeLevelColorProvider();
            }
            return provider;
        }

        /// <summary>
        /// 根据枚举类型 创建枚举 ColorProvider
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IColorProvider ColorProvider(this Enum source)
        {
            return CreateColorProvider(source.GetType());
        }

        /// <summary>
        /// 获取枚举值对应html颜色字符串 供前端使用
        /// </summary>
        /// <param name="source">枚举值</param>
        /// <returns></returns>
        [Description("获取枚举值对应html颜色字符串")]
        public static string GetColorTranslator(this Enum source)
        {
            return ColorTranslator.ToHtml(ColorProvider(source).GetColor(source));
        }

        #endregion


    }
}