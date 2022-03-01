﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lox
{
    public class Lox
    {
        static bool hadError = false;
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
        }
        private static void runPrompt()
        {

        }
        private static void run(string source)
        {
            var scanner = new Scanner(source);
            var tokens = scanner.scanTokens();
            foreach (var token in tokens)
            {
                Console.WriteLine($"{token.toString()}\n");
            }
        }
        public static void error(int line, string message)
        {
            report(line, "", message);
        }
        private static void report(int line, string where, string message)
        {
            Console.WriteLine($"[line {line}] error {where}: {message}");
            hadError = true;

        }
    }
}