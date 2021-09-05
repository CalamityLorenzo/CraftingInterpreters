﻿namespace CsLoxInterpreter.Calling
{
    interface ILoxCallable
    {
        Object Call(Interpreter interpreter, List<object> arguments);
        int Arity();
    }

    
}
