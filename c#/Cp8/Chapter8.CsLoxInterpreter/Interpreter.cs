using CsLoxInterpreter.Expressions;
using CsLoxInterpreter;
using CsLoxInterpreter.Details;
using CsLoxInterpreter.Errors;
using static CsLoxInterpreter.TokenType;
namespace CsLoxInterpreter
{
    class Interpreter : Stmt.Visitor<System.ValueTuple>,
                        Expr.ILoxVisitor<object>
    {
#region Expr.ILoxVisitor
        public object VisitBinaryExpr(Expr.Binary expr)
        {
            Object left = Evaluate(expr.left);
            Object right = Evaluate(expr.right);

            switch (expr.@operator.Type)
            {
                case MINUS:
                    CheckNumberOperands(expr.@operator, left, right);
                    return BasicBinary(left, right, (l, r) => l - r);
                case SLASH:
                    CheckNumberOperands(expr.@operator, left, right);
                    return BasicBinary(left, right, (l, r) => l / r);
                case STAR:
                    CheckNumberOperands(expr.@operator, left, right);
                    return BasicBinary(left, right, (l, r) => l * r);
                case PLUS:
                    if (left.GetType() == typeof(string) && right.GetType() == typeof(string))
                        return $"{left}{right}";
                    if (left.GetType() == typeof(double) && right.GetType() == typeof(double))
                        return (double)left + (double)right;
                    throw new RuntimeError(expr.@operator, "Operands should be string or numbers");
                    break;
                case GREATER:
                    CheckNumberOperands(expr.@operator, left, right);
                    return BasicBinary(left, right, (l, r) => l > r);
                case GREATER_EQUAL:
                    CheckNumberOperands(expr.@operator, left, right);
                    return BasicBinary(left, right, (l, r) => l >= r);
                case LESS:
                    CheckNumberOperands(expr.@operator, left, right);
                    return BasicBinary(left, right, (l, r) => l > r);
                case LESS_EQUAL:
                    CheckNumberOperands(expr.@operator, left, right);
                    return BasicBinary(left, right, (l, r) => l >= r);
                case BANG_EQUAL: return !IsEqual(left, right);
                case EQUAL_EQUAL: return IsEqual(left, right);

            }

            throw new RuntimeError(expr.@operator, "No matching operation found for operator.");
        }

        private void CheckNumberOperands(Token @operator, params object[] operands)
        {
            var isDouble = true;
            for (var x = 0; x < operands.Length; ++x)
            {
                var operand = operands[x];
                if (operand.GetType() != typeof(double)) { isDouble = false; break; }
            }

            if (!isDouble) throw new RuntimeError(@operator, "Operand must be a number");
        }

        private bool IsEqual(object l, object r)
        {
            if (l is null && r is null) return true;
            if (l is null) return false;

            return l.Equals(r);
        }

        private T BasicBinary<T>(object left, object right, Func<double, double, T> op)
        {
            return op((double)left, (double)right);
        }

        public object VisitGroupingExpr(Expr.Grouping expr)
        {
            return Evaluate(expr.expression);
        }

      
        public object VisitLiteralExpr(Expr.Literal expr)
        {
            return expr.value;
        }

        public object VisitUnaryExpr(Expr.Unary expr)
        {
            // This needs to be evaludated first, BEFORE we can apply the unary operator.
            // Post-Order traversal.
            var right = Evaluate(expr.right);
            switch (expr.@operator.Type)
            {
                case MINUS:
                    CheckNumberOperands(expr.@operator, right);
                    return -(double)right;
                case BANG:
                    return !isTruthy(right);
            }

            return null;
        } 

        private bool isTruthy(object right)
        {
            if (right == null) return false;
            if (right.GetType() == typeof(bool)) return (bool)right;
            return true;
        }

     

        #endregion

        #region Stmt.Visitor
        public ValueTuple VisitExpressionStmt(Stmt.Expression stmt)
        {
            Evaluate(stmt.expression);
            return new ValueTuple();
        }

        public ValueTuple VisitPrintStmt(Stmt.Print stmt)
        {
            var obj = Evaluate(stmt.expression);
            Console.WriteLine(Stringify(obj));
            return new ValueTuple();
        }
        #endregion

        public void Interpret(List<Stmt> statements)
        {
            try
            {
                foreach(Stmt stmt in statements)
                {
                    Execute(stmt);
                }
                
            }
            catch (RuntimeError ex)
            {
                CSLox.RuntimeError(ex);
            }
        }

        // We are just passing the expression back to the top of the tree walk
        private object Evaluate(Expr expr) => expr.Accept(this);


        private void Execute(Stmt stmt)
        {
            stmt.Accept(this);
        }

        private string Stringify(object obj)
        {
            if (obj is null) return "nil";
            else
            {

                if (obj.GetType() == typeof(double))
                {
                    var text = obj.ToString();
                    if (text.EndsWith(".0"))
                    {
                        text = text.Substring(0, text.Length - 2);
                    }
                    return text;
                }

                return obj?.ToString();
            }
        }

        public ValueTuple VisitVarStmt(Stmt.Var stmt)
        {
            throw new NotImplementedException();
        }
    }


}
