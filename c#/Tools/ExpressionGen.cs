﻿using System;
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
                "Conditional : Expr ifThen, Expr ifElse",
                "Call        : Expr callee, Token paren, List<Expr> arguments",
                "Get         : Expr @object, Token name",
                "Grouping : Expr expression",
                "Literal  : Object value",
                "Logical  : Expr left, Token @operator, Expr right",
                "Set      : Expr @object, Token name, Expr value",
                "Super    : Token keyword, Token method",
                "This     : Token keyword",
                "Unary    : Token @operator, Expr right",
                "Variable : Token name"
            });

            DefineAst.Build(outputDir, "Stmt", new List<string>()
            {
                "Block          : List<Stmt> statments",
                "Break          : ",
                "Class          : Token name, Expr.Variable superClass, List<Stmt.Function> methods",
                "ExpressionStmt : Expr expression",
                "Function       : Token Name, List<Token> params, List<Stmt> body",
                "If             : Expr condition, Stmt thenBranch, Stmt elseBranch",
                "Print          : Expr expression",
                "Return         : Token keyword, Expr value",
                "Var            : Token name, Expr initializer",
                "While          : Expr condition, Stmt body"
            });

        }
    }
}
