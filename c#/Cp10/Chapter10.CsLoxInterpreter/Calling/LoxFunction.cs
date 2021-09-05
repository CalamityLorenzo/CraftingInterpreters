using CsLoxInterpreter.Errors;
using CsLoxInterpreter.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsLoxInterpreter.Calling
{
    class LoxFunction : ILoxCallable
    {
        private readonly Stmt.Function functionDelcaration;
        public CSLoxEnvironment Closure { get; }

        public LoxFunction(Stmt.Function functionDelcaration, CSLoxEnvironment closure)
        {
            this.functionDelcaration = functionDelcaration;
            Closure = closure;
        }


        public int Arity() => this.functionDelcaration.Params.Count();

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            // The environment when/where the function was declared.
            CSLoxEnvironment environment = new CSLoxEnvironment(this.Closure);

            for (int i = 0; i < functionDelcaration.Params.Count; ++i)
            {
                environment.Define(functionDelcaration.Params[i].Lexeme, arguments[i]);
            }
            try
            {
                interpreter.ExecuteBlock(functionDelcaration.Body, environment);
            }
            catch(ReturnValue returnValue)
            {
                return returnValue.Value;
            }

            return null;
        }

        public override string ToString() => $"<fn {functionDelcaration.Name.Lexeme} >";
    }
}
