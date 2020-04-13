using CsLoxInterpreter;
using System;
// 
internal abstract class Expr
{
    internal class Binary : Expr
    {
        Binary(Expr left, Token @operator, Expr right)
        {
            Left = left;
            Operator = @operator;
            Right = right;
        }
        public Expr Left { get; }
        public Token Operator { get; }
        public Expr Right { get; }

    }
}