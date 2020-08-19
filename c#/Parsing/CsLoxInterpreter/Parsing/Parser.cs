using CsLoxInterpreter.Expressions;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq.Expressions;
using System.Text;
using static CsLoxInterpreter.TokenType;
namespace CsLoxInterpreter.Parsing
{
    /// <summary>
    /// After the scanner we are now working with Tokens.
    /// This is what the scanner produced after working on strings/ and chars.
    /// A recursive descent parser. Each rule is a rule in the grammar too.
    /// </summary>
    class Parser
    {
        private List<Token> tokens;
        private int current = 0;

        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
        }

        public Expr Parse()
        {
            try
            {
                return Expression();

            } catch(Exception ex)
            {
                return null;
            }

        }

        private Expr Expression()
        {
            return this.Equality();
        }
        /// <summary>
        /// comparison ( ( "!=" | "==" ) comparison )* ;
        /// </summary>
        /// <returns></returns>
        private Expr Equality()
        {
            return this.BinaryMethod(Comparison, EQUAL_EQUAL, BANG_EQUAL);
        }
        /// <summary>
        /// Production : comparison → addition ( ( ">" | ">=" | "<" | "<=" ) addition )* ;
        /// </summary>
        /// <returns></returns>
        private Expr Comparison()
        {
            return this.BinaryMethod(Addition, GREATER, GREATER_EQUAL, LESS, LESS_EQUAL);
        }

        /// <summary>
        /// addition       → multiplication ( ( "-" | "+" ) multiplication )* ;
        /// </summary>
        /// <returns></returns>
        private Expr Addition()
        {
            return this.BinaryMethod(Multiplication, MINUS, PLUS);
        }
        /// <summary>
        /// multiplication → unary ( ( "/" | "*" ) unary )* ;
        /// </summary>
        /// <returns></returns>
        private Expr Multiplication()
        {
            
            return this.BinaryMethod(Unary, SLASH, STAR);

        }
        /// <summary>
        ///  ( "!" | "-" ) unary  | primary ;
        /// </summary>
        /// <returns></returns>
        private Expr Unary()
        {
            if (match(BANG, MINUS))
            {
                Token @operator = Previous();
                Expr right = Primary();
                return new Expr.Unary(@operator, right);
            }
            return Primary();
        }

        /// <summary>
        /// primary → NUMBER | STRING | "false" | "true" | "nil" | "(" expression ")" ;
        /// </summary>
        /// <returns></returns>
        private Expr Primary()
        {
            if (match(FALSE)) return new Expr.Literal(false);
            if (match(TRUE)) return new Expr.Literal(true);
            if (match(NIL)) return new Expr.Literal(null);

            if (match(NUMBER, STRING))
                return new Expr.Literal(Previous().Literal);

            if (match(LEFT_PAREN))
            {
                Expr expr = Expression();
                Consume(RIGHT_PAREN, "Expect ')' after expression.");
                return new Expr.Grouping(expr);
            }

            throw ParserError(Peek(), "Expected expression.");
        }

        private Expr BinaryMethod(Func<Expr> NextMethod, params TokenType[] matches)
        {
            Expr expr = NextMethod();
            while (match(matches))
            {
                Token @operator = Previous();
                Expr right = NextMethod();
                expr = new Expr.Binary(expr, @operator, right);
            }
            return expr;
        }

        private Token Consume(TokenType type, string message)
        {

            if (Check(type)) return Advance();
            throw ParserError(Peek(), message);
        }

        private Exception ParserError(Token token, string message)
        {
            CSLox.Error(token, message);
            throw new Exception();
        }

        private void Synchronize()
        {
            this.Advance();
            while (!IsAtEnd())
            {
                if (Previous().TokenType == SEMICOLON) return;
                switch(Peek().TokenType)
                {
                    case CLASS:
                    case FUN:
                    case VAR:
                    case FOR:
                    case IF:
                    case WHILE:
                    case PRINT:
                    case RETURN:
                        return;
                }
            }
            Advance();
        }

        private bool match(params TokenType[] types)
        {
            foreach (var tokenType in types)
                if (Check(tokenType))
                {
                    Advance();
                    return true;
                }
            return false;
        }


        private bool Check(TokenType type)
        {
            if (IsAtEnd()) return false;
            return Peek().TokenType == type;
        }

        private bool IsAtEnd()
        {
            return Peek().TokenType == EOF;
        }

        private Token Peek()
        {
            return this.tokens[current];
        }

        private Token Advance()
        {
            if (!IsAtEnd()) current++;
            return Previous();
        }
        private Token Previous()
        {
            return this.tokens[current - 1];
        }


    }
}
