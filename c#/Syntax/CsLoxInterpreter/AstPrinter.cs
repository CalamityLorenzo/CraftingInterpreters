using System;
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


        public string VisitLiteralExpr(Expr.Literal expr)
        {
            if (expr.value == null) return "null";
            return expr.value.ToString();
        }

        public string VisitUnaryExpr(Expr.Unary expr) => Parenthesize(expr.@operator.Lexeme, expr.right);


        private string Parenthesize(string name, params Expr[] exprs)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("(").Append(name);
            foreach(var expr in exprs)
            {
                sb.Append("  ");
                sb.Append(expr.Accept(this));
            }
            
            sb.Append(")");
            return sb.ToString();
        }

    }

}