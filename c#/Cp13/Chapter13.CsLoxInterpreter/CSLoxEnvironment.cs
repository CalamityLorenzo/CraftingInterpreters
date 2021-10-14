using CsLoxInterpreter;
using CsLoxInterpreter.Errors;
using System.Collections.Generic;
using System;

namespace CsLoxInterpreter
{
    class CSLoxEnvironment
    {
        private readonly Dictionary<string, object> _Values = new();
        internal readonly CSLoxEnvironment _Enclosing;

        public CSLoxEnvironment()
        {
            this._Enclosing = null;
        }
        public CSLoxEnvironment(CSLoxEnvironment enclosing) => this._Enclosing = enclosing;

        public void Define(string name, Object value)
        {
            if (_Values.ContainsKey(name))
            {
                _Values[name] = value;
                return;
            }// throw new RuntimeError($"{name}, has already been defined.");
            this._Values.Add(name, value);
        }

        public object Get(Token name)
        {
            if (_Values.ContainsKey(name.Lexeme))
                return _Values[name.Lexeme];

            if (_Enclosing != null) return _Enclosing.Get(name);

            throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'");
        }

        public object GetAt(int distance, string name)
        {
            return Ancestor(distance)._Values[name];
        }

        private CSLoxEnvironment Ancestor(int distance)
        {
            CSLoxEnvironment environment = this;
            for (int i = 0; i < distance; i++)
            {
                environment = environment._Enclosing;
            }

            return environment;
        }

        internal void Assign(Token name, object value)
        {
            if (_Values.ContainsKey(name.Lexeme))
            {
                _Values[name.Lexeme] = value;
                return;
            }

            if (_Enclosing != null)
            {
                _Enclosing.Assign(name, value);
                return;
            }


            throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'");
        }

        internal void AssignAt(int distance, Token name, object value)
        {
            Ancestor(distance)._Values[name.Lexeme] = value;
        }
    }
}
