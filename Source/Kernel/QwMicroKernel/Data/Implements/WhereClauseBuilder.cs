using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace QwMicroKernel.Data.Implements
{
    public abstract class WhereClauseBuilder<TModel> : ExpressionVisitor, IWhereClauseBuilder<TModel>
            where TModel : class, new()
    {
        protected static readonly IMappingResolver EmptyMappingResolver = new EmptyMappingResolver();

        #region Private Fields

        private readonly StringBuilder _builder = new StringBuilder();
        private readonly IDictionary<string, object> _parameterValues = new Dictionary<string, object>();
        private readonly IMappingResolver _mappingResolver = null;
        private bool _startsWith = false;
        private bool _endsWith = false;
        private bool _contains = false;

        #endregion

        #region Ctor

        protected WhereClauseBuilder() : this(EmptyMappingResolver) { }

        protected WhereClauseBuilder(IMappingResolver mappingResolver)
        {
            this._mappingResolver = mappingResolver;
        }

        #endregion

        #region Private Methods

        private void Out(string s)
        {
            _builder.Append(s);
        }

        private void OutMember(Expression instance, MemberInfo member)
        {
            string mappedFieldName = _mappingResolver.ResolveFieldName<TModel>(member);
            Out(mappedFieldName);
        }

        #endregion

        #region Protected Properties

        protected virtual string And
        {
            get { return "AND"; }
        }
        protected virtual string Or
        {
            get { return "OR"; }
        }
        protected virtual string Equal
        {
            get { return "="; }
        }
        protected virtual string Not
        {
            get { return "NOT"; }
        }
        protected virtual string NotEqual
        {
            get { return "<>"; }
        }
        protected virtual string Like
        {
            get { return "LIKE"; }
        }
        protected virtual char LikeSymbol
        {
            get { return '%'; }
        }
        protected abstract char ParameterChar { get; }

        #endregion

        #region Protected Methods

        protected override Expression VisitBinary(BinaryExpression node)
        {
            string str;
            switch (node.NodeType)
            {
                case ExpressionType.Add:
                    str = "+";
                    break;
                case ExpressionType.AddChecked:
                    str = "+";
                    break;
                case ExpressionType.AndAlso:
                    str = this.And;
                    break;
                case ExpressionType.Divide:
                    str = "/";
                    break;
                case ExpressionType.Equal:
                    str = this.Equal;
                    break;
                case ExpressionType.GreaterThan:
                    str = ">";
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    str = ">=";
                    break;
                case ExpressionType.LessThan:
                    str = "<";
                    break;
                case ExpressionType.LessThanOrEqual:
                    str = "<=";
                    break;
                case ExpressionType.Modulo:
                    str = "%";
                    break;
                case ExpressionType.Multiply:
                    str = "*";
                    break;
                case ExpressionType.MultiplyChecked:
                    str = "*";
                    break;
                case ExpressionType.Not:
                    str = this.Not;
                    break;
                case ExpressionType.NotEqual:
                    str = this.NotEqual;
                    break;
                case ExpressionType.OrElse:
                    str = this.Or;
                    break;
                case ExpressionType.Subtract:
                    str = "-";
                    break;
                case ExpressionType.SubtractChecked:
                    str = "-";
                    break;
                default:
                    throw new NotSupportedException(node.NodeType.ToString());
            }

            Out("(");
            Visit(node.Left);
            Out(" ");
            Out(str);
            Out(" ");
            Visit(node.Right);
            Out(")");
            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Member.DeclaringType == typeof(TModel) ||
                typeof(TModel).IsSubclassOf(node.Member.DeclaringType))
            {
                string mappedFieldName = _mappingResolver.ResolveFieldName<TModel>(node.Member);
                Out(mappedFieldName);
            }
            else
            {
                if (node.Member is FieldInfo)
                {
                    ConstantExpression ce = node.Expression as ConstantExpression;
                    FieldInfo fi = node.Member as FieldInfo;
                    object fieldValue = fi.GetValue(ce.Value);
                    Expression constantExpr = Expression.Constant(fieldValue);
                    Visit(constantExpr);
                }
                else
                    throw new NotSupportedException(node.Member.GetType().FullName);
            }
            return node;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            string paramName = string.Format("{0}{1}", ParameterChar, Utils.GetUniqueIdentifier(5));
            Out(paramName);
            if (!_parameterValues.ContainsKey(paramName))
            {
                object v = null;
                if (_startsWith && node.Value is string)
                {
                    _startsWith = false;
                    v = node.Value.ToString() + LikeSymbol;
                }
                else if (_endsWith && node.Value is string)
                {
                    _endsWith = false;
                    v = LikeSymbol + node.Value.ToString();
                }
                else if (_contains && node.Value is string)
                {
                    _contains = false;
                    v = LikeSymbol + node.Value.ToString() + LikeSymbol;
                }
                else
                {
                    v = node.Value;
                }
                _parameterValues.Add(paramName, v);
            }
            return node;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            Out("(");
            Visit(node.Object);
            if (node.Arguments == null || node.Arguments.Count != 1)
                throw new NotSupportedException();
            Expression expr = node.Arguments[0];
            switch (node.Method.Name)
            {
                case "StartsWith":
                    _startsWith = true;
                    Out(" ");
                    Out(Like);
                    Out(" ");
                    break;
                case "EndsWith":
                    _endsWith = true;
                    Out(" ");
                    Out(Like);
                    Out(" ");
                    break;
                case "Equals":
                    Out(" ");
                    Out(Equal);
                    Out(" ");
                    break;
                case "Contains":
                    _contains = true;
                    Out(" ");
                    Out(Like);
                    Out(" ");
                    break;
                default:
                    throw new NotSupportedException(node.Method.Name);
            }
            if (expr is ConstantExpression || expr is MemberExpression)
                Visit(expr);
            else
                throw new NotSupportedException(expr.GetType().ToString());
            Out(")");
            return node;
        }

        protected override Expression VisitBlock(BlockExpression node)
        {
            throw new NotSupportedException(node.GetType().Name);
        }

        protected override CatchBlock VisitCatchBlock(CatchBlock node)
        {
            throw new NotSupportedException(node.GetType().Name);
        }

        protected override Expression VisitConditional(ConditionalExpression node)
        {
            throw new NotSupportedException(node.GetType().Name);
        }

        protected override Expression VisitDebugInfo(DebugInfoExpression node)
        {
            throw new NotSupportedException(node.GetType().Name);
        }

        protected override Expression VisitDefault(DefaultExpression node)
        {
            throw new NotSupportedException(node.GetType().Name);
        }

        protected override Expression VisitDynamic(DynamicExpression node)
        {
            throw new NotSupportedException(node.GetType().Name);
        }

        protected override ElementInit VisitElementInit(ElementInit node)
        {
            throw new NotSupportedException(node.GetType().Name);
        }

        protected override Expression VisitGoto(GotoExpression node)
        {
            throw new NotSupportedException(node.GetType().Name);
        }

        protected override Expression VisitExtension(Expression node)
        {
            throw new NotSupportedException(node.GetType().Name);
        }

        protected override Expression VisitIndex(IndexExpression node)
        {
            throw new NotSupportedException(node.GetType().Name);
        }

        protected override Expression VisitInvocation(InvocationExpression node)
        {
            throw new NotSupportedException(node.GetType().Name);
        }

        protected override Expression VisitLabel(LabelExpression node)
        {
            throw new NotSupportedException(node.GetType().Name);
        }

        protected override LabelTarget VisitLabelTarget(LabelTarget node)
        {
            throw new NotSupportedException(node.GetType().Name);
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            throw new NotSupportedException(node.GetType().Name);
        }

        protected override Expression VisitListInit(ListInitExpression node)
        {
            throw new NotSupportedException(node.GetType().Name);
        }

        protected override Expression VisitLoop(LoopExpression node)
        {
            throw new NotSupportedException(node.GetType().Name);
        }

        protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
        {
            throw new NotSupportedException(node.GetType().Name);
        }

        protected override MemberBinding VisitMemberBinding(MemberBinding node)
        {
            throw new NotSupportedException(node.GetType().Name);
        }

        protected override Expression VisitMemberInit(MemberInitExpression node)
        {
            throw new NotSupportedException(node.GetType().Name);
        }

        protected override MemberListBinding VisitMemberListBinding(MemberListBinding node)
        {
            throw new NotSupportedException(node.GetType().Name);
        }

        protected override MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding node)
        {
            throw new NotSupportedException(node.GetType().Name);
        }

        protected override Expression VisitNew(NewExpression node)
        {
            throw new NotSupportedException(node.GetType().Name);
        }

        protected override Expression VisitNewArray(NewArrayExpression node)
        {
            throw new NotSupportedException(node.GetType().Name);
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            throw new NotSupportedException(node.GetType().Name);
        }

        protected override Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
        {
            throw new NotSupportedException(node.GetType().Name);
        }

        protected override Expression VisitSwitch(SwitchExpression node)
        {
            throw new NotSupportedException(node.GetType().Name);
        }

        protected override SwitchCase VisitSwitchCase(SwitchCase node)
        {
            throw new NotSupportedException(node.GetType().Name);
        }

        protected override Expression VisitTry(TryExpression node)
        {
            throw new NotSupportedException(node.GetType().Name);
        }

        protected override Expression VisitTypeBinary(TypeBinaryExpression node)
        {
            throw new NotSupportedException(node.GetType().Name);
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            throw new NotSupportedException(node.GetType().Name);
        }

        #endregion

        #region IWhereClauseBuilder<T> Members

        public WhereClauseBuildResult BuildWhereClause(Expression<Func<TModel, bool>> expression)
        {
            this._builder.Clear();
            this._parameterValues.Clear();
            this.Visit(expression.Body);
            var result = new WhereClauseBuildResult(_builder.ToString(), _parameterValues);
            return result;
        }

        #endregion
    }
}
