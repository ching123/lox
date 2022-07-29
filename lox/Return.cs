using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lox
{
    public class Return : SystemException
    {
        public object? value;
        public Return(object? value)
        {
            this.value = value;
        }
    }
}
