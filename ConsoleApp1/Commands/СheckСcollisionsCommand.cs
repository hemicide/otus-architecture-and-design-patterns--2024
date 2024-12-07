using commands;
using SpaceBattle.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBattle.Commands
{
    public class СheckСcollisionsCommand : ICommand
    {
        IMovable _gameObject1;
        IMovable _gameObject2;

        public СheckСcollisionsCommand(IMovable obj1, IMovable obj2) { _gameObject1 = obj1; _gameObject2 = obj2; }

        public void Execute()
        {
            if (_gameObject1.GetPosition() == _gameObject2.GetPosition())
                throw new CollisionException();
        }
    }
}
