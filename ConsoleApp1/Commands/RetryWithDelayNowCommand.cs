using commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBattle.Commands
{
    public class RetryWithDelayNowCommand : ICommand
    {
        ICommand _command;
        CommandCollection _commandCollection;
        public RetryWithDelayNowCommand(CommandCollection cc, ICommand c) { _commandCollection = cc; _command = c; }
        public void Execute() => _commandCollection.Add(new RetryWithDelayCommand(_commandCollection, _command));
    }
}
