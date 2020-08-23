using System;
using System.Data;
using System.Linq;
using System.Text;
using CsLoxInterpreter.Expressions;

namespace CsLoxInterpreter
{
    internal class AstPrinter : Expr.ILoxVisitor<string>
    {
        public string Print(Expr expr)
        {
            return expr.Accept(this);
        }

        public string VisitBinaryExpr(Expr.Binary expr) => Parenthesize(expr.@operator.Lexeme, expr.left, expr.right);

        public string VisitGroupingExpr(Expr.Grouping expr) => Parenthesize("group", expr.expression);

        public string VisitLiteralExpr(Expr.Literal expr) => (expr.value == null) ? "null" : expr.value.ToString();
        
        public string VisitUnaryExpr(Expr.Unary expr) => Parenthesize(expr.@operator.Lexeme, expr.right);

        public string VisitTernaryExpr(Expr.Ternary expr) => $"({expr.Expression.Accept(this)} ? {expr.IfTrue.Accept(this)} : {expr.IfFalse.Accept(this)})";
        /// <summary>
        /// This method just wraps the current method in parens for formatting reasons.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="exprs"></param>
        /// <returns></returns>
        private string Parenthesize(string name, params Expr[] exprs)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("(");
            sb.Append(name);
            foreach (var expr in exprs)
            {
                sb.Append(" ");
                sb.Append(expr.Accept(this));
            }

            sb.Append(")");
            return sb.ToString();
        }
   

    }

}