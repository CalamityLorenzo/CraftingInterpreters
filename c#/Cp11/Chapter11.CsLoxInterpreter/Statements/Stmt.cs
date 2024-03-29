using CsLoxInterpreter;
using System;
using System.Collections.Generic;
namespace CsLoxInterpreter.Expressions
{
    internal abstract class Stmt
    {
        internal abstract T Accept<T>(IVisitor<T> visitor);
        internal interface IVisitor<T>
        {
            T VisitBlockStmt(Block stmt);
            T VisitBreakStmt(Break stmt);
            T VisitExpressionStmt(ExpressionStmt stmt);
            T VisitFunctionStmt(Function stmt);
            T VisitReturnStmt(Return stmt);
            T VisitIfStmt(If stmt);
            T VisitPrintStmt(Print stmt);
            T VisitWhileStmt(While stmt);
            T VisitVarStmt(Var stmt);
        }

        internal class Block : Stmt
        {
            internal Block(List<Stmt> statments)
            {
                this.Statments = statments;
            }

            internal override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitBlockStmt(this);
            }
            public List<Stmt> Statments { get; }
        }
        internal class Break : Stmt
        {
            internal Break()
            {
            }

            internal override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitBreakStmt(this);
            }
        }

        internal class ExpressionStmt : Stmt
        {
            internal ExpressionStmt(Expr expression)
            {
                this.Expression = expression;
            }


            internal override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitExpressionStmt(this);
            }
            public Expr Expression { get; }
        }


        internal class Function : Stmt
        {
            internal Function(Token Name, List<Token> @params, List<Stmt> body)
            {
                this.Name = Name;
                this.Params = @params;
                Body = body;
            }


            internal override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitFunctionStmt(this);
            }
            public Token Name { get; }
            public List<Token> Params { get; }
            public List<Stmt> Body { get; }
        }

        internal class If : Stmt
        {
            internal If(Expr condition, Stmt thenBranch, Stmt elseBranch)
            {
                this.Condition = condition;
                this.ThenBranch = thenBranch;
                this.ElseBranch = elseBranch;
            }


            internal override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitIfStmt(this);
            }
            public Expr Condition { get; }
            public Stmt ThenBranch { get; }
            public Stmt ElseBranch { get; }
        }


        internal class Print : Stmt
        {
            internal Print(Expr expression)
            {
                this.Expression = expression;
            }


            internal override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitPrintStmt(this);
            }
            public Expr Expression { get; }
        }

        internal class Return : Stmt
        {
            internal Return(Token keyword, Expr value)
            {
                this.Keyword = keyword;
                this.Value = value;
            }


            internal override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitReturnStmt(this);
            }
            public Token Keyword { get; }
            public Expr Value { get; }
        }

        internal class While : Stmt
        {
            internal While(Expr condition, Stmt body)
            {
                this.Condition = condition;
                this.Body = body;
            }


            internal override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitWhileStmt(this);
            }
            public Expr Condition { get; }
            public Stmt Body { get; }
        }


        internal class Var : Stmt
        {
            internal Var(Token name, Expr initializer)
            {
                this.Name = name;
                this.Initializer = initializer;
            }


            internal override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitVarStmt(this);
            }
            public Token Name { get; }
            public Expr Initializer { get; }
        }


    }
}
