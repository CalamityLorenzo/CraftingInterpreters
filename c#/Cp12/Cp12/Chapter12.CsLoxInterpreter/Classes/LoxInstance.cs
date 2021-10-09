using CsLoxInterpreter.Calling;
using CsLoxInterpreter.Errors;
using System;
using System.Collections.Generic;

namespace CsLoxInterpreter.Classes
{
    /// <summary>
    /// A Runtime instance of a class.
    /// </summary>
    internal class LoxInstance
    {
        private LoxClass loxClass;
        private Dictionary<string, object> fields = new Dictionary<string, object>();
        public LoxInstance(LoxClass loxClass)
        {
            this.loxClass = loxClass;
            
        }

        public object Get(Token name)
        {
            if(fields.ContainsKey(name.Lexeme))
                return fields[name.Lexeme];
            /// When we are returning a method
            /// Before it's actually called '()'
            /// Ensure this class instance environment as a closure around the method,
            /// Which means 'this' is correct no matter where the method is called eg callback.
            LoxFunction method = loxClass.FindMethod(name.Lexeme);
            if (method != null) return method.Bind(this);

            throw new RuntimeError(name, $"Undefined property '{name.Lexeme}'");
        }

        public override string ToString() => this.loxClass.Name + " instance";

        internal void Set(Token name, object value)
        {
            fields[name.Lexeme] = value;
        }
    }
}