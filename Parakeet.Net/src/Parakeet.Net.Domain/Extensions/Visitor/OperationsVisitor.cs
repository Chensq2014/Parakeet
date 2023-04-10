using System.Linq.Expressions;

namespace Parakeet.Net.Extensions.Visitor
{
    /// <summary>
    /// 理解学习表达式目录树  访问二叉树  访问类
    /// </summary>
    public class OperationsVisitor : ExpressionVisitor
    {
        public Expression Modify(Expression expression)
        {
            return this.Visit(expression);
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            //把加改为减
            //if (b.NodeType == ExpressionType.Add)
            //{
            //    Expression left = this.Visit(b.Left);
            //    Expression right = this.Visit(b.Right);
            //    return Expression.Subtract(left, right);
            //}

            return base.VisitBinary(b);
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            return base.VisitConstant(node);
        }
    }
}
