using CsLoxInterpreter.Expressions;
using System;
using System.Text;

namespace CsLoxInterpreter.Utilities
{
    internal class AstPrinter : Expr.ILoxVisitor<string>
    {
        private AstPrinter() { }
        public static string PrintExpression(Expr expr) => new AstPrinter().Print(expr);

        public string Print(Expr expr)
        {
            return expr.Accept(this);
        }

        public string VisitAssignExpr(Expr.Assign expr)
        {
            return Parenthesize(expr.Name.Lexeme, expr.Value);
        }

        public string VisitBinaryExpr(Expr.Binary expr)
        {
            return Parenthesize(expr.Operator.Lexeme, expr.Left, expr.Right);
        }

        public string VisitGroupingExpr(Expr.Grouping expr)
        {
            return Parenthesize("group", expr.Expression);
        }

        public string VisitLiteralExpr(Expr.Literal expr) => expr.Value == null ? "nil" : expr.Value.ToString();

        string Expr.ILoxVisitor<string>.VisitLogicalExpr(Expr.Logical expr)
        {
            throw new NotImplementedException();
        }

        public string VisitUnaryExpr(Expr.Unary expr) => Parenthesize(expr.Operator.Lexeme, expr.Right);

        public string VisitVariableExpr(Expr.Variable expr)
        {
            return Parenthesize(expr.Name.Lexeme, expr);
        }

        private string Parenthesize(string name, params Expr[] exprs)
        {
            var sb = new StringBuilder();

            sb.Append("(").Append(name);
            foreach (var expr in exprs)
            {
                sb.Append(" ");
                sb.Append(expr.Accept(this));
            }
            sb.Append(")");
            return sb.ToString();
        }

        private string VisitCallExpr(Expr.Call expr)
        {
            throw new NotImplementedException();
        }

        string Expr.ILoxVisitor<string>.VisitCallExpr(Expr.Call expr)
        {
            throw new NotImplementedException();
        }

        public string VisitGetExpr(Expr.Get expr)
        {
            throw new NotImplementedException();
        }

        public string VisitSetExpr(Expr.Set expr)
        {
            throw new NotImplementedException();
        }

        public string VisitThisExpr(Expr.This expr)
        {
            throw new NotImplementedException();
        }

        public string VisitSuperExpr(Expr.Super expr)
        {
            throw new NotImplementedException();
        }
    }
}
