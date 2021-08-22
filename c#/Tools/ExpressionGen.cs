using System;
using System.Collections.Generic;

namespace ExpressIonGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            //if ()
            //{
            //    Console.WriteLine("Useage: generate_ast <output_directory>");
            //    return;
            //}

            var outputDir = args.Length != 1 ?  System.Environment.CurrentDirectory : args[0];
            DefineAst.Build(outputDir, "Expr", new List<string>(){
                "Assign : Token name, Expr value",
                "Binary   : Expr left, Token @operator, Expr right",
                "Grouping : Expr expression",
                "Literal  : Object value",
                "Unary    : Token @operator, Expr right",
                "Variable : Token name"
            });

            DefineAst.Build(outputDir, "Stmt", new List<string>()
            {
                "Block : List<Stmt> statments",
                "Expression : Expr expression",
                "Print : Expr expression",
                "Var : Token name, Expr initializer"
            });

        }
    }
}
