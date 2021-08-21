using System;
using System.Collections.Generic;
namespace CsLoxInterpreter.Expressions
{
    public abstract class Stmt
    {
        internal abstract T Accept<T>(Visitor<T> visitor);
        internal interface Visitor<T>
        {
            T VisitExpressionStmt(Expression stmt);
            T VisitPrintStmt(Print stmt);
        }

        internal class Expression : Stmt
        {
            public Expr expression { get; }
            internal Expression(Expr expression)
            {
                this.expression = expression;
            }


            internal override T Accept<T>(Stmt.Visitor<T> visitor)
            {
                return visitor.VisitExpressionStmt(this);
            }
        }

        internal class Print : Stmt
        {
            internal Print(Expr expression)
            {
                this.expression = expression;
            }


            internal override T Accept<T>(Stmt.Visitor<T> visitor)
            {
                return visitor.VisitPrintStmt(this);
            }
            public Expr expression { get; }
        }


    }
}
