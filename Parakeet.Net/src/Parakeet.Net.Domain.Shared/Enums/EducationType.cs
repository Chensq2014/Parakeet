using System.ComponentModel;

namespace Parakeet.Net.Enums
{
    /// <summary>
    /// 教育程度
    /// </summary>
    [Description("教育程度")]
    public enum EducationType
    {
        /// <summary>
        /// 文盲
        /// </summary>
        [Description("文盲")]
        文盲 = 0,
        /// <summary>
        /// 小学
        /// </summary>
        [Description("小学")]
        小学 = 10,
        /// <summary>
        /// 初中
        /// </summary>
        [Description("初中")]
        初中 = 20,
        /// <summary>
        /// 中专
        /// </summary>
        [Description("中专")]
        中专 = 30,
        /// <summary>
        /// 高中
        /// </summary>
        [Description("高中")]
        高中 = 40,
        /// <summary>
        /// 大专
        /// </summary>
        [Description("大专")]
        大专 =50 ,
        /// <summary>
        /// 本科
        /// </summary>
        [Description("本科")]
        本科 = 60,
        /// <summary>
        /// 硕士
        /// </summary>
        [Description("硕士")]
        硕士 = 70,
        /// <summary>
        /// 博士
        /// </summary>
        [Description("博士")]
        博士 = 80,
        /// <summary>
        /// 教授
        /// </summary>
        [Description("教授")]
        教授 = 90,
        /// <summary>
        /// 专家
        /// </summary>
        [Description("专家")]
        专家 = 100,
        /// <summary>
        /// 科学家
        /// </summary>
        [Description("科学家")]
        科学家 = 1000
    }
}
