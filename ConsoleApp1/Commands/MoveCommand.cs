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
    public class MoveCommand : ICommand
    {
        IMovable _movable;
        public MoveCommand(IMovable obj) { _movable = obj; }

        public void Execute() => _movable.SetPosition(Vector2.Add(_movable.GetPosition(), _movable.GetVelocity()));
    }
}
