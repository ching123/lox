﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tool
{
    public class GenerateAst
    {
        public static void main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: GeneratgeAst <output directory>");
                Environment.Exit(64);
            }

            var outputDir = args[0];
            string[] types ={
                "Assign: Token name, Expr value",
                "Binary: Expr left, Token oper, Expr right",
                "Call: Expr callee, Token paren, List<Expr> arguments",
                "Get: Expr obj, Token name",
                "Grouping: Expr expression",
                "Literal: object? value",
                "Logical: Expr left, Token oper, Expr right",
                "Set: Expr obj, Token name, Expr value",
                "Super: Token keyword, Token method",
                "This: Token keyword",
                "Unary: Token oper, Expr right",
                "Variable: Token name"
            };
            defineAst(outputDir, "Expr", types);
            defineAst(outputDir, "Stmt", new List<string>{
                "Block: List<Stmt> statements",
                "Class: Token name, Expr.Variable? superclass, List<Stmt.Function> methods",
                "Expression: Expr expression",
                "Function: Token name, List<Token> parameters, List<Stmt> body",
                "If: Expr condition, Stmt thenBrance, Stmt? elseBranch",
                "Print: Expr expression",
                "Return: Token keyword, Expr? value",
                "Var: Token name, Expr? initalizer",
                "While: Expr condition, Stmt body",
            });
        }
        private static void defineAst(string outputDir, string baseName, IList<string> types)
        {
            using (var fs = new StreamWriter($"{outputDir}/{baseName}.cs"))
            {
                fs.WriteLine("using System;");
                fs.WriteLine("using System.Collections.Generic;");
                fs.WriteLine("using System.Linq;");
                fs.WriteLine("using System.Text;");
                fs.WriteLine("using System.Threading.Tasks;");
                fs.WriteLine();
                fs.WriteLine("namespace lox");
                fs.WriteLine("{");

                fs.WriteLine($"public abstract class {baseName}");
                fs.WriteLine("{");
                
                defineVisitor(fs, baseName, types);
                
                foreach (var type in types)
                {
                    var className = type.Split(':')[0].Trim();
                    var fields = type.Split(':')[1].Trim();
                    defineType(fs, baseName, className, fields);
                }
                fs.WriteLine("public abstract R accept<R>(Visitor<R> visitor);");
                fs.WriteLine("}");

                fs.WriteLine("}");

            }
        }
        private static void defineVisitor(StreamWriter writer, string baseName, IList<string> typeList)
        {
            writer.WriteLine("public interface Visitor<R>");
            writer.WriteLine("{");
            foreach (string type in typeList)
            {
                var className=type.Split(':')[0].Trim();
                writer.WriteLine($"public R visit{className}{baseName}({className} expr);");
            }
            writer.WriteLine("}");
        }
        private static void defineType(StreamWriter writer, string baseName, string className, string fieldList)
        {
            writer.WriteLine($"public class {className} : {baseName}");
            writer.WriteLine("{");

            writer.WriteLine($"public {className}({fieldList})");
            writer.WriteLine("{");
            var fields = fieldList.Split(',', StringSplitOptions.TrimEntries);
            foreach (var field in fields)
            {
                var name = field.Split(' ')[1];
                writer.WriteLine($"this.{name} = {name};");
            }
            writer.WriteLine("}");

            writer.WriteLine("public override R accept<R>(Visitor<R> visitor)");
            writer.WriteLine("{");
            writer.WriteLine($"return visitor.visit{className}{baseName}(this);");
            writer.WriteLine("}");

            foreach (var field in fields)
            {
                writer.WriteLine($"public readonly {field};");
            }

            writer.WriteLine("}");

        }
    }
}
