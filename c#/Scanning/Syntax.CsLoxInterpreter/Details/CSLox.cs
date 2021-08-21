
using Syntax.CsLoxInterpreter.Utilities;
using System;
using System.Collections.Generic;
using System.IO;

namespace Syntax.CsLoxInterpreter.Details
{
    public static class CSLox
    {
        static bool HadError = false;
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
                HadError = false;
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
                ChallengeParser parser = new ChallengeParser(Tokens);
                var expr = parser.Parse();
                if (HadError)
                {
                    Console.WriteLine("Csharp -Lox has died badly.");
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
            HadError = true;
        }
    }

}