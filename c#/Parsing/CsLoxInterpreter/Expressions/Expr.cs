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
            T VisitGroupingExpr(Grouping expr);
            T VisitLiteralExpr(Literal expr);
            T VisitUnaryExpr(Unary expr);
        }

        internal abstract T Accept<T>(ILoxVisitor<T> visitor);

        internal class Binary : Expr
        {
            internal Binary(Expr left, Token @operator, Expr right)
            {
                this.left = left;
                this.@operator = @operator;
                this.right = right;
            }

            internal override T Accept<T>(ILoxVisitor<T> visitor){
                return visitor.VisitBinaryExpr(this);
            }
            public Expr left { get; }
            public Token @operator { get; }
            public Expr right { get; }
        }

        internal class Grouping : Expr
        {
            internal Grouping(Expr expression)
            {
                this.expression = expression;
            }

            internal override T Accept<T>(ILoxVisitor<T> visitor){
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
