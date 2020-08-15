
using System;
using System.Collections.Generic;
using System.IO;

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
            if (HadError) throw new Exception("CS Lox has died, badly.");
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