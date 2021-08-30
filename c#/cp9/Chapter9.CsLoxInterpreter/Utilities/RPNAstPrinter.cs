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

        public string VisitAssignExpr(Expr.Assign expr)
        {
            return Printer(expr.Name.Lexeme, expr.Value);
        }

        public string VisitBinaryExpr(Expr.Binary expr)
        {
            return Printer(expr.Operator.Lexeme, expr.Left, expr.Right);
        }

        public string VisitConditional(Expr.Conditional expr)
        {
            throw new NotImplementedException();
        }

        public string VisitGroupingExpr(Expr.Grouping expr)
        {
            return Printer("group", expr.Expression);
        }

        public string VisitLiteralExpr(Expr.Literal expr) => expr.Value is null ? "nil" : expr.Value.ToString();

        public string VisitLogicalExpr(Expr.Logical expr)
        {
            throw new NotImplementedException();
        }

        public string VisitUnaryExpr(Expr.Unary expr) => Printer(expr.Operator.Lexeme, expr.Right);

        public string VisitVariableExpr(Expr.Variable expr)
        {
            return Printer(expr.Name.Lexeme, expr);
        }

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
