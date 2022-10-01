using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lox
{
    public class LoxClass : LoxCallable
    {
        private readonly string name;
        private readonly Dictionary<string, LoxFunction> methods = new Dictionary<string, LoxFunction>();
        public LoxClass(string name, Dictionary<string, LoxFunction> methods)
        {
            this.name = name;
            this.methods = methods;
        }
        public LoxFunction? findMethod(string name)
        {
            if (methods.ContainsKey(name))
            {
                return methods[name];
            }
            return null;
        }
        public int arity()
        {
            var initializer = findMethod("init");
            return (initializer != null) ? initializer.arity() : 0;
        }

        public object? call(Interpreter interpreter, List<object?> arguments)
        {
            var instance = new LoxInstance(this);
            var initializer = findMethod("init");
            if (initializer != null)
            {
                initializer.bind(instance).call(interpreter, arguments);
            }
            return instance;
        }
        public override string ToString()
        {
            return name;
        }
    }
}
