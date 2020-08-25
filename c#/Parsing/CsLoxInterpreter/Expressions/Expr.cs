using System;
using System.Collections.Generic;

namespace CsLoxInterpreter.Expressions
{
    internal abstract class Expr
    {
        // T Allow us to create more easily create typed implementaions
        // eg string, eg TokenRepString etc
        internal interface ILoxVisitor<T>
        {
            T VisitBinaryExpr(Binary expr);
            T VisitTernaryExpr(Ternary expr);
            T VisitGroupingExpr(Grouping expr);
            T VisitLiteralExpr(Literal expr);
            T VisitUnaryExpr(Unary expr);
            T VisitComma(Comma comma);
        }

        internal abstract T Accept<T>(ILoxVisitor<T> visitor);

        internal class Comma : Expr
        {
            public Comma(Expr left, Expr right)
            {
                Left = left;
                Right = right;
            }

            public Expr Left { get; }
            public Expr Right { get; }

            internal override T Accept<T>(ILoxVisitor<T> visitor)
            {
                return visitor.VisitComma(this);
            }
        }

        internal class Binary : Expr
        {
            internal Binary(Expr left, Token @operator, Expr right)
            {
                this.left = left;
                this.@operator = @operator;
                this.right = right;
            }

            internal override T Accept<T>(ILoxVisitor<T> visitor)
            {
                return visitor.VisitBinaryExpr(this);
            }
            public Expr left { get; }
            public Token @operator { get; }
            public Expr right { get; }
        }

        internal class Ternary : Expr
        {
            internal Ternary(Expr expression, Expr ifTrue, Expr ifFalse)
            {
                Expression = expression;
                IfTrue = ifTrue;
                IfFalse = ifFalse;
            }

            public Expr Expression { get; }
            public Expr IfTrue { get; }
            public Expr IfFalse { get; }

            internal override T Accept<T>(ILoxVisitor<T> visitor) => visitor.VisitTernaryExpr(this);

        }

        internal class Grouping : Expr
        {
            internal Grouping(Expr expression)
            {
                this.expression = expression;
            }

            internal override T Accept<T>(ILoxVisitor<T> visitor)
            {
                return visitor.VisitGroupingExpr(this);
            }
            public Expr expression { get; }
        }

        internal class Literal : Expr
        {
            internal Literal(Object value)
            {
                this.value = value;
            }


            internal override T Accept<T>(ILoxVisitor<T> visitor)
            {
                return visitor.VisitLiteralExpr(this);
            }
            public Object value { get; }
        }

        internal class Unary : Expr
        {
            internal Unary(Token @operator, Expr right)
            {
                this.@operator = @operator;
                this.right = right;
            }

            internal override T Accept<T>(ILoxVisitor<T> visitor)
            {
                return visitor.VisitUnaryExpr(this);
            }
            public Token @operator { get; }
            public Expr right { get; }
        }

    }
}
