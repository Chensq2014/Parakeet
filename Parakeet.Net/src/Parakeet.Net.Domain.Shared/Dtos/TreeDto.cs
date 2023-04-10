using System.ComponentModel;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// DxTreeView公共dto类
    /// </summary>
    [Description("DxTreeView控件公共类")]
    public class TreeDto : BaseTreeDto
    {
        public TreeDto()
        {
            DisplayDirection = "x";
            Expanded = true;
        }
    }
    /// <summary>
    /// DxTreeView公共基类
    /// </summary>
    [Description("DxTreeView控件公共基类")]
    public class TreeDto<TKey> : BaseTreeDto
    {
        public TreeDto()
        {
            DisplayDirection = "x";
            Expanded = true;
        }
        /// <summary>
        /// 节点Id 泛型的给出new覆盖基类中的整型Id
        /// </summary>
        [Description("节点")]
        public new TKey Id { get; set; }

        /// <summary>
        /// 父节点ParentId
        /// </summary>
        [Description("父级")]
        public new TKey ParentId { get; set; }
    }
}
