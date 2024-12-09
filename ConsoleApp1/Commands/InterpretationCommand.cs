using commands;
using factory;
using SpaceBattle.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBattle.Commands
{
    public class InterpretationCommand : ICommand
    {
        private readonly IMessage _message;

        public InterpretationCommand(IMessage message) => _message = message;

        public void Execute()
        {
            var cmd = IoC.Resolve<ICommand>("CreateCommand", _message);
            cmd.Execute();
        }
    }
}
