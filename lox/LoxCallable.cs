using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lox
{
    interface LoxCallable
    {
        int arity();
        object? call(Interpreter interpreter, List<object?> arguments);
    }
    public class Clock : LoxCallable
    {
        public int arity()
        {
            return 0;
        }

        public object? call(Interpreter interpreter, List<object?> arguments)
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
        public override string ToString()
        {
            return "<native fn>";
        }
    }
}
