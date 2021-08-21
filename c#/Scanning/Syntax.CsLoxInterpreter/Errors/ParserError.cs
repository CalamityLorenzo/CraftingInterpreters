using System;

namespace Syntax.CsLoxInterpreter.Errors
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
}
