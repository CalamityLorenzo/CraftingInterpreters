using CsLoxInterpreter;
using CsLoxInterpreter.Expressions;
using Unit = System.ValueTuple;
using System.Collections.Generic;
using CsLoxInterpreter.Details;

namespace Chapter11.CsLoxInterpreter.Statements
{
    internal enum FunctionType
    {
        NONE,
        FUNCTION
    }
    class Resolver : Expr.ILoxVisitor<Unit>, Stmt.IVisitor<Unit>
    {
        private readonly Interpreter interpreter;

        private readonly Stack<Dictionary<string, bool>> Scopes;
        private FunctionType CurrentFunction = FunctionType.NONE;
        public Resolver(Interpreter interpreter)
        {
            this.interpreter = interpreter;
            this.Scopes = new Stack<Dictionary<string, bool>>();
        }
        #region interfacae waffle
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
            this.BeginScope();
            this.Resolve(stmt.Statments);
            this.EndScope();

            return new Unit();
        }


        public Unit VisitBreakStmt(Stmt.Break stmt)
        {
            return new Unit();
        }

        public Unit VisitCallExpr(Expr.Call expr)
        {
            foreach (Expr argument in expr.Arguments)
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
            ResolveFunction(stmt, FunctionType.FUNCTION);

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
            if (CurrentFunction == FunctionType.NONE)
                CSLox.Error(stmt.Keyword, "Can't return from top-level code");
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
            if (Scopes.Count != 0) {
                var scope = Scopes.Peek();
                if (scope.ContainsKey(expr.Name.Lexeme) && scope[expr.Name.Lexeme] == false)
                {
                    CSLox.Error(expr.Name, "Can't read local variable in it's own initalizer");
                }
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

        #region CoolBit
        public void Resolve(List<Stmt> statements)
        {
            foreach (Stmt statement in statements)
                Resolve(statement);
        }

        private void Resolve(Stmt statement)
        {
            statement.Accept(this);
        }

        private void Resolve(Expr statement)
        {
            statement.Accept(this);
        }

        private void BeginScope()
        {
            this.Scopes.Push(new Dictionary<string, bool>());
        }

        private void EndScope()
        {
            this.Scopes.Pop();
        }
        private void Declare(Token name)
        {
            if (Scopes.Count == 0) return;
            var scope = Scopes.Peek();

            if (scope.ContainsKey(name.Lexeme))
            {
                CSLox.Error(name, "Already a variable with this name in this scope.");
            }
            else
                scope.Add(name.Lexeme, false);
        }

        private void Define(Token name)
        {
            if (Scopes.Count == 0) return;
            Scopes.Peek()[name.Lexeme] = true;

        }

        private void ResolveLocal(Expr expr, Token name)
        {
            var allScopes = this.Scopes.ToArray();
            for (int i = Scopes.Count - 1; i >= 0; i--)
            {
                if (allScopes[i].ContainsKey(name.Lexeme))
                {
                    var val = Scopes.Count() - 1 - i;
                    this.interpreter.Resolve(expr, val);
                    return;
                }
            }
        }

        private void ResolveFunction(Stmt.Function function, FunctionType type)
        {
            FunctionType enclosingFunction = CurrentFunction;
            CurrentFunction = type;
            BeginScope();
            foreach (Token param in function.Params)
            {
                Declare(param);
                Define(param);
            }
            Resolve(function.Body);
            EndScope();
            CurrentFunction = enclosingFunction;
        }

        #endregion
    }
}
