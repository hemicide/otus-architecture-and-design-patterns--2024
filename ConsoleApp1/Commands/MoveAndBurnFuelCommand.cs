using commands;
using SpaceBattle.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBattle.Commands
{
    public class MoveAndBurnFuelCommand : ICommand
    {
        MacroCommand _macrocommand;
        public MoveAndBurnFuelCommand(IFuelCheckable checkable, IMovable movable, IFuelBurnable burnable)
        {
            List<ICommand> cmds = new List<ICommand>()
            {
                new CheckFuelCommand(checkable),
                new MoveCommand(movable),
                new BurnFuelCommand(burnable),
            };

            _macrocommand = new MacroCommand(cmds);
        }

        public void Execute() => _macrocommand.Execute();
    }
}
