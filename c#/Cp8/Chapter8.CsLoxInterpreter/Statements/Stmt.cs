using CsLoxInterpreter;
using System;
using System.Collections.Generic;
namespace CsLoxInterpreter.Expressions
{
    internal abstract class Stmt
    {
        internal abstract T Accept<T>(Visitor<T> visitor);
        internal interface Visitor<T>
        {
            T VisitBlockStmt(Block stmt);
            T VisitExpressionStmt(ExpressionStmt stmt);
            T VisitPrintStmt(Print stmt);
            T VisitVarStmt(Var stmt);
        }

        internal class Block : Stmt
        {
            internal Block(List<Stmt> statments)
            {
                this.statments = statments;
            }

            internal override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitBlockStmt(this);
            }
            public List<Stmt> statments { get; }
        }
        internal class ExpressionStmt : Stmt
        {
            internal ExpressionStmt(Expr expression)
            {
                this.expression = expression;
            }


            internal override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitExpressionStmt(this);
            }
            public Expr expression { get; }
        }

        internal class Print : Stmt
        {
            internal Print(Expr expression)
            {
                this.expression = expression;
            }


            internal override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitPrintStmt(this);
            }
            public Expr expression { get; }
        }

        internal class Var : Stmt
        {
            internal Var(Token name, Expr initializer)
            {
                this.name = name;
                this.initializer = initializer;
            }


            internal override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitVarStmt(this);
            }
            public Token name { get; }
            public Expr initializer { get; }
        }


    }
}
