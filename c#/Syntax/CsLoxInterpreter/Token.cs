namespace CsLoxInterpreter
{
    public class Token
    {
        public TokenType TokenType { get; }
        public string Lexeme { get; }
        public object Literal { get; private set; }
        public int Line { get; private set; }
        internal Token(TokenType type, string lexeme, object literal, int line)
        {
            if (lexeme==null) throw new System.ArgumentException("Cannot be", nameof(lexeme));
            (TokenType, Lexeme, Literal, Line) = (type, lexeme, literal, line);
        }
        public override string ToString() => $"{this.TokenType}  {Lexeme} {Literal}";
    }
}