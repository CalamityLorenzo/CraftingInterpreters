using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace CsLoxInterpreter.Exceptions
{
    public class ParserException :Exception
    {
        public Token Token { get; }

        public ParserException()
        {
        }

        protected ParserException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ParserException(Token token, string message): base(message)
        {
            this.Token = token;
        }
    }
}
