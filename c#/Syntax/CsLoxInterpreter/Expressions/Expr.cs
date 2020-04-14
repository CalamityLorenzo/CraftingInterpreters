using System;
using System.Collection.Generic;
namespace Expressions
{
    internal abstract class Expr
    {
        abstract T Accept<T>(Visitor<T> visitor);
        interface Visitor<T>
        {
            T VisitBinaryExpr(Binary expr);
            T VisitGroupingExpr(Grouping expr);
            T VisitLiteralExpr(Literal expr);
            T VisitUnaryExpr(Unary expr);
        }

        static class Binary : Expr
        {
            Binary(Expr left, Token @operator, Expr right)
            {
                this.left = left;
                this.@operator = @operator;
                this.right = right;
            }


            override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.visitBinaryExpr(this);
            }
            public Expr left { get; }
            public Token @operator { get; }
            public Expr right { get; }
        }

        static class Grouping : Expr
        {
            Grouping(Expr expression)
            {
                this.expression = expression;
            }


            override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.visitGroupingExpr(this);
            }
            public Expr expression { get; }
        }

        static class Literal : Expr
        {
            Literal(Object value)
            {
                this.value = value;
            }


            override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.visitLiteralExpr(this);
            }
            public Object value { get; }
        }

        static class Unary : Expr
        {
            Unary(Token @operator, Expr right)
            {
                this.@operator = @operator;
                this.right = right;
            }


            override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.visitUnaryExpr(this);
            }
            public Token @operator { get; }
            public Expr right { get; }
        }


    }
}
