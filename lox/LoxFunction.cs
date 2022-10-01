using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lox
{
    public class LoxFunction : LoxCallable
    {
        private readonly Stmt.Function declaration;
        private readonly Env closure;
        private readonly bool isInitializer;
        public LoxFunction(Stmt.Function declaration, Env closure, bool isInitlizer)
        {
            this.declaration = declaration;
            this.closure = closure;
            this.isInitializer = isInitlizer;
        }
        public LoxFunction bind(LoxInstance instance)
        {
            var environment = new Env(closure);
            environment.define("this", instance);
            return new LoxFunction(declaration, environment, isInitializer);
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
                if (isInitializer) return closure.getAt(0, "this");
                return ex.value;
            }

            return isInitializer ? closure.getAt(0, "this") : null;
        }
        public override string ToString()
        {
            return $"<fn {declaration.name.lexeme}>";
        }
    }
}
