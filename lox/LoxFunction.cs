using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lox
{
    public class LoxFunction : LoxCallable
    {
        private Stmt.Function declaration;
        private Env closure;
        public LoxFunction(Stmt.Function declaration, Env closure)
        {
            this.declaration = declaration;
            this.closure = closure;
        }
        public int arity()
        {
            return this.declaration.parameters.Count();
        }

        public object? call(Interpreter interpreter, List<object?> arguments)
        {
            Env environment = new Env(closure);
            for (var i = 0; i < this.declaration.parameters.Count(); ++i)
            {
                environment.define(this.declaration.parameters[i].lexeme, arguments[i]);
            }
            try
            {
                interpreter.executeBlock(this.declaration.body, environment);
            }
            catch (Return ex)
            {
                return ex.value;
            }
            return null;
        }

        public string toString()
        {
            return $"<fn {declaration.name.lexeme}>";
        }
    }
}
