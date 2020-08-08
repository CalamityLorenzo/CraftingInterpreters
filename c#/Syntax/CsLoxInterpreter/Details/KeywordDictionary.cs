
using System.Collections.Generic;
using static CsLoxInterpreter.TokenType;
namespace CsLoxInterpreter.Details
{
    internal static class KeyWordStore
    {
        internal static Dictionary<string, TokenType> ReservedWords => new Dictionary<string, TokenType>
        {
                {"and", AND},
                {"class", CLASS},
                {"else", ELSE},
                {"false", FALSE},
                {"for", FOR},
                {"fun", FUN},
                {"if", IF},
                {"nil", NIL},
                {"or", OR},
                {"print", PRINT},
                {"return", RETURN},
                {"super", SUPER},
                {"this", THIS},
                {"true", TRUE},
                {"var", VAR},
                {"while", WHILE},
        };
    }

}