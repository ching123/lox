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
        public object? getAt(int distance, string name)
        {
            return ancestor(distance).values[name];
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
        public void assignAt(int distance, Token name, object? value)
        {
            ancestor(distance).values[name.lexeme] = value;
        }
        private Env ancestor(int distance)
        {
            var environment = this;
            for (var i = 0; i < distance; i++)
            {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                environment = environment.enclosing;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            }
#pragma warning disable CS8603 // Possible null reference return.
            return environment;
#pragma warning restore CS8603 // Possible null reference return.
        }
    }
}
