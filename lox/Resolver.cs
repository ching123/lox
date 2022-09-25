using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lox
{
    public class Resolver : Expr.Visitor<int?>, Stmt.Visitor<int?>
    {
        private readonly Interpreter interpreter;
        private readonly Stack<Dictionary<string,bool>> scopes = new Stack<Dictionary<string,bool>>();
        private FunctionType currentFunction = FunctionType.NONE;

        private enum FunctionType
        {
            NONE,
            FUNCTION,
        }
        public Resolver(Interpreter interpreter)
        {
            this.interpreter = interpreter;
        }

        public int? visitAssignExpr(Expr.Assign expr)
        {
            resolve(expr.value);
            resolveLocal(expr, expr.name);
            return null;
        }

        public int? visitBinaryExpr(Expr.Binary expr)
        {
            resolve(expr.left);
            resolve(expr.right);
            return null;
        }

        public int? visitBlockStmt(Stmt.Block expr)
        {
            beginScope();
            resolve(expr.statements);
            endScope();
            return null;
        }

        public int? visitCallExpr(Expr.Call expr)
        {
            resolve(expr.callee);
            foreach (var argument in expr.arguments)
            {
                resolve(argument);
            }
            return null;
        }

        public int? visitExpressionStmt(Stmt.Expression expr)
        {
            resolve(expr.expression);
            return null;
        }

        public int? visitFunctionStmt(Stmt.Function expr)
        {
            declare(expr.name);
            define(expr.name);
            resolveFunction(expr, FunctionType.FUNCTION);
            return null;
        }

        public int? visitGroupingExpr(Expr.Grouping expr)
        {
            resolve(expr.expression);
            return null;
        }

        public int? visitIfStmt(Stmt.If expr)
        {
            resolve(expr.condition);
            resolve(expr.thenBrance);
            if (expr.elseBranch != null) resolve(expr.elseBranch);
            return null;
        }

        public int? visitLiteralExpr(Expr.Literal expr)
        {
            return null;
        }

        public int? visitLogicalExpr(Expr.Logical expr)
        {
            resolve(expr.left);
            resolve(expr.right);
            return null;
        }

        public int? visitPrintStmt(Stmt.Print expr)
        {
            resolve(expr.expression);
            return null;
        }

        public int? visitReturnStmt(Stmt.Return expr)
        {
            if (currentFunction == FunctionType.NONE)
            {
                Lox.error(expr.keyword, "can not return from top-level code.");
            }
            if (expr.value != null)
            {
                resolve(expr.value);
            }
            return null;
        }

        public int? visitUnaryExpr(Expr.Unary expr)
        {
            resolve(expr.right);
            return null;
        }

        public int? visitVariableExpr(Expr.Variable expr)
        {
            if (scopes.Count != 0 &&
                scopes.Peek().GetValueOrDefault(expr.name.lexeme, true) == false)
            {
                Lox.error(expr.name, "can not read local variable in its own initializer.");
            }
            resolveLocal(expr, expr.name);
            return null;
        }

        public int? visitVarStmt(Stmt.Var expr)
        {
            declare(expr.name);
            if (expr.initalizer != null)
            {
                resolve(expr.initalizer);
            }
            define(expr.name);
            return null;
        }

        public int? visitWhileStmt(Stmt.While expr)
        {
            resolve(expr.condition);
            resolve(expr.body);
            return null;
        }
        public void resolve(List<Stmt> statements)
        {
            foreach(var stmt in statements)
            {
                resolve(stmt);
            }
        }
        private void resolve(Stmt stmt)
        {
            stmt.accept(this);
        }
        private void resolve(Expr expr)
        {
            expr.accept(this);
        }
        private void beginScope()
        {
            scopes.Push(new Dictionary<string, bool>());
        }
        private void endScope()
        {
            scopes.Pop();
        }
        private void declare(Token name)
        {
            if (scopes.Count == 0) return;
            var scope = scopes.Peek();
            if (scope.ContainsKey(name.lexeme))
            {
                Lox.error(name, "already a variable with this name in this scope.");
            }
            scope.Add(name.lexeme, false);
        }
        private void define(Token token)
        {
            if (scopes.Count == 0) return;
            var scope=scopes.Peek();
            scope[token.lexeme] = true;
        }
        private void resolveLocal(Expr expr, Token name)
        {
            for (int i = 0; i < scopes.Count; i++)
            {
                if (scopes.ElementAt(i).ContainsKey(name.lexeme))
                {
                    interpreter.resolve(expr, i);
                    return;
                }
            }
        }
        private void resolveFunction(Stmt.Function function, FunctionType type)
        {
            var enclosingFunction = currentFunction;
            currentFunction = type;

            beginScope();
            foreach (var parameter in function.parameters)
            {
                declare(parameter);
                define(parameter);
            }
            resolve(function.body);
            endScope();

            currentFunction = enclosingFunction;
        }
    }
}
