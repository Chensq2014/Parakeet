using System;
using System.Collections.Generic;

namespace Parakeet.Net.ROClient.Models
{
    public class ProjectAddedModel : ModelBase
    {
        public override string CommandName => "add_project";

        /// <summary>
        /// 项目Id
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 安监备案号
        /// </summary>
        public string RegisterNo { get; set; }

        /// <summary>
        /// 父级区域（省）
        /// </summary>
        public string ParentArea { get; set; }

        /// <summary>
        /// 项目区域（市）
        /// </summary>
        public string Area { get; set; }

        /// <summary>
        /// 子区域（区/县）
        /// </summary>
        public string SubArea { get; set; }

        /// <summary>
        /// 是否激活
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 项目地址信息
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 项目封面图
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 项目扩展
        /// </summary>
        public List<ProjectExtended> Extendeds { get; set; } = new List<ProjectExtended>();
    }
}