using commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBattle.Commands
{
    public class FirstRetryCommand : ICommand
    {
        ICommand _command;
        public FirstRetryCommand(ICommand c) { _command = c; }
        public void Execute() => _command.Execute();
    }
}
