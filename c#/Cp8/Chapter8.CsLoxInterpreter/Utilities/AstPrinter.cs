using CsLoxInterpreter.Expressions;
using System.Text;

namespace CsLoxInterpreter.Utilities
{
    public class AstPrinter : Expr.ILoxVisitor<string>
    {
        private AstPrinter() { }
        public static string PrintExpression(Expr expr) => new AstPrinter().Print(expr);

        public string Print(Expr expr)
        {
            return expr.Accept(this);
        }

        public string VisitAssignExpr(Expr.Assign expr)
        {
            return Parenthesize(expr.name.Lexeme, expr.value);
        }

        public string VisitBinaryExpr(Expr.Binary expr)
        {
            return Parenthesize(expr.@operator.Lexeme, expr.left, expr.right);
        }

        public string VisitGroupingExpr(Expr.Grouping expr)
        {
            return Parenthesize("group", expr.expression);
        }

        public string VisitLiteralExpr(Expr.Literal expr) => expr.value == null ? "nil" : expr.value.ToString();

        public string VisitUnaryExpr(Expr.Unary expr) => Parenthesize(expr.@operator.Lexeme, expr.right);

        public string VisitVariableExpr(Expr.Variable expr)
        {
            return Parenthesize(expr.name.Lexeme, expr);
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
    }
}
