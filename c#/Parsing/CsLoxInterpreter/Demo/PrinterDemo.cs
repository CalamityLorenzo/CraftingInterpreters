using CsLoxInterpreter.Expressions;
using static CsLoxInterpreter.TokenType;

namespace CsLoxInterpreter.Demo
{
    public static class DemoPrinter
    {
        public static string GoDemo()
        {
            Expr expression = new Expr.Binary(
            new Expr.Unary(
                new Token(MINUS, "-", null, 1),
                new Expr.Literal(123)),
            new Token(TokenType.STAR, "*", null, 1),
            new Expr.Grouping(
                new Expr.Literal(45.67)));

            return new AstPrinter().Print(expression);
        }

        public static string Chapter05()
        {
            Expr expression =
                new Expr.Binary(
                   new Expr.Binary(new Expr.Literal(1),
                        new Token(TokenType.PLUS, "+", null, 1)
                        , new Expr.Literal(2)),
                        new Token(TokenType.STAR, "*", null, 1),
                   new Expr.Binary(new Expr.Literal(4),
                        new Token(TokenType.MINUS, "-", null, 1)
                        , new Expr.Literal(3)));



            return new RpnAstPrinter().Print(expression);
        }
    }
}