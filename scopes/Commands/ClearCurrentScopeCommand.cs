using commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scopes.Commands
{
    public class ClearCurrentScopeCommand : ICommand
    {
        public void Execute()
        {
            InitCommand.currentScopes.Value = null;
        }
    }
}
