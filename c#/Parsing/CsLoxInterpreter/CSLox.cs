
using CsLoxInterpreter.Parsing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using static CsLoxInterpreter.TokenType;

namespace CsLoxInterpreter
{
    public static class CSLox
    {
        static bool HadError = false;
        public static void Main(string[] args)
        {
            var thisAssembly = System.AppDomain.CurrentDomain.FriendlyName;

            if (args.Length > 1)
            {
                Console.WriteLine($"Useage: {thisAssembly}");
            }
            else if (args.Length == 1)
                RunFile(args[0]); // Assume the argument is a file
            else
            {
                RunPrompt(); // Run the doo-hickey. REPL
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
                var line = Console.ReadLine();
                if (line == null || line == "#Quit") break;
                if(line!=string.Empty)
                    Run(line);
                HadError = false;
            }

        }

        private static void Run(string Source)
        {
            var scanner = new Scanner(Source);
            List<Token> Tokens = scanner.ScanTokens();
            Tokens.ForEach(token => Console.WriteLine(token));
            var parser = new Parser(Tokens);
            var completeExpression = parser.Parse();
            if (HadError) return;
            Console.WriteLine(new AstPrinter().Print(completeExpression));
        }

        internal static void Error(int line, string message)
        {
            Report(line, "", message);
        }

        internal static void Error(Token token, string message)
        {
            if (token.TokenType == EOF)
                Report(token.Line, " at end ", message);
            else
                Report(token.Line, $" at '{token.Lexeme}'", message);
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