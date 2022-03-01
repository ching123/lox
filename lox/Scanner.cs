using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lox
{
    public class Scanner
    {
        private string source;
        private readonly List<Token> tokens=new List<Token>();
        private int start = 0;
        private int current = 0;
        private int line = 1;
        private static readonly Dictionary<string, TokenType> keywords;
        static Scanner()
        {
            keywords = new Dictionary<string, TokenType>();
            keywords.Add("and", TokenType.AND);
            keywords.Add("class", TokenType.CLASS);
            keywords.Add("else", TokenType.ELSE);
            keywords.Add("false", TokenType.FALSE);
            keywords.Add("for", TokenType.FOR);
            keywords.Add("fun", TokenType.FUN);
            keywords.Add("if", TokenType.IF);
            keywords.Add("nil", TokenType.NIL);
            keywords.Add("or", TokenType.OR);
            keywords.Add("print", TokenType.PRINT);
            keywords.Add("return", TokenType.RETURN);
            keywords.Add("super", TokenType.SUPER);
            keywords.Add("this", TokenType.THIS);
            keywords.Add("true", TokenType.TRUE);
            keywords.Add("var", TokenType.VAR);
            keywords.Add("while", TokenType.WHILE);
        }
        public Scanner(string source)
        {
            this.source = source;
        }
        public List<Token> scanTokens()
        {
            while (!isAtEnd())
            {
                start = current;
                scanToken();
            }
            tokens.Add(new Token(TokenType.EOF, "", null, line));
            return tokens;
        }
        private void scanToken()
        {
            var c = advance();
            switch (c)
            {
                case '(':addToken(TokenType.LEFT_PAREN);break;
                case ')':addToken(TokenType.RIGHT_PAREN);break;
                case '{':addToken(TokenType.LEFT_BRACE);break;
                case '}':addToken(TokenType.RIGHT_BRACE);break;
                case ',':addToken(TokenType.COMMA);break;
                case '.':addToken(TokenType.DOT);break;
                case ';':addToken(TokenType.COMMA);break;
                case '+':addToken(TokenType.PLUS);break;
                case'-':addToken(TokenType.MINUS);break;
                case '*':addToken(TokenType.STAR);break;

                case '!':
                    addToken(match('=') ? TokenType.BANG_EQUAL : TokenType.BANG_EQUAL);
                    break;
                case '=':
                    addToken(match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
                    break;
                case '<':
                    addToken(match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
                    break;
                case '>':
                    addToken(match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
                    break;

                case '/':
                    if (match('/'))
                    {
                        while (peek() != '\n' && !isAtEnd()) advance();
                    }
                    else
                    {
                        addToken(TokenType.SLASH);
                    }
                    break;

                case ' ':
                case '\r':
                case '\t':
                    // ignore whitespace
                    break;
                case '\n':
                    line++;
                    break;

                case '"': stringLiteral(); break;

                
                default:
                    if (isDigit(c))
                    {
                        number();
                    }
                    else if (isAlpha(c))
                    {
                        identifier();
                    }
                    else
                    {
                        Lox.error(line, "unexpected character");
                    }
                    break;
            }
        }
        private void identifier()
        {
            while(isAlphaNumeric(peek())) advance();
            var text = source.Substring(start, current - start);
            TokenType type = TokenType.IDENTIFIER;
            if (keywords.ContainsKey(text))
            {
                type = keywords[text];
            }
            addToken(type);
        }
        private void number()
        {
            while (isDigit(peek())) advance();
            if (peek() == '.' && isDigit(peekNext()))
            {
                advance();  // consume the character .
                while (isDigit(peek())) advance();
            }
            addToken(TokenType.NUMBER,
                Convert.ToDouble(source.Substring(start, current - start)));
        }
        private void stringLiteral()
        {
            char c;
            while ((c = peek()) != '"' && !isAtEnd())
            {
                if (c == '\n') line++;
                advance();
            }
            if (isAtEnd())
            {
                Lox.error(line, "unterminated string.");
                return;
            }
            advance();  // consume the closing "
            var text = source.Substring(start+1, current - start - 2);
            addToken(TokenType.STRING, text);
        }

        private bool match(char expected)
        {
            if (isAtEnd() || source[current] != expected)
            {
                return false;
            }
            current++;
            return true;
        }
        private char peekNext()
        {
            return (current + 1) >= source.Length ? '\0' : source[current+1];
        }
        private char peek()
        {
            return isAtEnd() ? '\0' : source[current];
        }
        private void addToken(TokenType type)
        {
            addToken(type, null);
        }
        private void addToken(TokenType type, object? literal)
        {
            var text = source.Substring(start, current - start);
            tokens.Add(new Token(type, text, literal, line));
        }
        private char advance()
        {
            return source[current++];
        }
        private bool isAtEnd()
        {
            return current >= this.source.Length;
        }
        private bool isDigit(char c)
        {
            return c >= '0' && c <= '9';
        }
        private bool isAlpha(char c)
        {
            return (c >= 'a' && c <= 'z') ||
                (c >= 'A' && c <= 'Z') ||
                c == '-';
        }
        private bool isAlphaNumeric(char c)
        {
            return isAlpha(c) || isDigit(c);
        }
    }
}
