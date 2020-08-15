using System;
using System.Linq;
using System.Text;
using CsLoxInterpreter.Expressions;

namespace CsLoxInterpreter
{
    internal class RpnAstPrinter : Expr.ILoxVisitor<string>
    {
        public string Print(Expr expr)
        {
            return expr.Accept(this);
        }

        public string VisitBinaryExpr(Expr.Binary expr) => Format(expr.@operator.Lexeme, expr.left, expr.right);

        public string VisitGroupingExpr(Expr.Grouping expr) => Format("group", expr.expression);

        public string VisitLiteralExpr(Expr.Literal expr) => (expr.value == null) ? "null" : expr.value.ToString();
        
        public string VisitUnaryExpr(Expr.Unary expr) => Format(expr.@operator.Lexeme, expr.right);
        
        /// <summary>
        /// This method just wraps the current method in parens for formatting reasons.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="exprs"></param>
        /// <returns></returns>
        private string Format(string name, params Expr[] exprs)
        {
            StringBuilder sb = new StringBuilder();
            
            foreach (var expr in exprs)
            {
                sb.Append(" ");
                sb.Append(expr.Accept(this));
            }
            sb.Append($"{name} ");

            
            return sb.ToString();
        }

    }

}