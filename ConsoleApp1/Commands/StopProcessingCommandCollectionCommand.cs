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
        bool _force = false;
        public StopProcessingCommandCollectionCommand(bool force) { _force = force; }

        public void Execute() => CommandCollection.Stop(_force);
    }
}
