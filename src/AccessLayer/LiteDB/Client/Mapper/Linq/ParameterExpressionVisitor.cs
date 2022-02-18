using System.Linq.Expressions;

namespace LiteDB
{
    /// <summary>
    ///     Class used to test in an Expression member expression is based on parameter `x => x.Name` or variable `x => externalVar`
    /// </summary>
    internal class ParameterExpressionVisitor : ExpressionVisitor
    {
        #region Fields and Autoproperties

        public bool IsParameter { get; private set; }

        #endregion

        public static bool Test(Expression node)
        {
            var instance = new ParameterExpressionVisitor();

            instance.Visit(node);

            return instance.IsParameter;
        }

        /// <inheritdoc />
        protected override Expression VisitParameter(ParameterExpression node)
        {
            IsParameter = true;

            return base.VisitParameter(node);
        }
    }
}
