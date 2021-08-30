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
            T VisitLogicalExpr(Logical expr);
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
                this.Name = name;
                this.Value = value;
            }

            internal override T Accept<T>(ILoxVisitor<T> visitor)
            {
                return visitor.VisitAssignExpr(this);
            }
            public Token Name { get; }
            public Expr Value { get; }
        }

        public class Logical : Expr
        {
            internal Logical(Expr left, Token @operator, Expr right)
            {
                this.Left = left;
                this.@operator = @operator;
                this.Right = right;
            }


            internal override T Accept<T>(ILoxVisitor<T> visitor)
            {
                return visitor.VisitLogicalExpr(this);
            }
            public Expr Left { get; }
            public Token @operator { get; }
            public Expr Right { get; }
        }

        public class Binary : Expr
        {
            public Binary(Expr left, Token @operator, Expr right)
            {
                this.Left = left;
                this.Operator = @operator;
                this.Right = right;
            }

            internal override T Accept<T>(ILoxVisitor<T> visitor)
            {
                return visitor.VisitBinaryExpr(this);
            }
            public Expr Left { get; }
            public Token Operator { get; }
            public Expr Right { get; }
        }

        public class Grouping : Expr
        {
            public Grouping(Expr expression)
            {
                this.Expression = expression;
            }

            internal override T Accept<T>(ILoxVisitor<T> visitor)
            {
                return visitor.VisitGroupingExpr(this);
            }
            public Expr Expression { get; }
        }

        public class Literal : Expr
        {
            public Literal(object? value)
            {
                this.Value = value;
            }


            internal override T Accept<T>(ILoxVisitor<T> visitor)
            {
                return visitor.VisitLiteralExpr(this);
            }
            public Object? Value { get; }
        }

        public class Unary : Expr
        {
            public Unary(Token @operator, Expr right)
            {
                this.Operator = @operator;
                this.Right = right;
            }

            internal override T Accept<T>(ILoxVisitor<T> visitor)
            {
                return visitor.VisitUnaryExpr(this);
            }
            public Token Operator { get; }
            public Expr Right { get; }
        }
        public class Variable : Expr
        {
            internal Variable(Token name)
            {
                this.Name = name;
            }


            internal override T Accept<T>(ILoxVisitor<T> visitor)
            {
                return visitor.VisitVariableExpr(this);
            }
            public Token Name { get; }
        }
    }
}
