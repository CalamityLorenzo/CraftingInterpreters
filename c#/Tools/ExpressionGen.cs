using System;
using System.Collections.Generic;

namespace ExpressIonGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Useage: generate_ast <output_directory>");
                return;
            }

            var outputDir = args[0];
            DefineAst.Build(outputDir, "Expr", new List<string>(){
                "Binary   : Expr left, Token @operator, Expr right",
                "Grouping : Expr expression",
                "Literal  : Object value",
                "Unary    : Token @operator, Expr right"
            })
            ;

        }
    }
}
