using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace commands
{
    public class MacroCommand : ICommand
    {
        IEnumerable<ICommand> _cmds;

        public MacroCommand(IEnumerable<ICommand> cmds) { _cmds = cmds; }

        public void Execute()
        {
            foreach (var cmd in _cmds)
            {
                try
                {
                    cmd.Execute();
                }
                catch (Exception ex)
                {
                    throw new CommandException("MacroCommand exception", ex);
                }
            }
        }
    }
}
