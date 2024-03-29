﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lox
{
    public class Interpreter : Expr.Visitor<object?>, Stmt.Visitor<object?>
    {
        public static readonly Env globals = new Env();
        private Env environment = globals;
        private Dictionary<Expr, int> locals = new Dictionary<Expr, int>();
        public Interpreter()
        {
            globals.define("clock", new Clock());
        }
        public void interpret(List<Stmt> statements)
        {
            try
            {
                foreach (Stmt stmt in statements)
                {
                    execute(stmt);
                }
            }
            catch(RuntimeError ex)
            {
                Lox.runtimeError(ex);
            }
        }
        public void interpret(Expr? expression)
        {
            try
            {
                var result = evaluate(expression);
                Console.WriteLine(stringify(result));
            }
            catch(RuntimeError ex)
            {
                Lox.runtimeError(ex);
            }
        }
        private string stringify(object? obj)
        {
            if (obj == null) return "nil";
            if (obj is double)
            {
                var s = obj.ToString();
                if (s != null && s.EndsWith(".0"))
                {
                    return s.Substring(0, s.Length - 2);
                }
            }
            return obj.ToString() ?? "nil";
        }
        public object? visitBinaryExpr(Expr.Binary expr)
        {
            var left = evaluate(expr.left);
            var right = evaluate(expr.right);
            switch (expr.oper.type)
            {
                // arithmetic operations
                case TokenType.MINUS:
                    {
                        checkNumberOperands(expr.oper, left, right);
                        if (left != null && right != null)
                            return (double)left - (double)right;
                        break;
                    }
                case TokenType.STAR:
                    {
                        checkNumberOperands(expr.oper, left, right);
                        if (left != null && right != null)
                            return (double)left * (double)right;
                        break;
                    }
                case TokenType.SLASH:
                    {
                        checkNumberOperands(expr.oper, left, right);
                        if (left != null && right != null)
                            return (double)left / (double)right;
                        break;
                    }
                case TokenType.PLUS:
                    {
                        if (left is double && right is double)
                            return (double)left + (double)right;
                        if (left is string && right is string)
                            return (string)left + (string)right;
                        throw new RuntimeError(expr.oper, "Operands must be two numbers or two strings.");
                        //break;
                    }
                // comparion operations
                case TokenType.LESS:
                    {
                        checkNumberOperands(expr.oper, left, right);
                        if (left != null && right != null)
                            return (double)left < (double)right;
                        break;
                    }
                case TokenType.LESS_EQUAL:
                    {
                        checkNumberOperands(expr.oper, left, right);
                        if (left != null && right != null)
                            return (double)left <= (double)right;
                        break;
                    }
                case TokenType.GREATER:
                    {
                        checkNumberOperands(expr.oper, left, right);
                        if (left != null && right != null)
                            return (double)left > (double)right;
                        break;
                    }
                case TokenType.GREATER_EQUAL:
                    {
                        checkNumberOperands(expr.oper, left, right);
                        if (left != null && right != null)
                            return (double)left >= (double)right;
                        break;
                    }
                case TokenType.BANG_EQUAL:return !isEqual(expr.left, expr.right);
                case TokenType.EQUAL_EQUAL:return isEqual(expr.left, expr.right);
            }
            return null;
        }

        public object? visitGroupingExpr(Expr.Grouping expr)
        {
            return evaluate(expr.expression);
        }

        public object? visitLiteralExpr(Expr.Literal expr)
        {
            return expr.value;
        }

        public object? visitUnaryExpr(Expr.Unary expr)
        {
            var right = evaluate(expr.right);
            switch(expr.oper.type)
            {
                case TokenType.BANG: return !isTruthy(right);
                case TokenType.MINUS:
                    {
                        checkNumberOperand(expr.oper, right);
                        if (right != null)
                            return -(double)right;
                        break;
                    }
            }

            return null;
        }
        public void resolve(Expr expr, int depth)
        {
            locals.Add(expr, depth);
        }
        private void checkNumberOperands(Token oper, object? left, object? right)
        {
            if (left is double && right is double) return;
            throw new RuntimeError(oper, "Operand must be a number.");
        }
        private void checkNumberOperand(Token oper, object? operand)
        {
            if (operand is double) return;
            throw new RuntimeError(oper, "Operand must be a number.");
        }
        private object? evaluate(Expr? expr)
        {
            return expr?.accept(this);
        }
        public void executeBlock(List<Stmt> statements, Env envirenment)
        {
            var previous = this.environment;
            try
            {
                this.environment = envirenment;

                foreach (var statement in statements)
                {
                    execute(statement);
                }
            }
            finally
            {
                this.environment = previous;
            }
        }
        private void execute(Stmt stmt)
        {
            stmt.accept(this);
        }
        private bool isEqual(object? a, object? b)
        {
            if (a == null && b == null) return true;
            if (a == null) return false;
            return a.Equals(b);
        }
        private bool isTruthy(object? obj)
        {
            if (obj == null) return false;
            if (obj is bool) return (bool)obj;
            return true;
        }

        public object? visitExpressionStmt(Stmt.Expression expr)
        {
            evaluate(expr.expression);
            return null;
        }

        public object? visitPrintStmt(Stmt.Print expr)
        {
            var value = evaluate(expr.expression);
            Console.WriteLine(stringify(value));
            return null;
        }

        public object? visitVariableExpr(Expr.Variable expr)
        {
            return lookupVariable(expr.name, expr);
            //return environment.get(expr.name);
        }

        public object? visitVarStmt(Stmt.Var expr)
        {
            object? value = null;
            if (expr.initalizer != null)
            {
                value = evaluate(expr.initalizer);
            }
            environment.define(expr.name.lexeme, value);
            return null;
        }

        public object? visitAssignExpr(Expr.Assign expr)
        {
            var value = evaluate(expr.value);
            int distance;
            if (locals.TryGetValue(expr, out distance))
            {
                environment.assignAt(distance, expr.name, value);
            }
            else
            {
                globals.assign(expr.name, value);
            }
            return value;
        }

        public object? visitBlockStmt(Stmt.Block expr)
        {
            executeBlock(expr.statements, new Env(this.environment));
            return null;
        }

        public object? visitIfStmt(Stmt.If expr)
        {
            if (isTruthy(evaluate(expr.condition)))
            {
                execute(expr.thenBrance);
            }
            else if (expr.elseBranch != null)
            {
                execute(expr.elseBranch);
            }
            return null;
        }

        public object? visitLogicalExpr(Expr.Logical expr)
        {
            var left = evaluate(expr.left);
            if (expr.oper.type == TokenType.OR)
            {
                if (isTruthy(left)) return left;
            }
            else
            {
                if (!isTruthy(left)) return left;
            }
            
            return evaluate(expr.right);
        }

        public object? visitWhileStmt(Stmt.While expr)
        {
            while (isTruthy(evaluate(expr.condition)))
            {
                execute(expr.body);
            }
            return null;
        }

        public object? visitCallExpr(Expr.Call expr)
        {
            var callee = evaluate(expr.callee);
            if (!(callee is LoxCallable))
                throw new RuntimeError(expr.paren, "Can only call functions and classes.");
            LoxCallable function = (LoxCallable)callee;
            if (function.arity() != expr.arguments.Count())
                throw new RuntimeError(expr.paren, $"Expected {function.arity()} arguments but got ${expr.arguments.Count()}.");
            
            List<object?> arguments = new List<object?>();
            foreach (var argument in expr.arguments)
            {
                arguments.Add(evaluate(argument));
            }
            return function.call(this, arguments);
        }

        public object? visitFunctionStmt(Stmt.Function expr)
        {
            var function = new LoxFunction(expr, environment, false);
            environment.define(expr.name.lexeme, function);
            return null;
        }

        public object? visitReturnStmt(Stmt.Return expr)
        {
            object? val = null;
            if (expr.value != null)
            {
                val = evaluate(expr.value);
            }
            throw new Return(val);
        }
        private object? lookupVariable(Token name, Expr expr)
        {
            int distance = 0;
            if (locals.TryGetValue(expr, out distance))
            {
                return environment.getAt(distance, name.lexeme);
            }
            else
            {
                return globals.get(name);
            }
        }

        public object? visitClassStmt(Stmt.Class expr)
        {
            object? superclass = null;
            if (expr.superclass != null)
            {
                superclass = evaluate(expr.superclass);
                if (superclass is not LoxClass)
                {
                    throw new RuntimeError(expr.superclass.name,
                        "superclass must be a class.");
                }
            }

            environment.define(expr.name.lexeme, null);

            var enclosing = environment;
            if (expr.superclass != null)
            {
                environment = new Env(environment);
                environment.define("super", superclass);
            }

            var methods = new Dictionary<string, LoxFunction>();
            foreach (var method in expr.methods)
            {
                methods[method.name.lexeme] =
                    new LoxFunction(method, environment, method.name.lexeme.Equals("init"));
            }

            if (expr.superclass != null)
            {
                environment = enclosing;
            }

            var cls = new LoxClass(expr.name.lexeme, (LoxClass?)superclass, methods);
            environment.assign(expr.name, cls);
            return null;
        }

        public object? visitGetExpr(Expr.Get expr)
        {
            var obj = evaluate(expr.obj);
            if (obj is LoxInstance)
            {
                return ((LoxInstance)obj).Get(expr.name);
            }
            throw new RuntimeError(expr.name, "only instance have properties.");
        }

        public object? visitSetExpr(Expr.Set expr)
        {
            var obj = evaluate(expr.obj);
            if (obj is not LoxInstance)
            {
                throw new RuntimeError(expr.name, "only instance have fields.");
            }
            var value = evaluate(expr.value);
            var instance = (LoxInstance)(obj);
            instance.Set(expr.name, value);
            return value;
        }

        public object? visitThisExpr(Expr.This expr)
        {
            return lookupVariable(expr.keyword, expr);
        }

        public object? visitSuperExpr(Expr.Super expr)
        {
            var distance = 0;
            locals.TryGetValue(expr, out distance);
            var superclass = environment.getAt(distance, "super");
            if (superclass == null)
            {
                throw new RuntimeError(expr.method, $"'super' not found at property '{expr.method.lexeme}'.");
            }
            var obj = environment.getAt(distance - 1, "this");
            if (obj == null)
            {
                throw new RuntimeError(expr.method, $"not found 'this' at property '{expr.method.lexeme}'");
            }
            var method = ((LoxClass)superclass).findMethod(expr.method.lexeme);
            if (method == null)
            {
                throw new RuntimeError(expr.method, $"undefined property '{expr.method.lexeme}'.");
            }
            return method.bind((LoxInstance)obj);
        }
    }
}
