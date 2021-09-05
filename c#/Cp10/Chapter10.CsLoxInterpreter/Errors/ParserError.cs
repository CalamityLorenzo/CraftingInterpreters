using System;
using System.Runtime.Serialization;

namespace CsLoxInterpreter.Errors
{
    class ParserError : Exception
    {
        public ParserError()
        {
        }

        public ParserError(string? message) : base(message)
        {
        }

        public ParserError(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }

    class RuntimeError : Exception
    {
        public Token? Token;


        public RuntimeError()
        {
        }

        public RuntimeError(string? message) : base(message)
        {
        }

        public RuntimeError(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        public RuntimeError(Token @operator, string message) : base(message)
        {
            this.Token = @operator;
        }

        protected RuntimeError(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
    class BreakException : Exception { }
}
