using System;

namespace Parakeet.Net.Helper
{
    /// <summary>
    /// 反射类型帮助类
    /// </summary>
    public static class ReflectTypeHelper
    {
        /// <summary>
        /// 根据类型字符串获取类型 es 数据类型
        /// 核心类型	字符串类型	string,text,keyword	结构化搜索，全文文本搜索、聚合、排序等
        /// 整数类型 integer,long,short,byte 字段的长度越短，索引和搜索的效率越高。
        /// 浮点类型 double,float,half_float,scaled_float
        ///     逻辑类型    boolean
        ///     日期类型    date
        ///     范围类型    range
        ///     二进制类型   binary
        ///     复合类型    数组类型 array
        /// 对象类型 object
        ///     嵌套类型    nested
        ///     地理类型    地理坐标类型 geo_point
        /// 地理地图 geo_shape
        /// 特殊类型 IP类型    ip
        ///     范围类型    completion
        ///     令牌计数类型  token_count
        ///     附件类型    attachment
        ///     抽取类型    percolator
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type GetTypeByEsTypeString(string type)
        {
            switch (type.ToLower())
            {
                case "boolean":
                case "bool":
                    return Type.GetType("System.Boolean", true, true);
                case "byte":
                    return Type.GetType("System.Byte", true, true);
                case "sbyte":
                    return Type.GetType("System.SByte", true, true);
                case "char":
                    return Type.GetType("System.Char", true, true);
                case "decimal":
                    return Type.GetType("System.Decimal", true, true);
                case "double":
                    return Type.GetType("System.Double", true, true);
                case "scaled_float":
                case "half_float":
                case "float":
                    return Type.GetType("System.Single", true, true);
                case "integer":
                case "int":
                    return Type.GetType("System.Int32", true, true);
                case "uint":
                    return Type.GetType("System.UInt32", true, true);
                case "long":
                    return Type.GetType("System.Int64", true, true);
                case "ulong":
                    return Type.GetType("System.UInt64", true, true);
                case "object":
                    return Type.GetType("System.Object", true, true);
                case "short":
                    return Type.GetType("System.Int16", true, true);
                case "ushort":
                    return Type.GetType("System.UInt16", true, true);
                case "date":
                case "datetime":
                    return Type.GetType("System.DateTime", true, true);
                case "guid":
                    return Type.GetType("System.Guid", true, true);
                case "string":
                default:
                    //return Type.GetType(type, true, true);
                    return Type.GetType("System.String", true, true);
            }
        }
    }
}
