using CsLoxInterpreter.Expressions;
using Syntax.CsLoxInterpreter.Details;
using Syntax.CsLoxInterpreter.Errors;
using static Syntax.CsLoxInterpreter.TokenType;
namespace Syntax.CsLoxInterpreter
{
    class Parser
    {
        public List<Token> Tokens { get; set; }
        public int Current = 0;

        public Parser(List<Token> tokens)
        {
            this.Tokens = tokens;
        }

        public List<Stmt> Parse()
        {
            var statements = new List<Stmt>();
            while(!IsAtEnd())
            {
                statements.Add(Statement());
            }
            return statements;
        }

        private Stmt Statement()
        {
            if (Match(PRINT)) return PrintStatement();

            return ExpressionStatement();
        }

        private Stmt ExpressionStatement()
        {
            Expr expr = Expression();
            Consume(SEMICOLON, "Expect ';' after expression.");
            return new Stmt.Expression(expr);
        }

        private Stmt PrintStatement()
        {
            Expr expr = Expression();
            Consume(SEMICOLON, "Expect ';' after value.");
            return new Stmt.Print(expr);
        }

        private Expr Expression() { return Equality(); }

        private Expr Equality()
        {
            var expr = Comparision();
            while (Match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL))
            {
                var @operator = Previous();
                Expr right = Comparision(); // NExt one in the tree;

                expr = new Expr.Binary(expr, @operator, right);
            }
            return expr;
        }

        private Expr Comparision()
        {

            var expr = Term();
            while (Match(TokenType.GREATER, GREATER_EQUAL, LESS, LESS_EQUAL))
            {
                var @operator = Previous();
                var right = Term();
                expr = new Expr.Binary(expr, @operator, right);
            }
            return expr;
        }

        private Expr Term()
        {
            var expr = Factor();
            while (Match(PLUS, MINUS))
            {
                var @operator = Previous();
                var right = Factor();
                expr = new Expr.Binary(expr, @operator, right);
            }
            return expr;
        }

        private Expr Factor()
        {
            var expr = Unary();
            while (Match(STAR, SLASH))
            {
                var @operator = Previous();
                var right = Unary();
                expr = new Expr.Binary(expr, @operator, right);
            }
            return expr;
        }

        private Expr Unary()
        {
            if (Match(BANG, MINUS))
            {
                var @operator = Previous();
                var right = Unary();
                return new Expr.Unary(@operator, right);
            }
            return Primary();
        }

        private Expr Primary()
        {
            if (Match(FALSE)) return new Expr.Literal(false);
            if (Match(TRUE)) return new Expr.Literal(true);
            if (Match(NIL)) return new Expr.Literal(value: null);

            if (Match(NUMBER, STRING)) return new Expr.Literal(Previous().Literal);

            if (Match(LEFT_PAREN))
            {
                // Back to the top
                Expr expr = Expression();
                Consume(RIGHT_PAREN, "Expect ')' after expression");
                return new Expr.Grouping(expr);
            }
            throw Error(Peek(), "Parser error");
        }

        private Token Consume(TokenType type, string message)
        {
            if (Check(type)) return Advance();

            throw Error(Peek(), message);
        }

        private void Synchronize()
        {
            Advance();
            while (!IsAtEnd())
            {
                if (Previous().Type == SEMICOLON) return;

                switch (Peek().Type)
                {
                    case CLASS:
                    case FOR:
                    case FUN:
                    case IF:
                    case PRINT:
                    case RETURN:
                    case VAR:
                    case WHILE:
                        return;
                }
            }

            Advance();
        }

        private Exception Error(Token token, string message)
        {
            CSLox.Error(token, message);
            return new ParserError(message);
        }

        private bool Match(params TokenType[] types)
        {
            foreach (var type in types)
            {
                if (Check(type))
                {
                    Advance();
                    return true;
                }
            }
            return false;
        }

        private bool Check(TokenType type)
        {
            if (IsAtEnd()) return false;
            return Peek().Type == type;
        }

        private Token Peek()
        {
            return this.Tokens[Current];
        }

        private Token Advance()
        {
            if (!IsAtEnd()) Current++;
            return Previous();
        }

        private Token Previous()
        {
            return this.Tokens[Current - 1];
        }

        private bool IsAtEnd()
        {
            return Peek().Type == TokenType.EOF;
        }

    }
}
