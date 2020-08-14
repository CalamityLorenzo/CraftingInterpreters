using System;
using CsLoxInterpreter;
using CsLoxInterpreter.Demo;

namespace ParserApp
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine(DemoPrinter.GoDemo());
            Console.WriteLine(DemoPrinter.Chapter05());

            if (args.Length>0)
                Console.WriteLine(args[0]);
            CsLoxInterpreter.CSLox.Main(args);
            Console.WriteLine("and breathe..");
        }
    }
}
