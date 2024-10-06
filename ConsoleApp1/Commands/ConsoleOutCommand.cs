using commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBattle.Commands
{
    public class ConsoleOutCommand : ICommand
    {
        Exception _ex;
        public ConsoleOutCommand(Exception e) { _ex = e; }
        public void Execute() => Console.WriteLine(_ex.Message);
    }
}
