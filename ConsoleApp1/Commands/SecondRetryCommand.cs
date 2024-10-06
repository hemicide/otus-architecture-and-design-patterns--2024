using commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBattle.Commands
{
    public class SecondRetryCommand : ICommand
    {
        ICommand _command;
        public SecondRetryCommand(ICommand c) { _command = c; }
        public void Execute() => _command.Execute();
    }
}
