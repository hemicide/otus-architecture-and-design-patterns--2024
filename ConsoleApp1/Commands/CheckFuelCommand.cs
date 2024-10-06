using commands;
using SpaceBattle.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBattle.Commands
{
    public class CheckFuelCommand : ICommand
    {
        IFuelCheckable _fuel;
        public CheckFuelCommand(IFuelCheckable obj) { _fuel = obj; }

        public void Execute()
        {
            if (!_fuel.Check())
                throw new CommandException();
        }
    }
}
