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
    public class FindSectorAndCheckСollision : ICommand
    {
        IMovable _movable;
        public FindSectorAndCheckСollision(IMovable obj) { _movable = obj; }

        public void Execute() => _movable.GetPosition();
    }
}
