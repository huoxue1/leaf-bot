using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace leaf.core.errors
{
    public class ConvertTypeError : Exception
    {
        public ConvertTypeError() { }

        public ConvertTypeError(string message) : base(message) { }


        public ConvertTypeError(string message,Exception innerException) : base(message, innerException) { }
    }
}
