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
        public RetryWithDelayNowCommand(ICommand c) { _command = c; }
        public void Execute() => CommandCollection.Add(new RetryWithDelayCommand(_command));
    }
}
