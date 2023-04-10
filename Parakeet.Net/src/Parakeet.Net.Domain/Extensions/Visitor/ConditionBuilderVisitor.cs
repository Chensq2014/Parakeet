using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Parakeet.Net.Extensions.Visitor
{
    /// <summary>
    /// 将表达式目录树解析为sql
    /// </summary>
    public class ConditionBuilderVisitor : ExpressionVisitor
    {
        private readonly Stack<string> _stringStack = new Stack<string>();

        public string Condition()
        {
            string condition = string.Concat(this._stringStack.ToArray());
            this._stringStack.Clear();
            return condition;
        }

        /// <summary>
        /// 如果是二元表达式
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitBinary(BinaryExpression node)
        {
            if (node == null) throw new ArgumentNullException("BinaryExpression");

            this._stringStack.Push(")");
            base.Visit(node.Right);//解析右边
            this._stringStack.Push(" " + node.NodeType.ToSqlOperator() + " ");
            base.Visit(node.Left);//解析左边
            this._stringStack.Push("(");

            return node;
        }
        protected override Expression VisitMember(MemberExpression node)
        {
            if (node == null) throw new ArgumentNullException("MemberExpression");
            this._stringStack.Push(" [" + node.Member.Name + "] ");
            return node;
        }
        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (node == null) throw new ArgumentNullException("ConstantExpression");
            this._stringStack.Push(" '" + node.Value + "' ");
            return node;
        }
        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m == null) throw new ArgumentNullException("MethodCallExpression");
            
            string format;
            switch (m.Method.Name)
            {
                case "StartsWith":
                    format = "({0} LIKE {1}+'%')";
                    break;

                case "Contains":
                    format = "({0} LIKE '%'+{1}+'%')";
                    break;

                case "EndsWith":
                    format = "({0} LIKE '%'+{1})";
                    break;

                default:
                    throw new NotSupportedException(m.NodeType + " is not supported!");
            }
            this.Visit(m.Object);
            this.Visit(m.Arguments[0]);
            string right = this._stringStack.Pop();
            string left = this._stringStack.Pop();
            this._stringStack.Push(String.Format(format, left, right));

            return m;
        }
    }

    internal static class SqlOperator
    {
        internal static string ToSqlOperator(this ExpressionType type)
        {
            switch (type)
            {
                case (ExpressionType.AndAlso):
                case (ExpressionType.And):
                    return "AND";
                case (ExpressionType.OrElse):
                case (ExpressionType.Or):
                    return "OR";
                case (ExpressionType.Not):
                    return "NOT";
                case (ExpressionType.NotEqual):
                    return "<>";
                case ExpressionType.GreaterThan:
                    return ">";
                case ExpressionType.GreaterThanOrEqual:
                    return ">=";
                case ExpressionType.LessThan:
                    return "<";
                case ExpressionType.LessThanOrEqual:
                    return "<=";
                case (ExpressionType.Equal):
                    return "=";
                default:
                    throw new Exception("不支持该方法");
            }

        }
    }
}
