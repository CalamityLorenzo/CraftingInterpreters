
using Chapter7.CsLoxInterpreter;
using Syntax.CsLoxInterpreter.Errors;
using Syntax.CsLoxInterpreter.Utilities;

namespace Syntax.CsLoxInterpreter.Details
{
    public static class CSLox
    {

        private static readonly Interpreter interpreter = new Interpreter();
        static bool hadError = false;
        private static bool hadRuntimeError = false;

        public static void Main(string[] args)
        {
            var thisAssembly = System.AppDomain.CurrentDomain.FriendlyName;

            if (args.Length > 1)
            {
                Console.WriteLine($"Usage: {thisAssembly} \"filename\" \nor: {thisAssembly} for REPL");
            }
            else if (args.Length == 1)
                RunFile(args[0]);
            else
            {
                RunPrompt();
            }

        }

        private static void RunFile(string filePath)
        {
            var rawFile = File.ReadAllText(filePath);
            Run(rawFile);
        }

        private static void RunPrompt()
        {
            for (; ; )
            {
                Console.Write("> ");
                Run(Console.ReadLine());
                hadError = false;
            }

        }

        private static void Run(string Source)
        {
            if (!String.IsNullOrEmpty(Source))
            {
                var scanner = new Scanner(Source);
                List<Token> Tokens = scanner.ScanTokens();
                Tokens.ForEach(token => Console.WriteLine(token));
                // Tokens into meaningful expressions.
                Parser parser = new Parser(Tokens);
                var expr = parser.Parse();
                interpreter.Interpret(expr);
                if (hadError)
                {
                    Console.WriteLine("Csharp -Lox has died badly.");
                    return;
                }

                if (hadRuntimeError)
                {
                    Console.WriteLine("Csharp - Runtime smash..");
                    return;
                }

                Console.WriteLine(AstPrinter.PrintExpression(expr));
            }
        }

        internal static void Error(Token token, string message)
        {
            if (token.Type == TokenType.EOF)
            {
                Report(token.Line, " at end ", message);
            }
            else
            {
                Report(token.Line, " at '" + token.Lexeme + "'", message);
            }
        }

        internal static void Error(int line, string message)
        {
            Report(line, "", message);
        }

        private static void Report(int line, string where, string message)
        {
            var currentForeground = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[line {line}] Error {where} : {message}");
            Console.ForegroundColor = currentForeground;
            hadError = true;
        }

        // "Gracfully" report the error back to the user, without bombing.
        internal static void RuntimeError(RuntimeError ex)
        {
            Console.WriteLine($"{ex.Message}\n[Line: {ex.Token.Line}]");
            hadRuntimeError = true;
        }
    }

}