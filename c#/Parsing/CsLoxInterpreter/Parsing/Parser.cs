using CsLoxInterpreter.Exceptions;
using CsLoxInterpreter.Expressions;
using System;
using System.Collections.Generic;
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
        /// <summary>
        /// Entry point
        /// </summary>
        /// <returns></returns>
        public Expr Parse()
        {
            try
            {
                return Comma();

            }
            catch (ParserException ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }
        /// <summary>
        ///  comma  →  expression ("," expression)*

        /// </summary>
        /// <returns></returns>
        private Expr Comma()
        {
            Expr expr = this.Expression();

            while (match(COMMA))
            {
                // Token @operator = this.Previous();
                Expr right = this.Expression();
                expr = new Expr.Comma(expr, right);
            }
            return expr;
        }

        /// <summary>
        /// ternary -> equality ("?" expression ":" ternary)?
        /// </summary>
        /// <returns></returns>
        private Expr Ternary()
        {
            Expr expr = this.Equality();
            if (match(QUESTION))
            {
                Expr thenC = this.Expression();
                Consume(COLON, "Expect ':' and yet none was forth coming");
                Expr elseBranch = Ternary();
                expr = new Expr.Ternary(expr, thenC, elseBranch);
            }

            return expr;
        }

        private Expr Expression()
        {
            return this.Ternary();
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
        /// addition → multiplication ( ( "-" | "+" ) multiplication )* ;
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
        /// primary → NUMBER | STRING | "false" | "true" | "nil" | !=| == |< | <=| > | >= | + | * | / "(" expression ")" ;
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
                Expr expr = Comma();
                // We have just processed an expression, the next value MUST be a Right Parens to close the expression,
                Consume(RIGHT_PAREN, "Expect ')' after expression.");
                return new Expr.Grouping(expr);
            }
            // Error productions
            // We fell alll the way to the bottom to discover these schumcks on at the start of an expression
            // This is an illegal stae. but I don't want to cause a ruckuse.
            // inform eat the matched/orphand right hand  operand and move on.
            if (match(BANG_EQUAL, EQUAL_EQUAL))
            {
                ParserError(Previous(), "Missing left-hand operand.");
                Equality();
                return new Expr.Literal("nil");
            };

            if (match(GREATER, GREATER_EQUAL, LESS, LESS_EQUAL))
            {
                ParserError(Previous(), "Missing left-hand operand.");
                Comparison();
                return new Expr.Literal("nil");
            };

            if (match(PLUS))
            {
                ParserError(Previous(), "Missing left-hand operand.");
                Addition();
                return new Expr.Literal("nil");
            };
            if (match(STAR, SLASH))
            {
                ParserError(Previous(), "Missing left-hand operand.");
                Multiplication();
                return new Expr.Literal("nil");
            };

            throw ParserError(Peek(), "Expected expression.");
        }
        /// <summary>
        /// Helper method to extract Binary Expressions.
        /// </summary>
        /// <param name="NextMethod"></param>
        /// <param name="matches"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Certain contruscts have explicit strcuture (like ternary x ? a : B) if Explict type is not provided then
        /// we have stumnled intoa blind alley and should bomb
        /// </summary>
        /// <param name="type"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private Token Consume(TokenType type, string message)
        {
            if (Check(type)) return Advance();
            throw ParserError(Peek(), message);
        }

        /// <summary>
        /// Report the error then throw the error.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private ParserException ParserError(Token token, string message)
        {
            CSLox.Error(token, message);
            return new ParserException(token, message);
        }
        /// <summary>
        /// Error handling.
        /// If we have a non-fatal error put the parser in state where it can continue.
        /// This means moving along until the end of statement(;) or a viab ekeyword is encountered
        /// </summary>
        private void Synchronize()
        {
            this.Advance();
            while (!IsAtEnd())
            {
                if (Previous().TokenType == SEMICOLON) return;
                switch (Peek().TokenType)
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
