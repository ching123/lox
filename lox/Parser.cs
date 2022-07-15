using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace lox
{
    /// <summary>
    /// expression     → equality ;
    /// equality       → comparison(( "!=" | "==" ) comparison )* ;
    /// comparison     → term(( ">" | ">=" | "<" | "<=" ) term )* ;
    /// term           → factor(( "-" | "+" ) factor )* ;
    /// factor         → unary(( "/" | "*" ) unary )* ;
    /// unary          → ( "!" | "-" ) unary
    ///                | primary ;
    /// primary        → NUMBER | STRING | "true" | "false" | "nil"
    ///                | "(" expression ")" ;
    /// </summary>
    public class Parser
    {
        private class ParseError : Exception { }
        private List<Token> tokens;
        private int current = 0;
        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
        }
        public List<Stmt> parse()
        {
            try
            {
                List<Stmt> statements = new List<Stmt>();
                while(!isEnd())
                {
                    var statement = declaration();
                    if (statement != null)
                    {
                        statements.Add(statement);
                    }
                }

                return statements;
            }
            catch (ParseError ex)
            {
                Console.WriteLine(ex.Message);
                return new List<Stmt>();
            }
        }
        public Expr? parseExpression()
        {
            try
            {
                return expression();
            }
            catch (ParseError ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        private Stmt? declaration()
        {
            try
            {
                if (match(TokenType.VAR)) return varDeclaration();
                return statement();
            }
            catch (ParseError)
            {
                synchronize();
                return null;
            }
        }
        private Stmt statement()
        {
            if (match(TokenType.PRINT)) return printStatement();
            if (match(TokenType.LEFT_BRACE)) return new Stmt.Block(block());
            return expressionStatement();
        }
        private List<Stmt> block()
        {
            List<Stmt> statements = new List<Stmt>();
            while(!check(TokenType.RIGHT_BRACE) && !isEnd())
            {
                var statement = declaration();
                if (statement != null)
                {
                    statements.Add(statement);
                }
            }
            consume(TokenType.RIGHT_BRACE, "Expect '}' after block.");
            return statements;
        }
        private Stmt varDeclaration()
        {
            var name = consume(TokenType.IDENTIFIER, "Expect variable name.");
            Expr? initializer = null;
            if (match(TokenType.EQUAL))
            {
                initializer = expression();
            }
            consume(TokenType.SEMICOLON, "Expect ';' after variable declaration.");
            return new Stmt.Var(name, initializer);
        }
        private Stmt printStatement()
        {
            var value = expression();
            consume(TokenType.SEMICOLON, "Expect ';' after value.");
            return new Stmt.Print(value);
        }
        private Stmt expressionStatement()
        {
            var expr = expression();
            consume(TokenType.SEMICOLON, "Expect ';' after expression.");
            return new Stmt.Expression(expr);
        }
        private Expr expression()
        {
            return assignment();
        }
        private Expr assignment()
        {
            var expr = equality();
            if(match(TokenType.EQUAL))
            {
                var prev = previous();
                var value = assignment();
                if (expr is Expr.Variable)
                {
                    var name = ((Expr.Variable)expr).name;
                    return new Expr.Assign(name, value);
                }
                error(prev, "Invalid assignment target.");
            }
            return expr;
        }
        private Expr equality()
        {
            var expr = comparison();
            while(match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL))
            {
                var oper = previous();
                var right = comparison();
                expr = new Expr.Binary(expr, oper, right);
            }
            return expr;
        }
        private Expr comparison()
        {
            var expr = term();
            while(match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL))
            {
                var oper = previous();
                var right=term();
                expr = new Expr.Binary(expr, oper, right);
            }
            return expr;
        }
        private Expr term()
        {
            var expr = factor();
            while(match(TokenType.PLUS, TokenType.MINUS))
            {
                var oper = previous();
                var right = factor();
                expr = new Expr.Binary(expr, oper, right);
            }
            return expr;
        }
        private Expr factor()
        {
            var expr = unary();
            while (match(TokenType.STAR, TokenType.SLASH))
            {
                var oper = previous();
                var right = unary();
                expr = new Expr.Binary(expr, oper, right);
            }
            return expr;
        }
        private Expr unary()
        {
            if (match(TokenType.BANG, TokenType.MINUS))
            {
                var oper = previous();
                var right = unary();
                return new Expr.Unary(oper, right);
            }
            return primary();
        }
        private Expr primary()
        {
            if (match(TokenType.FALSE)) return new Expr.Literal(false);
            else if (match(TokenType.TRUE)) return new Expr.Literal(true);
            else if (match(TokenType.NIL)) return new Expr.Literal(null);
            else if (match(TokenType.NUMBER, TokenType.STRING)) return new Expr.Literal(previous().literal);
            else if (match(TokenType.LEFT_PAREN))
            {
                var expr = expression();
                consume(TokenType.RIGHT_PAREN, "Expect ')' after expression.");
                return new Expr.Grouping(expr);
            }
            else if (match(TokenType.IDENTIFIER)) return new Expr.Variable(previous());

            throw error(peek(), "Expect expression.");
        }
        private Token consume(TokenType type, string message)
        {
            if (check(type)) return advance();

            throw error(peek(), message);
        }
        private ParseError error(Token token, string message)
        {
            Lox.error(token, message);
            
            return new ParseError();
        }
        private void synchronize()
        {
            advance();
            while(!isEnd())
            {
                if (previous().type == TokenType.SEMICOLON) return;
                switch (peek().type)
                {
                    case TokenType.CLASS:
                    case TokenType.FUN:
                    case TokenType.VAR:
                    case TokenType.FOR:
                    case TokenType.IF:
                    case TokenType.WHILE:
                    case TokenType.PRINT:
                    case TokenType.RETURN:
                        return;
                }
                advance();
            }
        }
        private bool match(params TokenType[] types)
        {
            foreach(var t in types)
            {
                if (check(t))
                {
                    advance();
                    return true;
                }
            }
            return false;
        }
        private bool check(TokenType type)
        {
            return (isEnd()) ? false : peek().type == type;
        }
        private Token advance()
        {
            if (!isEnd()) current++;
            return previous();
        }
        private bool isEnd()
        {
            return peek().type == TokenType.EOF;
        }
        private Token peek()
        {
            return tokens.ElementAt(current);
        }
        private Token previous()
        {
            return tokens.ElementAt(current - 1);
        }
    }
}
