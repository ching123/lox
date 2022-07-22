using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lox
{
    public class Env
    {
        private readonly Env? enclosing;
        private Dictionary<string, object?> values = new Dictionary<string, object?>();
        public Env()
        {
            enclosing = null;
        }
        public Env(Env enclosing)
        {
            this.enclosing = enclosing;
        }
        public object? get(Token token)
        {
            if (values.ContainsKey(token.lexeme)) return values[token.lexeme];
            if(enclosing != null) return enclosing.get(token);

            throw new RuntimeError(token, $"Undefined variable '{token.lexeme}'.");
        }
        public void define(string name, object? value)
        {
            values.Add(name, value);
        }
        public void assign(Token name, object? value)
        {
            if (values.ContainsKey(name.lexeme))
            {
                values[name.lexeme] = value;
                return;
            }
            if (enclosing != null)
            {
                enclosing.assign(name, value);
                return;
            }
            throw new RuntimeError(name, $"Undefined variable {name.lexeme}");
        }
    }
}
