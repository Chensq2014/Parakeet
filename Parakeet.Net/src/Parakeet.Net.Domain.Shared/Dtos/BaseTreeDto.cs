using Parakeet.Net.Enums;
using System;
using System.ComponentModel;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// DxTreeView公共基类
    /// </summary>
    [Serializable, Description("DxTreeView公共基类")]
    public class BaseTreeDto : BaseTreeDto<Guid>
    {
        /// <summary>
        /// 父节点ParentIdString
        /// </summary>
        [Description("父级")]
        public Guid? ParentId { get; set; }
    }
    [Serializable, Description("DxTreeView公共泛型基类")]
    public class BaseTreeDto<TPrimaryKey> : BaseDto<TPrimaryKey>
    {
        /// <summary>
        /// 父节点FatherId ParentIdString 
        /// </summary>
        [Description("父级")]
        public TPrimaryKey FatherId { get; set; }

        /// <summary>
        /// 数据库Id
        /// </summary>
        [Description("数据库Id")]
        public Guid DbId { get; set; }

        /// <summary>
        /// 是否选中
        /// </summary>
        [Description("是否选中")]
        public bool Selected { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Description("排序")]
        public decimal? Sort { get; set; }

        /// <summary>
        /// 节点展示名称
        /// </summary>
        [Description("节点展示名称")]
        public string Name { get; set; }

        /// <summary>
        /// 节点展示的Key
        /// </summary>
        [Description("节点展示的Key")]
        public string Key { get; set; }

        /// <summary>
        /// 组件是否被展开
        /// </summary>
        [Description("组件是否被展开")]
        public bool Expanded { get; set; }

        /// <summary>
        /// 组件是否被禁用
        /// </summary>
        [Description("是否禁用")]
        public bool Disabled { get; set; }

        /// <summary>
        /// 备注，可以作为title 标注上去
        /// </summary>
        [Description("备注")]
        public string Remark { get; set; }

        /// <summary>
        /// 标识展示的icon 图片
        /// </summary>
        [Description("css图标类")]
        public string TypeIcon { get; set; }

        /// <summary>
        /// 是否展示selectBox
        /// </summary>
        [Description("是否显示selectBox")]
        public bool ShowSelectBox { get; set; }

        /// <summary>
        /// 默认为x
        /// x：横向，y：纵向
        /// </summary>
        [Description("展示方向")]
        public string DisplayDirection { get; set; }

        /// <summary>
        /// 扩展数据点
        /// </summary>
        [Description("扩展动态数据")]
        public dynamic Data { get; set; }

        /// <summary>
        /// 节点层级
        /// </summary>
        [Description("层级")]
        public TreeNodeLevel Level { get; set; }
    }
}
