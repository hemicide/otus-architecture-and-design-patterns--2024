using commands;
using SpaceBattle.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBattle.Commands
{
    public class RotateCommand : ICommand
    {
        IRotable _rotate;
        public RotateCommand(IRotable obj) { _rotate = obj; }

        public void Execute() => _rotate.SetDirection((_rotate.GetDirection() + _rotate.GetAngularVelocity()) % _rotate.GetDirectionsNumber());
    }
}
