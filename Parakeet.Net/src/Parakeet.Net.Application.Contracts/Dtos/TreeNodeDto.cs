using System;
using Parakeet.Net.Enums;
using Parakeet.Net.ValueObjects;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    ///     构造目录树类
    /// </summary>
    public class TreeNodeDto
    {
        /// <summary>
        ///     真实数据表的Id
        /// </summary>
        public Guid? PrimaryId { get; set; }

        /// <summary>
        ///     数据Id 设计与PrimaryId相同
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        ///     父级数据Id 设计与父级PrimaryId相同
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        ///     excel数据Id 根据此Id查mogodb中对应Id存放的表格数据
        /// </summary>
        public Guid? ExcelDataId { get; set; }

        /// <summary>
        ///     节点数据源类型 （目录/模板/表格单位工程/表格）
        /// </summary>
        public TreeNodeType TreeNodeType { get; set; }

        /// <summary>
        ///     节点名称 （数据源名称）
        /// </summary>
        public string Name { get; set; }

        public Attachment Attachment { get; set; }

        /// <summary>
        ///     节点描述 （辅助说明）
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        ///     是否构造节点  构造节点的Id为虚假的
        /// </summary>
        public bool IsVirtual { get; set; }

        /// <summary>
        ///     用户模板或者表格排序
        /// </summary>
        public decimal? SerialNumber { get; set; }

        /// <summary>
        ///     node节点添加顺序
        /// </summary>
        public decimal? Rank { get; set; }
    }
}