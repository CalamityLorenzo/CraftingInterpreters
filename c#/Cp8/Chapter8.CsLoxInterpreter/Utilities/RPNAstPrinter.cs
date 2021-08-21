using CsLoxInterpreter.Expressions;
using System.Text;

namespace CsLoxInterpreter.Utilities
{
    public class RpnAstPrinter  : Expr.ILoxVisitor<string>
    {
        private RpnAstPrinter () { }
        public static string PrintExpression(Expr expr) => new RpnAstPrinter ().Print(expr);

        public string Print(Expr expr)
        {
            return expr.Accept(this);
        }
        public string VisitBinaryExpr(Expr.Binary expr)
        {
            return Printer(expr.@operator.Lexeme, expr.left, expr.right);
        }

        public string VisitGroupingExpr(Expr.Grouping expr)
        {
            return Printer("group", expr.expression);
        }

        public string VisitLiteralExpr(Expr.Literal expr) => expr.value is null ? "nil" : expr.value.ToString();
       

        public string VisitUnaryExpr(Expr.Unary expr) => Printer(expr.@operator.Lexeme, expr.right);
        

        private string Printer(string name, params Expr[] exprs)
        {
            var sb = new StringBuilder();
            foreach (var expr in exprs)
            {
                sb.Append(expr.Accept(this));
                sb.Append(" ");
            }
            sb.Append($"{name}");
            return sb.ToString();
        }
    }
}
