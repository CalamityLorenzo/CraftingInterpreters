using CsLoxInterpreter.Calling;
using CsLoxInterpreter.Calling.NativeFunctions;
using CsLoxInterpreter.Details;
using CsLoxInterpreter.Errors;
using CsLoxInterpreter.Expressions;
using static CsLoxInterpreter.TokenType;
using Unit = System.ValueTuple;

namespace CsLoxInterpreter
{
    class Interpreter : Stmt.IVisitor<System.ValueTuple>,
                        Expr.ILoxVisitor<object>
    {
        private CSLoxEnvironment _Globals = new();
        private CSLoxEnvironment _Environment;
        private Dictionary<Expr, int> _Locals = new Dictionary<Expr, int>();
        public CSLoxEnvironment Globals => _Globals;


        public Interpreter()
        {
            _Globals.Define("clock", new ClockLoxCallable());
            _Environment = _Globals;
        }

        #region Expr.ILoxVisitor
        public object VisitBinaryExpr(Expr.Binary expr)
        {
            Object left = Evaluate(expr.Left);
            Object right = Evaluate(expr.Right);

            switch (expr.Operator.Type)
            {
                case MINUS:
                    CheckNumberOperands(expr.Operator, left, right);
                    return BasicBinary(left, right, (l, r) => l - r);
                case SLASH:
                    CheckNumberOperands(expr.Operator, left, right);
                    return BasicBinary(left, right, (l, r) => l / r);
                case STAR:
                    CheckNumberOperands(expr.Operator, left, right);
                    return BasicBinary(left, right, (l, r) => l * r);
                case PLUS:
                    if (left.GetType() == typeof(string) && right.GetType() == typeof(string))
                        return $"{left}{right}";
                    if (left.GetType() == typeof(double) && right.GetType() == typeof(double))
                        return (double)left + (double)right;
                    throw new RuntimeError(expr.Operator, "Operands should be string or numbers");
                case GREATER:
                    CheckNumberOperands(expr.Operator, left, right);
                    return BasicBinary(left, right, (l, r) => l > r);
                case GREATER_EQUAL:
                    CheckNumberOperands(expr.Operator, left, right);
                    return BasicBinary(left, right, (l, r) => l >= r);
                case LESS:
                    CheckNumberOperands(expr.Operator, left, right);
                    return BasicBinary(left, right, (l, r) => l < r);
                case LESS_EQUAL:
                    CheckNumberOperands(expr.Operator, left, right);
                    return BasicBinary(left, right, (l, r) => l <= r);
                case BANG_EQUAL: return !IsEqual(left, right);
                case EQUAL_EQUAL: return IsEqual(left, right);

            }

            throw new RuntimeError(expr.Operator, "No matching operation found for operator.");
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
            return Evaluate(expr.Expression);
        }


        public object VisitLiteralExpr(Expr.Literal expr)
        {
            return expr.Value;
        }

        public object VisitUnaryExpr(Expr.Unary expr)
        {
            // This needs to be evaludated first, BEFORE we can apply the unary operator.
            // Post-Order traversal.
            var right = Evaluate(expr.Right);
            switch (expr.Operator.Type)
            {
                case MINUS:
                    CheckNumberOperands(expr.Operator, right);
                    return -(double)right;
                case BANG:
                    return !isTruthy(right);
            }

            return null;
        }

        public object VisitLogicalExpr(Expr.Logical expr)
        {
            object left = Evaluate(expr.Left);

            if (expr.@operator.Type == TokenType.OR)
            {
                if (isTruthy(left)) return left;
            }
            else
            {
                if (!isTruthy(left)) return left;
            }
            return Evaluate(expr.Right);
        }

        internal void Resolve(Expr expr, int depth)
        {
            this._Locals.Add(expr, depth);
        }

        public object VisitAssignExpr(Expr.Assign expr)
        {
            object value = Evaluate(expr.Value);
            if (_Locals.ContainsKey(expr))
            {
                _Environment.AssignAt(_Locals[expr], expr.Name, value);
            } else
                _Globals.Assign(expr.Name, value);
            return value;

        }


        #endregion

        #region Stmt.Visitor
        public Unit VisitExpressionStmt(Stmt.ExpressionStmt stmt)
        {
            Evaluate(stmt.Expression);
            return new Unit();
        }

        public Unit VisitPrintStmt(Stmt.Print stmt)
        {
            var obj = Evaluate(stmt.Expression);
            Console.WriteLine(Stringify(obj));
            return new ValueTuple();
        }
        public Unit VisitVarStmt(Stmt.Var stmt)
        {
            object value = null;
            if (stmt.Initializer != null)
                value = Evaluate(stmt.Initializer);

            _Environment.Define(stmt.Name.Lexeme, value);
            return new ValueTuple();
        }

        public object VisitVariableExpr(Expr.Variable expr)
        {
            return LookupVariable(expr.Name, expr);
            //return _Environment.Get(expr.Name);
        }

        public Unit VisitWhileStmt(Stmt.While stmt)
        {
            try
            {
                while (isTruthy(Evaluate(stmt.Condition)))
                {
                    Execute(stmt.Body);
                }
            }
            catch (BreakException ex)
            {
                // Do nothing.
            }

            return new Unit();
        }

        public Unit VisitBreakStmt(Stmt.Break stmt)
        {
            // Weird...use the parent language expcetion handler.....
            throw new BreakException();
        }

        public Unit VisitBlockStmt(Stmt.Block stmt)
        {
            ExecuteBlock(stmt.Statments, new CSLoxEnvironment(_Environment));
            return new Unit();
        }

        public Unit VisitIfStmt(Stmt.If stmt)
        {
            if (isTruthy(Evaluate(stmt.Condition)))
            {
                Execute(stmt.ThenBranch);
            }
            else if (stmt.ElseBranch != null)
            {
                Execute(stmt.ElseBranch);
            }
            return new Unit();
        }
        public object VisitCallExpr(Expr.Call expr)
        {
            object callee = Evaluate(expr.Callee);
            List<object> arguments = new();
            foreach (Expr argument in expr.Arguments)
            {
                arguments.Add(Evaluate(argument));
            }

            if (!(callee is ILoxCallable))
                throw new RuntimeError(expr.Paren, "Can only call functions and classes.");

            ILoxCallable function = (ILoxCallable)callee;
            if (arguments.Count != function.Arity())
            {
                throw new RuntimeError(expr.Paren, $"Expected {function.Arity()} arguments but got {arguments.Count}.");
            }

            return function.Call(this, arguments);
        }

        public Unit VisitFunctionStmt(Stmt.Function stmt)
        {
            LoxFunction function = new(stmt, this._Environment);
            this._Environment.Define(stmt.Name.Lexeme, function);
            return new Unit();
        }

        public Unit VisitReturnStmt(Stmt.Return stmt)
        {
            object value = null;
            if (stmt.Value != null)
                value = Evaluate(stmt.Value);
            throw new ReturnValue(value);
        }

        #endregion

        public void Interpret(List<Stmt> statements)
        {
            try
            {
                foreach (Stmt stmt in statements)
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


        private object LookupVariable(Token name, Expr expr)
        {
            if (this._Locals.ContainsKey(expr))
                return this._Environment.GetAt(this._Locals[expr], name.Lexeme);
            else
                return this._Globals.Get(name);
        }

        private void Execute(Stmt stmt)
        {
            stmt.Accept(this);
        }

        internal void ExecuteBlock(List<Stmt> statements, CSLoxEnvironment environment)
        {
            var previous = this._Environment;

            try
            {
                this._Environment = environment;
                foreach (Stmt statement in statements)
                {
                    Execute(statement);
                }
            }
            finally
            {
                this._Environment = previous;
            }
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
                        text = text[0..^2];
                    }
                    return text;
                }

                return obj?.ToString();
            }
        }

        private bool isTruthy(object right)
        {
            if (right == null) return false;
            if (right.GetType() == typeof(bool)) return (bool)right;
            return true;
        }


    }

}
