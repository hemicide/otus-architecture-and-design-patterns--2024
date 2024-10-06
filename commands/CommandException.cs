using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace commands
{
    [Serializable]
    public class CommandException : Exception
    {
        public CommandException()
        {
        }

        public CommandException(string? message) : base(message)
        {
        }

        public CommandException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
