using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public interface ICommand
    {
        public void Execute();
    }

    public class MoveCommand : ICommand
    {
        IMovable _movable;
        public MoveCommand(IMovable obj) { _movable = obj; }

        public void Execute() => _movable.SetPosition(Vector2.Add(_movable.GetPosition(), _movable.GetVelocity()));
    }

    public class RotateCommand : ICommand
    {
        IRotable _rotate;
        public RotateCommand(IRotable obj) { _rotate = obj; }

        public void Execute() => _rotate.SetDirection((_rotate.GetDirection() + _rotate.GetAngularVelocity()) % _rotate.GetDirectionsNumber());
    }
}
