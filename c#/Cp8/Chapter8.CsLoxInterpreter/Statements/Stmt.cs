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
            T VisitExpressionStmt(Expression stmt);
            T VisitPrintStmt(Print stmt);
            T VisitVarStmt(Var stmt);
        }

        internal class Expression : Stmt
        {
            internal Expression(Expr expression)
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
