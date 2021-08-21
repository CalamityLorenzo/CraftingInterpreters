using CsLoxInterpreter.Details;

namespace CsLoxInterpreter
{
    public class Scanner
    {
        private Dictionary<string, TokenType> KeyWords = KeyWordStore.ReservedWords;
        private List<Token> Tokens { get; } = new List<Token>();
        private string Source { get; }
        private int Start = 0;     // Start position of the current token.
        private int Current = 0;    // OffSet in current string
        private int Line = 1;

        public Scanner(string source)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                throw new ArgumentException("message", nameof(source));
            }
            Source = source;
        }

        public List<Token> ScanTokens()
        {
            try
            {   // Each loop of the scanner is to process 1 token or nested tokens.
                while (!isAtEnd())
                {
                    // Start will be the first char of the current token, in a multi-char token eg. Identifers, strings, numbers etc.
                    // With nested tokens this is not used and a seperate start indicator must be provided in the method.
                    this.Start = this.Current;
                    ScanToken();
                }
                this.Tokens.Add(new Token(TokenType.EOF, "", null, Line));
                return Tokens;
            }
            catch (Exception)
            {
                Console.WriteLine($"{Line} {Start} {Current}");
                throw;
            }
        }

        private void ScanToken()
        {
            char c = Advance();
            switch (c)
            {
                case '(': AddToken(TokenType.LEFT_PAREN); break;
                case ')': AddToken(TokenType.RIGHT_PAREN); break;
                case '{': AddToken(TokenType.LEFT_BRACE); break;
                case '}': AddToken(TokenType.RIGHT_BRACE); break;
                case ',': AddToken(TokenType.COMMA); break;
                case '.': AddToken(TokenType.DOT); break;
                case '-': AddToken(TokenType.MINUS); break;
                case '+': AddToken(TokenType.PLUS); break;
                case ';': AddToken(TokenType.SEMICOLON); break;
                case '*': AddToken(TokenType.STAR); break;
                case '?': AddToken(TokenType.QUESTION); break;
                case ':': AddToken(TokenType.SEMICOLON); break;
                case '!': AddToken(Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG); break;
                case '=': AddToken(Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL); break;
                case '<': AddToken(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS); break;
                case '>': AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER); break;
                case '/':
                    if (Match('/'))
                    {
                        // A comment goes until the end of a line.
                        while (Peek() != '\n' && !isAtEnd()) Advance();
                    }
                    else if (Match('*'))
                    {
                        MultiLineComment();
                    }
                    else
                    {
                        AddToken(TokenType.SLASH);
                    }
                    break;
                case ' ':
                case '\r':
                case '\t':
                    break; // ignoring whitespace and carry on.
                case '\n':
                    this.Line++; // update the line, and carry on.
                    break;
                case '"': String(); break;
                default:
                    if (Char.IsDigit(c))
                        Number();
                    else if (IsAlpha(c))
                    {
                        Identifier();
                    }
                    else
                    {
                        CSLox.Error(this.Line, $"Unexpected Character '{c}'");
                    }
                    break;
            }
        }

        private void MultiLineComment()
        {
            // we start inside a multi-line comment.
            // but it could be nested
            while ((Peek() == '*' && PeekNext() == '/') == false && !isAtEnd())
            {
                // Move through the body of the multi
                if (Peek() == '\n') Line++;
                
                // We are already in a comment so eating chars is safe
                // We start this method INSIDE the comment.
                if(Match('/') && Match('*'))
                    MultiLineComment();
                Advance();
            }
            
            if (isAtEnd())
                CSLox.Error(this.Line, "Unterminated multi-line comment");
            Advance();Advance();
        }
        private void Identifier()
        {
            while (IsAlphaNumeric(Peek())) Advance();
            // What did we just scan, and is it a reserved key word?
            string text = Source.Substring(this.Start, Current - Start);
            var token = KeyWords.TryGetValue(text, out var matchedToken) ? matchedToken : TokenType.IDENTIFIER;
            AddToken(token);
        }

        private bool IsAlphaNumeric(char c)
        {
            return IsAlpha(c) || Char.IsDigit(c);
        }

        private bool IsAlpha(char c)
        {   // Identifiers can start with an underscore.
            return Char.IsLetter(c) || c == '_';
        }

        private void Number()
        {
            // move along a bit
            while (Char.IsDigit(Peek())) Advance();

            // here we have listed out the number
            // or maybe just the first part
            if (Peek() == '.' && Char.IsDigit(PeekNext()))
            {
                // We want that .
                Advance();
                while (Char.IsDigit(Peek())) Advance();

            }

            AddToken(TokenType.NUMBER, Double.Parse(Source.Substring(Start, Current - Start)));
        }



        private void String()
        {
            while (Peek() != '"' && !isAtEnd())
            {
                if (Peek() == '\n') Line++;
                Advance();
            }

            if (isAtEnd())
            {
                CSLox.Error(Line, "Unterminated string");
                return;
            }
            Advance();
            // ignore the surrounding quotes!
            var value = Source.Substring(Start + 1, (Current - 1) - (Start + 1));
            AddToken(TokenType.STRING, value);
        }

        /// returns next but does not advance
        private char Peek()
        {
            if (isAtEnd()) return '\0';
            return Source[Current];
        }

        // Returns next +1 does not advance
        private char PeekNext()
        {
            if (Current + 1 >= Source.Length) return '\0';
            return Source[Current + 1];
        }

        // Returns the NEXT character
        // And Moves forward.
        private char Advance()
        {
            Current++;
            return this.Source[Current - 1];
        }

        // Tests the next character
        // Advances if true.
        private bool Match(char expected)
        {
            if (this.isAtEnd()) return false;
            // tests current (+1)
            if (this.Source[Current] != expected) return false;
            this.Current++;
            return true;
        }
        // Add token to list.
        private void AddToken(TokenType tokenType)
        {
            AddToken(tokenType, null);
        }

        private void AddToken(TokenType tokenType, object literal)
        {
            var text = Source.Substring(this.Start, Current - Start);
            Tokens.Add(new Token(tokenType, text, literal, Line));
        }


        private bool isAtEnd() => this.Current >= this.Source.Length;
    }
}
