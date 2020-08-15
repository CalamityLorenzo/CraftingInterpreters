

using System;
using System.Collections.Generic;
using CsLoxInterpreter.Details;
using static CsLoxInterpreter.TokenType;
namespace CsLoxInterpreter
{
    internal class Scanner
    {
        private Dictionary<string, TokenType> KeyWords = KeyWordStore.ReservedWords;
        /// <summary>
        /// Tokens found in Source
        /// </summary>
        private List<Token> Tokens { get; } = new List<Token>();
        /// <summary>
        /// Corpus being examined
        /// </summary>
        private string Source { get; }
        // OffSet in current string
        private int Start = 0;
        // OffSet in current string (Re-aligned with Start and beginning of each loop)
        private int Current = 0;
        private int Line = 1;

        /// <summary>
        /// The whole body to parsed.
        /// </summary>
        /// <param name="source"></param>
        public Scanner(string source)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                throw new ArgumentException("message", nameof(source));
            }
            Source = source;
        }

        /// <summary>
        /// Scanner engine and clent entrypoint 
        /// </summary>
        /// <returns></returns>
        public List<Token> ScanTokens()
        {
            try
            {   // Recursive parser
                while (!isAtEnd())
                {
                    // Set iterators
                    this.Start = this.Current;
                    ScanToken();
                }
                // Add an EOF at the end for completeness
                this.Tokens.Add(new Token(EOF, "", null, Line));
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
                case '(': AddToken(LEFT_PAREN); break;
                case ')': AddToken(RIGHT_PAREN); break;
                case '{': AddToken(TokenType.LEFT_BRACE); break;
                case '}': AddToken(TokenType.RIGHT_BRACE); break;
                case ',': AddToken(TokenType.COMMA); break;
                case '.': AddToken(TokenType.DOT); break;
                case '-': AddToken(TokenType.MINUS); break;
                case '+': AddToken(TokenType.PLUS); break;
                case ';': AddToken(TokenType.SEMICOLON); break;
                case '*': AddToken(TokenType.STAR); break;
                case '!': AddToken(Match('=') ? BANG_EQUAL : BANG); break;
                case '=': AddToken(Match('=') ? EQUAL_EQUAL : EQUAL); break;
                case '<': AddToken(Match('=') ? LESS_EQUAL : LESS); break;
                case '>': AddToken(Match('=') ? GREATER_EQUAL : GREATER); break;
                case '/':
                    if (Match('/')){
                        // A comment goes until the end of a line.
                        while (Peek() != '\n' && !isAtEnd()) Advance();
                    } else if (Match('*')){
                        NaiveMultiLineComment();
                    }else{
                        AddToken(SLASH);
                    }
                    break;
                case ' ':
                case '\r':
                case '\t':
                    break; // ignoring whitespace and carry on.
                case '\n':
                    this.Line++; // update the line nunber, and carry on.
                    break;
                case '"': StringSequence(); break;
                    // Capturing all Numbers is simpler than 1 case per number.
                case char val when (Char.IsDigit(val)): Number(); break;
                    // Capturing all a-Z AND _
                case char val when IsAlpha(val): Identifier(); break;
                default:
                    CSLox.Error(this.Line, "Unexpected Character");
                    break;
            }
        }

        private void NaiveMultiLineComment(int nested =1)
        {
            // we start inside a multi-line comment.
            // but it could be nested
            
            while (!isAtEnd())
            {
                // Move through the body of the multi
                if (Peek() == '\n') Line++;
                if (Peek() == '*' && PeekNext() == '/'){
                    Advance();
                    break;
                }

                if (Peek() == '/' && PeekNext() == '*'){
                    Advance();Advance();
                    NaiveMultiLineComment(nested+1);
                }
                // Annoying this cannot be done until the end. 
                // We have not checked the current char
                Advance();
            }
            if (isAtEnd())
                CSLox.Error(this.Line, "Unterminated multi-line comment");
            else
            {
                // Here we are at  the end of the multi line comment
                // munch the last char.
                 Advance();
            }
        }

        private void Identifier()
        {
            while (IsAlphaNumeric(Peek())) Advance();
            // What did we just scan, and is it a reserved key word?
            string text = Source.Substring(this.Start, Current - Start);
            var token = KeyWords.TryGetValue(text, out var matchedToken) ? matchedToken : IDENTIFIER;
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
                // We want that '.'
                Advance();
                while (Char.IsDigit(Peek())) Advance();

            }

            AddToken(NUMBER, Double.Parse(Source.Substring(Start, Current - Start)));
        }
        /// <summary>
        /// A string has been started "
        /// this collects all the characters until the end.
        /// </summary>
        private void StringSequence()
        {
            // Move until we reach the end of a string;
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
            // Move past the final "
            Advance();
            // ignore the surrounding quotes, and create the token
            var value = Source.Substring(Start + 1, (Current - 1) - (Start + 1));
            AddToken(STRING, value);
        }

        /// <summary>
        /// returns next character but does not advance
        /// A Fundamental
        /// </summary>
        /// <returns></returns>
        private char Peek()
        {   //OBO Current-1 == Actual current token
            if (isAtEnd()) return '\0';
            return Source[Current];
        }
        /// <summary>
        /// Returns next +1 does not advance
        /// </summary>
        /// <returns></returns>
        private char PeekNext()
        {
            if (Current + 1 >= Source.Length) return '\0';
            return Source[Current + 1];
        }

        /// <summary>
        /// Returns the NEXT character
        /// And Increments the current.
        /// A Fundamental
        /// </summary>
        /// <returns></returns>
        private char Advance()
        {
            Current++;
            return this.Source[Current - 1];
        }

        /// <summary>
        /// Tests for the next character Advances if true.
        /// Combines Peek/Advance
        /// </summary>
        /// <param name="expected"></param>
        /// <returns></returns>
        private bool Match(char expected)
        {
            if (this.isAtEnd()) return false;
            // tests current (+1) OBO
            if (this.Source[Current] != expected) return false;
            this.Current++;
            return true;
        }

        /// <summary>
        /// Adds token to the scanning results
        /// </summary>
        /// <param name="tokenType"></param>
        private void AddToken(TokenType tokenType)
        {
            AddToken(tokenType, null);
        }
        /// <summary>
        /// Adds token to the scanning results, also any information
        /// </summary>
        /// <param name="tokenType"></param>
        private void AddToken(TokenType tokenType, object literal)
        {
            var text = Source.Substring(this.Start, Current - Start);
            Tokens.Add(new Token(tokenType, text, literal, Line));
        }

        /// <summary>
        /// Test to see if we have reaced the soure end.
        /// </summary>
        /// <returns></returns>
        private bool isAtEnd() => this.Current >= this.Source.Length;
    }
}
