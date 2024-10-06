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
    public class ChangeVelocityCommand : ICommand
    {
        IChangeVelocity _changeVelosity;
        Vector2 _velocity = Vector2.Zero;
        public ChangeVelocityCommand(IChangeVelocity obj, Vector2 newV)
        {
            _changeVelosity = obj;
            _velocity = newV;
        }

        public void Execute() => _changeVelosity.SetVelocity(_velocity);
    }
}
