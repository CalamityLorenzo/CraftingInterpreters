using CsLoxInterpreter.Errors;
using CsLoxInterpreter.Expressions;

namespace CsLoxInterpreter.Calling
{
    class LoxFunction : ILoxCallable
    {
        private readonly Stmt.Function functionDeclaration;
        public CSLoxEnvironment Closure { get; }

        public LoxFunction(Stmt.Function functionDeclaration, CSLoxEnvironment closure)
        {
            Closure = closure;
            this.functionDeclaration = functionDeclaration;
        }


        public int Arity() => this.functionDeclaration.Params.Count();

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            // The environment when/where the function was declared.
            CSLoxEnvironment environment = new CSLoxEnvironment(this.Closure);
            for (int i = 0; i < functionDeclaration.Params.Count; ++i)
            {
                environment.Define(functionDeclaration.Params[i].Lexeme, arguments[i]);
            }
            try
            {
                interpreter.ExecuteBlock(functionDeclaration.Body, environment);
            }
            catch(ReturnValue returnValue)
            {
                return returnValue.Value;
            }

            return null;
        }

        public override string ToString() => $"<fn {functionDeclaration.Name.Lexeme} >";
    }
}
