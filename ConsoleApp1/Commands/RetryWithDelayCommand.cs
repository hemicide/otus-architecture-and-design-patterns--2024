using commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBattle.Commands
{
    public class RetryWithDelayCommand : ICommand
    {
        ICommand _command;
        public RetryWithDelayCommand(ICommand c) { _command = c; }
        public void Execute() => CommandCollection.Add(_command);
    }
}
