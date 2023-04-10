using System.Linq.Expressions;

namespace Parakeet.Net.Extensions.Visitor
{
    public class NewExpressionVisitor : ExpressionVisitor
    {
        public ParameterExpression NewParameter { get; private set; }
        public NewExpressionVisitor(ParameterExpression param)
        {
            this.NewParameter = param;
        }
        public Expression Replace(Expression exp)
        {
            return this.Visit(exp);
        }
        protected override Expression VisitParameter(ParameterExpression node)
        {
            return this.NewParameter;
        }
    }
}
