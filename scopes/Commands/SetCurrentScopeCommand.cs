using commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scopes.Commands
{
    public class SetCurrentScopeCommand : ICommand
    {
        object _scope;

        public SetCurrentScopeCommand(object scope)
        {
            _scope = scope;
        }

        public void Execute()
        {
            InitCommand.currentScopes.Value = _scope;
        }
    }
}
