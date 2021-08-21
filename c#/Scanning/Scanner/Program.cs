using Syntax.CsLoxInterpreter;
using Syntax.CsLoxInterpreter.Expressions;
using Syntax.CsLoxInterpreter.Utilities;
using System;
namespace app
{
    class Program
    {

        static void Main(string[] args)
        {
            Syntax.CsLoxInterpreter.Details.CSLox.Main(args);
        }

        static void Old(string[] args)
        {
            // CsLoxInterpreter.Details.CSLox.Main(args);
            var expr = new Expr.Binary(
                   new Expr.Unary(
                           new Token(TokenType.MINUS, "-", null, 1),
                           new Expr.Literal(123)),
                   new Token(TokenType.STAR, "*", null, 1),
                   new Expr.Grouping(
                       new Expr.Literal(45.67)));


            var expr2 = new Expr.Binary(
                    new Expr.Binary(new Expr.Literal(100), new Token(TokenType.PLUS, "*", null, 1), new Expr.Literal(45)),
                    new Token(TokenType.STAR, "*", null, 1),
                    new Expr.Binary(new Expr.Literal(80), new Token(TokenType.SLASH, "/", null, 1), new Expr.Literal(42)));
            Console.WriteLine(AstPrinter.PrintExpression(expr));
            Console.WriteLine(RpnAstPrinter.PrintExpression(expr));

            Console.WriteLine(AstPrinter.PrintExpression(expr2));
            Console.WriteLine(RpnAstPrinter.PrintExpression(expr2));
        }
    }
}
