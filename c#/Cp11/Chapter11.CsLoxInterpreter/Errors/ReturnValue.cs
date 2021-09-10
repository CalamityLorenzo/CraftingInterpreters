using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsLoxInterpreter.Errors
{
    internal class ReturnValue : Exception
    {
        public object Value { get; }

        public ReturnValue()
        {
            Value = new object();
        }

        public ReturnValue(object value)
        {
            this.Value = value;
        }
    }
}
