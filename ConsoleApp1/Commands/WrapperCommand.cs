using commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBattle.Commands
{
    public class WrapperCommand : ICommand
    {
        Action _action;

        public WrapperCommand(Action action) => _action = action;

        public void Execute() => _action();
    }
}
