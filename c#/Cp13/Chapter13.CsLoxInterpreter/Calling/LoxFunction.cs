using CsLoxInterpreter.Classes;
using CsLoxInterpreter.Errors;
using CsLoxInterpreter.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CsLoxInterpreter.Calling
{
    class LoxFunction : ILoxCallable
    {
        // A hack to stop init from re-initialising a class instance over and over agaain.
        // As it's managed by the interepter, you can't actually declare an init function and get away with it either.
        public bool IsInitialiser { get; private set; }

        private readonly Stmt.Function functionDelcaration;
        public CSLoxEnvironment Closure { get; }
        public LoxFunction(Stmt.Function functionDelcaration, CSLoxEnvironment closure, bool isInitialiser)
        {
            this.functionDelcaration = functionDelcaration;
            Closure = closure;
            IsInitialiser = isInitialiser;
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
            catch(ReturnValue returnValue) // ugh....
            {
                /// EArly returns in constructors eg ( init(){return;})
                /// Allow it and return 'this'. constructors are weir,d
                if (IsInitialiser) return Closure.GetAt(0, "this");
                return returnValue.Value;
            }
            // Hack to ensure init() ie Constructors only ever return the instance 'this';
            if (this.IsInitialiser) return Closure.GetAt(0, "this");

            return null;
        }

        public LoxFunction Bind(LoxInstance instance)
        {
            CSLoxEnvironment environment = new CSLoxEnvironment(this.Closure);
            environment.Define("this", instance);
            // redelcared this function with the correct constext AT RUNTIME.
            return new LoxFunction(functionDelcaration, environment, IsInitialiser);
        }

        public override string ToString() => $"<fn {functionDelcaration.Name.Lexeme} >";


    }
}
