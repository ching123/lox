using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lox
{
    public enum TokenType
    {
        // single-character tokens
        LEFT_PAREN, RIGHT_PAREN, LEFT_BRACE, RIGHT_BRACE,
        COMMA, DOT, MINUS, PLUS, SEMICOLON, SLASH, STAR,
        // one or two character tokens
        BANG, BANG_EQUAL,
        EQUAL, EQUAL_EQUAL,
        GREATER, GREATER_EQUAL,
        LESS, LESS_EQUAL,
        // literals
        IDENTIFIER, STRING, NUMBER,
        // keywords
        AND, OR, CLASS, IF, ELSE, FALSE, TRUE, FUN, FOR,
        NIL, PRINT, RETURN, SUPER, THIS, VAR, WHILE,

        EOF
    }
}
