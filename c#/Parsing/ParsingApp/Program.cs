using System;
using CsLoxInterpreter;
namespace ParsingApp
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
                Console.WriteLine(args[0]);
            CsLoxInterpreter.CSLox.Main(args);
            Console.WriteLine("and parsed..");
        }
    }
}
