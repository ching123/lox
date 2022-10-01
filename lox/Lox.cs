using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lox
{

    public class Lox
    {
        static Interpreter interpreter = new Interpreter();
        static bool hadError = false;
        static bool hadRuntimeError = false;
        public static void main(string[] args)
        {
            if (args.Length > 1)
            {
                Console.WriteLine("usage: lox [script]");
            }
            else if (args.Length == 1)
            {
                runFile(args[0]);
            }
            else
            {
                runPrompt();
            }

        }
        private static void runFile(string path)
        {
            var text = File.ReadAllText(path, Encoding.UTF8);
            run(text);

            if (hadError) Environment.Exit(65);
            if (hadRuntimeError) Environment.Exit(70);
        }
        private static void runPrompt()
        {

        }
        private static void run(string source)
        {
            var scanner = new Scanner(source);
            var tokens = scanner.scanTokens();
            var parser = new Parser(tokens);
            var statements = parser.parse();

            if (hadError) return;

            var resolver = new Resolver(interpreter);
            resolver.resolve(statements);

            if (hadError) return;

            interpreter.interpret(statements);
            
            //Console.WriteLine($"{new AstPrinter().print(expression)}\n");

            //foreach (var token in tokens)
            //{
            //    Console.WriteLine($"{token.ToString()}\n");
            //}
        }
        public static void runtimeError(RuntimeError error)
        {
            Console.WriteLine($"{error.Message}\n[line {error.token.line}]");
            hadRuntimeError = true;
        }
        public static void error(int line, string message)
        {
            report(line, "", message);
        }
        public static void error(Token token, string message)
        {
            if (token.type == TokenType.EOF)
            {
                report(token.line, "at end", message);
            }
            else
            {
                report(token.line, $"at '{token.lexeme}'", message);
            }
        }
        private static void report(int line, string where, string message)
        {
            Console.WriteLine($"[line {line}] error {where}: {message}");
            hadError = true;

        }
    }
}
