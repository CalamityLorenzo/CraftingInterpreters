using CsLoxInterpreter.Calling;
using System;
using System.Collections.Generic;

namespace CsLoxInterpreter.Classes
{
    /// <summary>
    /// Reuntime representation of a CLASS (note: not an instance)
    /// This is an instantiated factory for createing class instances.
    /// </summary>
    internal class LoxClass : ILoxCallable
    {
        public string Name { get; }
        public Dictionary<string, LoxFunction> Methods { get; }

        public LoxClass(string name, Dictionary<string, LoxFunction> methods)
        {
            this.Name = name;
            this.Methods = methods;
        }

        public int Arity()
        {
            LoxFunction initialiser = FindMethod("init");
            if (initialiser == null) return 0;
            return initialiser.Arity();
        }

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            LoxInstance instance = new LoxInstance(this);
            LoxFunction initaliser = FindMethod("init");
            if (initaliser != null)
            {
                initaliser.Bind(instance)                   // runtime binding
                          .Call(interpreter, arguments);    // Create instnace.
            }
            return instance;
        }

        internal LoxFunction FindMethod(string name)
        {
            if (Methods.ContainsKey(name)) return Methods[name];
            return null;
        }

        public override string ToString() => $"<cl> {Name}";
    }
}