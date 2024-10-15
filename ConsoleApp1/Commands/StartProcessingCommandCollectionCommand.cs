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
        public StartProcessingCommandCollectionCommand() { }

        public void Execute() => CommandCollection.BackgroundLoop();
    }
}
