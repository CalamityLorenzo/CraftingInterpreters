using CsLoxInterpreter;

namespace CsLoxInterpreter.Expressions
{
    public abstract class Expr
    {
        // T Allow us to create more easily create typed implementaions
        // eg string, eg TokenRepString etc
        internal abstract T Accept<T>(ILoxVisitor<T> visitor);

        internal interface ILoxVisitor<T>
        {
            T VisitAssignExpr(Assign expr);
            T VisitBinaryExpr(Binary expr);
            T VisitGroupingExpr(Grouping expr);
            T VisitLiteralExpr(Literal expr);
            T VisitUnaryExpr(Unary expr);
            T VisitVariableExpr(Variable expr);

        }

        public class Assign : Expr
        {
            // l-side is the identifer
            // r side is an expression, that will be assigned to our object on evaluation.
            // it's not evalulated until it's required.
            internal Assign(Token name, Expr value)
            {
                this.name = name;
                this.value = value;
            }

            internal override T Accept<T>(ILoxVisitor<T> visitor)
            {
                return visitor.VisitAssignExpr(this);
            }
            public Token name { get; }
            public Expr value { get; }
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
            public Literal(object? value)
            {
                this.value = value;
            }


            internal override T Accept<T>(ILoxVisitor<T> visitor)
            {
                return visitor.VisitLiteralExpr(this);
            }
            public Object? value { get; }
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
        public class Variable : Expr
        {
            internal Variable(Token name)
            {
                this.name = name;
            }


            internal override T Accept<T>(ILoxVisitor<T> visitor)
            {
                return visitor.VisitVariableExpr(this);
            }
            public Token name { get; }
        }
    }
}
