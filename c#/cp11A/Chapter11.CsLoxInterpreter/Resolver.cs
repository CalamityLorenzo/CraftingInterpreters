using CsLoxInterpreter;
using CsLoxInterpreter.Details;
using CsLoxInterpreter.Expressions;
using Unit = System.ValueTuple;
namespace Chapter11.CsLoxInterpreter
{
    internal class Resolver : Expr.ILoxVisitor<Unit>, Stmt.IVisitor<Unit>
    {
        private List<Dictionary<string, bool>> _Scopes = new List<Dictionary<string, bool>>();
        private Interpreter _Interpreter;

        public Resolver(Interpreter interpreter)
        {
            this._Interpreter = interpreter;
        }

        internal void Resolve(List<Stmt> statements)
        {
            foreach(Stmt statement in statements)
            {
                Resolve(statement);
            }
        }

        private void Resolve(Stmt stmt) => stmt.Accept(this);
        private void Resolve(Expr expr) => expr.Accept(this);

        private void ResolveLocal(Expr expr, Token name)
        {
            var xCount = _Scopes.Count;
            for (var i = xCount - 1; i >= 0; i--)
            {
                if (_Scopes.ElementAt(i).ContainsKey(name.Lexeme))
                {
                    this._Interpreter.Resolve(expr, xCount - 1 - i);
                    return;
                }
            }
        }

        private void ResolveFunction(Stmt.Function function)
        {
            BeginScope();
            foreach(var param in function.Params)
            {
                Declare(param);
                Define(param);
            }
            Resolve(function.Body);
            EndScope();
        }

        private void BeginScope()
        {
            this._Scopes.Add(new Dictionary<string, bool>());
        }
        private void EndScope()
        { 
            this._Scopes.RemoveAt(_Scopes.Count - 1);
        }


        private void Declare(Token name)
        {
            if (_Scopes.Count == 0) return;
            Dictionary<string, bool> scope = _Scopes[_Scopes.Count - 1];
            if (scope.ContainsKey(name.Lexeme))
                CSLox.Error(name, "Already a variabled with this name in this scope .");

            scope.Add(name.Lexeme, false);

        }

        private void Define(Token name)
        {
            if (_Scopes.Count == 0) return;
            _Scopes[_Scopes.Count - 1][name.Lexeme] = true;
        }

        #region ifaces
        public Unit VisitAssignExpr(Expr.Assign expr)
        {
            Resolve(expr.Value);
            ResolveLocal(expr, expr.Name);
            return new Unit();
        }

        public Unit VisitBinaryExpr(Expr.Binary expr)
        {
            Resolve(expr.Left);
            Resolve(expr.Right);

            return new Unit();
        }

        public Unit VisitBlockStmt(Stmt.Block stmt)
        {
            BeginScope();
            Resolve(stmt.Statments);
            EndScope();
            return new Unit();

        }

        public Unit VisitBreakStmt(Stmt.Break stmt)
        {
            return new Unit();
        }

        public Unit VisitCallExpr(Expr.Call expr)
        {
            foreach(Expr argument in expr.Arguments)
            {
                Resolve(argument);
            }
            return new Unit();
        }

        public Unit VisitExpressionStmt(Stmt.ExpressionStmt stmt)
        {
            Resolve(stmt.Expression);
            return new Unit();
        }

        public Unit VisitFunctionStmt(Stmt.Function stmt)
        {
            Declare(stmt.Name);
            Define(stmt.Name);
            ResolveFunction(stmt);
            return new Unit();
        }

        

        public Unit VisitGroupingExpr(Expr.Grouping expr)
        {
            Resolve(expr.Expression);
            return new Unit();
        }

        public Unit VisitIfStmt(Stmt.If stmt)
        {
            Resolve(stmt.Condition);
            Resolve(stmt.ThenBranch);
            if (stmt.ElseBranch != null) Resolve(stmt.ElseBranch);
            return new Unit();
        }

        public Unit VisitLiteralExpr(Expr.Literal expr)
        {
            return new Unit();
        }

        public Unit VisitLogicalExpr(Expr.Logical expr)
        {
            Resolve(expr.Left);
            Resolve(expr.Right);
            return new Unit();
        }

        public Unit VisitPrintStmt(Stmt.Print stmt)
        {
            Resolve(stmt.Expression);
            return new Unit();
        }

        public Unit VisitReturnStmt(Stmt.Return stmt)
        {
            if (stmt.Value != null)
            {
                Resolve(stmt.Value);
            }
            return new Unit();
        }

        public Unit VisitUnaryExpr(Expr.Unary expr)
        {
            Resolve(expr.Right);
            return new Unit();
        }

        public Unit VisitVariableExpr(Expr.Variable expr)
        {
            if (_Scopes.Count != 0 && _Scopes[_Scopes.Count - 1].ContainsKey(expr.Name.Lexeme) && 
                    _Scopes[_Scopes.Count - 1][expr.Name.Lexeme] == false)
            {
                CSLox.Error(expr.Name, "Can't read local variable in it own initalizer.");
            }
            ResolveLocal(expr, expr.Name);
            return new Unit();
        }

        

        public Unit VisitVarStmt(Stmt.Var stmt)
        {
            Declare(stmt.Name);
            if (stmt.Initializer != null)
            {
                Resolve(stmt.Initializer);
            }
            Define(stmt.Name);
            return new Unit();
        }


        public Unit VisitWhileStmt(Stmt.While stmt)
        {
            Resolve(stmt.Condition);
            Resolve(stmt.Body);
            return new Unit();
        }
        #endregion
    }
}
