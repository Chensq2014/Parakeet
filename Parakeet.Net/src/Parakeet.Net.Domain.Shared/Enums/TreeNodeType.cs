using System.ComponentModel;

namespace Parakeet.Net.Enums
{
    /// <summary>
    ///     构造tree目录数据类型
    /// </summary>
    [Description("构造tree目录数据类型")]
    public enum TreeNodeType
    {
        #region 提供给项目综合目录树

        /// <summary>
        ///     目录、普通目录、汇总目录、特殊目录
        /// </summary>
        [Description("目录")] 目录 = 0,

        /// <summary>
        ///     汇总:验收记录
        /// </summary>
        [Description("验收记录")] 验收记录 = 1,

        /// <summary>
        ///     汇总:单位工程
        /// </summary>
        [Description("单位工程")] 单位工程 = 2,

        /// <summary>
        ///     汇总:汇总表格
        /// </summary>
        [Description("汇总表格")] 汇总表格 = 3,

        /// <summary>
        ///     汇总:构造目录 如汇总的分部子分部分项等节点
        /// </summary>
        [Description("构造目录")] 构造目录 = 4,

        /// <summary>
        ///     目录表格
        /// </summary>
        [Description("目录表格")] 目录表格 = 9,

        /// <summary>
        ///     模板
        /// </summary>
        [Description("模板")] 模板 = 10,
        
        /// <summary>
        ///     模板文件
        /// </summary>
        [Description("模板文件")] 模板文件 = 11,

        /// <summary>
        ///     表格单位工程
        /// </summary>
        [Description("表格单位工程")] 表格单位工程 = 20,

        /// <summary>
        ///     模板表格
        /// </summary>
        [Description("模板表格")] 模板表格 = 30,

        /// <summary>
        ///     模板范例表格
        /// </summary>
        [Description("模板范例表格")] 模板范例表格 = 31,

        /// <summary>
        ///     原始记录表
        /// </summary>
        [Description("原始记录表")] 原始记录表 = 40,

        /// <summary>
        ///     报验表
        /// </summary>
        [Description("报验表")] 报验表 = 50,
        #endregion

        #region 提供给项目单位工程参见单位目录树

        /// <summary>
        ///     项目
        /// </summary>
        [Description("项目")] 项目 = 100,

        /// <summary>
        ///     项目信息
        /// </summary>
        [Description("项目信息")] 项目信息 = 110,

        /// <summary>
        ///     项目参建单位
        /// </summary>
        [Description("项目参建单位")] 项目参建单位 = 120,

        /// <summary>
        ///     项目单位工程
        /// </summary>
        [Description("项目单位工程")] 项目单位工程 = 130,

        /// <summary>
        ///     项目单位工程信息
        /// </summary>
        [Description("项目单位工程信息")] 项目单位工程信息 = 140,

        /// <summary>
        ///     单位工程参建单位
        /// </summary>
        [Description("单位工程参建单位")] 项目单位工程参建单位 = 150

        #endregion
    }
}