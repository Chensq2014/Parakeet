namespace ConsoleApp.Models
{
    /// <summary>
    /// 树节点 中序遍历
    /// </summary>
    public class TreeNode
    {
        public int Value { get; set; }
        public TreeNode Left { get; set; }
        public TreeNode Right { get; set; }
        public TreeNode(int value, TreeNode left = null, TreeNode right = null)
        {
            Value = value;
            Left = left;
            Right = right;
        }


        /// <summary>
        /// 中序遍历
        /// </summary>
        /// <param name="root"></param>
        /// <param name="nodes"></param>
        public void InorderTraversal(TreeNode root, List<int> nodes)
        {
            if (root != null)
            {
                InorderTraversal(root.Left, nodes);
                nodes.Add(root.Value);
                InorderTraversal(root.Right, nodes);
            }
        }


        /// <summary>
        /// 求根节点的二叉树深度
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public int MaxDepth(TreeNode root)
        {
            if (root == null)
            {
                return 0;//叶子节点的深度为0
            }

            var leftDepth = MaxDepth(root.Left);
            var rightDepth = MaxDepth(root.Right);
            return leftDepth + rightDepth + 1;//有左右节点的根节点 深度至少为1
        }
    }
}
