using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lox
{
    public class LoxInstance
    {
        private LoxClass cls;
        private Dictionary<string, object?> fields = new Dictionary<string, object?>();
        public LoxInstance(LoxClass cls)
        {
            this.cls = cls;
        }
        public object? Get(Token name)
        {
            if (fields.ContainsKey(name.lexeme))
            {
                return fields[name.lexeme];
            }
            var method = cls.findMethod(name.lexeme);
            if (method != null)
            {
                return method.bind(this);
            }
            throw new RuntimeError(name, $"undefined property '{name.lexeme}'.");
        }
        public void Set(Token name, object? value)
        {
            fields[name.lexeme] = value;
        }
        public override string ToString()
        {
            return $"{cls.ToString()} instance";
        }
    }
}
