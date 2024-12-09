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
    public class MoveMacroCommand : ICommand
    {
        IMovable _movable;
        IPlayfield _playfield;

        public MoveMacroCommand(IMovable obj, IPlayfield field) { _movable = obj; _playfield = field; }

        public void Execute()
        {
            new MoveCommand(_movable).Execute();

            try
            {
                new CheckGameObjectCollisionCommand(_movable, _playfield).Execute();
            }
            catch (Exception ex)
            {
                throw new CollisionException("Collision exception", ex);
            }
        }
    }
}
