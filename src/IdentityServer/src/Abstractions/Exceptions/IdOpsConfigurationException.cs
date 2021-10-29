using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdOps.Exceptions
{
    public class IdOpsConfigurationException : Exception
    {
        public IdOpsConfigurationException(string? message) : base(message)
        {
        }
    }
}
