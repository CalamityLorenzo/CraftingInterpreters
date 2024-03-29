
using System.Collections.Generic;

namespace CsLoxInterpreter.Details
{
    internal static class KeyWordStore
    {
        internal static Dictionary<string, TokenType> ReservedWords => new Dictionary<string, TokenType>
        {
            {"and", TokenType.AND},
            {"break", TokenType.BREAK },
            {"class", TokenType.CLASS},
            {"else", TokenType.ELSE},
            {"false", TokenType.FALSE},
            {"for", TokenType.FOR},
            {"fun", TokenType.FUN},
            {"if", TokenType.IF},
            {"nil", TokenType.NIL},
            {"or", TokenType.OR},
            {"print", TokenType.PRINT},
            {"return", TokenType.RETURN},
            {"super", TokenType.SUPER},
            {"this", TokenType.THIS},
            {"true", TokenType.TRUE},
            {"var", TokenType.VAR},
            {"while", TokenType.WHILE},
        };
    }

}