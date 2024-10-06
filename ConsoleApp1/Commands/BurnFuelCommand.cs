using commands;
using SpaceBattle.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBattle.Commands
{
    public class BurnFuelCommand : ICommand
    {
        IFuelBurnable _fuel;
        public BurnFuelCommand(IFuelBurnable obj) { _fuel = obj; }

        public void Execute() => _fuel.Burn();
    }
}
