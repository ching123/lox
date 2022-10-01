using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lox
{
    public class AstPrinter : Expr.Visitor<string>
    {
        public static void main(string[] args)
        {
            Expr expression = new Expr.Binary(
                new Expr.Unary(
                    new Token(TokenType.MINUS, "-", null, 1),
                    new Expr.Literal(123)),
                new Token(TokenType.STAR, "*", null, 1),
                new Expr.Grouping(
                    new Expr.Literal(45.67)));

            Console.WriteLine(new AstPrinter().print(expression));
        }
        public string print(Expr? expression)
        {
            if (expression == null) return String.Empty;

            return expression.accept(this);
        }

        public string visitAssignExpr(Expr.Assign expr)
        {
            throw new NotImplementedException();
        }

        public string visitBinaryExpr(Expr.Binary expr)
        {
            return parenthesize(expr.oper.lexeme, expr.left, expr.right);
            //return $"({expr.left.accept(this)}{expr.oper.lexeme}{expr.right.accept(this)})";
        }

        public string visitCallExpr(Expr.Call expr)
        {
            throw new NotImplementedException();
        }

        public string visitGetExpr(Expr.Get expr)
        {
            throw new NotImplementedException();
        }

        public string visitGroupingExpr(Expr.Grouping expr)
        {
            return parenthesize("group", expr.expression);
            //return $"({expr.expression.accept(this)})";
        }
        public string visitLiteralExpr(Expr.Literal expr)
        {
            return (expr.value?.ToString()) ?? "Nil";
            //return $"({expr.value.ToString()})";
        }

        public string visitLogicalExpr(Expr.Logical expr)
        {
            throw new NotImplementedException();
        }

        public string visitSetExpr(Expr.Set expr)
        {
            throw new NotImplementedException();
        }

        public string visitThisExpr(Expr.This expr)
        {
            throw new NotImplementedException();
        }

        public string visitUnaryExpr(Expr.Unary expr)
        {
            return parenthesize(expr.oper.lexeme, expr.right);
            //return $"({expr.oper.lexeme}{expr.right.accept(this)})";
        }

        public string visitVariableExpr(Expr.Variable expr)
        {
            throw new NotImplementedException();
        }

        private string parenthesize(string name, params Expr[] exprs)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("(").Append(name);
            foreach (var expr in exprs)
            {
                builder.Append(" ");
                builder.Append(expr.accept(this));
            }
            builder.Append(")");

            return builder.ToString();
        }
    }
}
