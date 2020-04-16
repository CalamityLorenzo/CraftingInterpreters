using CsLoxInterpreter.Expressions;

namespace CsLoxInterpreter.Demo
{
    public static class DemoPrinter
    {
        public static string GoDemo()
        {
            Expr expression = new Expr.Binary(
            new Expr.Unary(
                new Token(TokenType.MINUS, "-", null, 1),
                new Expr.Literal(123)),
            new Token(TokenType.STAR, "*", null, 1),
            new Expr.Grouping(
                new Expr.Literal(45.67)));

            return new AstPrinter().Print(expression);
        }
    }
}