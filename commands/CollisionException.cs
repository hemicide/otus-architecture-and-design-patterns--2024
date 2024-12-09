using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace commands
{

    public class CollisionException : Exception
    {
        public CollisionException()
        {
        }

        public CollisionException(string? message) : base(message)
        {
        }

        public CollisionException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
