using commands;
using SpaceBattle.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBattle.Commands
{

    public class StopProcessingCommandCollectionCommand : ICommand
    {
        CommandCollection _commandCollection;
        bool _force = false;
        public StopProcessingCommandCollectionCommand(CommandCollection cc, bool force) { _commandCollection = cc; _force = force; }

        public void Execute() => _commandCollection.Stop(_force);
    }
}
