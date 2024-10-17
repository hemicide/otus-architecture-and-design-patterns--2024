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

    public class StartProcessingCommandCollectionCommand : ICommand
    {
        CommandCollection _commandCollection;
        public StartProcessingCommandCollectionCommand(CommandCollection cc) { _commandCollection = cc; }

        public void Execute() => _commandCollection.BackgroundLoop();
    }
}
