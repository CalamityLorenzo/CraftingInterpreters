using System;

namespace Syntax.CsLoxInterpreter.Expressions
{
    public abstract class Expr
    {
        // T Allow us to create more easily create typed implementaions
        // eg string, eg TokenRepString etc
        internal abstract T Accept<T>(ILoxVisitor<T> visitor);

        internal interface ILoxVisitor<T>
        {
            T VisitBinaryExpr(Binary expr);
            T VisitGroupingExpr(Grouping expr);
            T VisitLiteralExpr(Literal expr);
            T VisitUnaryExpr(Unary expr);
            T VisitTernary(Ternary ternary);
        }

        public class Binary : Expr
        {
            public Binary(Expr left, Token @operator, Expr right)
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

        public class Ternary : Expr
        {
            public Expr cond { get; }
            public Expr @true { get; }
            public Expr @false { get; }

            public Ternary(Expr cond, Expr @true, Expr @false)
            {
                this.cond = cond;
                this.@true = @true;
                this.@false = @false;
            }

            internal override T Accept<T>(ILoxVisitor<T> visitor)
            {
                return visitor.VisitTernary(this);
            }
        }

        public class Grouping : Expr
        {
            public Grouping(Expr expression)
            {
                this.expression = expression;
            }

            internal override T Accept<T>(ILoxVisitor<T> visitor)
            {
                return visitor.VisitGroupingExpr(this);
            }
            public Expr expression { get; }
        }

        public class Literal : Expr
        {
            public Literal(Object value)
            {
                this.value = value;
            }


            internal override T Accept<T>(ILoxVisitor<T> visitor)
            {
                return visitor.VisitLiteralExpr(this);
            }
            public Object value { get; }
        }

        public class Unary : Expr
        {
            public Unary(Token @operator, Expr right)
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
