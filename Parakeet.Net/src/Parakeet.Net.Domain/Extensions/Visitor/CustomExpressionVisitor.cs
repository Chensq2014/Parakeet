using Microsoft.Data.SqlClient;
using System;
using Serilog;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Parakeet.Net.CustomAttributes;

namespace Parakeet.Net.Extensions.Visitor
{
    public class CustomExpressionVisitor : ExpressionVisitor
    {
        private Stack<string> _conditionStack = new Stack<string>();
        private List<SqlParameter> _sqlParameterList = new List<SqlParameter>();
        private object _tempValue = null;


        public string GetWhere(out List<SqlParameter> sqlParameters)
        {
            string where = string.Concat(this._conditionStack.ToArray());
            this._conditionStack.Clear();
            sqlParameters = _sqlParameterList;
            return where;
        }


        public override Expression Visit(Expression node)
        {
            Log.Logger.Information($"Visit入口：{node.NodeType} {node.Type} {node.ToString()}");
            return base.Visit(node);
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            Log.Logger.Information($"VisitBinary：{node.NodeType} {node.Type} {node.ToString()}");
            this._conditionStack.Push(" ) ");
            base.Visit(node.Right);
            this._conditionStack.Push(node.NodeType.ToSqlOperator());
            base.Visit(node.Left);
            this._conditionStack.Push(" ( ");
            return node;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            Log.Logger.Information($"VisitConstant：{node.NodeType} {node.Type} {node.ToString()}");
            //this.ConditionStack.Push($"'{node.Value.ToString()}'");
            this._tempValue = node.Value;
            //栈里面不要值，要的是@PropertyName,但是从后往前，先有值再有属性--但是二者是连续的
            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            Log.Logger.Information($"VisitMember：{node.NodeType} {node.Type} {node.ToString()}");
            //this.ConditionStack.Push($"{node.Member.GetMappingName()}");
            if (node.Expression is ConstantExpression)
            {
                var value1 = this.InvokeValue(node);
                var value2 = this.ReflectionValue(node);
                //this.ConditionStack.Push($"'{value1}'");
                this._tempValue = value1;
            }
            else
            {
                //this.ConditionStack.Push($"{node.Member.Name}");
                //this.ConditionStack.Push($"{node.Member.GetMappingName()}");//映射数据
                if (this._tempValue != null)
                {
                    string name = node.Member.GetMappingName();
                    string paraName = $"@{name}{this._sqlParameterList.Count}";
                    string sOperator = this._conditionStack.Pop();
                    this._conditionStack.Push(paraName);
                    this._conditionStack.Push(sOperator);
                    this._conditionStack.Push(name);

                    var tempValue = this._tempValue;
                    this._sqlParameterList.Add(new SqlParameter(paraName, tempValue));
                    this._tempValue = null;
                }
            }
            return node;
        }

        private object InvokeValue(MemberExpression member)
        {
            var objExp = Expression.Convert(member, typeof(object));//struct需要
            return Expression.Lambda<Func<object>>(objExp).Compile().Invoke();
        }

        private object ReflectionValue(MemberExpression member)
        {
            var obj = (member.Expression as ConstantExpression).Value;
            return (member.Member as FieldInfo).GetValue(obj);
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m == null) throw new ArgumentNullException("MethodCallExpression");

            this.Visit(m.Arguments[0]);
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
            this._conditionStack.Push(format);
            this.Visit(m.Object);
            var left = this._conditionStack.Pop();
            format = this._conditionStack.Pop();
            var right = this._conditionStack.Pop();
            this._conditionStack.Push(string.Format(format, left, right));

            return m;
        }
    }
}
