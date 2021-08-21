namespace Syntax.CsLoxInterpreter
{
    public class Token
    {
        public TokenType Type { get; }
        public string Lexeme { get; }
        public object Literal { get; private set; }
        public int Line { get; private set; }
        public  Token(TokenType type, string lexeme, object literal, int line)
        {
            if (lexeme==null) throw new System.ArgumentException("Cannot be null", nameof(lexeme));
            (Type, Lexeme, Literal, Line) = (type, lexeme, literal, line);
        }
        public override string ToString() => $"{this.Type}  {Lexeme} {Literal}";
    }
}