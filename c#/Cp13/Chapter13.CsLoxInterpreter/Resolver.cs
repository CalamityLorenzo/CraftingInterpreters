 
using CsLoxInterpreter.Details;
using CsLoxInterpreter.Expressions;
using CsLoxInterpreter.Utilities;
using System.Collections.Generic;
using Unit = System.ValueTuple;

namespace CsLoxInterpreter
{

    // Compile time resolution of certain tasks.
    // resolution of variables assignment to ensure the compile time state maintained.
    // eg static scopes and binding to ensure closures do not change  an expected value.
    // Ensuring returns do not appear in top-level code.
    internal class Resolver : Expr.ILoxVisitor<Unit>, Stmt.IVisitor<Unit>
    {
        internal enum FunctionType
        {
            NONE,
            FUNCTION,
            METHOD,
            INITIALISER
        }

        internal enum ClassType
        {
            NONE,
            CLASS,
            SUBCLASS
        }
        private ClassType CurrentClass = ClassType.NONE;

        private readonly Interpreter interpreter;

        // the list of variables (by Lexeme) that appear in thecode, and if they are properly initalised
        // We do not manage the global scope. That's handeled entirely in the interpreter environment.
        private readonly List<Dictionary<string, bool>> Scopes;
        // In this scopes, are wecurrently in a function.
        private FunctionType CurrentFunction = FunctionType.NONE;
        public Resolver(Interpreter interpreter)
        {
            this.interpreter = interpreter;
            this.Scopes = new List<Dictionary<string, bool>>();
        }
        #region basicInterfaces
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
            // Functions, and block statements where scopes are resolved.
            this.BeginScope();
            this.Resolve(stmt.Statments);
            this.EndScope();
            return new Unit();
        }


        public Unit VisitBreakStmt(Stmt.Break stmt)
        {
            return new Unit();
        }

        public Unit VisitClassStmt(Stmt.Class stmt)
        {
            ClassType enclosingClass = this.CurrentClass;
            CurrentClass = ClassType.CLASS;
            Declare(stmt.Name);
            Define(stmt.Name);

            if(stmt.SuperClass!=null && stmt.Name.Lexeme == stmt.SuperClass.Name.Lexeme)
            {
                CSLox.Error(stmt.SuperClass.Name, "A class can't inherit from itself.");
            }
            /// Resolver doesn't resolve Global variables and that environment. (Where most inheritence will occur.)
            /// Lox allows delcaraions inside of existing blocks, and as they are variable must be resolved.
            if (stmt.SuperClass != null)
            {
                CurrentClass = ClassType.SUBCLASS;
                Resolve(stmt.SuperClass);
            }
            if (stmt.SuperClass != null)
            {
                BeginScope();
                Scopes.Peek().Add("super", true);
            }
            BeginScope();
            Scopes.Peek().Add("this", true);

            foreach (Stmt.Function method in stmt.Methods)
            {
                FunctionType declaration = FunctionType.METHOD;

                if (method.Name.Lexeme.Equals("init"))
                {
                    declaration = FunctionType.INITIALISER;
                }
                ResolveFunction(method, declaration);
            }

            EndScope();

            if (stmt.SuperClass != null)
                EndScope();
            this.CurrentClass = enclosingClass;
            return new Unit();
        }


        // FIX THIS !!!
        // WAIT LURN THIS!
        public Unit VisitCallExpr(Expr.Call expr)
        {
            Resolve(expr.Callee);

            foreach (Expr argument in expr.Arguments)
            {
                Resolve(argument);
            }
            return new Unit();
        }

        public Unit VisitGetExpr(Expr.Get expr)
        {
            Resolve(expr.Object);
            return new Unit();
        }

        public Unit VisitSetExpr(Expr.Set expr)
        {
            Resolve(expr.Value);
            Resolve(expr.Object);
            return new Unit();
        }

        public Unit VisitSuperExpr(Expr.Super expr)
        {
            if(CurrentClass==ClassType.NONE)
            {
                CSLox.Error(expr.Keyword, "Can't use 'super' outside of a class.");
            }else if(CurrentClass!= ClassType.SUBCLASS)
            {
                CSLox.Error(expr.Keyword, "Can't use 'super' in a class with no superclass");
            }

            ResolveLocal(expr, expr.Keyword);
            return new Unit();
        }
        public Unit VisitExpressionStmt(Stmt.ExpressionStmt stmt)
        {
            Resolve(stmt.Expression);
            return new Unit();
        }

        public Unit VisitFunctionStmt(Stmt.Function stmt)
        {
            // Function names are variable types, to be decalared and defined in the same step.
            // THis allows recursion to handle self name resoluion.
            // nb: the funciton name and, arguments are all in the same scope.
            // the body not.
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
                // Small hack to stop 'erberts return from a constructor.
                // Terminates current instancec
                if (CurrentFunction == FunctionType.INITIALISER)
                    CSLox.Error(stmt.Keyword, "Can't return a value from an initialiser.");

                Resolve(stmt.Value);
            }
            return new Unit();
        }

        public Unit VisitThisExpr(Expr.This expr)
        {
            if (this.CurrentClass == ClassType.NONE)
            {
                CSLox.Error(expr.Keyword,
                    "Can't use 'this' outside of a class.");
                return new Unit();
            }
            ResolveLocal(expr, expr.Keyword);
            return new Unit();
        }
        public Unit VisitUnaryExpr(Expr.Unary expr)
        {
            Resolve(expr.Right);
            return new Unit();
        }

        public Unit VisitVariableExpr(Expr.Variable expr)
        {
            if (Scopes.Count != 0)
            {
                var scope = Scopes.Peek();
                if (scope.ContainsKey(expr.Name.Lexeme) && scope[expr.Name.Lexeme] == false)
                {
                    CSLox.Error(expr.Name, "Can't read local variable in it's own initalizer");
                }
            }
            // Where a variable is referenced
            // we send that info into to the interpreter
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

        #region Resolver
        /// <summary>
        /// Variations on resolving tactics
        /// depending on Expr/Statment/list
        /// </summary>
        /// <param name="statements"></param>
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

        private void ResolveLocal(Expr expr, Token name)
        {
            for (int i = Scopes.Count - 1; i >= 0; i--)
            {
                if (Scopes[i].ContainsKey(name.Lexeme))
                {
                    var depth = Scopes.Count - 1 - i;
                    this.interpreter.Resolve(expr, depth); // the expression to be evaluated at runtime.
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

        private void BeginScope()
        {
            this.Scopes.Add(new Dictionary<string, bool>());
        }

        private void EndScope()
        {
            this.Scopes.RemoveAt(this.Scopes.Count - 1);
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
            // Assign true to the variable so it can be used in the scope.
            if (Scopes.Count == 0) return;
            Scopes.Peek()[name.Lexeme] = true;

        }

        #endregion
    }
}
